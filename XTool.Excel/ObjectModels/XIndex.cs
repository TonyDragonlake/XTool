using System;
using System.Collections.Generic;
using System.Text;
using FantaziaDesign.Core;

namespace XTool.Excel.ObjectModels
{
	public enum XIndexFormat
	{
		Digital,
		Letter
	}

	public struct XIndex : IEquatable<XIndex>, IDeepCopyable<XIndex>
	{
		private int m_index;
		private bool m_isAbsoluted;

		public int Index { get => m_index; set => m_index = value; }
		public bool IsAbsoluted { get => m_isAbsoluted; set => m_isAbsoluted = value; }

		public XIndex(string xlRef)
		{
			if (xlRef == null) throw new ArgumentNullException(nameof(xlRef));
			ParseXIndex(ref xlRef, out m_index, out m_isAbsoluted);
		}

		public XIndex(int index, bool isAbsoluted = false)
		{
			m_index = index;
			m_isAbsoluted = isAbsoluted;
		}

		public void SetXIndex(string xlRef)
		{
			if (xlRef == null) throw new ArgumentNullException(nameof(xlRef));
			ParseXIndex(ref xlRef, out m_index, out m_isAbsoluted);
		}

		private static void ParseXIndex(ref string xlRef, out int result, out bool isAbsoluted)
		{
			bool flag = false;
			isAbsoluted = false;
			result = 0;
			var len = xlRef.Length;
			for (int index = 0; index < len; index++)
			{
				var item = xlRef[index];
				if (char.IsWhiteSpace(item))
				{
					continue;
				}
				if (!flag)
				{
					isAbsoluted = item == '$';
					flag = true;
					if (isAbsoluted)
					{
						continue;
					}
				}
				bool success = false;
				if (item >= '0' && item <= '9')
				{
					success = ParseIndexInDigitalMode(ref xlRef, ref index, out result);
				}
				else if ((item >= 'a' && item <= 'z') || (item >= 'A' && item <= 'Z'))
				{
					success = ParseIndexInLetterMode(ref xlRef, ref index, out result);
				}
				if (!success)
				{
					throw new ArgumentException($"{nameof(xlRef)} parse failed", nameof(xlRef));
				}
				break;
			}
		}

		private static bool ParseIndexInDigitalMode(ref string xlRef, ref int index, out int result)
		{
			var values = new List<int>(xlRef.Length - index);
			do
			{
				var item = xlRef[index];
				if (char.IsWhiteSpace(item))
				{
					index++;
					continue;
				}
				if (item < '0' || item > '9')
				{
					result = 0;
					return false;
				}
				values.Add(item - '0');
				index++;
			} while (index < xlRef.Length);

			var exp = values.Count - 1;
			var baseVal = '9' - '0' + 1;
			result = 0;
			foreach (var item in values)
			{
				result += Convert.ToInt32(item * Math.Pow(baseVal, exp));
				exp--;
			}
			return true;
		}

		private static bool ParseIndexInLetterMode(ref string xlRef, ref int index, out int result)
		{
			var values = new List<int>(xlRef.Length - index);
			do
			{
				var item = xlRef[index];
				if (char.IsWhiteSpace(item))
				{
					index++;
					continue;
				}
				item = char.ToUpper(item);
				if (item < 'A' || item > 'Z')
				{
					result = 0;
					return false;
				}
				values.Add(item - 'A' + 1);
				index++;
			} while (index < xlRef.Length);

			var exp = values.Count - 1;
			var baseVal = 'Z' - 'A' + 1;
			result = 0;
			foreach (var item in values)
			{
				result += Convert.ToInt32(item * Math.Pow(baseVal, exp));
				exp--;
			}
			return true;
		}

		private static string IndexToStringInLetterMode(int index, bool isAbsoluted)
		{
			var baseVal = 'Z' - 'A' + 1;
			var builder = new StringBuilder();
			while (index > 0)
			{
				// number = xN * 26 ^ N + ... + x1 * 26 ^ 1 + x0 * 26 ^ 0, where x >= 0 && x < 26
				// number / 26 - x0 * 26 ^ -1 = xN * 26 ^ (N-1) + ... + x1 * 26 ^ 0
				// number % 26 = x0 * 26 ^ 0 = x0

				index--;
				var div = index / baseVal;
				var mod = index % baseVal;
				var str = Convert.ToChar(mod + 'A');
				builder.Insert(0, str);
				index = div;
			}
			if (isAbsoluted && builder.Length > 0)
			{
				builder.Insert(0, '$');
			}
			return builder.ToString();
		}

		public int GetRelativeIndex(int absoluteIndex)
		{
			if (m_isAbsoluted)
			{
				return m_index - absoluteIndex + 1;
			}
			return m_index;
		}


		public override bool Equals(object obj)
		{
			if (obj is XIndex xIndex)
			{
				return Equals(xIndex);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_index.GetHashCode() ^ m_isAbsoluted.GetHashCode();
		}

		public override string ToString()
		{
			if (m_isAbsoluted)
			{
				return $"${m_index}";
			}
			return $"{m_index}";
		}

		public string ToString(XIndexFormat format)
		{
			if (format == XIndexFormat.Digital)
			{
				return ToString();
			}
			else
			{
				return IndexToStringInLetterMode(m_index, m_isAbsoluted);
			}
		}

		public bool Equals(XIndex other)
		{
			return m_index == other.m_index && m_isAbsoluted == other.m_isAbsoluted;
		}

		public void ZeroIndex()
		{
			m_index = 0;
			m_isAbsoluted = false;
		}

		public XIndex DeepCopy()
		{
			return new XIndex(m_index, m_isAbsoluted);
		}

		public void DeepCopyValueFrom(XIndex obj)
		{
			m_index = obj.m_index;
			m_isAbsoluted = obj.m_isAbsoluted;
		}

		public object Clone()
		{
			return DeepCopy();
		}
	}
}
