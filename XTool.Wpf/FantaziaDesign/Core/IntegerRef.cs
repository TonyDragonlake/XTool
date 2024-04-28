using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FantaziaDesign.Core
{
	public sealed class IntegerRef :
		TypedRef<CompatibleInt32>,
		IEquatable<IntegerRef>,
		IEquatable<int>,
		IComparable<IntegerRef>,
		IComparable<int>,
		IComparable,
		IConvertible,
		ICloneable,
		IEnumerable<byte>,
		IEnumerable
	{
		public static IntegerRef Make(int innerValue)
		{
			return new IntegerRef(innerValue);
		}

		private IntegerRef(int innerVal) : base(innerVal) { }

		private static bool TryAsInteger(object obj, out int val)
		{
			var convertible = obj as IConvertible;
			if (convertible != null)
			{
				val = convertible.ToInt32(null);
				return true;
			}
			val = 0;
			return false;
		}

		#region IEquatable
		public bool Equals(IntegerRef other)
		{
			if (other is null)
			{
				return false;
			}
			return m_item == other.m_item;
		}

		public bool Equals(int other)
		{
			return m_item == other;
		}

		public override bool Equals(object obj)
		{
			if (obj is null)
			{
				return false;
			}
			if (TryAsInteger(obj, out var str))
			{
				return m_item == str;
			}
			return false;
		}
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new IntegerRef(m_item);
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
			return m_item;
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
			return ((IConvertible)m_item).ToString(provider);
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible)m_item).ToType(conversionType, provider);
		}
		#endregion

		#region IComparable
		public int CompareTo(int other)
		{
			return m_item.CompareTo(other);
		}

		public int CompareTo(IntegerRef other)
		{
			if (other is null)
			{
				return 1;
			}
			return m_item.CompareTo(other.m_item);
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			if (TryAsInteger(obj, out var val))
			{
				return m_item.CompareTo(val);
			}
			throw new ArgumentException($"{obj.GetType()} cannot compare by IntegerRef");
		}
		#endregion

		#region IEnumerable
		public IEnumerator<byte> GetEnumerator() => m_item.GetEnumerator();

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
			return m_item.Value.ToString();
		}

		public override int GetHashCode()
		{
			return m_item.GetHashCode();
		}

		public static implicit operator int(IntegerRef intr) => (intr?.m_item.Value).GetValueOrDefault();

		public static implicit operator IntegerRef(int val) => Make(val);
	}
}
