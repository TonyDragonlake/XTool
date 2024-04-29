using System;
using FantaziaDesign.Model;
using FantaziaDesign.Resourcable;
using FantaziaDesign.Wpf.Message;

namespace XTool.Excel.WindowModels
{
	public sealed class SettingConfigurationsDialog : AcceptCancelDialog
	{
		private AppSettingData m_settingData;

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
			get => m_settingData.QueryCurrentLanguageIndex();
			set
			{
				if (m_settingData.TryUpdateCurrentLanguageIndex(value))
				{
					RaisePropertyChangedEvent(nameof(CurrentLanguageIndex));
				}
			}
		}

		public SettingConfigurationsDialog(AppSettingData settingData) : base()
		{
			if (settingData is null)
			{
				throw new ArgumentNullException(nameof(settingData));
			}
			ContentTemplateNameKey = "SettingsDialogTemplate";
			m_settingData = settingData;
		}

		public NotifiableCollectionBase<string> LanguagePackageNames => LanguagePackageManager.Current.PackageNames;

		protected override void ExecuteAcceptCommand(object parameter)
		{
			base.ExecuteAcceptCommand(parameter);
			AppSettings.AcceptSettings(m_settingData);
			AppSettings.Apply();
		}

		protected override void ExecuteCancelCommand(object parameter)
		{
			base.ExecuteCancelCommand(parameter);
		}

	}
}
