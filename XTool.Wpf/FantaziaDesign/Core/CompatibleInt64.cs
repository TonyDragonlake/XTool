using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace FantaziaDesign.Core
{
	[StructLayout(LayoutKind.Explicit)]
	public struct CompatibleInt64 :
		IEquatable<CompatibleInt64>,
		ICompatible<long>,
		ICompatible<ulong>,
		ICompatible<double>,
		IComparable<CompatibleInt64>,
		IComparable,
		IConvertible,
		ICloneable,
		IPseudoArray<byte>,
		IPseudoArray<sbyte>,
		IPseudoArray<short>,
		IPseudoArray<ushort>,
		IPseudoArray<char>,
		IPseudoArray<int>,
		IPseudoArray<uint>,
		IPseudoArray<float>,
		IPseudoArray<long>,
		IPseudoArray<ulong>,
		IPseudoArray<double>
	{
		[FieldOffset(0)]
		private long m_val;

		[FieldOffset(0)]
		private ulong m_uval;

		[FieldOffset(0)]
		private double m_fval;

		[FieldOffset(0)]
		private byte m_byte1;
		[FieldOffset(1)]
		private byte m_byte2;
		[FieldOffset(2)]
		private byte m_byte3;
		[FieldOffset(3)]
		private byte m_byte4;
		[FieldOffset(4)]
		private byte m_byte5;
		[FieldOffset(5)]
		private byte m_byte6;
		[FieldOffset(6)]
		private byte m_byte7;
		[FieldOffset(7)]
		private byte m_byte8;

		[FieldOffset(0)]
		private sbyte m_sbyte1;
		[FieldOffset(1)]
		private sbyte m_sbyte2;
		[FieldOffset(2)]
		private sbyte m_sbyte3;
		[FieldOffset(3)]
		private sbyte m_sbyte4;
		[FieldOffset(4)]
		private sbyte m_sbyte5;
		[FieldOffset(5)]
		private sbyte m_sbyte6;
		[FieldOffset(6)]
		private sbyte m_sbyte7;
		[FieldOffset(7)]
		private sbyte m_sbyte8;

		[FieldOffset(0)]
		private short m_short1;
		[FieldOffset(2)]
		private short m_short2;
		[FieldOffset(4)]
		private short m_short3;
		[FieldOffset(6)]
		private short m_short4;

		[FieldOffset(0)]
		private ushort m_ushort1;
		[FieldOffset(2)]
		private ushort m_ushort2;
		[FieldOffset(4)]
		private ushort m_ushort3;
		[FieldOffset(6)]
		private ushort m_ushort4;

		[FieldOffset(0)]
		private char m_char1;
		[FieldOffset(2)]
		private char m_char2;
		[FieldOffset(4)]
		private char m_char3;
		[FieldOffset(6)]
		private char m_char4;

		[FieldOffset(0)]
		private int m_int1;
		[FieldOffset(4)]
		private int m_int2;

		[FieldOffset(0)]
		private uint m_uint1;
		[FieldOffset(4)]
		private uint m_uint2;

		[FieldOffset(0)]
		private float m_float1;
		[FieldOffset(4)]
		private float m_float2;

		public long Value { get => m_val; set => m_val = value; }
		public ulong UValue { get => m_uval; set => m_uval = value; }
		public double FValue { get => m_fval; set => m_fval = value; }

		#region IPseudoArray
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_byte1;
					case 1: return m_byte2;
					case 2: return m_byte3;
					case 3: return m_byte4;
					case 4: return m_byte5;
					case 5: return m_byte6;
					case 6: return m_byte7;
					case 7: return m_byte8;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,7]");
			}
			set
			{
				switch (index)
				{
					case 0: m_byte1 = value; return;
					case 1: m_byte2 = value; return;
					case 2: m_byte3 = value; return;
					case 3: m_byte4 = value; return;
					case 4: m_byte5 = value; return;
					case 5: m_byte6 = value; return;
					case 6: m_byte7 = value; return;
					case 7: m_byte8 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,7]");
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
					case 2: return m_sbyte3;
					case 3: return m_sbyte4;
					case 4: return m_sbyte5;
					case 5: return m_sbyte6;
					case 6: return m_sbyte7;
					case 7: return m_sbyte8;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,7]");
			}
			set
			{
				switch (index)
				{
					case 0: m_sbyte1 = value; return;
					case 1: m_sbyte2 = value; return;
					case 2: m_sbyte3 = value; return;
					case 3: m_sbyte4 = value; return;
					case 4: m_sbyte5 = value; return;
					case 5: m_sbyte6 = value; return;
					case 6: m_sbyte7 = value; return;
					case 7: m_sbyte8 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,7]");
			}
		}
		char IPseudoArray<char>.this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_char1;
					case 1: return m_char2;
					case 2: return m_char3;
					case 3: return m_char4;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
			set
			{
				switch (index)
				{
					case 0: m_char1 = value; return;
					case 1: m_char2 = value; return;
					case 2: m_char3 = value; return;
					case 3: m_char4 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
		}
		ushort IPseudoArray<ushort>.this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_ushort1;
					case 1: return m_ushort2;
					case 2: return m_ushort3;
					case 3: return m_ushort4;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
			set
			{
				switch (index)
				{
					case 0: m_ushort1 = value; return;
					case 1: m_ushort2 = value; return;
					case 2: m_ushort3 = value; return;
					case 3: m_ushort4 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
		}
		short IPseudoArray<short>.this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_short1;
					case 1: return m_short2;
					case 2: return m_short3;
					case 3: return m_short4;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
			set
			{
				switch (index)
				{
					case 0: m_short1 = value; return;
					case 1: m_short2 = value; return;
					case 2: m_short3 = value; return;
					case 3: m_short4 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
		}
		float IPseudoArray<float>.this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_float1;
					case 1: return m_float2;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_float1 = value; return;
					case 1: m_float2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
		}
		uint IPseudoArray<uint>.this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_uint1;
					case 1: return m_uint2;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_uint1 = value; return;
					case 1: m_uint2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
		}
		int IPseudoArray<int>.this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_int1;
					case 1: return m_int2;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_int1 = value; return;
					case 1: m_int2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
		}
		long IPseudoArray<long>.this[int index] { get => m_val; set => m_val = value; }
		ulong IPseudoArray<ulong>.this[int index] { get => m_uval; set => m_uval = value; }
		double IPseudoArray<double>.this[int index] { get => m_fval; set => m_fval = value; }
		int IPseudoArray<byte>.Count => 8;
		int IPseudoArray<sbyte>.Count => 8;
		int IPseudoArray<short>.Count => 4;
		int IPseudoArray<ushort>.Count => 4;
		int IPseudoArray<char>.Count => 4;
		int IPseudoArray<int>.Count => 2;
		int IPseudoArray<uint>.Count => 2;
		int IPseudoArray<float>.Count => 2;
		int IPseudoArray<long>.Count => 1;
		int IPseudoArray<ulong>.Count => 1;
		int IPseudoArray<double>.Count => 1;
		#endregion

		#region Ctor
		public CompatibleInt64(long val) : this()
		{
			m_val = val;
		}

		public CompatibleInt64(ulong uval) : this()
		{
			m_uval = uval;
		}

		public CompatibleInt64(double fval) : this()
		{
			m_fval = fval;
		}
		#endregion

		#region implicit operator
		public static implicit operator CompatibleInt64(long val) => new CompatibleInt64(val);
		public static implicit operator CompatibleInt64(ulong val) => new CompatibleInt64(val);
		public static implicit operator CompatibleInt64(double val) => new CompatibleInt64(val);
		public static implicit operator long(CompatibleInt64 val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(CompatibleInt64 left, CompatibleInt64 right) => left.m_val == right.m_val;
		public static bool operator !=(CompatibleInt64 left, CompatibleInt64 right) => left.m_val != right.m_val;
		#endregion

		#region IEquatable
		public override bool Equals(object obj)
		{
			return obj is CompatibleInt64 integer && Equals(integer);
		}

		public bool Equals(CompatibleInt64 other)
		{
			return m_val == other.m_val;
		}

		public override int GetHashCode()
		{
			return m_val.GetHashCode();
		}

		public bool Equals(long other)
		{
			return m_val == other;
		}

		public bool Equals(ulong other)
		{
			return m_uval == other;
		}

		public bool Equals(double other)
		{
			return m_fval == other;
		}
		#endregion

		#region IComparable
		public int CompareTo(CompatibleInt64 other)
		{
			return m_val.CompareTo(other.m_val);
		}

		public int CompareTo(long other)
		{
			return m_val.CompareTo(other);
		}

		public int CompareTo(ulong other)
		{
			return m_uval.CompareTo(other);
		}

		public int CompareTo(double other)
		{
			return m_fval.CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			if (obj is CompatibleInt64 ci)
			{
				return CompareTo(ci);
			}
			if (obj is long @long)
			{
				return CompareTo(@long);
			}
			if (obj is ulong @ulong)
			{
				return CompareTo(@ulong);
			}
			if (obj is double @double)
			{
				return CompareTo(@double);
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
		int IConvertible.ToInt32(IFormatProvider provider) => ((IConvertible)m_val).ToInt32(provider);
		long IConvertible.ToInt64(IFormatProvider provider) => m_val;
		sbyte IConvertible.ToSByte(IFormatProvider provider) => ((IConvertible)m_val).ToSByte(provider);
		float IConvertible.ToSingle(IFormatProvider provider) => ((IConvertible)m_val).ToSingle(provider);
		string IConvertible.ToString(IFormatProvider provider) => m_val.ToString(provider);
		object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)m_val).ToType(conversionType, provider);
		ushort IConvertible.ToUInt16(IFormatProvider provider) => ((IConvertible)m_val).ToUInt16(provider);
		uint IConvertible.ToUInt32(IFormatProvider provider) => ((IConvertible)m_val).ToUInt32(provider);
		ulong IConvertible.ToUInt64(IFormatProvider provider) => ((IConvertible)m_val).ToUInt64(provider);
		#endregion

		#region ICompatible
		public void AsCompatible(out long value)
		{
			value = m_val;
		}

		public void AsCompatible(out ulong value)
		{
			value = m_uval;
		}

		public void AsCompatible(out double value)
		{
			value = m_fval;
		}
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new CompatibleInt64(m_val);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<byte> BytesEnumeration(CompatibleInt64 ci)
		{
			yield return ci.m_byte1;
			yield return ci.m_byte2;
			yield return ci.m_byte3;
			yield return ci.m_byte4;
			yield return ci.m_byte5;
			yield return ci.m_byte6;
			yield return ci.m_byte7;
			yield return ci.m_byte8;
			yield break;
		}

		private static IEnumerable<sbyte> SBytesEnumeration(CompatibleInt64 ci)
		{
			yield return ci.m_sbyte1;
			yield return ci.m_sbyte2;
			yield return ci.m_sbyte3;
			yield return ci.m_sbyte4;
			yield return ci.m_sbyte5;
			yield return ci.m_sbyte6;
			yield return ci.m_sbyte7;
			yield return ci.m_sbyte8;
			yield break;
		}

		private static IEnumerable<short> Int16Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_short1;
			yield return ci.m_short2;
			yield return ci.m_short3;
			yield return ci.m_short4;
			yield break;
		}

		private static IEnumerable<ushort> UInt16Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_ushort1;
			yield return ci.m_ushort2;
			yield return ci.m_ushort3;
			yield return ci.m_ushort4;
			yield break;
		}

		private static IEnumerable<char> CharEnumeration(CompatibleInt64 ci)
		{
			yield return ci.m_char1;
			yield return ci.m_char2;
			yield return ci.m_char3;
			yield return ci.m_char4;
			yield break;
		}

		private static IEnumerable<int> Int32Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_int1;
			yield return ci.m_int2;
			yield break;
		}

		private static IEnumerable<uint> UInt32Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_uint1;
			yield return ci.m_uint2;
			yield break;
		}

		private static IEnumerable<float> Float32Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_float1;
			yield return ci.m_float2;
			yield break;
		}

		private static IEnumerable<long> Int64Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_val;
			yield break;
		}

		private static IEnumerable<ulong> UInt64Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_uval;
			yield break;
		}

		private static IEnumerable<double> Float64Enumeration(CompatibleInt64 ci)
		{
			yield return ci.m_fval;
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

		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			return Int32Enumeration(this).GetEnumerator();
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return CharEnumeration(this).GetEnumerator();
		}

		IEnumerator<uint> IEnumerable<uint>.GetEnumerator()
		{
			return UInt32Enumeration(this).GetEnumerator();
		}

		IEnumerator<float> IEnumerable<float>.GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}

		IEnumerator<long> IEnumerable<long>.GetEnumerator()
		{
			return Int64Enumeration(this).GetEnumerator();
		}

		IEnumerator<ulong> IEnumerable<ulong>.GetEnumerator()
		{
			return UInt64Enumeration(this).GetEnumerator();
		}

		IEnumerator<double> IEnumerable<double>.GetEnumerator()
		{
			return Float64Enumeration(this).GetEnumerator();
		}
		#endregion

	}
}
