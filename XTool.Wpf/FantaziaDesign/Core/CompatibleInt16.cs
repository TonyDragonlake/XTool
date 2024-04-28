using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FantaziaDesign.Core
{
	[StructLayout(LayoutKind.Explicit)]
	public struct CompatibleInt16 :
		IEquatable<CompatibleInt16>,
		ICompatible<short>,
		ICompatible<ushort>,
		ICompatible<char>,
		IComparable<CompatibleInt16>,
		IComparable,
		IConvertible,
		ICloneable,
		IPseudoArray<byte>,
		IPseudoArray<sbyte>,
		IPseudoArray<short>,
		IPseudoArray<ushort>,
		IPseudoArray<char>
	{
		[FieldOffset(0)]
		private short m_val;

		[FieldOffset(0)]
		private ushort m_uval;

		[FieldOffset(0)]
		private char m_cval;

		[FieldOffset(0)]
		private byte m_byte1;
		[FieldOffset(1)]
		private byte m_byte2;

		[FieldOffset(0)]
		private sbyte m_sbyte1;
		[FieldOffset(1)]
		private sbyte m_sbyte2;

		public short Value { get => m_val; set => m_val = value; }
		public ushort UValue { get => m_uval; set => m_uval = value; }
		public char CValue { get => m_cval; set => m_cval = value; }

		#region IPseudoArray
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_byte1;
					case 1: return m_byte2;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_byte1 = value; return;
					case 1: m_byte2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
		}
		sbyte IPseudoArray<sbyte>.this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_sbyte1;
					case 1: return m_sbyte2;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_sbyte1 = value; return;
					case 1: m_sbyte2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
		}
		short IPseudoArray<short>.this[int index] { get => m_val; set => m_val = value; }
		ushort IPseudoArray<ushort>.this[int index] { get => m_uval; set => m_uval = value; }
		char IPseudoArray<char>.this[int index] { get => m_cval; set => m_cval = value; }
		int IPseudoArray<byte>.Count => 2;
		int IPseudoArray<sbyte>.Count => 2;
		int IPseudoArray<short>.Count => 1;
		int IPseudoArray<ushort>.Count => 1;
		int IPseudoArray<char>.Count => 1;
		#endregion

		#region Ctor
		public CompatibleInt16(short val) : this()
		{
			m_val = val;
		}

		public CompatibleInt16(ushort uval) : this()
		{
			m_uval = uval;
		}

		public CompatibleInt16(char cval) : this()
		{
			m_cval = cval;
		}
		#endregion

		#region implicit operator
		public static implicit operator CompatibleInt16(short val) => new CompatibleInt16(val);
		public static implicit operator CompatibleInt16(ushort val) => new CompatibleInt16(val);
		public static implicit operator CompatibleInt16(char val) => new CompatibleInt16(val);
		public static implicit operator short(CompatibleInt16 val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(CompatibleInt16 left, CompatibleInt16 right) => left.m_val == right.m_val;
		public static bool operator !=(CompatibleInt16 left, CompatibleInt16 right) => left.m_val != right.m_val;
		#endregion

		#region IEquatable
		public override bool Equals(object obj)
		{
			return obj is CompatibleInt16 integer && Equals(integer);
		}

		public bool Equals(CompatibleInt16 other)
		{
			return m_val == other.m_val;
		}

		public override int GetHashCode()
		{
			return m_val.GetHashCode();
		}

		public bool Equals(short other)
		{
			return m_val == other;
		}

		public bool Equals(ushort other)
		{
			return m_uval == other;
		}

		public bool Equals(char other)
		{
			return m_cval == other;
		}
		#endregion

		#region IComparable
		public int CompareTo(CompatibleInt16 other)
		{
			return m_val.CompareTo(other.m_val);
		}

		public int CompareTo(short other)
		{
			return m_val.CompareTo(other);
		}

		public int CompareTo(ushort other)
		{
			return m_uval.CompareTo(other);
		}

		public int CompareTo(char other)
		{
			return m_cval.CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			if (obj is CompatibleInt16 ci)
			{
				return CompareTo(ci);
			}
			if (obj is short @short)
			{
				return CompareTo(@short);
			}
			if (obj is ushort @ushort)
			{
				return CompareTo(@ushort);
			}
			if (obj is char @char)
			{
				return CompareTo(@char);
			}
			return m_val.CompareTo(obj);
		}
		#endregion

		#region TypeCode
		public TypeCode GetTypeCode()
		{
			return m_val.GetTypeCode();
		}
		#endregion

		#region IConvertible
		bool IConvertible.ToBoolean(IFormatProvider provider) => ((IConvertible)m_val).ToBoolean(provider);
		byte IConvertible.ToByte(IFormatProvider provider) => ((IConvertible)m_val).ToByte(provider);
		char IConvertible.ToChar(IFormatProvider provider) => ((IConvertible)m_val).ToChar(provider);
		DateTime IConvertible.ToDateTime(IFormatProvider provider) => ((IConvertible)m_val).ToDateTime(provider);
		decimal IConvertible.ToDecimal(IFormatProvider provider) => ((IConvertible)m_val).ToDecimal(provider);
		double IConvertible.ToDouble(IFormatProvider provider) => ((IConvertible)m_val).ToDouble(provider);
		short IConvertible.ToInt16(IFormatProvider provider) => ((IConvertible)m_val).ToInt16(provider);
		int IConvertible.ToInt32(IFormatProvider provider) => m_val;
		long IConvertible.ToInt64(IFormatProvider provider) => ((IConvertible)m_val).ToInt64(provider);
		sbyte IConvertible.ToSByte(IFormatProvider provider) => ((IConvertible)m_val).ToSByte(provider);
		float IConvertible.ToSingle(IFormatProvider provider) => ((IConvertible)m_val).ToSingle(provider);
		string IConvertible.ToString(IFormatProvider provider) => m_val.ToString(provider);
		object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)m_val).ToType(conversionType, provider);
		ushort IConvertible.ToUInt16(IFormatProvider provider) => ((IConvertible)m_val).ToUInt16(provider);
		uint IConvertible.ToUInt32(IFormatProvider provider) => ((IConvertible)m_val).ToUInt32(provider);
		ulong IConvertible.ToUInt64(IFormatProvider provider) => ((IConvertible)m_val).ToUInt64(provider);
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new CompatibleInt16(m_val);
		}
		#endregion

		#region ICompatible
		public void AsCompatible(out short value)
		{
			value = m_val;
		}

		public void AsCompatible(out ushort value)
		{
			value = m_uval;
		}

		public void AsCompatible(out char value)
		{
			value = m_cval;
		}
		#endregion

		#region Enumeration
		private static IEnumerable<byte> BytesEnumeration(CompatibleInt16 ci)
		{
			yield return ci.m_byte1;
			yield return ci.m_byte2;
			yield break;
		}

		private static IEnumerable<sbyte> SBytesEnumeration(CompatibleInt16 ci)
		{
			yield return ci.m_sbyte1;
			yield return ci.m_sbyte2;
			yield break;
		}

		private static IEnumerable<short> Int16Enumeration(CompatibleInt16 ci)
		{
			yield return ci.m_val;
			yield break;
		}

		private static IEnumerable<ushort> UInt16Enumeration(CompatibleInt16 ci)
		{
			yield return ci.m_uval;
			yield break;
		}

		private static IEnumerable<char> CharEnumeration(CompatibleInt16 ci)
		{
			yield return ci.m_cval;
			yield break;
		}
		#endregion

		#region IEnumerable
		public IEnumerator<byte> GetEnumerator()
		{
			return BytesEnumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return BytesEnumeration(this).GetEnumerator();
		}

		IEnumerator<sbyte> IEnumerable<sbyte>.GetEnumerator()
		{
			return SBytesEnumeration(this).GetEnumerator();
		}

		IEnumerator<short> IEnumerable<short>.GetEnumerator()
		{
			return Int16Enumeration(this).GetEnumerator();
		}

		IEnumerator<ushort> IEnumerable<ushort>.GetEnumerator()
		{
			return UInt16Enumeration(this).GetEnumerator();
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return CharEnumeration(this).GetEnumerator();
		}
		#endregion
	}
}
