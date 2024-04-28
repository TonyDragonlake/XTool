using System;
using FantaziaDesign.Core;
using FantaziaDesign.Model;
using FantaziaDesign.Resourcable;
using FantaziaDesign.Theme;
using FantaziaDesign.Wpf.Message;
using FantaziaDesign.Wpf.Theme;

namespace XTool.Excel.WindowModels
{
	public sealed class SettingConfigurationsDialog : AcceptCancelDialog
	{
		public sealed class SettingData : IDeepCopyable<SettingData>
		{
			private bool m_isFollowingSystemTheme;
			private bool m_isUsingLightTheme = true;
			private int m_currentLanguageIndex;

			public bool IsFollowingSystemTheme { get => m_isFollowingSystemTheme; set => m_isFollowingSystemTheme = value; }
			public bool IsUsingLightTheme { get => m_isUsingLightTheme; set => m_isUsingLightTheme = value; }
			public int CurrentLanguageIndex { get => m_currentLanguageIndex; set => m_currentLanguageIndex = value; }

			public object Clone()
			{
				return DeepCopy();
			}

			public SettingData DeepCopy()
			{
				return new SettingData()
				{
					m_isFollowingSystemTheme = this.m_isFollowingSystemTheme,
					m_isUsingLightTheme = this.m_isUsingLightTheme,
					m_currentLanguageIndex = this.m_currentLanguageIndex,
				};
			}

			public void DeepCopyValueFrom(SettingData obj)
			{
				if (obj is null)
				{
					return;
				}

				m_isFollowingSystemTheme = obj.m_isFollowingSystemTheme;
				m_isUsingLightTheme = obj.m_isUsingLightTheme;
				m_currentLanguageIndex = obj.m_currentLanguageIndex;
			}

			public static SettingData FromAppSettings()
			{
				return new SettingData()
				{
					m_isFollowingSystemTheme = ThemeManager.Current.DependOnSystemTheme,
					m_isUsingLightTheme = ThemeManager.Current.CurrentThemeType == ThemeType.LightMode,
					m_currentLanguageIndex = LanguagePackageManager.Current.CurrentPackageIndex,
				};
			}
		}

		private SettingData m_settingData;

		public bool IsFollowingSystemTheme
		{
			get => m_settingData.IsFollowingSystemTheme;
			set
			{
				if (m_settingData.IsFollowingSystemTheme != value)
				{
					m_settingData.IsFollowingSystemTheme = value;
					RaisePropertyChangedEvent(nameof(IsFollowingSystemTheme));
				}
			}
		}


		public bool IsUsingLightTheme
		{
			get => m_settingData.IsUsingLightTheme;
			set
			{
				if (m_settingData.IsUsingLightTheme != value)
				{
					m_settingData.IsUsingLightTheme = value;
					RaisePropertyChangedEvent(nameof(IsUsingLightTheme));
				}
			}
		}

		public int CurrentLanguageIndex
		{
			get => m_settingData.CurrentLanguageIndex;
			set
			{
				if (m_settingData.CurrentLanguageIndex != value)
				{
					m_settingData.CurrentLanguageIndex = value;
					RaisePropertyChangedEvent(nameof(CurrentLanguageIndex));
				}
			}
		}

		private void ApplyThemeChange()
		{
			var isFollowSystemTheme = m_settingData.IsFollowingSystemTheme;
			ThemeManager.Current.DependOnSystemTheme = isFollowSystemTheme;
			if (!isFollowSystemTheme)
			{
				bool isLightTheme = ThemeManager.Current.CurrentThemeType == ThemeType.LightMode;
				if (m_settingData.IsUsingLightTheme != isLightTheme)
				{
					ThemeManager.Current.ApplyCurrentTheme(m_settingData.IsUsingLightTheme ? ThemeType.LightMode : ThemeType.DarkMode);
				}
			}
		}

		private void ApplyLanguageChange()
		{
			var index = m_settingData.CurrentLanguageIndex;
			var pkgNames = LanguagePackageManager.Current.PackageNames;
			var currentPkgName = LanguagePackageManager.Current.CurrentPackageName;
			var count = pkgNames.Count;
			if (count > 0 && count > index && index >= 0)
			{
				var nowPkgName = pkgNames[index];
				if (!string.Equals(currentPkgName, nowPkgName, StringComparison.OrdinalIgnoreCase))
				{
					LanguagePackageManager.Current.TrySelectLanguage(nowPkgName);
				}
			}
		}

		public SettingConfigurationsDialog() : this(SettingData.FromAppSettings())
		{
		}

		public SettingConfigurationsDialog(SettingData settingData) : base()
		{
			if (settingData is null)
			{
				settingData = new SettingData();
			}
			ContentTemplateNameKey = "SettingsDialogTemplate";
			m_settingData = settingData;
		}

		public NotifiableCollectionBase<string> LanguagePackageNames => LanguagePackageManager.Current.PackageNames;

		protected override void ExecuteAcceptCommand(object parameter)
		{
			base.ExecuteAcceptCommand(parameter);
			ApplyThemeChange();
			ApplyLanguageChange();
		}

		protected override void ExecuteCancelCommand(object parameter)
		{
			base.ExecuteCancelCommand(parameter);
		}

	}
}
