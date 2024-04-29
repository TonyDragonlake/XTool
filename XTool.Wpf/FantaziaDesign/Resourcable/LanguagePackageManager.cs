using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FantaziaDesign.Core;
using FantaziaDesign.Events;
using FantaziaDesign.Model;

namespace FantaziaDesign.Resourcable
{
	public sealed class LanguagePackageManager : IResourceProvider<string, string>, INotifyResourceChanged<string, string>
	{
		public static readonly LanguagePackageManager Current = new LanguagePackageManager();

		public static string LcidToCultureName(int lcid)
		{
			return CultureInfo.GetCultureInfo(lcid).Name;
		}

		private long m_uId;
		private LanguagePackage m_currentPkg;
		private Dictionary<string, LanguagePackage> m_pkgDict;
		private NotifiableCollectionBase<string> m_pkgNames;
		private WeakEvent<ResourceChangedEventHandler<string, string>> m_resourceChangedEvent;
		//private ResourceChangedEventHandler<string, string> m_resourceChangedHandler;

		public event ResourceChangedEventHandler<string, string> ResourceChanged { add => AddHandler(value); remove => RemoveHandler(value); }

		private void RemoveHandler(ResourceChangedEventHandler<string, string> value)
		{
			//var thisHandler = m_resourceChangedHandler;
			//ResourceChangedEventHandler<string, string> tempHandler;
			//do
			//{
			//	tempHandler = thisHandler;
			//	var newHandler = (ResourceChangedEventHandler<string, string>)Delegate.Remove(tempHandler, value);
			//	thisHandler = Interlocked.CompareExchange(ref m_resourceChangedHandler, newHandler, tempHandler);
			//}
			//while (thisHandler != tempHandler);
			m_resourceChangedEvent.RemoveHandler(value);
		}

		private void AddHandler(ResourceChangedEventHandler<string, string> value)
		{
			//var thisHandler = m_resourceChangedHandler;
			//ResourceChangedEventHandler<string, string> tempHandler;
			//do
			//{
			//	tempHandler = thisHandler;
			//	var newHandler = (ResourceChangedEventHandler<string, string>)Delegate.Combine(tempHandler, value);
			//	thisHandler = Interlocked.CompareExchange(ref m_resourceChangedHandler, newHandler, tempHandler);
			//}
			//while (thisHandler != tempHandler);
			m_resourceChangedEvent.AddHandler(value);
		}

		public string ProviderName => nameof(LanguagePackageManager);

		public long UId => m_uId;

		public string CurrentPackageName => m_currentPkg?.LanguageKey ?? string.Empty;

		public int CurrentPackageIndex => m_pkgNames.IndexOf(CurrentPackageName);

		public NotifiableCollectionBase<string> PackageNames { get => m_pkgNames;}

		private LanguagePackageManager()
		{
			m_uId = SnowflakeUId.Next();
			m_pkgDict = new Dictionary<string, LanguagePackage>();
			m_pkgNames = new NotifiableList<string>();
			m_resourceChangedEvent = new WeakEvent<ResourceChangedEventHandler<string, string>>();
		}

		public LanguagePackage TryLoadLanguagePackage(Stream inputStream, IValueConverter<Stream, LanguagePackage> converter)
		{
			if (inputStream is null)
			{
				throw new ArgumentNullException(nameof(inputStream));
			}

			if (converter is null)
			{
				throw new ArgumentNullException(nameof(converter));
			}
			var pkg = converter.ConvertTo(inputStream);
			return TryAddPackage(pkg);
		}

		public LanguagePackage TryLoadLanguagePackage(string inputString, IValueConverter<string, LanguagePackage> converter)
		{
			if (string.IsNullOrWhiteSpace(inputString))
			{
				return null;
			}

			if (converter is null)
			{
				throw new ArgumentNullException(nameof(converter));
			}
			var pkg = converter.ConvertTo(inputString);
			return TryAddPackage(pkg);
		}

		private LanguagePackage TryAddPackage(LanguagePackage package)
		{
			if (package != null)
			{
				var result = package.LanguageKey;
				if (!m_pkgDict.ContainsKey(result))
				{
					m_pkgDict.Add(result, package);
					m_pkgNames.Add(result);
					return package;
				}
			}
			return null;
		}

		public bool TrySelectLanguage(string languageKey)
		{
			if (m_pkgDict.TryGetValue(languageKey, out var result))
			{
				m_currentPkg = result;
				foreach (var item in m_currentPkg)
				{
					RaiseResourceChangedEvent(new ResourceChangedEventArgs<string, string>(item.Key, item.Value));
				}
				return true;
			}
			return false;
		}

		public bool TrySelectLanguage(int lcid)
		{
			return TrySelectLanguage(LcidToCultureName(lcid));
		}

		public string ProvideResource(string txtKey, string defaultText = null)
		{
			if (m_currentPkg is null)
			{
				if (string.IsNullOrWhiteSpace(defaultText))
				{
					return string.Empty;
				}
				return defaultText;
			}
			return m_currentPkg.ProvideResource(txtKey, defaultText);
		}

		public bool CombineWith(IResourceProvider<string, string> provider)
		{
			return false;
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return m_currentPkg?.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_currentPkg?.GetEnumerator();
		}

		private void RaiseResourceChangedEvent(IResourceContainer<string, string> eventArgs)
		{
			m_resourceChangedEvent.RaiseEvent(new WeakResourceChangedEventArgs<string, string>(this, eventArgs));
		}

	}


}
