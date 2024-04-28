using System;
using System.Collections.Generic;
using System.Linq;
using FantaziaDesign.Core;
using System.Collections;
using FantaziaDesign.Graphics;

namespace FantaziaDesign.Theme
{
	public enum ThemeType
	{
		InvalidMode = -1,
		DarkMode,
		LightMode,
	}

	public class ThemeBrush : IEquatable<ThemeBrush>
	{
		private string m_key = string.Empty;
		private GenericBrush m_brush = GenericBrush.Empty;
		protected bool isSealed = false;
		public ThemeBrush()
		{
		}

		public ThemeBrush(string key, GenericBrush brush)
		{
			if (!string.IsNullOrWhiteSpace(key))
			{
				m_key = key;
			}
			m_brush = brush ?? throw new ArgumentNullException(nameof(brush));
		}

		public string Key { get => GetKey(); set => SetKey(value); }

		private string GetKey()
		{
			if (string.IsNullOrWhiteSpace(m_key))
			{
				m_key = string.Empty;
			}
			return m_key;
		}

		private void SetKey(string value)
		{
			if (string.IsNullOrWhiteSpace(m_key))
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					m_key = string.Empty;
				}
				else if (!string.Equals(m_key, value))
				{
					m_key = value;
					isSealed = true;
				}
			}
			else
			{
				if (isSealed)
				{
					return;
				}
				if (!string.IsNullOrWhiteSpace(value) && !string.Equals(m_key, value))
				{
					m_key = value;
					isSealed = true;
				}
			}
		}

		public GenericBrush Brush { get => m_brush; set => SetBrush(value); }

		private void SetBrush(GenericBrush value)
		{
			if (GenericBrush.IsNullOrEmpty(value))
			{
				if (!GenericBrush.IsEmpty(m_brush))
				{
					m_brush = GenericBrush.Empty;
				}
			}
			else
			{
				m_brush = value;
			}
		}

		public bool Equals(ThemeBrush other)
		{
			if (other is null)
			{
				return false;
			}

			return string.Equals(m_key, other.m_key);
		}

		public override int GetHashCode()
		{
			return m_key.GetHashCode();
		}

		public static ThemeBrush FromSimpleColor(string key, uint colorUInt)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return null;
			}
			return new ThemeBrush(key, new SimpleColorBrush(colorUInt));
		}
	}

	public class ThemeColor : IEquatable<ThemeColor>
	{
		private string m_key = string.Empty;
		private ColorF4 m_color = new ColorF4();
		protected bool isSealed = false;

		public ThemeColor()
		{
		}

		public ThemeColor(string key, ColorF4 color)
		{
			if (!string.IsNullOrWhiteSpace(key))
			{
				m_key = key;
			}
			m_color = color ?? throw new ArgumentNullException(nameof(color));
		}

		public string Key { get => GetKey(); set => SetKey(value); }

		private string GetKey()
		{
			if (string.IsNullOrWhiteSpace(m_key))
			{
				m_key = string.Empty;
			}
			return m_key;
		}

		private void SetKey(string value)
		{
			if (string.IsNullOrWhiteSpace(m_key))
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					m_key = string.Empty;
				}
				else if (!string.Equals(m_key, value))
				{
					m_key = value;
					isSealed = true;
				}
			}
			else
			{
				if (isSealed)
				{
					return;
				}
				if (!string.IsNullOrWhiteSpace(value) && !string.Equals(m_key, value))
				{
					m_key = value;
					isSealed = true;
				}
			}
		}

		public ColorF4 Color { get => GetColor(); set => SetColor(value); }

		private ColorF4 GetColor()
		{
			if (m_color is null)
			{
				m_color = new ColorF4();
			}
			return m_color;
		}

		private void SetColor(ColorF4 value)
		{
			if (value is null)
			{
				if (m_color is null)
				{
					m_color = new ColorF4();
				}
				else
				{
					m_color.SetColorFromFloatArgb(0, 0, 0, 0);
				}
			}
			else
			{
				if (m_color is null)
				{
					m_color = value.DeepCopy();
				}
				else
				{
					m_color.DeepCopyValueFrom(value);
				}
			}
		}

		public bool Equals(ThemeColor other)
		{
			if (other is null)
			{
				return false;
			}

			return string.Equals(m_key, other.m_key);
		}

		public override int GetHashCode()
		{
			return m_key.GetHashCode();
		}

		public static ThemeColor FromColorUInt(string key, uint colorUInt)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return null;
			}
			return new ThemeColor(key, new ColorF4(colorUInt));
		}
	}

	public class ThemeKey : IEquatable<ThemeKey>, ISealable
	{
		private string m_name = string.Empty;
		private ThemeType m_themeType = ThemeType.InvalidMode;
		private bool m_isSealed = false;
		private string m_themeKeyString;

		public static bool IsNullOrEmpty(ThemeKey themeKey)
		{
			if (themeKey is null)
			{
				return true;
			}
			if (string.IsNullOrWhiteSpace(themeKey.m_name))
			{
				return true;
			}
			if (themeKey.m_themeType == ThemeType.InvalidMode)
			{
				return true;
			}
			return false;
		}

		public static ThemeKey Empty => new ThemeKey();

		public ThemeKey(string name, ThemeType themeType)
		{
			m_name = name;
			m_themeType = themeType;
		}

		public static ThemeKey MakeSealedKey(string name, ThemeType themeType)
		{
			if (string.IsNullOrWhiteSpace(name) || themeType == ThemeType.InvalidMode)
			{
				return Empty;
			}
			return new ThemeKey(name, themeType, true);
		}

		private ThemeKey()
		{
			m_isSealed = true;
		}

		private ThemeKey(string name, ThemeType themeType, bool isSealedKey)
		{
			m_name = name;
			m_themeType = themeType;
			m_themeKeyString = $"{m_themeType}{{{m_name}}}";
			m_isSealed = isSealedKey;
		}

		public bool IsSealed => m_isSealed;

		public string Name
		{
			get => m_name;
			set
			{
				if (m_isSealed)
				{
					return;
				}
				m_name = value;
			}
		}

		public ThemeType ThemeType
		{
			get => m_themeType;
			set
			{
				if (m_isSealed)
				{
					return;
				}
				m_themeType = value;
			}
		}

		public bool Equals(ThemeKey other)
		{
			if (other is null)
			{
				return false;
			}

			return m_name == other.m_name && m_themeType == other.m_themeType;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ThemeKey);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public void Seal()
		{
			m_isSealed = true;
			m_themeKeyString = $"{m_themeType}{{{m_name}}}";
		}

		public override string ToString()
		{
			if (m_isSealed)
			{
				return m_themeKeyString;
			}
			return $"{m_themeType}{{{m_name}}}";
		}

		public void Unseal()
		{
			m_isSealed = false;
		}

		public static implicit operator string(ThemeKey themeKey)
		{
			if (IsNullOrEmpty(themeKey))
			{
				return string.Empty;
			}
			return themeKey.ToString();
		}
	}

	public class ThemeInfoEx
	{
		private ThemeKey m_themeKey;
		private HashSet<string> m_brushKeyTable = new HashSet<string>();
		private HashSet<string> m_colorKeyTable = new HashSet<string>();

		protected internal HashSet<ThemeBrush> m_themeBrushes = new HashSet<ThemeBrush>();
		protected internal HashSet<ThemeColor> m_themeColors = new HashSet<ThemeColor>();

		private int m_brushesState = 0;
		private int m_colorsState = 0;

		private ThemeBrush[] m_themeBrushesArrayCache;
		private ThemeColor[] m_themeColorsArrayCache;

		public ThemeInfoEx(string name, ThemeType themeType)
		{
			var m_name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
			m_themeKey = new ThemeKey(m_name, themeType);
		}

		public ThemeKey Key => m_themeKey;

		public string Name { get => m_themeKey.Name; set => m_themeKey.Name = value; }

		public ThemeType ThemeType { get => m_themeKey.ThemeType; set => m_themeKey.ThemeType = value; }

		public ThemeBrush[] ThemeBrushes
		{
			get
			{
				UpdateThemeBrushes();
				return m_themeBrushesArrayCache;
			}
			set => SetThemeBrushesArray(value);
		}

		public ThemeColor[] ThemeColors
		{
			get
			{
				UpdateThemeColors();
				return m_themeColorsArrayCache;
			}
			set => SetThemeColorsArray(value);
		}

		public bool HasThemeBrush => m_themeBrushesArrayCache != null && m_themeBrushesArrayCache.Length > 0;

		public bool HasThemeColor => m_themeColorsArrayCache != null && m_themeColorsArrayCache.Length > 0;

		public int Count => GetTotalElementsCount();

		public int GetTotalElementsCount()
		{
			int count = 0;
			if (HasThemeBrush)
			{
				count += m_themeBrushesArrayCache.Length;
			}
			if (HasThemeColor)
			{
				count += m_themeColorsArrayCache.Length;
			}
			return count;
		}

		public bool IsReadOnly => false;

		protected internal void SetThemeBrushesArray(ThemeBrush[] themeBrushesArray)
		{
			if (themeBrushesArray is null)
			{
				return;
			}
			if (themeBrushesArray.Length > 0)
			{
				m_themeBrushes.Clear();
				m_brushKeyTable.Clear();
				foreach (var brush in themeBrushesArray)
				{
					if (TryAddBrushKey(brush.Key))
					{
						m_themeBrushes.Add(brush);
					}
				}
				m_brushesState = 1;
				UpdateThemeBrushes();
			}
		}

		public void UpdateThemeBrushes()
		{
			if (m_brushesState == 0)
			{
				return;
			}
			if (m_brushesState == 2)
			{
				if (HasThemeBrush)
				{
					m_themeBrushes.Clear();
					m_brushKeyTable.Clear();
					int totalLen = m_themeBrushesArrayCache.Length;
					for (int i = 0; i < totalLen; i++)
					{
						ThemeBrush brush = m_themeBrushesArrayCache[i];
						if (TryAddBrushKey(brush.Key))
						{
							m_themeBrushes.Add(brush);
						}
					}
					if (m_themeBrushes.Count != totalLen)
					{
						m_themeBrushesArrayCache = m_themeBrushes.ToArray();
					}
				}
				m_brushesState = 0;
				return;
			}
			if (m_themeBrushes.Count > 0)
			{
				if (m_brushesState == 1)
				{
					m_themeBrushesArrayCache = m_themeBrushes.ToArray();
					m_brushesState = 0;
				}
			}
			else
			{
				m_themeBrushesArrayCache = null;
			}
		}

		protected internal void SetThemeColorsArray(ThemeColor[] themeColorsArray)
		{
			if (themeColorsArray is null)
			{
				return;
			}
			if (themeColorsArray.Length > 0)
			{
				m_themeColors.Clear();
				m_colorKeyTable.Clear();
				foreach (var color in themeColorsArray)
				{
					if (TryAddColorKey(color.Key))
					{
						m_themeColors.Add(color);
					}
				}
				m_colorsState = 1;
				UpdateThemeColors();
			}
		}

		public void UpdateThemeColors()
		{
			if (m_colorsState == 0)
			{
				return;
			}
			if (m_colorsState == 2)
			{
				if (HasThemeColor)
				{
					m_themeColors.Clear();
					m_colorKeyTable.Clear();
					int totalLen = m_themeColorsArrayCache.Length;
					for (int i = 0; i < totalLen; i++)
					{
						ThemeColor color = m_themeColorsArrayCache[i];
						if (TryAddColorKey(color.Key))
						{
							m_themeColors.Add(color);
						}
					}
					if (m_themeColors.Count != totalLen)
					{
						m_themeColorsArrayCache = m_themeColors.ToArray();
					}
				}
				m_colorsState = 0;
				return;
			}
			if (m_themeColors.Count > 0)
			{
				if (m_colorsState == 1)
				{
					m_themeColorsArrayCache = m_themeColors.ToArray();
					m_colorsState = 0;
				}
			}
			else
			{
				m_themeColorsArrayCache = null;
			}
		}

		public bool Equals(ThemeInfoEx other)
		{
			if (other is null)
			{
				return false;
			}

			return m_themeKey.Equals(other.m_themeKey);
		}

		public override string ToString()
		{
			return m_themeKey.ToString();
		}

		public override bool Equals(object obj)
		{
			return obj is ThemeInfoEx themeInfoEx ? Equals(themeInfoEx) : false;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public bool AddThemeBrush(ThemeBrush themeBrush)
		{
			if (themeBrush is null)
			{
				return false;
			}
			if (TryAddBrushKey(themeBrush.Key) && m_themeBrushes.Add(themeBrush))
			{
				m_brushesState = 1;
				return true;
			}
			return false;
		}

		private bool TryAddBrushKey(string key)
		{
			return !m_colorKeyTable.Contains(key) && m_brushKeyTable.Add(key);
		}

		public void ClearThemeBrush()
		{
			m_brushKeyTable.Clear();
			m_themeBrushes.Clear();
			m_brushesState = 1;
		}

		public void ClearThemeColor()
		{
			m_colorKeyTable.Clear();
			m_themeColors.Clear();
			m_colorsState = 1;
		}

		public bool ContainsThemeBrush(ThemeBrush item)
		{
			if (item is null)
			{
				return false;
			}

			return m_themeBrushes.Contains(item);
		}

		public bool RemoveThemeBrush(ThemeBrush item)
		{
			if (item is null)
			{
				return false;
			}

			return m_brushKeyTable.Remove(item.Key) && m_themeBrushes.Remove(item);
		}

		public bool AddThemeColor(ThemeColor themeColor)
		{
			if (themeColor is null)
			{
				return false;
			}
			if (TryAddColorKey(themeColor.Key) && m_themeColors.Add(themeColor))
			{
				m_colorsState = 1;
				return true;
			}
			return false;
		}

		private bool TryAddColorKey(string key)
		{
			return !m_brushKeyTable.Contains(key) && m_colorKeyTable.Add(key);
		}

		public bool ContainsThemeColor(ThemeColor item)
		{
			if (item is null)
			{
				return false;
			}

			return m_themeColors.Contains(item);
		}

		public bool RemoveThemeColor(ThemeColor item)
		{
			if (item is null)
			{
				return false;
			}

			return m_colorKeyTable.Remove(item.Key) && m_themeColors.Remove(item);
		}

		public void FreezeKey()
		{
			m_themeKey.Seal();
		}
	}

	public class ThemeContent : ICollection<ThemeBrush>, ICollection<ThemeColor>
	{
		private ThemeInfoEx m_info;

		public ThemeContent(ThemeInfoEx info)
		{
			m_info = info ?? throw new ArgumentNullException(nameof(info));
		}

		public ThemeContent(string name, ThemeType themeType)
		{
			m_info = new ThemeInfoEx(name, themeType);
		}

		public string Name => m_info.Name;

		public ThemeType ThemeType => m_info.ThemeType;

		public int Count => m_info.GetTotalElementsCount();

		public bool IsReadOnly => false;

		public ThemeInfoEx ThemeInfo => m_info;

		public void Add(ThemeBrush item)
		{
			m_info.AddThemeBrush(item);
		}

		public void Add(ThemeColor item)
		{
			m_info.AddThemeColor(item);
		}

		public void Clear()
		{
			m_info.ClearThemeBrush();
			m_info.ClearThemeColor();
		}

		public bool Contains(ThemeBrush item)
		{
			return m_info.ContainsThemeBrush(item);
		}

		public bool Contains(ThemeColor item)
		{
			return m_info.ContainsThemeColor(item);
		}

		public void CopyTo(ThemeBrush[] array, int arrayIndex)
		{
			if (array is null)
			{
				return;
			}

			m_info.m_themeBrushes.CopyTo(array, arrayIndex);
		}

		public void CopyTo(ThemeColor[] array, int arrayIndex)
		{
			if (array is null)
			{
				return;
			}

			m_info.m_themeColors.CopyTo(array, arrayIndex);
		}

		public IEnumerator<ThemeBrush> GetEnumerator()
		{
			return m_info.m_themeBrushes.GetEnumerator();
		}

		public bool Remove(ThemeBrush item)
		{
			return m_info.RemoveThemeBrush(item);
		}

		public bool Remove(ThemeColor item)
		{
			return m_info.RemoveThemeColor(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_info.m_themeBrushes.GetEnumerator();
		}

		IEnumerator<ThemeColor> IEnumerable<ThemeColor>.GetEnumerator()
		{
			return m_info.m_themeColors.GetEnumerator();
		}
	}

}
