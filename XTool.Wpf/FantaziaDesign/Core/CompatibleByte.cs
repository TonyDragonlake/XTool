using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Core
{
	public interface ICompatible<T> : IEquatable<T>, IComparable<T>
	{
		void AsCompatible(out T value);
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct CompatibleByte:
		IEquatable<CompatibleByte>,
		ICompatible<byte>,
		ICompatible<sbyte>,
		IComparable<CompatibleByte>,
		IComparable,
		IConvertible,
		ICloneable,
		IPseudoArray<byte>,
		IPseudoArray<sbyte>
	{
		[FieldOffset(0)]
		private byte m_val;
		[FieldOffset(0)]
		private sbyte m_sval;

		public byte Value { get => m_val; set => m_val = value; }
		public sbyte SValue { get => m_sval; set => m_sval = value; }



		#region IPseudoArray
		public byte this[int index] { get => m_val; set => m_val = value; }
		sbyte IPseudoArray<sbyte>.this[int index] { get => m_sval; set => m_sval = value; }
		int IPseudoArray<byte>.Count => 1;
		int IPseudoArray<sbyte>.Count => 1;
		#endregion

		#region Ctor
		public CompatibleByte(byte val) : this()
		{
			m_val = val;
		}

		public CompatibleByte(sbyte sval) : this()
		{
			m_sval = sval;
		}
		#endregion

		#region implicit operator
		public static implicit operator CompatibleByte(byte val) => new CompatibleByte(val);
		public static implicit operator CompatibleByte(sbyte val) => new CompatibleByte(val);
		public static implicit operator byte(CompatibleByte val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(CompatibleByte left, CompatibleByte right) => left.m_val == right.m_val;
		public static bool operator !=(CompatibleByte left, CompatibleByte right) => left.m_val != right.m_val;
		#endregion

		#region IEquatable
		public override bool Equals(object obj)
		{
			return obj is CompatibleByte b && Equals(b);
		}

		public bool Equals(CompatibleByte other)
		{
			return m_val == other.m_val;
		}

		public override int GetHashCode()
		{
			return m_val.GetHashCode();
		}

		public bool Equals(byte other)
		{
			return m_val == other;
		}

		public bool Equals(sbyte other)
		{
			return m_sval == other;
		}
		#endregion

		#region IComparable
		public int CompareTo(CompatibleByte other)
		{
			return m_val.CompareTo(other.m_val);
		}

		public int CompareTo(byte other)
		{
			return m_val.CompareTo(other);
		}

		public int CompareTo(sbyte other)
		{
			return m_sval.CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			if (obj is CompatibleByte cb)
			{
				return CompareTo(cb);
			}
			if (obj is byte @byte)
			{
				return CompareTo(@byte);
			}
			if (obj is sbyte @sbyte)
			{
				return CompareTo(@sbyte);
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
		byte IConvertible.ToByte(IFormatProvider provider) => m_val;
		char IConvertible.ToChar(IFormatProvider provider) => ((IConvertible)m_val).ToChar(provider);
		DateTime IConvertible.ToDateTime(IFormatProvider provider) => ((IConvertible)m_val).ToDateTime(provider);
		decimal IConvertible.ToDecimal(IFormatProvider provider) => ((IConvertible)m_val).ToDecimal(provider);
		double IConvertible.ToDouble(IFormatProvider provider) => ((IConvertible)m_val).ToDouble(provider);
		short IConvertible.ToInt16(IFormatProvider provider) => ((IConvertible)m_val).ToInt16(provider);
		int IConvertible.ToInt32(IFormatProvider provider) => ((IConvertible)m_val).ToInt32(provider);
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
		public void AsCompatible(out sbyte value)
		{
			value = m_sval;
		}

		public void AsCompatible(out byte value)
		{
			value = m_val;
		}
		#endregion

		#region Enumeration
		private static IEnumerable<byte> BytesEnumeration(CompatibleByte cb)
		{
			yield return cb.m_val;
			yield break;
		}

		private static IEnumerable<sbyte> SBytesEnumeration(CompatibleByte cb)
		{
			yield return cb.m_sval;
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
		#endregion
	}


}
