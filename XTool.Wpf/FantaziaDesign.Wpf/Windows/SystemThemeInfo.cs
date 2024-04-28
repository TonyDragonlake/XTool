using System;
using FantaziaDesign.Core;
using FantaziaDesign.Interop;
using Microsoft.Win32;

namespace FantaziaDesign.Wpf.Windows
{
	public static class SystemThemeInfo
	{
		private const byte c_dwmCompositionMask = 0b0001;
		private const byte c_sysLightThemeMask = 0b0010;
		private const byte c_appLightThemeMask = 0b0100;
		private const byte c_colorPrevalenceMask = 0b1000;

		public static readonly InfoChangedEventManager EventManager = new InfoChangedEventManager();

		public sealed class InfoChangedEventManager
		{
			public event EventHandler SystemThemeInfoChanged;

			internal InfoChangedEventManager()
			{
			}

			public void RaiseSystemThemeInfoChangedEvent()
			{
				SystemThemeInfoChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private static byte s_info;
		public static bool IsDwmCompositionEnabled { get => BitUtil.GetBitStatus(ref s_info, c_dwmCompositionMask); private set => SetInfoValue(c_dwmCompositionMask, value); }
		public static bool IsSysUsingLightTheme { get => BitUtil.GetBitStatus(ref s_info, c_sysLightThemeMask); private set => SetInfoValue(c_sysLightThemeMask, value); }
		public static bool IsAppUsingLightTheme { get => BitUtil.GetBitStatus(ref s_info, c_appLightThemeMask); private set => SetInfoValue(c_appLightThemeMask, value); }
		public static bool IsColorPrevalenceEnabled { get => BitUtil.GetBitStatus(ref s_info, c_colorPrevalenceMask); private set => SetInfoValue(c_colorPrevalenceMask, value); }

		public static bool CanUseThemeColorizationBrush
		{
			get
			{
				var version = ApplicationEnvironment.OSVersion;
				var supported = version.Major >= 10 && version.Build >= 17763;
				return supported && (!supported || !IsColorPrevalenceEnabled);
			}
		}

		private static void SetInfoValue(byte mask, bool actived)
		{
			var oldVal = s_info;
			BitUtil.SetBitStatus(ref s_info, mask, actived);
			if (oldVal != s_info)
			{
				//EventManager.RaiseSystemThemeInfoChangedEvent(oldVal, s_info, mask);
				EventManager.RaiseSystemThemeInfoChangedEvent();
			}
		}

		public static void RefreshThemeColorization()
		{
			var personalizePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
			var dwmPath = "Software\\Microsoft\\Windows\\DWM";
			var personalizeKey = Registry.CurrentUser.OpenSubKey(personalizePath, false);
			if (personalizeKey != null)
			{
				var appvalue = personalizeKey.GetValue("AppsUseLightTheme");
				if (appvalue != null)
				{
					IsAppUsingLightTheme = Convert.ToBoolean(appvalue);
				}
				var sysvalue = personalizeKey.GetValue("SystemUsesLightTheme");
				if (sysvalue != null)
				{
					IsSysUsingLightTheme = Convert.ToBoolean(sysvalue);
				}
			}
			var dwmKey = Registry.CurrentUser.OpenSubKey(dwmPath, false);
			if (dwmKey != null)
			{
				var colorvalue = dwmKey.GetValue("ColorPrevalence");
				if (colorvalue != null)
				{
					IsColorPrevalenceEnabled = Convert.ToBoolean(colorvalue);
				}
			}
		}

		public static void RefreshDwmCompositionSettings()
		{
			var isEnabled = false;
			Dwmapi.DwmIsCompositionEnabled(ref isEnabled);
			IsDwmCompositionEnabled = isEnabled;
		}
	}

}
