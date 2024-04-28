using System;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using FantaziaDesign.Core;
using FantaziaDesign.Input;
using FantaziaDesign.Events;
using FantaziaDesign.Model;
using FantaziaDesign.Model.Document;
using FantaziaDesign.Wpf.Message;

namespace XTool.Excel.WindowModels
{
	public sealed class FilterConfigurationsManager : NotifiableList<IDocument>
	{
		private static readonly object s_lockObj = new object();

		private static FilterConfigurationsManager s_current;

		public static FilterConfigurationsManager Current 
		{
			get
			{
				lock (s_lockObj)
				{
					if (s_current is null)
					{
						s_current = new FilterConfigurationsManager();
						s_current.Initialize();
					}
					return s_current;
				}
			}
		}

		private const string FilterConfigsFileName = "FilterConfigsList.history";

		public string ConfigurationsListFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilterConfigsFileName);

		private bool m_isHistoryChanged;

		private List<FilterValuesSetting> m_currentContentRoot;
		private int m_selectedIndex;
		private int m_currentDocIndex;
		private IDocument m_anonymousDoc;
		private RelayCommand m_saveCurrentConfigurationCommand;
		private RelayCommand m_saveCurrentConfigurationAsCommand;
		private RelayCommand m_addNewConfigurationCommand;
		private RelayAsyncCommand m_openConfigurationListDialogCommand;
		private RelayCommand m_submitConfigurationCommand;
		private RelayCommand m_addExistedConfigurationsCommand;
		private WeakEvent<EventHandler<TypedRef<bool>>> m_collectContentRootEvent;
		private WeakEvent<EventHandler> m_currentDocumentChangedEvent;
		private WeakEvent<EventHandler<TypedRef<bool>>> m_submitConfigEvent;
		private FilterConfigurationsDialog m_configDialog;
		private bool m_isConfigSubmitted;
		private SaveFileDialog m_saveFileDialog;

		public string BaseDirectory { get; set; }

		public int SelectedIndex 
		{ 
			get => m_selectedIndex; 
			set => this.SetPropertyIfChanged(ref m_selectedIndex, value, nameof(SelectedIndex)); 
		}
		
		public int CurrentDocumentIndex { get => m_currentDocIndex; }

		public string CurrentDocumentPath
		{
			get
			{
				var doc = CurrentDocument;
				if (doc.IsAnonymous)
				{
					return "<Untitled>";
				}
				return doc.FileInfo.FullName;
			}
		}

		internal IDocument CurrentDocument
		{
			get
			{
				if (m_currentDocIndex < 0 || m_currentDocIndex >= Count)
				{
					if (m_anonymousDoc is null)
					{
						m_anonymousDoc = FilterConfigurationFile.Factory.CreateDocument();
					}
					return m_anonymousDoc;
				}
				return m_items[m_currentDocIndex];
			}
		}

		public IReadOnlyList<FilterValuesSetting> CurrentContentRoot => m_currentContentRoot;

		public RelayCommand SaveCurrentConfigurationCommand => m_saveCurrentConfigurationCommand;

		public RelayCommand SaveCurrentConfigurationAsCommand => m_saveCurrentConfigurationAsCommand;

		public RelayCommand AddNewConfigurationCommand => m_addNewConfigurationCommand;

		public RelayAsyncCommand OpenConfigurationListDialogCommand => m_openConfigurationListDialogCommand;

		public RelayCommand SubmitConfigurationCommand => m_submitConfigurationCommand;

		public RelayCommand AddExistedConfigurationsCommand => m_addExistedConfigurationsCommand;

		public bool IsConfigurationSubmitted => m_isConfigSubmitted;

		public event EventHandler<TypedRef<bool>> CollectingContentRoot
		{
			add { m_collectContentRootEvent.AddHandler(value); }
			remove { m_collectContentRootEvent.RemoveHandler(value); }
		}

		public event EventHandler CurrentDocumentChanged
		{
			add { m_currentDocumentChangedEvent.AddHandler(value); }
			remove { m_currentDocumentChangedEvent.RemoveHandler(value); }
		}

		public event EventHandler<TypedRef<bool>> SubmittingConfiguration
		{
			add { m_submitConfigEvent.AddHandler(value); }
			remove { m_submitConfigEvent.RemoveHandler(value); }
		}

		public FilterConfigurationsManager()
		{
			m_selectedIndex = -1;
			m_currentDocIndex = -1;
			m_collectContentRootEvent = new WeakEvent<EventHandler<TypedRef<bool>>>();
			m_currentDocumentChangedEvent = new WeakEvent<EventHandler>();
			m_submitConfigEvent = new WeakEvent<EventHandler<TypedRef<bool>>>();
			m_saveCurrentConfigurationCommand = new RelayCommand(ExecuteSaveCurrentConfigurationCommand);
			m_saveCurrentConfigurationAsCommand = new RelayCommand(ExecuteSaveCurrentConfigurationAsCommand);
			m_addNewConfigurationCommand = new RelayCommand(ExecuteAddNewConfigurationCommand);
			m_openConfigurationListDialogCommand = new RelayAsyncCommand(ExecuteOpenConfigurationListDialogCommand);
			m_submitConfigurationCommand = new RelayCommand(ExecuteSubmitConfigurationCommand);
			m_addExistedConfigurationsCommand = new RelayCommand(ExecuteAddExistedConfigurationsCommand);
			m_configDialog = new FilterConfigurationsDialog(this)
			{
				DialogTitle = XResourceManager.GetResourceUIText("Title_SelectConfiguration"),
				MaskOpacity = 0.5
			};
		}

		public void Initialize()
		{
			var filePath = ConfigurationsListFilePath;
			if (File.Exists(filePath))
			{
				using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (var streamReader = new StreamReader(fileStream))
					{
						string path;
						var factory = FilterConfigurationFile.Factory;
						do
						{
							path = streamReader.ReadLine();
							if (factory.TryGetExistedDocument(path, out var document))
							{
								m_items.Add(document);
							}
						} while (path != null);
					}
				}
			}
		}

		public void Save()
		{
			if (!m_isHistoryChanged)
			{
				return;
			}

			var filePath = ConfigurationsListFilePath;
			using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
			{
				using (var streamWriter = new StreamWriter(fileStream))
				{
					foreach (var document in m_items)
					{
						var fileInfo = document.FileInfo;
						if (fileInfo != null)
						{
							fileInfo.Refresh();
							if (fileInfo.Exists)
							{
								streamWriter.WriteLine(fileInfo.FullName);
							}
						}
					}
				}
			}
			m_isHistoryChanged = false;
		}

		public void ApplySelection()
		{
			if (ContainsIndex(m_selectedIndex) && m_currentDocIndex != m_selectedIndex)
			{
				m_currentDocIndex = m_selectedIndex;
				var doc = CurrentDocument;
				if (!doc.IsLoaded)
				{
					doc.Load();
				}
				m_currentContentRoot = doc.Content?.ContentRoot as List<FilterValuesSetting>;
				m_isConfigSubmitted = false;
				RaisePropertyChangedEvent(nameof(CurrentDocumentPath));
				m_currentDocumentChangedEvent.RaiseEvent(new WeakEventArgs(this, EventArgs.Empty));
			}
		}

		public bool TrySetContentRootForCurrentDocument(List<FilterValuesSetting> settings)
		{
			if (m_currentContentRoot is null && settings.Count > 0)
			{
				var doc = CurrentDocument;
				IDocumentContent documentContent;
				if (doc.IsLoaded && doc.Content != null)
				{
					documentContent = doc.Content;
				}
				else
				{
					documentContent = new FilterConfiguration();
					doc.OverrideContent(documentContent);
				}
				documentContent.TrySetContentRoot(settings);
				m_currentContentRoot = settings;
				m_isConfigSubmitted = false;
				return true;
			}
			return false;
		}

		public void UnloadDocuments()
		{
			foreach (var item in m_items)
			{
				item.Unload();
			}
		}

		protected override void NotifyCollectionChangedEvent(NotifyCollectionChangedEventArgs eventArgs)
		{
			m_isHistoryChanged = true;
			base.NotifyCollectionChangedEvent(eventArgs);
		}

		public void ResetCurrentDocument()
		{
			m_currentDocIndex = -1;
			m_anonymousDoc = null;
		}
		
		public void InvalidateCurrentContentRoot()
		{
			m_currentContentRoot = null;
			m_isConfigSubmitted = false;
		}

		private void ExecuteAddNewConfigurationCommand()
		{
			if (ContainsIndex(m_currentDocIndex))
			{
				m_currentDocIndex = -1;
				m_anonymousDoc = FilterConfigurationFile.Factory.CreateDocument();
				RaisePropertyChangedEvent(nameof(CurrentDocumentPath));
			}
		}

		private Task ExecuteOpenConfigurationListDialogCommand(CancellationToken token)
		{
			return MessageCenter.Show("FilterAssistantWindowMessageHost", m_configDialog);
		}

		private void ExecuteSaveCurrentConfigurationCommand()
		{
			var currentDoc = CurrentDocument;
			if (currentDoc.IsSaved && m_currentContentRoot != null)
			{
				return;
			}
			var eventArgs = new WeakEventArgs<TypedRef<bool>>(this, false);
			if (m_currentContentRoot is null)
			{
				m_collectContentRootEvent.RaiseEvent(eventArgs);
				if (!eventArgs.EventArgs)
				{
					return;
				}
			}
			if (m_currentContentRoot is null)
			{
				return;
			}
			ExecuteSaveFileDialog(currentDoc);
		}

		private void ExecuteSaveCurrentConfigurationAsCommand()
		{
			var currentDoc = CurrentDocument;
			var eventArgs = new WeakEventArgs<TypedRef<bool>>(this, false);
			if (m_currentContentRoot is null)
			{
				m_collectContentRootEvent.RaiseEvent(eventArgs);
			}
			ExecuteSaveFileDialog(currentDoc, true);
		}

		private void ExecuteSaveFileDialog(IDocument doc, bool isSaveAsMode = false)
		{
			var isAnonymous = doc.IsAnonymous;
			if (isAnonymous || isSaveAsMode)
			{
				if (m_saveFileDialog is null)
				{
					var builder = new StringBuilder();
					DocumentType.FormatDocumentType(builder, doc.DocumentType);
					builder.Append(DocumentType.Spliter);
					DocumentType.FormatAny(builder);
					var filter = builder.ToString();
					m_saveFileDialog = new SaveFileDialog();
					m_saveFileDialog.Filter = filter;
					m_saveFileDialog.AddExtension = true;
				}
				m_saveFileDialog.Title = XResourceManager.GetResourceUIText("Title_DialogSave");
				m_saveFileDialog.InitialDirectory = BaseDirectory;
				if (m_saveFileDialog.ShowDialog().GetValueOrDefault())
				{
					var filePath = m_saveFileDialog.FileName;
					if (isAnonymous)
					{
						doc.OverrideFileInfo(filePath);
						doc.Save();
						AddOrReplaceDocument(doc);
						m_selectedIndex = m_currentDocIndex;
						m_anonymousDoc = null;
						RaisePropertyChangedEvent(nameof(CurrentDocumentPath));
					}
					else if (doc.FilePathEquals(filePath))
					{
						doc.Save();
					}
					else
					{
						if (doc.TrySaveAs(filePath, out var newDoc))
						{
							AddOrReplaceDocument(newDoc);
							m_selectedIndex = m_currentDocIndex;
							m_anonymousDoc = null;
							RaisePropertyChangedEvent(nameof(CurrentDocumentPath));
						}
					}
				}
			}
			else
			{
				doc.Save();
			}
		}

		private void AddOrReplaceDocument(IDocument doc)
		{
			doc.FileInfo.Refresh();
			var filePath = doc.FileInfo.FullName;
			int targetIndex = FindIndexOf(filePath);
			if (targetIndex < 0)
			{
				Add(doc);
				m_currentDocIndex = Count - 1;
			}
			else
			{
				SetItem(targetIndex, doc);
				m_currentDocIndex = targetIndex;
			}
		}

		private int FindIndexOf(string filePath)
		{
			int targetIndex = -1;
			for (int i = 0; i < m_items.Count; i++)
			{
				if (m_items[i].FilePathEquals(filePath))
				{
					targetIndex = i;
					break;
				}
			}
			return targetIndex;
		}

		private void ExecuteSubmitConfigurationCommand()
		{
			if (m_isConfigSubmitted)
			{
				return;
			}
			var eventArgs = new WeakEventArgs<TypedRef<bool>>(this, false);
			if (m_currentContentRoot is null)
			{
				m_collectContentRootEvent.RaiseEvent(eventArgs);
				if (!eventArgs.EventArgs)
				{
					return;
				}
			}
			m_submitConfigEvent.RaiseEvent(eventArgs);
			m_isConfigSubmitted = eventArgs.EventArgs;
		}

		private void ExecuteAddExistedConfigurationsCommand()
		{
			var factory = FilterConfigurationFile.Factory;
			var builder = new StringBuilder();
			DocumentType.FormatDocumentType(builder, factory.DocumentType);
			builder.Append(DocumentType.Spliter);
			DocumentType.FormatAny(builder);
			var filter = builder.ToString();
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = filter;
			openFileDialog.Title = XResourceManager.GetResourceUIText("Title_DialogOpen");
			openFileDialog.AddExtension = true;
			openFileDialog.InitialDirectory = BaseDirectory;
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog().GetValueOrDefault())
			{
				var filePaths = openFileDialog.FileNames;
				if (filePaths != null && filePaths.Length > 0)
				{
					foreach (var filePath in filePaths)
					{
						if (factory.TryGetExistedDocument(filePath, out var document))
						{
							Add(document);
						}
					}
				}
			}
		}
	}
}
