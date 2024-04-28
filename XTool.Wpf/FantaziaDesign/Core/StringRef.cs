using System;
using System.Collections;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public sealed class StringRef :
		TypedRef<string>,
		IEquatable<StringRef>,
		IEquatable<string>,
		IComparable<StringRef>,
		IComparable<string>,
		IComparable,
		IConvertible,
		ICloneable,
		IEnumerable<char>,
		IEnumerable,
		IFormattable
	{
		public static readonly StringRef Empty = new StringRef(string.Empty);

		public static readonly StringRef NullString = new StringRef(null);

		public static StringRef Make(string innerString)
		{
			if (innerString is null)
			{
				return NullString;
			}

			if (0 >= innerString.Length)
			{
				return Empty;
			}

			return new StringRef(innerString);
		}

		//private string m_item;

		public bool IsStringNullOrWhitespace => string.IsNullOrWhiteSpace(m_item);

		public bool IsStringNullOrEmpty => string.IsNullOrEmpty(m_item);

		public bool IsStringNull => m_item == null;

		private StringRef(string innerStr) : base(innerStr) { }

		private static bool TryAsString(object obj, out string str)
		{
			var convertible = obj as IConvertible;
			if (convertible != null)
			{
				str = convertible.ToString(null);
				return true;
			}
			str = null;
			return false;
		}

		#region IEquatable
		public bool Equals(StringRef other)
		{
			if (other is null)
			{
				return false;
			}
			return string.Equals(m_item, other.m_item);
		}

		public override bool Equals(string other)
		{
			return string.Equals(m_item, other);
		}

		public override bool Equals(object obj)
		{
			if (obj is null)
			{
				return false;
			}
			if (TryAsString(obj, out var str))
			{
				return string.Equals(m_item, str);
			}
			return false;
		}
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new StringRef(m_item);
		}
		#endregion

		#region IConvertible
		TypeCode IConvertible.GetTypeCode()
		{
			return m_item.GetTypeCode();
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToBoolean(provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToChar(provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToSByte(provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToByte(provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToInt16(provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToUInt16(provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToInt32(provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToUInt32(provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToInt64(provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToUInt64(provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToSingle(provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToDouble(provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToDecimal(provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToDateTime(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return m_item;
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToType(conversionType, provider);
		}
		#endregion

		#region IComparable
		public int CompareTo(string other)
		{
			return string.Compare(m_item, other);
		}

		public int CompareTo(StringRef other)
		{
			if (other is null)
			{
				return 1;
			}
			return string.Compare(m_item, other.m_item);
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			if (TryAsString(obj, out var str))
			{
				return string.Compare(m_item, str);
			}
			throw new ArgumentException($"{obj.GetType()} cannot compare by StringRef");
		}
		#endregion

		#region IEnumerable
		public IEnumerator<char> GetEnumerator() => m_item?.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		#endregion

		#region IFormattable
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, format, m_item);
		}
		#endregion

		public override string ToString()
		{
			return m_item;
		}

		public override int GetHashCode()
		{
			if (m_item is null)
			{
				return 0;
			}
			return m_item.GetHashCode();
		}

		public static bool IsNullOrWhiteSpace(StringRef strr)
		{
			if (strr is null) return true;
			return strr.IsStringNullOrWhitespace;
		}

		public static implicit operator string(StringRef strr) => strr?.m_item;

		public static implicit operator StringRef(string str) => Make(str);
	}
}
