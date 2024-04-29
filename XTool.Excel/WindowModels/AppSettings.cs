using FantaziaDesign.Core;
using FantaziaDesign.Resourcable;
using FantaziaDesign.Theme;
using FantaziaDesign.Wpf.Theme;
using System;
using System.IO;
using System.Linq;

namespace XTool.Excel.WindowModels
{
	public class AppSettingData : IDeepCopyable<AppSettingData>, IEquatable<AppSettingData>
	{
		private bool m_isFollowingSystemTheme;
		private bool m_isUsingLightTheme = true;
		private string m_currentLanguage;

		public bool IsFollowingSystemTheme { get => m_isFollowingSystemTheme; set => m_isFollowingSystemTheme = value; }
		public bool IsUsingLightTheme { get => m_isUsingLightTheme; set => m_isUsingLightTheme = value; }
		public string CurrentLanguage { get => m_currentLanguage; set => m_currentLanguage = value; }

		public object Clone()
		{
			return DeepCopy();
		}

		public AppSettingData DeepCopy()
		{
			return new AppSettingData()
			{
				m_isFollowingSystemTheme = m_isFollowingSystemTheme,
				m_isUsingLightTheme = m_isUsingLightTheme,
				m_currentLanguage = m_currentLanguage,
			};
		}

		public void DeepCopyValueFrom(AppSettingData obj)
		{
			if (obj is null)
			{
				return;
			}

			m_isFollowingSystemTheme = obj.m_isFollowingSystemTheme;
			m_isUsingLightTheme = obj.m_isUsingLightTheme;
			m_currentLanguage = obj.m_currentLanguage;
		}

		public bool Equals(AppSettingData other)
		{
			if (other is null)
			{
				return false;
			}

			return m_isFollowingSystemTheme == other.m_isFollowingSystemTheme
				&& m_isUsingLightTheme == other.m_isUsingLightTheme
				&& string.Equals(m_currentLanguage, other.m_currentLanguage, StringComparison.OrdinalIgnoreCase);
		}

		public int QueryCurrentLanguageIndex()
		{
			return LanguagePackageManager.Current.PackageNames.IndexOf(m_currentLanguage);
		}

		public bool TryUpdateCurrentLanguageIndex(int index)
		{
			var pkgNames = LanguagePackageManager.Current.PackageNames;
			if (0 <= index && index < pkgNames.Count)
			{
				var selectedName = pkgNames[index];
				if (!string.Equals(m_currentLanguage, selectedName, System.StringComparison.OrdinalIgnoreCase))
				{
					m_currentLanguage = selectedName;
					return true;
				}
			}
			return false;
		}

	}


	public static class AppSettings
	{
		private static bool s_isInitialized;

		private static ThemeContent defaultLightThemeDict
			= new ThemeContent("XTool#DefaultLightModeTheme", ThemeType.LightMode)
			{
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Neutral.Primary.Normal"             ,0xFF000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Neutral.Secondary.Normal"           ,0xE4000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Neutral.Tertiary.Normal"            ,0x9E000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.NeutralOnAccent.Primary.Normal"     ,0xFFFFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.NeutralOnAccent.Secondary.Normal"   ,0xE4FFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.NeutralOnAccent.Tertiary.Normal"    ,0xB3FFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Primary.Normal"           ,0xB3FFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Secondary.Normal"         ,0x80F9F9F9) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Tertiary.Normal"          ,0x4DF9F9F9) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Neutral.Primary.Normal"         ,0x0F000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Neutral.Secondary.Normal"       ,0x29000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Neutral.Tertiary.Normal"        ,0x37000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Primary.Hover"            ,0xB3CCCCCC) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Secondary.Hover"          ,0x80C7C7C7) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Tertiary.Hover"           ,0x4DC7C7C7) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Primary.Actived"          ,0xB3999999) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Secondary.Actived"        ,0x80959595) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Tertiary.Actived"         ,0x4D959595) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Accent.Primary.Normal"              ,0xFF001968) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.AccentOnAccent.Primary.Normal"      ,0xFFE0F9FF) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Accent.Primary.Normal"            ,0xFFE0F9FF) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Accent.Primary.Normal"          ,0x3398ECFE) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Accent.Primary.Hover"             ,0xFFcae0e6) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Accent.Primary.Actived"           ,0xFF9daeb3) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.AccentOnAccent.Primary.Normal"    ,0xFF005EB7) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.AccentOnAccent.Primary.Normal"  ,0x33005EB7) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.AccentOnAccent.Primary.Hover"     ,0xFF0055a5) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.AccentOnAccent.Primary.Actived"   ,0xFF004280) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContent.Fill.Success"                      ,0xFF0F7B0F) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContent.Fill.Caution"                      ,0xFF9D5D00) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContent.Fill.Critical"                     ,0xFFC42B1C) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Attention"                  ,0xFFF7F7F7) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Success"                    ,0xFFDFF6DD) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Caution"                    ,0xFFFFF4CE) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Critical"                   ,0xFFFDE7E9) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Solid"                              ,0xFFF3F3F3) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Layer"                              ,0x80FFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Mask"                               ,0x4D000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Highlight"                          ,0xFFFFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Disabled"                           ,0xFF808080) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Disabled"                         ,0x4DF9F9F9) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Disabled"                       ,0x5C000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Selection.Fill.Primary"                          ,0x7F005EB7) ,
				ThemeColor.FromColorUInt("DColor.Content.Fill.Accent.Primary.Normal"                ,0xFF001968)
			};
		
		private static ThemeContent defaultDarkThemeDict
			= new ThemeContent("XTool#DefaultDarkModeTheme", ThemeType.DarkMode)
			{
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Neutral.Primary.Normal"             ,0xFFFFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Neutral.Secondary.Normal"           ,0xE4FFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Neutral.Tertiary.Normal"            ,0x9EFFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.NeutralOnAccent.Primary.Normal"     ,0xFF000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.NeutralOnAccent.Secondary.Normal"   ,0xE4000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.NeutralOnAccent.Tertiary.Normal"    ,0xB3000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Primary.Normal"           ,0xB3000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Secondary.Normal"         ,0x80060606) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Tertiary.Normal"          ,0x4D060606) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Neutral.Primary.Normal"         ,0x0FFFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Neutral.Secondary.Normal"       ,0x29FFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Neutral.Tertiary.Normal"        ,0x37FFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Primary.Hover"            ,0xB3333333) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Secondary.Hover"          ,0x80383838) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Tertiary.Hover"           ,0x4D383838) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Primary.Actived"          ,0xB3666666) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Secondary.Actived"        ,0x806A6A6A) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Neutral.Tertiary.Actived"         ,0x4D6A6A6A) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Accent.Primary.Normal"              ,0xFFFFE697) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.AccentOnAccent.Primary.Normal"      ,0xFF1F0600) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Accent.Primary.Normal"            ,0xFF1F0600) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Accent.Primary.Normal"          ,0x33671301) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Accent.Primary.Hover"             ,0xFF351F19) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Accent.Primary.Actived"           ,0xFF62514C) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.AccentOnAccent.Primary.Normal"    ,0xFFFFA148) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.AccentOnAccent.Primary.Normal"  ,0x33FFA148) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.AccentOnAccent.Primary.Hover"     ,0xFFFFAA5A) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.AccentOnAccent.Primary.Actived"   ,0xFFFFBD7F) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContent.Fill.Success"                      ,0xFF0F7B0F) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContent.Fill.Caution"                      ,0xFF9D5D00) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContent.Fill.Critical"                     ,0xFFC42B1C) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Attention"                  ,0xFFF7F7F7) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Success"                    ,0xFFDFF6DD) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Caution"                    ,0xFFFFF4CE) ,
				ThemeBrush.FromSimpleColor("DBrush.SignalContainer.Fill.Critical"                   ,0xFFFDE7E9) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Solid"                              ,0xFF0C0C0C) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Layer"                              ,0x80000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Mask"                               ,0x4DFFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Surface.Fill.Highlight"                          ,0xFF000000) ,
				ThemeBrush.FromSimpleColor("DBrush.Content.Fill.Disabled"                           ,0xFF808080) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Fill.Disabled"                         ,0x4D060606) ,
				ThemeBrush.FromSimpleColor("DBrush.Container.Stroke.Disabled"                       ,0x5CFFFFFF) ,
				ThemeBrush.FromSimpleColor("DBrush.Selection.Fill.Primary"                          ,0x7FFFA148) ,
				ThemeColor.FromColorUInt("DColor.Content.Fill.Accent.Primary.Normal"                ,0xFFFFE697)
			};

		private static AppSettingData s_settingData;
		private static AppSettingData s_settingDataFileCache;

		private static YamlStreamToTypedObjectConverter<AppSettingData> s_converter = new YamlStreamToTypedObjectConverter<AppSettingData>();

		public static AppSettingData SettingData => s_settingData?.DeepCopy();

		public static AppSettingData SettingDataCache => s_settingData;

		private const string SettingsFileName = "application.settings.yaml";

		public static string SettingsFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFileName);

		public static void Initialize()
		{
			if (s_isInitialized)
			{
				return;
			}

			// theme setting
			ThemeManager.Current.SetDefaultThemeFromContent(defaultLightThemeDict);
			var themeInfo = defaultLightThemeDict.ThemeInfo;
			ThemeManager.Current.SetThemeBrushKeywords(themeInfo.ThemeBrushes?.Select(brush => brush.Key));
			ThemeManager.Current.SetThemeColorKeywords(themeInfo.ThemeColors?.Select(color => color.Key));
			ThemeManager.Current.SetDefaultThemeFromContent(defaultDarkThemeDict);

			// language setting
			var langPkgMgr = LanguagePackageManager.Current;
			langPkgMgr.TryLoadLanguagePackage(
				XResourceManager.GetAddInUIResource("XTool.Excel.Assets.Language.lang_en-US.xml"),
				XResourceManager.LanguagePackageStringParser);
			langPkgMgr.TryLoadLanguagePackage(
				XResourceManager.GetAddInUIResource("XTool.Excel.Assets.Language.lang_zh-CN.xml"),
				XResourceManager.LanguagePackageStringParser);
			// read setting object
			s_settingData = ReadAppSettingData();
			if (s_settingData is null)
			{
				s_settingData = new AppSettingData();
			}
			else
			{
				s_settingDataFileCache = s_settingData.DeepCopy();
			}
			s_isInitialized = true;
		}

		public static void AcceptSettings(AppSettingData settingData)
		{
			s_settingData.DeepCopyValueFrom(settingData);
		}

		public static void Apply()
		{
			if (s_isInitialized)
			{
				var isFollowSystemTheme = s_settingData.IsFollowingSystemTheme;
				ThemeManager.Current.DependOnSystemTheme = isFollowSystemTheme;
				if (!isFollowSystemTheme)
				{
					bool isLightTheme = ThemeManager.Current.CurrentThemeType == ThemeType.LightMode;
					if (s_settingData.IsUsingLightTheme != isLightTheme)
					{
						ThemeManager.Current.ApplyCurrentTheme(s_settingData.IsUsingLightTheme ? ThemeType.LightMode : ThemeType.DarkMode);
					}
				}
				var nowPkgName = s_settingData.CurrentLanguage;
				var currentPkgName = LanguagePackageManager.Current.CurrentPackageName;
				if (!string.Equals(currentPkgName, nowPkgName, StringComparison.OrdinalIgnoreCase))
				{
					LanguagePackageManager.Current.TrySelectLanguage(nowPkgName);
				}
			}
		}

		private static AppSettingData ReadAppSettingData()
		{
			var filePath = SettingsFilePath;
			if (File.Exists(filePath))
			{
				MemoryStream memoryStream = new MemoryStream();
				using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					fileStream.CopyTo(memoryStream);
				}
				if (memoryStream.Length > 0)
				{
					memoryStream.Seek(0, SeekOrigin.Begin);
					return s_converter.ConvertTo(memoryStream);
				}
			}
			return null;
		}

		public static void SyncToFile()
		{
			if (s_settingDataFileCache is null || !s_settingDataFileCache.Equals(s_settingData))
			{
				s_settingDataFileCache = s_settingData.DeepCopy();
				var m_fileInfo = new FileInfo(SettingsFilePath);
				using (var fileStream = m_fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
				{
					using (var tempStream = s_converter.ConvertBack(s_settingDataFileCache))
					{
						fileStream.Seek(0, SeekOrigin.Begin);
						fileStream.SetLength(0);
						tempStream.CopyTo(fileStream);
						fileStream.Flush(true);
					}
				}
			}
		}
	}
}
