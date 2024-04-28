using System;
using System.Collections.Generic;
using FantaziaDesign.Core;

namespace XTool.Excel.WindowModels
{
	public sealed class FilterConfiguration : IEquatable<FilterConfiguration>, IDocumentContent
	{
		public static readonly Type s_contentType = typeof(List<FilterValuesSetting>);

		private long m_uId;
		private IDocument m_parentDoc;
		private List<FilterValuesSetting> m_settings;

		public FilterConfiguration()
		{
			m_uId = SnowflakeUId.Next();
		}

		public FilterConfiguration(FilterConfigurationFile parentDocument) : this()
		{
			m_parentDoc = parentDocument ?? throw new ArgumentNullException(nameof(parentDocument));
		}

		public long UId => m_uId;

		public IReadOnlyList<FilterValuesSetting> Settings => m_settings;

		public Type ContentType => s_contentType;

		public object ContentRoot => m_settings;

		public IDocument ParentDocument => m_parentDoc;

		public object Clone()
		{
			return DeepCopy();
		}

		public IDocumentContent DeepCopy()
		{
			List<FilterValuesSetting> settings = null;
			if (m_settings != null)
			{
				settings = new List<FilterValuesSetting>(m_settings.Count);
				foreach (var setting in m_settings)
				{
					settings.Add(new FilterValuesSetting(setting));
				}
			}
			return new FilterConfiguration
			{
				m_settings = settings
			};
		}

		public void DeepCopyValueFrom(IDocumentContent obj)
		{
			if (obj.TryGetContentRootAs<List<FilterValuesSetting>>(out var settings))
			{
				if (m_settings != null)
				{
					m_settings.Clear();
					m_settings.Capacity = settings.Count;
				}
				else
				{
					m_settings = new List<FilterValuesSetting>(settings.Count);
				}
				foreach (var setting in settings)
				{
					m_settings.Add(new FilterValuesSetting(setting));
				}
			}
			else
			{
				m_settings = null;
			}
		}

		public bool Equals(FilterConfiguration other)
		{
			return !(other is null) && m_uId == other.m_uId;
		}

		public bool TryGetContentRootAs<T>(out T contentRoot)
		{
			if (typeof(T).IsAssignableFrom(s_contentType))
			{
				contentRoot = (T)(object)m_settings;
				return true;
			}
			contentRoot = default(T);
			return false;
		}

		public bool TrySetContentRoot(object contentRoot)
		{
			if (contentRoot is null)
			{
				return false;
			}

			var type = contentRoot.GetType();
			if (s_contentType.IsAssignableFrom(type))
			{
				m_settings = contentRoot as List<FilterValuesSetting>;
				if (m_parentDoc != null)
				{
					m_parentDoc.OnContentRootChanged();
				}
				return true;
			}
			return false;
		}

		public bool TrySetParentDocument(IDocument parentDocument)
		{
			if (m_parentDoc is null)
			{
				if (parentDocument != null && parentDocument.OverrideContent(this))
				{
					m_parentDoc = parentDocument;
					return true;
				}
			}
			else
			{
				if (parentDocument is null)
				{
					m_parentDoc = null;
					return true;
				}
				if (m_parentDoc.UId != parentDocument.UId && parentDocument.OverrideContent(this))
				{
					m_parentDoc = parentDocument;
					return true;
				}
			}
			return false;
		}
	}
}
