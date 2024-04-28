using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using FantaziaDesign.Core;

namespace FantaziaDesign.Resourcable
{
	public class LanguagePackage : IEquatable<LanguagePackage>, IResourceProvider<string, string>
	{
		public static readonly string ClassName = "langpkg";
		public static readonly string ItemName = "text";
		public static readonly string KeyProperty = "langKey";
		public static readonly string ItemKeyProperty = "txtKey";

		public sealed class Creator : IDisposable
		{
			private LanguagePackage m_pkg;

			public Creator()
			{
				m_pkg = new LanguagePackage();
			}

			public Creator(string languageKey, Dictionary<StringRef, StringRef> textDictionary)
			{
				m_pkg = new LanguagePackage();
				m_pkg.m_langKey = languageKey;
				m_pkg.m_textDict = textDictionary;
			}

			public string LanguageKey
			{
				get { ThrowIfDisposed(); return m_pkg.LanguageKey; }
				set { ThrowIfDisposed(); m_pkg.m_langKey = value; }
			}

			public Dictionary<StringRef, StringRef> TextDictionary
			{
				get { ThrowIfDisposed(); return m_pkg.m_textDict; }
				set { ThrowIfDisposed(); m_pkg.m_textDict = value; }
			}

			public LanguagePackage PackageInstance
			{
				get
				{
					ThrowIfDisposed();
					m_pkg.m_uId = SnowflakeUId.Next();
					return m_pkg;
				}
			}

			private void ThrowIfDisposed()
			{
				if (m_pkg is null)
				{
					throw new ObjectDisposedException("Cannot operate Creator after disposed");
				}
			}

			public void Dispose()
			{
				m_pkg = null;
			}
		}

		private long m_uId;
		private string m_langKey;
		private CultureInfo m_cultureInfo;
		private Dictionary<StringRef, StringRef> m_textDict;

		public string LanguageKey => m_langKey;

		public CultureInfo Culture
		{
			get
			{
				if (m_cultureInfo is null)
				{
					m_cultureInfo = CultureInfo.GetCultureInfo(m_langKey);
				}
				return m_cultureInfo;
			}
		}

		public long UId => m_uId;

		string IResourceProvider<string, string>.ProviderName => m_langKey;

		private LanguagePackage()
		{
		}

		public bool Equals(LanguagePackage other)
		{
			if (other is null)
			{
				return false;
			}
			return string.Equals(m_langKey, other.m_langKey, StringComparison.OrdinalIgnoreCase);
		}

		public string ProvideResource(string txtKey, string defaultText = null)
		{
			if (!string.IsNullOrWhiteSpace(txtKey) && m_textDict != null)
			{
				if (m_textDict.TryGetValue(txtKey, out StringRef result))
				{
					return result;
				}
			}
			if (string.IsNullOrWhiteSpace(defaultText))
			{
				return string.Empty;
			}
			return defaultText;
		}

		public bool CombineWith(IResourceProvider<string, string> provider)
		{
			return false;
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return Enumeration().GetEnumerator();
		}

		private IEnumerable<KeyValuePair<string, string>> Enumeration()
		{
			foreach (var item in m_textDict)
			{
				yield return new KeyValuePair<string, string>(item.Key, item.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
