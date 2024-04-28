using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using FantaziaDesign.Core;
using FantaziaDesign.Model;
using XTool.Excel.ObjectModels;
using XTool.Windows;
using System.Diagnostics;
using FantaziaDesign.Input;
using System.Threading;
using System.Threading.Tasks;
using FantaziaDesign.Wpf.Message;
using FantaziaDesign.Model.Message;
using System.Reflection;

namespace XTool.Excel.WindowModels
{
	public class FilterAssistantManager : PropertyNotifier, IDisposable
	{
		private Application m_appHost;
		private XAutoFilter m_filter;
		private FilterAssistantWindow m_view;
		private FieldFiltersManager m_fieldFiltersManager;
		private FilterConfigurationsManager m_configurationsManager;

		private RelayAsyncCommand m_openAboutDialogCommand;
		private RelayAsyncCommand m_openSettingDialogCommand;
		private static AssemblyInfoAccessor s_assemblyInfoAccessor = new AssemblyInfoAccessor(Assembly.GetExecutingAssembly(), true);

#if DEBUG
		private bool m_isSimulating;
#endif
		private bool m_disposedValue;
		public string FilterRangeAddress => m_filter.Range?.Address;
		public string FilterRangeFullAddress => m_filter.Range?.FullAddress;

		public FieldItemsContainer Fields { get => m_fieldFiltersManager.Fields; }
		public FieldFiltersManager FieldFiltersManager { get => m_fieldFiltersManager; }
		public FilterConfigurationsManager ConfigurationsManager { get => m_configurationsManager;  }
		public RelayAsyncCommand OpenAboutDialogCommand { get => m_openAboutDialogCommand; }
		public RelayAsyncCommand OpenSettingsDialogCommand { get => m_openSettingDialogCommand; }


		public FilterAssistantManager(Application appHost) : base()
		{
			m_appHost = appHost;
			m_filter = new XAutoFilter();
			m_fieldFiltersManager = new FieldFiltersManager();
			m_configurationsManager = FilterConfigurationsManager.Current;
			m_configurationsManager.CollectingContentRoot += OnCollectingContentRoot;
			m_configurationsManager.CurrentDocumentChanged += OnCurrentDocumentChanged;
			m_configurationsManager.SubmittingConfiguration += OnSubmittingConfiguration;
			FilterConfigurationFile.Factory.DocumentType.SetDocumentConverter(new YamlStreamToFilterValuesSettingsConverter());
			m_openAboutDialogCommand = new RelayAsyncCommand(OnOpenAboutDialog);
			m_openSettingDialogCommand = new RelayAsyncCommand(OnOpenSettingsDialog);
		}

		private async Task OnOpenAboutDialog(CancellationToken token)
		{
			await MessageCenter.Show("FilterAssistantWindowMessageHost", 
				new AboutInformationDialog(s_assemblyInfoAccessor) 
				{ 
					CloseOnClickAway = true, 
					MaskOpacity = 0.5, 
					DialogTitle = XResourceManager.GetResourceUIText("MenuHeader_AboutXTool"),
				});
		}

		private async Task OnOpenSettingsDialog(CancellationToken token)
		{
			await MessageCenter.Show("FilterAssistantWindowMessageHost",
				new SettingConfigurationsDialog()
				{
					CloseOnClickAway = true,
					MaskOpacity = 0.5,
					DialogTitle = XResourceManager.GetResourceUIText("MenuHeader_Settings"),
				});
		}

#if DEBUG
		public FilterAssistantManager()
		{
			m_isSimulating = true;
			m_filter = new XAutoFilter();
			m_fieldFiltersManager = new FieldFiltersManager();
			m_configurationsManager = FilterConfigurationsManager.Current;
			m_configurationsManager.CollectingContentRoot += OnCollectingContentRoot;
			m_configurationsManager.CurrentDocumentChanged += OnCurrentDocumentChanged;
			m_configurationsManager.SubmittingConfiguration += OnSubmittingConfiguration;
			FilterConfigurationFile.Factory.DocumentType.SetDocumentConverter(new YamlStreamToFilterValuesSettingsConverter());
		}
#endif
		public static bool ApplyFilterValuesSettings(XAutoFilter filter, IReadOnlyList<IFilterValuesSetting> settings)
		{
			if (filter is null)
			{
				return false;
			}
			if (settings is null)
			{
				return false;
			}
			if (settings.Count > 0 && filter.HasParent)
			{
				var rawRng = filter.Range.Parent;
				var sheet = rawRng.Worksheet;
				rawRng.Select();
				var app = rawRng.Application;
				var cmdBars = app.CommandBars;
				if (cmdBars.GetEnabledMso("SortClear"))
				{
					cmdBars.ExecuteMso("SortClear");
				}
				foreach (var item in settings)
				{
					rawRng.AutoFilter(item.FieldIndex, item.Criteria, XlAutoFilterOperator.xlFilterValues);
				}
				filter.SetParent(sheet.AutoFilter);
				Marshal.ReleaseComObject(cmdBars);
				Marshal.ReleaseComObject(app);
				Marshal.ReleaseComObject(sheet);
				cmdBars = null;
				app = null;
				sheet = null;
				return true;
			}
			return false;
		}

		public void LaunchWindowView()
		{
			if (m_view != null)
			{
				return;
			}
			var dir = GetActivedWorkbookPath();
			if (!string.IsNullOrWhiteSpace(dir))
			{
				m_configurationsManager.BaseDirectory = dir;
			}
			var sheet = m_appHost.ActiveSheet as Worksheet;
			if (sheet != null)
			{
				m_filter.SetParent(sheet.AutoFilter);
				if (m_filter.HasParent)
				{
					SetupFieldFiltersManager();
					Marshal.ReleaseComObject(sheet);
					sheet = null;
					var hwnd = GetActiveWindowHandle();
					ShowWindow(hwnd);
				}
				else
				{
					var result = System.Windows.MessageBox.Show(
						XResourceManager.GetResourceUIText("Content_AutoApplyFilter"),
						XResourceManager.GetResourceUIText("Title_XToolFilterAssistant"),
						System.Windows.MessageBoxButton.OKCancel,
						System.Windows.MessageBoxImage.Asterisk,
						System.Windows.MessageBoxResult.Cancel
						);
					if (result == System.Windows.MessageBoxResult.OK)
					{
						var usedRange = sheet.UsedRange;
						var cmdBars = m_appHost.CommandBars;
						usedRange.Select();
						cmdBars.ExecuteMso("Filter");
						Marshal.ReleaseComObject(cmdBars);
						Marshal.ReleaseComObject(usedRange);
						cmdBars = null;
						usedRange = null;
						m_filter.SetParent(sheet.AutoFilter);
						Marshal.ReleaseComObject(sheet);
						sheet = null;
						if (m_filter.HasParent)
						{
							SetupFieldFiltersManager();
							var hwnd = GetActiveWindowHandle();
							ShowWindow(hwnd);
						}
					}
					else
					{
						Marshal.ReleaseComObject(sheet);
						sheet = null;
					}
				}
			}
		}

		private string GetActivedWorkbookPath()
		{
			var workbook = m_appHost.ActiveWorkbook;
			var path = workbook.Path;
			Marshal.ReleaseComObject(workbook);
			workbook = null;
			return path;
		}

		private void OnSubmittingConfiguration(object sender, TypedRef<bool> e)
		{
			var root = m_configurationsManager.CurrentContentRoot;
			var result = ApplyFilterValuesSettings(m_filter, root);
			TypedRef<bool>.CriticalSetItem(e, result);
		}

		private void OnCurrentDocumentChanged(object sender, EventArgs e)
		{
			var root = m_configurationsManager.CurrentContentRoot;
			m_fieldFiltersManager.SetFromFilterValuesSettings(root);
		}

		private void OnCollectingContentRoot(object sender, TypedRef<bool> e)
		{
			var filterSettings = m_fieldFiltersManager.AsFilterValuesSettings();
			var result = m_configurationsManager.TrySetContentRootForCurrentDocument(filterSettings);
			TypedRef<bool>.CriticalSetItem(e, result);
		}

#if DEBUG
		public bool MakeSimulationData(int dataCount)
		{
			if (!m_isSimulating)
			{
				return false;
			}

			if (dataCount <= 0)
			{
				return false;
			}
			m_fieldFiltersManager.SetupFromRange(GetFieldItems(dataCount));
			return true;
		}

		private IEnumerable<FieldItem> GetFieldItems(int dataCount)
		{
			for (int i = 0; i < dataCount; i++)
			{
				yield return new FieldItem(new XAddress(1, true, i + 1, true), $"DataItem_{i}");
			}
			yield break;
		}

		public void LaunchSimulationWindowView(IntPtr parentWindowHandle)
		{
			if (m_isSimulating)
			{
				ShowWindow(parentWindowHandle);
			}
		}
#endif
		private void SetupFieldFiltersManager()
		{
			var range = m_filter.Range;
			var fields = range.AtRow(new XIndex(1)).Select(cell => new FieldItem(range, cell));
			m_fieldFiltersManager.SetupFromRange(fields);
		}

		private void ShowWindow(IntPtr hwnd)
		{
			m_view = new FilterAssistantWindow();
			m_view.DataContext = this;

			if (hwnd != IntPtr.Zero)
			{
				m_view.InteropHelper.Owner = hwnd;
			}

			m_view.Closed += OnWindowViewClosed;
			m_view.ShowDialog();

			//m_view = new FilterAssistantWindow();
			//m_view.DataContext = this;

			//if (hwnd != IntPtr.Zero)
			//{
			//	m_view.InteropHelper.Owner = hwnd;
			//}

			//m_view.Closed += OnWindowViewClosed;
			//m_view.ShowDialog();

			//Dispatcher.Run();



			//if (hwnd != IntPtr.Zero)
			//{
			//	m_view.InteropHelper.Owner = hwnd;
			//}
			//m_view.Show();
		}


		private IntPtr GetActiveWindowHandle()
		{
			int hwnd = 0;
			var window = m_appHost.ActiveWindow;
			if (window != null)
			{
				hwnd = window.Hwnd;
				Marshal.ReleaseComObject(window);
				window = null;
			}

			using (var currentProcess = Process.GetCurrentProcess())
			{
				var ptr = currentProcess.MainWindowHandle;
				if (ptr.ToInt32() != hwnd)
				{
					return ptr;
				}
			}

			return new IntPtr(hwnd);
		}

		private void OnWindowViewClosing(object sender, CancelEventArgs e)
		{
			if (m_configurationsManager.IsConfigurationSubmitted)
			{
				return;
			}

			if (m_fieldFiltersManager.HasFilters)
			{
				var result = System.Windows.MessageBox.Show(
					XResourceManager.GetResourceUIText("Content_ApplyCurrentFilterSettings"),
					XResourceManager.GetResourceUIText("Title_XToolFilterAssistant"),
					System.Windows.MessageBoxButton.OKCancel,
					System.Windows.MessageBoxImage.Question,
					System.Windows.MessageBoxResult.Cancel
					);
				if (result == System.Windows.MessageBoxResult.OK)
				{
					m_configurationsManager.SubmitConfigurationCommand.Execute(null);
				}
			}
			e.Cancel = false;
		}

		private void OnWindowViewClosed(object sender, EventArgs e)
		{
			var wnd = sender as FilterAssistantWindow;
			wnd.Closed -= OnWindowViewClosed;
			m_view = null;
			m_configurationsManager.Save();
			m_configurationsManager.ResetCurrentDocument();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposedValue)
			{
				if (disposing)
				{
					m_configurationsManager.CollectingContentRoot -= OnCollectingContentRoot;
					m_configurationsManager.CurrentDocumentChanged -= OnCurrentDocumentChanged;
					m_configurationsManager.SubmittingConfiguration -= OnSubmittingConfiguration;
					m_fieldFiltersManager = null;
					m_configurationsManager = null;
				}
				if (m_appHost != null)
				{
					Marshal.ReleaseComObject(m_appHost);
					m_appHost = null;
				}
				if (m_filter != null)
				{
					m_filter.Dispose();
				}
				// TODO: 释放未托管的资源(未托管的对象)并重写终结器
				// TODO: 将大型字段设置为 null
				m_disposedValue = true;
			}
		}

		// // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
		// ~FilterAssistantManager()
		// {
		//     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
