using System;
using System.Collections.Generic;
using FantaziaDesign.Resourcable;

namespace XTool.Excel.ResourceModels
{
	public class UITextResource : ResourceNotifier<string, string>, IDisposable
	{
		private class __Factory : IResourceNotifierFactory<string, string>
		{
			private static readonly Dictionary<string, UITextResource> s_txtResDict = new Dictionary<string, UITextResource>();

			public string FactoryName => nameof(UITextResource);

			public IResourceNotifier<string, string> CreateFromKey(string key)
			{
				if (!s_txtResDict.TryGetValue(key, out var res))
				{
					res = new UITextResource(key);
					s_txtResDict.Add(key, res);
				}
				return res;
			}
		}

		public static readonly IResourceNotifierFactory<string, string> Factory = new __Factory();

		private bool m_isDisposed;

		private UITextResource(string key) : base(key)
		{
			LanguagePackageManager.Current.ResourceChanged += OnResourceChanged;
		}

		private void OnResourceChanged(object sender, IResourceContainer<string, string> args)
		{
			if (string.Equals(args.Key, Key))
			{
				RaisePropertyChangedEvent(nameof(Resource));
			}
		}

		public override string Resource => LanguagePackageManager.Current.ProvideResource(Key, string.Empty);

		public override void SetResource(string resource)
		{
		}

		public void Dispose()
		{
			if (m_isDisposed)
			{
				return;
			}
			LanguagePackageManager.Current.ResourceChanged -= OnResourceChanged;
			m_isDisposed = true;
		}
	}

}
