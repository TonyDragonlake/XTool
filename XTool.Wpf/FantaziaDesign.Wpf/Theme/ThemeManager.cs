using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FantaziaDesign.Core;
using FantaziaDesign.Theme;
using FantaziaDesign.Wpf.Windows;
using FantaziaDesign.Wpf.Graphics;

namespace FantaziaDesign.Wpf.Theme
{
	public static class ResourceDictionaryUtil
	{
		private static readonly Type s_resourceDictionaryType = typeof(ResourceDictionary);
		//private static readonly FieldInfo s_fieldInfo = s_resourceDictionaryType.GetField("_baseDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
		private static Func<ResourceDictionary, Hashtable> GetBaseDictionary_ResourceDictionary;

		public static bool ContainsInCurrent(this ResourceDictionary resourceDictionary, object key)
		{
			if (GetBaseDictionary_ResourceDictionary is null)
			{
				GetBaseDictionary_ResourceDictionary =
				ReflectionUtil.BindInstanceFieldGetterToDelegate<ResourceDictionary, Hashtable>("_baseDictionary", ReflectionUtil.NonPublicInstance);
			}
			var ht = GetBaseDictionary_ResourceDictionary(resourceDictionary);
			if (ht != null)
			{
				return ht.ContainsKey(key);
			}
			return false;
		}

		public static bool FindWhere(this ResourceDictionary resourceDictionary, object key, out ResourceDictionary resDictLoc)
		{
			if (resourceDictionary.ContainsInCurrent(key))
			{
				resDictLoc = resourceDictionary;
				return true;
			}
			else
			{
				resDictLoc = null;
				bool flag = false;
				int num = resourceDictionary.MergedDictionaries.Count - 1;
				while (num > -1 && !flag)
				{
					var currentRes = resourceDictionary.MergedDictionaries[num];
					if (currentRes != null)
					{
						flag = currentRes.FindWhere(key, out resDictLoc);
					}
					num--;
				}
				return flag;
			}
		}

	}

	public static class ThemeUtil
	{
		public static bool TryMakeColorDictionary(this IEnumerable<ThemeColor> themeColors, out Dictionary<string, Color> colorDictionary)
		{
			colorDictionary = null;
			if (themeColors != null)
			{
				colorDictionary = new Dictionary<string, Color>();
				foreach (var tColor in themeColors)
				{
					colorDictionary.Add(tColor.Key, tColor.Color.ToColor());
				}
				return colorDictionary.Count > 0;
			}
			return false;
		}

		public static bool TryFeedColorDictionary(this IEnumerable<ThemeColor> themeColors, Dictionary<string, Color> colorDictionary)
		{
			if (themeColors != null && colorDictionary != null)
			{
				foreach (var tColor in themeColors)
				{
					colorDictionary.Add(tColor.Key, tColor.Color.ToColor());
				}
				return colorDictionary.Count > 0;
			}
			return false;
		}

		public static bool TryMakeBrushDictionary(this IEnumerable<ThemeBrush> themeBrushes, out Dictionary<string, Brush> brushDictionary)
		{
			brushDictionary = null;
			if (themeBrushes != null)
			{
				brushDictionary = new Dictionary<string, Brush>();
				foreach (var tBrush in themeBrushes)
				{
					if (tBrush.Brush.TryMakeBrush(out Brush brush))
					{
						brushDictionary.Add(tBrush.Key, brush);
					}
					else
					{
						brushDictionary.Add(tBrush.Key, null);
					}
				}
				return brushDictionary.Count > 0;
			}
			return false;
		}

		public static bool TryFeedBrushDictionary(this IEnumerable<ThemeBrush> themeBrushes, Dictionary<string, Brush> brushDictionary)
		{
			if (themeBrushes != null && brushDictionary != null)
			{
				foreach (var tBrush in themeBrushes)
				{
					if (tBrush.Brush.TryMakeBrush(out Brush brush))
					{
						brushDictionary.Add(tBrush.Key, brush);
					}
					else
					{
						brushDictionary.Add(tBrush.Key, null);
					}
				}
				return brushDictionary.Count > 0;
			}
			return false;
		}

		public static bool TryMakeThemeFromXmlFile(string fileName, out ThemeEx theme)
		{
			theme = null;
			//if (ThemeInfoEx.TryMakeThemeInfoFromXmlFile(fileName, out ThemeInfoEx themeInfo))
			//{
			//	themeInfo.FreezeKey();
			//	theme = new ThemeEx(themeInfo);
			//	return true;
			//}
			return false;
		}

		public static bool ToXmlFile(this ThemeEx theme, string fileName)
		{
			if (theme is null || theme.IsInvalid)
			{
				return false;
			}
			var themeinfo = theme.ThemeInfo;
			//return themeinfo.ToXmlFile(fileName);
			return false;
		}
	}

	public class ThemeEx : IEquatable<ThemeEx>
	{
		private ThemeInfoEx _themeInfo;
		private Dictionary<string, Brush> _themeBrushesInstance;
		private Dictionary<string, Color> _themeColorsInstance;

		public ThemeEx(ThemeInfoEx themeInfo)
		{
			_themeInfo = themeInfo ?? throw new ArgumentNullException(nameof(themeInfo));
		}

		public bool IsInvalid => _themeInfo is null;
		public string ThemeName => IsInvalid ? "Invalid Theme" : _themeInfo.Name;
		public ThemeType ThemeType => IsInvalid ? ThemeType.InvalidMode : _themeInfo.ThemeType;
		public ThemeInfoEx ThemeInfo { get => _themeInfo; }
		public ThemeKey Key => IsInvalid ? ThemeKey.Empty : _themeInfo.Key;
		public string StringKey => Key;

		public Dictionary<string, Brush> ThemeBrushes => GetThemeBrushes();
		public Dictionary<string, Color> ThemeColors => GetThemeColors();

		public bool Equals(ThemeEx other)
		{
			if (other is null)
			{
				return false;
			}

			if (IsInvalid)
			{
				return false;
			}

			return _themeInfo.Equals(other._themeInfo);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ThemeEx);
		}

		public override int GetHashCode()
		{
			return _themeInfo.GetHashCode();
		}

		public Dictionary<string, Brush> GetThemeBrushes()
		{
			if (IsInvalid)
			{
				return null;
			}

			if (_themeBrushesInstance is null)
			{
				if (!_themeInfo.ThemeBrushes.TryMakeBrushDictionary(out _themeBrushesInstance))
				{
					_themeBrushesInstance = null;
				}
			}
			else if (_themeBrushesInstance.Count <= 0)
			{
				_themeInfo.ThemeBrushes.TryFeedBrushDictionary(_themeBrushesInstance);
			}

			return _themeBrushesInstance;
		}

		public Dictionary<string, Color> GetThemeColors()
		{
			if (IsInvalid)
			{
				return null;
			}

			if (_themeColorsInstance is null)
			{
				if (!_themeInfo.ThemeColors.TryMakeColorDictionary(out _themeColorsInstance))
				{
					_themeColorsInstance = null;
				}
			}
			else if (_themeColorsInstance.Count <= 0)
			{
				_themeInfo.ThemeColors.TryFeedColorDictionary(_themeColorsInstance);
			}

			return _themeColorsInstance;
		}

		public bool ReplaceTheme(ThemeInfoEx themeInfo, bool overrideContent = false)
		{
			if (themeInfo is null)
			{
				return false;
			}

			if (!overrideContent && _themeInfo.Equals(themeInfo))
			{
				return false;
			}

			_themeInfo = themeInfo;
			_themeBrushesInstance.Clear();
			_themeColorsInstance.Clear();
			return true;
		}

		public override string ToString()
		{
			return _themeInfo.ToString();
		}

		public static bool IsKeyEquals(ThemeEx theme, string key)
		{
			if (theme is null)
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(key))
			{
				return false;
			}
			return string.Equals(theme.StringKey, key);
		}

		public static bool IsKeyEquals(ThemeEx theme, ThemeKey key)
		{
			if (theme is null)
			{
				return false;
			}

			if (ThemeKey.IsNullOrEmpty(key))
			{
				return false;
			}
			return key.Equals(theme.Key);
		}
	}

	public class ThemeManager
	{
		private static ThemeManager s_current = new ThemeManager();

		public static ThemeManager Current => s_current;

		private static HashSet<ResourceDictionary> s_cachedResourceDict = new HashSet<ResourceDictionary>();

		private static string MakeKeyWithoutCheck(ThemeType themeType, string themeName)
		{
			return $"{themeType}{{{themeName}}}";
		}

		public static ThemeType GetThemeTypeFromThemekey(string themeKey)
		{
			if (string.IsNullOrWhiteSpace(themeKey))
			{
				throw new ArgumentException("themeKey is null or whitespace", nameof(themeKey));
			}

			if (themeKey.StartsWith("LightMode"))
			{
				return ThemeType.LightMode;
			}
			return ThemeType.DarkMode;
		}

		public static void SaveThemeFile(ThemeEx theme, string fileName)
		{
			ThemeUtil.ToXmlFile(theme, fileName);
		}

		public bool DependOnSystemTheme
		{
			get => _dependOnSystemTheme;
			set 
			{
				if (_dependOnSystemTheme != value)
				{
					_dependOnSystemTheme = value;
					if (_dependOnSystemTheme)
					{
						SystemThemeInfo.EventManager.SystemThemeInfoChanged += EventManager_SystemThemeInfoChanged;
						ApplyCurrentTheme((ThemeType)Convert.ToInt32(SystemThemeInfo.IsAppUsingLightTheme));
					}
					else
					{
						SystemThemeInfo.EventManager.SystemThemeInfoChanged -= EventManager_SystemThemeInfoChanged;
					}
				}
			}
		}

		private void EventManager_SystemThemeInfoChanged(object sender, EventArgs e)
		{
			ApplyCurrentTheme((ThemeType)Convert.ToInt32(SystemThemeInfo.IsAppUsingLightTheme));
		}


		public void SetDefaultThemeFromContent(ThemeContent themeContent, bool overrideContent = false)
		{
			if (themeContent is null)
			{
				throw new ArgumentNullException(nameof(themeContent));
			}

			var info = themeContent.ThemeInfo;
			info.FreezeKey();
			var themeType = info.ThemeType;
			var key = info.Key;
			if (_themeResource.TryGetValue(key, out ThemeEx themeEx))
			{
				if (themeEx is null)
				{
					_themeResource[key] = new ThemeEx(info);
				}
				else if (overrideContent)
				{
					themeEx.ReplaceTheme(info, overrideContent);
				}
				else
				{
					info.Key.Unseal();
				}
			}
			else
			{
				_themeResource.Add(key, new ThemeEx(info));
			}

			switch (themeType)
			{
				case ThemeType.DarkMode:
					{
						_defaultDarkModeKey = key;
					}
					break;
				case ThemeType.LightMode:
					{
						_defaultLightModeKey = key;
					}
					break;
				default:
					return;
			}
		}

		public void SetThemeBrushKeywords(IEnumerable<string> keywords)
		{
			if (keywords is null)
			{
				return;
			}
			_brushesKeywords.Clear();
			_brushesKeywords.AddRange(keywords);
		}

		public void SetThemeColorKeywords(IEnumerable<string> keywords)
		{
			if (keywords is null)
			{
				return;
			}
			_colorsKeywords.Clear();
			_colorsKeywords.AddRange(keywords);
		}

		private LinkedList<string> BuildLinkedKeywordsList()
		{
			var linked = new LinkedList<string>();
			foreach (var bkw in _brushesKeywords)
			{
				linked.AddLast(bkw);
			}

			foreach (var ckw in _colorsKeywords)
			{
				linked.AddLast(ckw);
			}
			return linked;
		}

		private ThemeEx _currentLightTheme;
		private ThemeEx _currentDarkTheme;

		private ThemeKey _defaultLightModeKey = ThemeKey.Empty;
		private ThemeKey _defaultDarkModeKey = ThemeKey.Empty;

		private ThemeType _currentThemeType = ThemeType.LightMode;

		private List<string> _brushesKeywords = new List<string>();
		private List<string> _colorsKeywords = new List<string>();

		private Dictionary<string, ThemeEx> _themeResource;
		private bool _dependOnSystemTheme;

		private ThemeManager()
		{
			_themeResource = new Dictionary<string, ThemeEx>();
		}

		public ThemeType CurrentThemeType { get => _currentThemeType; private set => _currentThemeType = value; }

		public string CurrentLightThemeStringKey => _currentLightTheme is null ? _defaultLightModeKey : _currentLightTheme.Key;

		public string CurrentDarkThemeStringKey => _currentDarkTheme is null ? _defaultDarkModeKey : _currentDarkTheme.Key;

		public ThemeKey CurrentLightThemeKey
		{
			get
			{
				if (_currentLightTheme is null)
				{
					return _defaultLightModeKey;
				}
				return _currentLightTheme.Key;
			}
		}

		public ThemeKey CurrentDarkThemeKey
		{
			get
			{
				if (_currentDarkTheme is null)
				{
					return _defaultDarkModeKey;
				}
				return _currentDarkTheme.Key;
			}
		}

		public ThemeKey CurrentThemeKey
		{
			get
			{
				switch (_currentThemeType)
				{
					case ThemeType.DarkMode:
						return CurrentDarkThemeKey;
					case ThemeType.LightMode:
						return CurrentLightThemeKey;
					default:
						return null;
				}
			}
		}

		public bool TryFindTheme(out ThemeEx theme, ThemeKey themeKey)
		{
			if (ThemeKey.IsNullOrEmpty(themeKey))
			{
				throw new ArgumentException("themeKey is null or whitespace", nameof(themeKey));
			}
			if (_themeResource.TryGetValue(themeKey, out theme))
			{
				return true;
			}
			theme = null;
			return false;
		}

		public bool TryFindThemeByStringKey(out ThemeEx theme, string themeKey)
		{
			if (string.IsNullOrWhiteSpace(themeKey))
			{
				throw new ArgumentException("themeKey is null or whitespace", nameof(themeKey));
			}
			if (_themeResource.TryGetValue(themeKey, out theme))
			{
				return true;
			}
			theme = null;
			return false;
		}

		public bool TryFindTheme(out ThemeEx theme, ThemeType themeType, string themeName = null)
		{
			if (string.IsNullOrWhiteSpace(themeName))
			{
				string key = themeType == ThemeType.DarkMode ? _defaultDarkModeKey : _defaultLightModeKey;
				if (string.IsNullOrWhiteSpace(key))
				{
					throw new ArgumentException("themeName is NullOrWhiteSpace with no alternative default theme");
				}
				if (_themeResource.TryGetValue(key, out theme))
				{
					return true;
				}
				theme = null;
				return false;
			}
			return TryFindTheme(out theme, ThemeKey.MakeSealedKey(themeName, themeType));
		}

		private bool SelectDefaultThemeInternal(ThemeType themeType, out ThemeEx selectedTheme)
		{
			var key = themeType == ThemeType.LightMode ? _defaultLightModeKey : _defaultDarkModeKey;
			selectedTheme = null;
			if (ThemeKey.IsNullOrEmpty(key))
			{
				return false;
			}
			if (themeType == ThemeType.LightMode)
			{
				if (ThemeEx.IsKeyEquals(_currentLightTheme, key) || TryFindTheme(out _currentLightTheme, key))
				{
					selectedTheme = _currentLightTheme;
					return true;
				}
			}
			else if (themeType == ThemeType.DarkMode)
			{
				if (ThemeEx.IsKeyEquals(_currentDarkTheme, key) || TryFindTheme(out _currentDarkTheme, key))
				{
					selectedTheme = _currentDarkTheme;
					return true;
				}
			}
			return false;
		}

		public bool SelectDefaultTheme(ThemeType themeType)
		{
			return SelectDefaultThemeInternal(themeType, out _);
		}

		public bool SelectDefaultTheme(ThemeType themeType, out ThemeKey themeKey)
		{
			themeKey = null;
			if (SelectDefaultThemeInternal(themeType, out ThemeEx themeEx))
			{
				themeKey = themeEx.ThemeInfo.Key;
				return true;
			}
			return false;
		}

		public bool SelectDefaultTheme(ThemeType themeType, out string themeKey)
		{
			themeKey = null;
			if (SelectDefaultThemeInternal(themeType, out ThemeEx themeEx))
			{
				themeKey = themeEx.StringKey;
				return true;
			}
			return false;
		}

		private bool SelectAsCurrentThemeInternal(ThemeType themeType, string themeName)
		{
			string key;
			if (string.IsNullOrWhiteSpace(themeName))
			{
				key = themeType == ThemeType.LightMode ? _defaultLightModeKey : _defaultDarkModeKey;
			}
			else
			{
				key = MakeKeyWithoutCheck(themeType, themeName);
			}
			if (themeType == ThemeType.LightMode)
			{
				if (_currentLightTheme != null && _currentLightTheme.StringKey == key)
				{
					return true;
				}
				else
				{
					return TryFindTheme(out _currentLightTheme, themeType, themeName);
				}
			}
			else
			{
				if (_currentDarkTheme != null && _currentDarkTheme.ToString() == key)
				{
					return true;
				}
				else
				{
					return TryFindTheme(out _currentDarkTheme, themeType, themeName);
				}
			}
		}

		public bool SelectAsCurrentTheme(ThemeType themeType, string themeName)
		{
			if (string.IsNullOrWhiteSpace(themeName))
			{
				throw new ArgumentException("Unknown themeName", nameof(themeName));
			}
			return SelectAsCurrentThemeInternal(themeType, themeName);
		}

		private bool EnsureCurrentThemeOrDefaultInternal(ThemeType themeType, out ThemeEx selectedTheme)
		{
			selectedTheme = null;
			if (themeType == ThemeType.LightMode)
			{
				if (_currentLightTheme is null)
				{
					if (!TryFindTheme(out _currentLightTheme, _defaultLightModeKey))
					{
						selectedTheme = null;
						return false;
					}
				}
				selectedTheme = _currentLightTheme;
				return true;
			}
			else if (themeType == ThemeType.DarkMode)
			{
				if (_currentDarkTheme is null)
				{
					if (!TryFindTheme(out _currentDarkTheme, _defaultDarkModeKey))
					{
						selectedTheme = null;
						return false;
					}
				}
				selectedTheme = _currentDarkTheme;
				return true;
			}
			return false;
		}

		public bool ApplyCurrentTheme(ThemeType themeType, ResourceDictionary resourceDictionary = null)
		{
			if (resourceDictionary == null)
			{
				if (Application.Current.Resources != null)
				{
					resourceDictionary = Application.Current.Resources;
				}
			}

			if (resourceDictionary == null)
			{
				return false;
			}

			if (EnsureCurrentThemeOrDefaultInternal(themeType, out ThemeEx curTheme))
			{
				var brushDict = curTheme.ThemeBrushes;
				var colorDict = curTheme.ThemeColors;
				var count = curTheme.ThemeInfo.GetTotalElementsCount();
				var keywords = BuildLinkedKeywordsList();
				var minLen = Math.Min(keywords.Count, count);
				if (minLen <= 0)
				{
					return false;
				}
				string keyword = string.Empty;
				LinkedListNode<string> currentNode;
				// Quick Find in Cache
				if (s_cachedResourceDict.Count > 0)
				{
					foreach (var res in s_cachedResourceDict)
					{
						currentNode = keywords.First;
						while (keywords.Count > 0)
						{
							if (currentNode is null)
							{
								break;
							}

							keyword = currentNode.Value;
							var nextNode = currentNode.Next;
							if (res.ContainsInCurrent(keyword))
							{
								object source = null;
								if (brushDict.TryGetValue(keyword, out Brush brush))
								{
									source = brush;
								}
								else if (colorDict.TryGetValue(keyword, out Color color))
								{
									source = color;
								}
								else
								{
									currentNode = nextNode;
									continue;
								}
								res[keyword] = source;
								keywords.Remove(currentNode);
							}

							currentNode = nextNode;
						}
					}
					if (keywords.Count <= 0)
					{
						_currentThemeType = themeType;
						return true;
					}
				}

				currentNode = keywords.First;
				while (keywords.Count > 0)
				{
					if (currentNode is null)
					{
						break;
					}

					keyword = currentNode.Value;
					var nextNode = currentNode.Next;
					if (resourceDictionary.FindWhere(keyword, out ResourceDictionary resDict))
					{
						object source = null;
						if (brushDict.TryGetValue(keyword, out Brush brush))
						{
							source = brush;
						}
						else if (colorDict.TryGetValue(keyword, out Color color))
						{
							source = color;
						}
						else
						{
							currentNode = nextNode;
							continue;
						}
						resDict[keyword] = source;
						keywords.Remove(currentNode);
						s_cachedResourceDict.Add(resDict);
					}
					currentNode = nextNode;
				}

				if (keywords.Count <= 0)
				{
					_currentThemeType = themeType;
					return true;
				}

				//HashSet<int> notFoundIndex = new HashSet<int>();
				//for (int i = 0; i < minLen; i++)
				//{
				//	notFoundIndex.Add(i);
				//	keyword = Theme.ThemeBrushesLevelDef[i];
				//	if (resourceDictionary.FindWhere(keyword, out ResourceDictionary resDict))
				//	{
				//		s_cachedResourceDict.Add(resDict);
				//	}
				//}
				//if (s_cachedResourceDict.Count > 0)
				//{
				//	foreach (var res in s_cachedResourceDict)
				//	{
				//		int[] loopIndex = notFoundIndex.ToArray();
				//		if (loopIndex.Length > 0)
				//		{
				//			foreach (var index in loopIndex)
				//			{
				//				keyword = Theme.ThemeBrushesLevelDef[index];
				//				if (res.ContainsInCurrent(keyword))
				//				{
				//					object source = null;
				//					if (brushDict.TryGetValue(keyword, out Brush brush))
				//					{
				//						source = brush;
				//					}
				//					else if (colorDict.TryGetValue(keyword, out Color color))
				//					{
				//						source = color;
				//					}
				//					else
				//					{
				//						continue;
				//					}
				//					res[keyword] = source;
				//					notFoundIndex.Remove(index);
				//				}
				//			}
				//		}
				//		else
				//		{
				//			break;
				//		}
				//	}
				//	if (notFoundIndex.Count <= 0)
				//	{
				//		_currentThemeType = themeType;
				//		return true;
				//	}
				//}
			}
			return false;
		}

		public bool ApplyTheme(ThemeKey themeKey, ResourceDictionary resourceDictionary = null)
		{
			if (ThemeKey.IsNullOrEmpty(themeKey))
			{
				return false;
			}
			return SelectAsCurrentThemeInternal(themeKey.ThemeType, themeKey.Name) 
				&& ApplyCurrentTheme(themeKey.ThemeType, resourceDictionary);
		}

		public bool AddTheme(ThemeEx theme)
		{
			if (theme != null && !_themeResource.ContainsKey(theme.ToString()))
			{
				_themeResource.Add(theme.StringKey, theme);
				return true;
			}
			return false;
		}

		public IEnumerable<ThemeEx> ThemeCollection => _themeResource is null ? null : _themeResource.Values;

		public bool LoadThemeFromFile(string fileName, out ThemeKey themeKey)
		{
			if (ThemeUtil.TryMakeThemeFromXmlFile(fileName, out ThemeEx theme))
			{
				themeKey = theme.Key;
				if (!_themeResource.ContainsKey(themeKey))
				{
					_themeResource.Add(themeKey, theme);
					return true;
				}
				return false;
			}
			themeKey = ThemeKey.Empty;
			return false;
		}

		public void Clear()
		{
			_themeResource.Clear();
			s_cachedResourceDict.Clear();
			_currentThemeType = ThemeType.LightMode;
		}

	}
}
