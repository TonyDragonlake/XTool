using FantaziaDesign.Core;
using System;
using System.IO;
using FantaziaDesign.Model.Document;

namespace XTool.Excel.WindowModels
{
	public sealed class FilterConfigurationFile : IDocument
	{
		public sealed class DocumentFactory : IDocumentFactory<FilterConfigurationFile>
		{
			private DocumentType m_docType;
			internal DocumentFactory()
			{
				m_docType = DocumentType.Register(nameof(FilterConfigurationFile))
					.AddExtension("fcfg");
			}

			public DocumentType DocumentType => m_docType;

			public FilterConfigurationFile CreateDocument()
			{
				return new FilterConfigurationFile();
			}

			public bool TryGetExistedDocument(string filePath, out FilterConfigurationFile document)
			{
				if (File.Exists(filePath))
				{
					document = new FilterConfigurationFile() { m_fileInfo = new FileInfo(filePath), m_isSaved = true };
					return true;
				}
				document = null;
				return false;
			}
		}

		public static readonly IDocumentFactory<FilterConfigurationFile> Factory = new DocumentFactory();

		private long m_uId;
		private FileInfo m_fileInfo;
		private IDocumentContent m_docContent;
		private bool m_isSaved;
		private bool m_isChangingDocContent;

		private FilterConfigurationFile()
		{
			m_uId = SnowflakeUId.Next();
		}

		public FileInfo FileInfo => m_fileInfo;

		public IDocumentContent Content { get => m_docContent; }

		public DocumentType DocumentType => Factory.DocumentType;

		public bool IsLoaded => m_docContent?.ContentRoot != null;

		public bool IsSaved => m_isSaved;

		public bool IsAnonymous => m_fileInfo is null;

		public long UId => m_uId;

		public bool IsChangingDocumentContent => m_isChangingDocContent;

		public bool OverrideFileInfo(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				if (IsAnonymous)
				{
					return false;
				}
				m_fileInfo = null;
				m_isSaved = false;
				return true;
			}
			if (IsAnonymous)
			{
				m_fileInfo = new FileInfo(filePath);
				m_isSaved = false;
				return true;
			}
			m_fileInfo.Refresh();
			if (!string.Equals(m_fileInfo.FullName, filePath, StringComparison.OrdinalIgnoreCase))
			{
				m_fileInfo = new FileInfo(filePath);
				m_isSaved = false;
				return true;
			}
			return false;
		}

		public bool OverrideContent(IDocumentContent content)
		{
			if (m_isChangingDocContent)
			{
				return true;
			}

			if (((object)content) is null)
			{
				m_docContent = null;
			}
			else if (content.ContentType == FilterConfiguration.s_contentType)
			{
				m_isChangingDocContent = true;
				if (m_docContent is null)
				{
					m_docContent = content;
					m_docContent.TrySetParentDocument(this);
				}
				else if (m_docContent.UId != content.UId)
				{
					m_docContent.TrySetParentDocument(null);
					m_docContent = content;
					m_docContent.TrySetParentDocument(this);
				}
				m_isChangingDocContent = false;
			}
			m_isSaved = false;
			return true;
		}

		public bool Load()
		{
			if (!IsLoaded && m_fileInfo != null)
			{
				m_fileInfo.Refresh();
				var loader = Factory.DocumentType?.DocumentConverter;
				if (m_fileInfo.Exists && loader != null)
				{
					object obj = null;
					using (var fileStream = m_fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						obj = loader.ConvertTo(fileStream);
					}
					if (m_docContent is null)
					{
						m_docContent = new FilterConfiguration(this);
					}
					m_docContent.TrySetContentRoot(obj);
				}
			}
			return IsLoaded;
		}

		public bool Save()
		{
			if (!m_isSaved)
			{
				m_fileInfo.Refresh();
				var saver = Factory.DocumentType?.DocumentConverter;
				if (saver != null)
				{
					using (var fileStream = m_fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
					{
						object content = m_docContent?.ContentRoot;
						if (content is null)
						{
							fileStream.Seek(0, SeekOrigin.Begin);
							fileStream.SetLength(0);
							fileStream.Flush(true);
						}
						else
						{
							using (var tempStream = saver.ConvertBack(m_docContent.ContentRoot))
							{
								fileStream.Seek(0, SeekOrigin.Begin);
								fileStream.SetLength(0);
								tempStream.CopyTo(fileStream);
								fileStream.Flush(true);
							}
						}
					}
					m_isSaved = true;
				}
			}
			return m_isSaved;
		}

		public bool TrySaveAs(string filePath, out IDocument document)
		{
			var file = Factory.CreateDocument();
			file.m_fileInfo = new FileInfo(filePath);
			var docContent = m_docContent.DeepCopy();
			file.OverrideContent(docContent);
			document = file;
			return document.Save();
		}

		public void OnContentRootChanged()
		{
			m_isSaved = false;
		}

		public bool FilePathEquals(string filePath)
		{
			if (m_fileInfo is null)
			{
				return false;
			}
			if (string.IsNullOrWhiteSpace(filePath))
			{
				return false;
			}
			m_fileInfo.Refresh();
			return string.Equals(m_fileInfo.FullName, filePath, StringComparison.OrdinalIgnoreCase);
		}

		public bool Unload()
		{
			if (IsLoaded)
			{
				m_isChangingDocContent = true;
				m_docContent.TrySetParentDocument(null);
				m_isChangingDocContent = false;
			}
			return !IsLoaded;
		}
	}
}
