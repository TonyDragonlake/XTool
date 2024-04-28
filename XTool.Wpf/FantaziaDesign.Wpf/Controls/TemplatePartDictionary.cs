using FantaziaDesign.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FantaziaDesign.Wpf.Controls
{
	public sealed class TemplatePartDictionary<TFrameworkElement> : 
		IReadOnlyDictionary<string, DependencyObject> 
		where TFrameworkElement : FrameworkElement
	{
		private static TemplatePartAttribute[] s_templatePartAttris;

		public static IReadOnlyList<TemplatePartAttribute> TemplatePartAttributes
		{
			get
			{
				if (s_templatePartAttris is null)
				{
					s_templatePartAttris = Attribute.GetCustomAttributes(typeof(TFrameworkElement), true)
						.OfType<TemplatePartAttribute>()
						.ToArray();
				}
				return s_templatePartAttris;
			}
		}

		public static void InvalidateTemplatePartAttributes()
		{
			s_templatePartAttris = null;
		}

		private TFrameworkElement m_parent;
		private Dictionary<string, DependencyObject> m_parts;

		public TemplatePartDictionary(TFrameworkElement parent)
		{
			m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}
		
		public TFrameworkElement Parent => m_parent;
		public bool IsTemplatePartsInitialized => m_parts != null;

		public IEnumerable<string> Keys => m_parts?.Keys;

		public IEnumerable<DependencyObject> Values => m_parts?.Values;

		public int Count => (m_parts?.Count).GetValueOrDefault();

		public DependencyObject this[string key] => m_parts?[key];

		public void InitializeTemplateParts()
		{
			if (IsTemplatePartsInitialized)
			{
				return;
			}
			m_parts = new Dictionary<string, DependencyObject>();
			var attris = TemplatePartAttributes;
			if (attris != null)
			{
				foreach (var attr in attris)
				{
					var name = attr.Name;
					var dObj = FrameworkUtil.GetTemplateChild(m_parent, name);
					if (dObj != null)
					{
						m_parts.Add(name, dObj);
					}
				}
			}
		}

		public void InvalidateTemplateParts()
		{
			m_parts = null;
		}

		public bool ContainsKey(string key)
		{
			if (string.IsNullOrWhiteSpace(key) || m_parts is null)
			{
				return false;
			}
			return m_parts.ContainsKey(key);
		}

		public bool TryGetValue(string key, out DependencyObject value)
		{
			if (string.IsNullOrWhiteSpace(key) || m_parts is null)
			{
				value = null;
				return false;
			}
			return m_parts.TryGetValue(key, out value);
		}

		public bool TryGetValue<T>(string key, out T value) where T : DependencyObject
		{
			if (string.IsNullOrWhiteSpace(key) || m_parts is null)
			{
				value = null;
				return false;
			}
			if (m_parts.TryGetValue(key, out var dObj))
			{
				value = dObj as T;
				return value != null;
			}
			value = null;
			return false;
		}

		public IEnumerator<KeyValuePair<string, DependencyObject>> GetEnumerator()
		{
			if (m_parts is null)
			{
				return CollectionHelper.EnumerationEmpty<KeyValuePair<string, DependencyObject>>().GetEnumerator();
			}
			return m_parts.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
