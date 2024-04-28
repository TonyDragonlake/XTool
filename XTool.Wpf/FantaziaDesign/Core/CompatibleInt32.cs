using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Core
{
	[StructLayout(LayoutKind.Explicit)]
	public struct CompatibleInt32 : 
		IEquatable<CompatibleInt32>,
		ICompatible<int>,
		ICompatible<uint>,
		ICompatible<float>,
		IComparable<CompatibleInt32>,
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
		IPseudoArray<float>
	{
		[FieldOffset(0)]
		private int m_val;

		[FieldOffset(0)]
		private uint m_uval;

		[FieldOffset(0)]
		private float m_fval;

		[FieldOffset(0)]
		private byte m_byte1;
		[FieldOffset(1)]
		private byte m_byte2;
		[FieldOffset(2)]
		private byte m_byte3;
		[FieldOffset(3)]
		private byte m_byte4;

		[FieldOffset(0)]
		private sbyte m_sbyte1;
		[FieldOffset(1)]
		private sbyte m_sbyte2;
		[FieldOffset(2)]
		private sbyte m_sbyte3;
		[FieldOffset(3)]
		private sbyte m_sbyte4;

		[FieldOffset(0)]
		private short m_short1;
		[FieldOffset(2)]
		private short m_short2;

		[FieldOffset(0)]
		private ushort m_ushort1;
		[FieldOffset(2)]
		private ushort m_ushort2;

		[FieldOffset(0)]
		private char m_char1;
		[FieldOffset(2)]
		private char m_char2;

		public int Value { get => m_val; set => m_val = value; }
		public uint UValue { get => m_uval; set => m_uval = value; }
		public float FValue { get => m_fval; set => m_fval = value; }

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
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
			set
			{
				switch (index)
				{
					case 0: m_byte1 = value; return;
					case 1: m_byte2 = value; return;
					case 2: m_byte3 = value; return;
					case 3: m_byte4 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
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
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
			set
			{
				switch (index)
				{
					case 0: m_sbyte1 = value; return;
					case 1: m_sbyte2 = value; return;
					case 2: m_sbyte3 = value; return;
					case 3: m_sbyte4 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
		}
		int IPseudoArray<int>.this[int index] { get => m_val; set => m_val = value; }
		uint IPseudoArray<uint>.this[int index] { get => m_uval; set => m_uval = value; }
		float IPseudoArray<float>.this[int index] { get => m_fval; set => m_fval = value; }
		char IPseudoArray<char>.this[int index] 
		{
			get
			{
				switch (index)
				{
					case 0: return m_char1;
					case 1: return m_char2;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_char1 = value; return;
					case 1: m_char2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
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
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_ushort1 = value; return;
					case 1: m_ushort2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
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
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
			set
			{
				switch (index)
				{
					case 0: m_short1 = value; return;
					case 1: m_short2 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,1]");
			}
		}
		int IPseudoArray<byte>.Count => 4;
		int IPseudoArray<sbyte>.Count => 4;
		int IPseudoArray<short>.Count => 2;
		int IPseudoArray<ushort>.Count => 2;
		int IPseudoArray<char>.Count => 2;
		int IPseudoArray<int>.Count => 1;
		int IPseudoArray<uint>.Count => 1;
		int IPseudoArray<float>.Count => 1;
		#endregion

		#region Ctor
		public CompatibleInt32(int val) : this()
		{
			m_val = val;
		}

		public CompatibleInt32(uint uval) : this()
		{
			m_uval = uval;
		}

		public CompatibleInt32(float fval) : this()
		{
			m_fval = fval;
		}
		#endregion

		#region implicit operator
		public static implicit operator CompatibleInt32(int val) => new CompatibleInt32(val);
		public static implicit operator CompatibleInt32(uint val) => new CompatibleInt32(val);
		public static implicit operator CompatibleInt32(float val) => new CompatibleInt32(val);
		public static implicit operator int(CompatibleInt32 val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(CompatibleInt32 left, CompatibleInt32 right) => left.m_val == right.m_val;
		public static bool operator !=(CompatibleInt32 left, CompatibleInt32 right) => left.m_val != right.m_val;
		#endregion

		#region IEquatable
		public override bool Equals(object obj)
		{
			return obj is CompatibleInt32 integer && Equals(integer);
		}

		public bool Equals(CompatibleInt32 other)
		{
			return m_val == other.m_val;
		}

		public override int GetHashCode()
		{
			return m_val.GetHashCode();
		}

		public bool Equals(int other)
		{
			return m_val == other;
		}

		public bool Equals(uint other)
		{
			return m_uval == other;
		}

		public bool Equals(float other)
		{
			return m_fval == other;
		}
		#endregion

		#region IComparable
		public int CompareTo(CompatibleInt32 other)
		{
			return m_val.CompareTo(other.m_val);
		}

		public int CompareTo(int other)
		{
			return m_val.CompareTo(other);
		}

		public int CompareTo(uint other)
		{
			return m_uval.CompareTo(other);
		}

		public int CompareTo(float other)
		{
			return m_fval.CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			if (obj is CompatibleInt32 ci)
			{
				return CompareTo(ci);
			}
			if (obj is int @int)
			{
				return CompareTo(@int);
			}
			if (obj is uint @uint)
			{
				return CompareTo(@uint);
			}
			if (obj is float @float)
			{
				return CompareTo(@float);
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
			return new CompatibleInt32(m_val);
		}
		#endregion

		#region ICompatible
		public void AsCompatible(out int value)
		{
			value = m_val;
		}

		public void AsCompatible(out uint value)
		{
			value = m_uval;
		}

		public void AsCompatible(out float value)
		{
			value = m_fval;
		}
		#endregion

		#region Enumeration
		private static IEnumerable<byte> BytesEnumeration(CompatibleInt32 ci)
		{
			yield return ci.m_byte1;
			yield return ci.m_byte2;
			yield return ci.m_byte3;
			yield return ci.m_byte4;
			yield break;
		}

		private static IEnumerable<sbyte> SBytesEnumeration(CompatibleInt32 ci)
		{
			yield return ci.m_sbyte1;
			yield return ci.m_sbyte2;
			yield return ci.m_sbyte3;
			yield return ci.m_sbyte4;
			yield break;
		}

		private static IEnumerable<short> Int16Enumeration(CompatibleInt32 ci)
		{
			yield return ci.m_short1;
			yield return ci.m_short2;
			yield break;
		}

		private static IEnumerable<ushort> UInt16Enumeration(CompatibleInt32 ci)
		{
			yield return ci.m_ushort1;
			yield return ci.m_ushort2;
			yield break;
		}

		private static IEnumerable<char> CharEnumeration(CompatibleInt32 ci)
		{
			yield return ci.m_char1;
			yield return ci.m_char2;
			yield break;
		}

		private static IEnumerable<int> Int32Enumeration(CompatibleInt32 ci)
		{
			yield return ci.m_val;
			yield break;
		}

		private static IEnumerable<uint> UInt32Enumeration(CompatibleInt32 ci)
		{
			yield return ci.m_uval;
			yield break;
		}

		private static IEnumerable<float> Float32Enumeration(CompatibleInt32 ci)
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

		IEnumerator<sbyte> IEnumerable<sbyte>.GetEnumerator()
		{
			return SBytesEnumeration(this).GetEnumerator();
		}
		#endregion
	}
}
