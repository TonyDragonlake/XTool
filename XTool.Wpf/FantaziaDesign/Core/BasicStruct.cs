using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Core
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Vec4b :
		IDeepCopyable<Vec4b>,
		IEquatable<Vec4b>,
		ICloneable,
		IPseudoArray<byte>,
		IPseudoArray<int>,
		IPseudoArray<uint>
	{
		[FieldOffset(0)]
		private int m_val;

		[FieldOffset(0)]
		private uint m_uval;

		[FieldOffset(0)]
		private byte m_byte1;
		[FieldOffset(1)]
		private byte m_byte2;
		[FieldOffset(2)]
		private byte m_byte3;
		[FieldOffset(3)]
		private byte m_byte4;

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
		int IPseudoArray<int>.this[int index] { get => m_val; set => m_val = value; }
		uint IPseudoArray<uint>.this[int index] { get => m_uval; set => m_uval = value; }
		int IPseudoArray<byte>.Count => 4;
		int IPseudoArray<int>.Count => 1;
		int IPseudoArray<uint>.Count => 1;
		#endregion

		#region Ctor

		public Vec4b(IPseudoArray<byte> array): this()
		{
			if (array is null)
			{
				return;
			}

			if (array is Vec4b vec4b)
			{
				m_val = vec4b.m_val;
			}
			else
			{
				var count = array.Count;
				const int elementCount = 4;
				if (count > elementCount)
				{
					count = elementCount;
				}
				for (int i = 0; i < count; i++)
				{
					try
					{
						this[i] = array[i];
					}
					finally
					{
					}
				}
			}
		}

		public Vec4b(int val) : this()
		{
			m_val = val;
		}

		public Vec4b(uint uval) : this()
		{
			m_uval = uval;
		}

		public Vec4b(params byte[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 4)
				{
					length = 4;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}
		#endregion

		#region implicit operator
		public static implicit operator Vec4b(int val) => new Vec4b(val);
		public static implicit operator Vec4b(uint val) => new Vec4b(val);
		public static implicit operator int(Vec4b val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(Vec4b left, Vec4b right) => Equals(left, right);
		public static bool operator !=(Vec4b left, Vec4b right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec4b DeepCopy()
		{
			return new Vec4b(m_val);
		}

		public void DeepCopyValueFrom(Vec4b obj)
		{
			m_val = obj.m_val;
		}
		#endregion

		#region IEquatable

		private static bool Equals(Vec4b left, Vec4b right)
		{
			return left.m_val == right.m_val;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vec4b vec4b)
			{
				return Equals(vec4b);
			}
			return false;
		}

		public bool Equals(Vec4b other)
		{
			return Equals(this, other);
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
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec4b(m_val);
		}
		#endregion

		#region Enumeration
		private static IEnumerable<byte> BytesEnumeration(Vec4b vec4b)
		{
			yield return vec4b.m_byte1;
			yield return vec4b.m_byte2;
			yield return vec4b.m_byte3;
			yield return vec4b.m_byte4;
			yield break;
		}

		private static IEnumerable<int> Int32Enumeration(Vec4b vec4b)
		{
			yield return vec4b.m_val;
			yield break;
		}

		private static IEnumerable<uint> UInt32Enumeration(Vec4b vec4b)
		{
			yield return vec4b.m_uval;
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

		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			return Int32Enumeration(this).GetEnumerator();
		}

		IEnumerator<uint> IEnumerable<uint>.GetEnumerator()
		{
			return UInt32Enumeration(this).GetEnumerator();
		}
		#endregion
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec2i :
		IDeepCopyable<Vec2i>,
		IEquatable<Vec2i>,
		ICloneable,
		IPseudoArray<int>,
		IPseudoArray<long>
	{
		[FieldOffset(0)]
		private long m_val;

		[FieldOffset(0)]
		private int m_int1;
		[FieldOffset(4)]
		private int m_int2;

		#region IPseudoArray
		public int this[int index]
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
		int IPseudoArray<int>.Count => 2;
		int IPseudoArray<long>.Count => 1;
		#endregion

		#region Ctor
		public Vec2i(IPseudoArray<int> array) : this()
		{
			if (array is null)
			{
				return;
			}
			if (array is Vec2i vec2i)
			{
				m_val = vec2i.m_val;
			}
			else
			{
				var count = array.Count;
				const int elementCount = 2;
				if (count > elementCount)
				{
					count = elementCount;
				}
				for (int i = 0; i < count; i++)
				{
					try
					{
						this[i] = array[i];
					}
					finally
					{
					}
				}
			}
		}

		public Vec2i(long val) : this()
		{
			m_val = val;
		}

		public Vec2i(params int[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 2)
				{
					length = 2;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}
		#endregion

		#region implicit operator
		public static implicit operator Vec2i(long val) => new Vec2i(val);
		public static implicit operator long(Vec2i val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(Vec2i left, Vec2i right) => Equals(left, right);
		public static bool operator !=(Vec2i left, Vec2i right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec2i DeepCopy()
		{
			return new Vec2i(m_val);
		}

		public void DeepCopyValueFrom(Vec2i obj)
		{
			m_val = obj.m_val;
		}
		#endregion

		#region IEquatable
		private static bool Equals(Vec2i left, Vec2i right)
		{
			return left.m_val == right.m_val;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vec2i vec2i)
			{
				return Equals(vec2i);
			}
			return false;
		}

		public bool Equals(Vec2i other)
		{
			return Equals(this, other);
		}

		public override int GetHashCode()
		{
			return m_val.GetHashCode();
		}

		public bool Equals(long other)
		{
			return m_val == other;
		}
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec2i(m_val);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<int> Int32Enumeration(Vec2i vec2i)
		{
			yield return vec2i.m_int1;
			yield return vec2i.m_int2;
			yield break;
		}

		private static IEnumerable<long> Int64Enumeration(Vec2i vec2i)
		{
			yield return vec2i.m_val;
			yield break;
		}

		#endregion

		#region IEnumerable
		public IEnumerator<int> GetEnumerator()
		{
			return Int32Enumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Int32Enumeration(this).GetEnumerator();
		}

		IEnumerator<long> IEnumerable<long>.GetEnumerator()
		{
			return Int64Enumeration(this).GetEnumerator();
		}
		#endregion
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec2ui :
	IDeepCopyable<Vec2ui>,
	IEquatable<Vec2ui>,
	ICloneable,
	IPseudoArray<uint>,
	IPseudoArray<ulong>
	{
		[FieldOffset(0)]
		private ulong m_val;

		[FieldOffset(0)]
		private uint m_int1;
		[FieldOffset(4)]
		private uint m_int2;

		#region IPseudoArray
		public uint this[int index]
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
		ulong IPseudoArray<ulong>.this[int index] { get => m_val; set => m_val = value; }
		int IPseudoArray<uint>.Count => 2;
		int IPseudoArray<ulong>.Count => 1;
		#endregion

		#region Ctor
		public Vec2ui(IPseudoArray<uint> array) : this()
		{
			if (array is null)
			{
				return;
			}
			if (array is Vec2ui vec2i)
			{
				m_val = vec2i.m_val;
			}
			else
			{
				var count = array.Count;
				const int elementCount = 2;
				if (count > elementCount)
				{
					count = elementCount;
				}
				for (int i = 0; i < count; i++)
				{
					try
					{
						this[i] = array[i];
					}
					finally
					{
					}
				}
			}
		}

		public Vec2ui(ulong val) : this()
		{
			m_val = val;
		}

		public Vec2ui(params uint[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 2)
				{
					length = 2;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}
		#endregion

		#region implicit operator
		public static implicit operator Vec2ui(ulong val) => new Vec2ui(val);
		public static implicit operator ulong(Vec2ui val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(Vec2ui left, Vec2ui right) => Equals(left, right);
		public static bool operator !=(Vec2ui left, Vec2ui right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec2ui DeepCopy()
		{
			return new Vec2ui(m_val);
		}

		public void DeepCopyValueFrom(Vec2ui obj)
		{
			m_val = obj.m_val;
		}
		#endregion

		#region IEquatable
		private static bool Equals(Vec2ui left, Vec2ui right)
		{
			return left.m_val == right.m_val;
		}

		public override bool Equals(object obj)
		{
			if (obj is Vec2ui vec2i)
			{
				return Equals(vec2i);
			}
			return false;
		}

		public bool Equals(Vec2ui other)
		{
			return Equals(this, other);
		}

		public override int GetHashCode()
		{
			return m_val.GetHashCode();
		}

		public bool Equals(ulong other)
		{
			return m_val == other;
		}
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec2ui(m_val);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<uint> UInt32Enumeration(Vec2ui vec2i)
		{
			yield return vec2i.m_int1;
			yield return vec2i.m_int2;
			yield break;
		}

		private static IEnumerable<ulong> UInt64Enumeration(Vec2ui vec2i)
		{
			yield return vec2i.m_val;
			yield break;
		}

		#endregion

		#region IEnumerable
		public IEnumerator<uint> GetEnumerator()
		{
			return UInt32Enumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return UInt32Enumeration(this).GetEnumerator();
		}

		IEnumerator<ulong> IEnumerable<ulong>.GetEnumerator()
		{
			return UInt64Enumeration(this).GetEnumerator();
		}
		#endregion
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec4i :
		IDeepCopyable<Vec4i>,
		IEquatable<Vec4i>,
		ICloneable,
		IPseudoArray<int>
	{

		[FieldOffset(0)]
		private int m_int1;
		[FieldOffset(4)]
		private int m_int2;
		[FieldOffset(8)]
		private int m_int3;
		[FieldOffset(12)]
		private int m_int4;


		#region IPseudoArray
		public int this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_int1;
					case 1: return m_int2;
					case 2: return m_int3;
					case 3: return m_int4;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
			set
			{
				switch (index)
				{
					case 0: m_int1 = value; return;
					case 1: m_int2 = value; return;
					case 2: m_int3 = value; return;
					case 3: m_int4 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
		}
		int IPseudoArray<int>.Count => 4;
		#endregion

		#region Ctor
		public Vec4i(IPseudoArray<int> array) : this()
		{
			if (array is null)
			{
				return;
			}
			var count = array.Count;
			const int elementCount = 4;
			if (count > elementCount)
			{
				count = elementCount;
			}
			for (int i = 0; i < count; i++)
			{
				try
				{
					this[i] = array[i];
				}
				finally
				{
				}
			}
		}

		public Vec4i(params int[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 4)
				{
					length = 4;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}

		#endregion

		#region equality operator
		public static bool operator ==(Vec4i left, Vec4i right) => Equals(left, right);
		public static bool operator !=(Vec4i left, Vec4i right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec4i DeepCopy()
		{
			return new Vec4i(this);
		}

		public void DeepCopyValueFrom(Vec4i obj)
		{
			m_int1 = obj.m_int1;
			m_int2 = obj.m_int2;
			m_int3 = obj.m_int3;
			m_int4 = obj.m_int4;
		}
		#endregion

		#region IEquatable
		private static bool Equals(Vec4i left, Vec4i right)
		{
			return left.m_int1 == right.m_int1
				&& left.m_int2 == right.m_int2
				&& left.m_int3 == right.m_int3
				&& left.m_int4 == right.m_int4;
		}


		public override bool Equals(object obj)
		{
			if (obj is Vec4i vec4i)
			{
				return Equals(vec4i);
			}
			return false;
		}

		public bool Equals(Vec4i other)
		{
			return Equals(this, other);
		}

		public override int GetHashCode()
		{
			return m_int1.GetHashCode()
				^ m_int2.GetHashCode()
				^ m_int3.GetHashCode()
				^ m_int4.GetHashCode();
		}

		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec4i(this);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<int> Int32Enumeration(Vec4i vec4i)
		{
			yield return vec4i.m_int1;
			yield return vec4i.m_int2;
			yield return vec4i.m_int3;
			yield return vec4i.m_int4;
			yield break;
		}
		#endregion

		#region IEnumerable
		public IEnumerator<int> GetEnumerator()
		{
			return Int32Enumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Int32Enumeration(this).GetEnumerator();
		}
		#endregion
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec2f :
		IDeepCopyable<Vec2f>,
		IEquatable<Vec2f>,
		ICloneable,
		IPseudoArray<float>,
		IPseudoArray<long>
	{
		[FieldOffset(0)]
		private long m_val;

		[FieldOffset(0)]
		private float m_float1;
		[FieldOffset(4)]
		private float m_float2;

		#region IPseudoArray
		public float this[int index]
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
		long IPseudoArray<long>.this[int index] { get => m_val; set => m_val = value; }
		int IPseudoArray<float>.Count => 2;
		int IPseudoArray<long>.Count => 1;
		#endregion

		#region Ctor
		public Vec2f(IPseudoArray<float> array) : this()
		{
			if (array is null)
			{
				return;
			}
			;
			if (array is Vec2f vec2f)
			{
				m_val = vec2f.m_val;
			}
			else
			{
				var count = array.Count;
				const int elementCount = 2;
				if (count > elementCount)
				{
					count = elementCount;
				}
				for (int i = 0; i < count; i++)
				{
					try
					{
						this[i] = array[i];
					}
					finally
					{
					}
				}
			}
		}

		public Vec2f(long val) : this()
		{
			m_val = val;
		}

		public Vec2f(params float[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 2)
				{
					length = 2;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}

		#endregion

		#region implicit operator
		public static implicit operator Vec2f(long val) => new Vec2f(val);
		public static implicit operator long(Vec2f val) => val.m_val;
		#endregion

		#region equality operator
		public static bool operator ==(Vec2f left, Vec2f right) => Equals(left, right);
		public static bool operator !=(Vec2f left, Vec2f right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec2f DeepCopy()
		{
			return new Vec2f(m_val);
		}

		public void DeepCopyValueFrom(Vec2f obj)
		{
			m_val = obj.m_val;
		}
		#endregion

		#region IEquatable
		private static bool Equals(Vec2f left, Vec2f right)
		{
			return left.m_val == right.m_val;
		}

		public override bool Equals(object obj)
		{
			if(obj is Vec2f vec2f)
			{
				return Equals(vec2f);
			}
			return false;
		}

		public bool Equals(Vec2f other)
		{
			return Equals(this, other);
		}

		public override int GetHashCode()
		{
			return m_val.GetHashCode();
		}

		public bool Equals(long other)
		{
			return m_val == other;
		}
		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec2f(m_val);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<float> Float32Enumeration(Vec2f vec2f)
		{
			yield return vec2f.m_float1;
			yield return vec2f.m_float2;
			yield break;
		}

		private static IEnumerable<long> Int64Enumeration(Vec2f vec2f)
		{
			yield return vec2f.m_val;
			yield break;
		}
		#endregion

		#region IEnumerable

		public IEnumerator<float> GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}

		IEnumerator<long> IEnumerable<long>.GetEnumerator()
		{
			return Int64Enumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}
		#endregion

	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec4f :
		IDeepCopyable<Vec4f>,
		IEquatable<Vec4f>,
		ICloneable,
		IPseudoArray<float>
	{

		[FieldOffset(0)]
		private float m_float1;
		[FieldOffset(4)]
		private float m_float2;
		[FieldOffset(8)]
		private float m_float3;
		[FieldOffset(12)]
		private float m_float4;

		#region IPseudoArray
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_float1;
					case 1: return m_float2;
					case 2: return m_float3;
					case 3: return m_float4;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
			set
			{
				switch (index)
				{
					case 0: m_float1 = value; return;
					case 1: m_float2 = value; return;
					case 2: m_float3 = value; return;
					case 3: m_float4 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,3]");
			}
		}
		int IPseudoArray<float>.Count => 4;
		#endregion

		#region Ctor
		public Vec4f(IPseudoArray<float> array) : this()
		{
			if (array is null)
			{
				return;
			}
			var count = array.Count;
			const int elementCount = 4;
			if (count > elementCount)
			{
				count = elementCount;
			}
			for (int i = 0; i < count; i++)
			{
				try
				{
					this[i] = array[i];
				}
				finally
				{
				}
			}
		}

		public Vec4f(params float[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 4)
				{
					length = 4;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}

		#endregion

		#region equality operator
		public static bool operator ==(Vec4f left, Vec4f right) => Equals(left, right);
		public static bool operator !=(Vec4f left, Vec4f right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec4f DeepCopy()
		{
			return new Vec4f(this);
		}

		public void DeepCopyValueFrom(Vec4f obj)
		{
			m_float1 = obj.m_float1;
			m_float2 = obj.m_float2;
			m_float3 = obj.m_float3;
			m_float4 = obj.m_float4;
		}
		#endregion

		#region IEquatable
		private static bool Equals(Vec4f left, Vec4f right)
		{
			return left.m_float1 == right.m_float1
				&& left.m_float2 == right.m_float2
				&& left.m_float3 == right.m_float3
				&& left.m_float4 == right.m_float4;
		}


		public override bool Equals(object obj)
		{
			if (obj is Vec4f vec4f)
			{
				return Equals(vec4f);
			}
			return false;
		}

		public bool Equals(Vec4f other)
		{
			return Equals(this, other);
		}

		public override int GetHashCode()
		{
			return m_float1.GetHashCode()
				^ m_float2.GetHashCode()
				^ m_float3.GetHashCode()
				^ m_float4.GetHashCode();
		}

		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec4f(this);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<float> Float32Enumeration(Vec4f vec4f)
		{
			yield return vec4f.m_float1;
			yield return vec4f.m_float2;
			yield return vec4f.m_float3;
			yield return vec4f.m_float4;
			yield break;
		}
		#endregion

		#region IEnumerable
		public IEnumerator<float> GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}
		#endregion
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec6f :
		IDeepCopyable<Vec6f>,
		IEquatable<Vec6f>,
		ICloneable,
		IPseudoArray<float>
	{

		[FieldOffset(0)]
		private float m_float1;
		[FieldOffset(4)]
		private float m_float2;
		[FieldOffset(8)]
		private float m_float3;
		[FieldOffset(12)]
		private float m_float4;
		[FieldOffset(16)]
		private float m_float5;
		[FieldOffset(20)]
		private float m_float6;

		#region IPseudoArray
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_float1;
					case 1: return m_float2;
					case 2: return m_float3;
					case 3: return m_float4;
					case 4: return m_float5;
					case 5: return m_float6;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,5]");
			}
			set
			{
				switch (index)
				{
					case 0: m_float1 = value; return;
					case 1: m_float2 = value; return;
					case 2: m_float3 = value; return;
					case 3: m_float4 = value; return;
					case 4: m_float5 = value; return;
					case 5: m_float6 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,5]");
			}
		}
		int IPseudoArray<float>.Count => 6;
		#endregion

		#region Ctor

		public Vec6f(IPseudoArray<float> array) : this()
		{
			if (array is null)
			{
				return;
			}
			var count = array.Count;
			const int elementCount = 6;
			if (count > elementCount)
			{
				count = elementCount;
			}
			for (int i = 0; i < count; i++)
			{
				try
				{
					this[i] = array[i];
				}
				finally
				{
				}
			}
		}

		public Vec6f(params float[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 6)
				{
					length = 6;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}

		#endregion

		#region equality operator
		public static bool operator ==(Vec6f left, Vec6f right) => Equals(left, right);
		public static bool operator !=(Vec6f left, Vec6f right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec6f DeepCopy()
		{
			return new Vec6f(this);
		}

		public void DeepCopyValueFrom(Vec6f obj)
		{
			m_float1 = obj.m_float1;
			m_float2 = obj.m_float2;
			m_float3 = obj.m_float3;
			m_float4 = obj.m_float4;
			m_float5 = obj.m_float5;
			m_float6 = obj.m_float6;
		}
		#endregion

		#region IEquatable
		private static bool Equals(Vec6f left, Vec6f right)
		{
			return left.m_float1 == right.m_float1
				&& left.m_float2 == right.m_float2
				&& left.m_float3 == right.m_float3
				&& left.m_float4 == right.m_float4
				&& left.m_float5 == right.m_float5
				&& left.m_float6 == right.m_float6;
		}


		public override bool Equals(object obj)
		{
			if(obj is Vec6f vec6f)
			{
				return Equals(vec6f);
			}
			return false;
		}

		public bool Equals(Vec6f other)
		{
			return Equals(this, other);
		}

		public override int GetHashCode()
		{
			return m_float1.GetHashCode()
				^ m_float2.GetHashCode()
				^ m_float3.GetHashCode()
				^ m_float4.GetHashCode()
				^ m_float5.GetHashCode()
				^ m_float6.GetHashCode();
		}

		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec6f(this);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<float> Float32Enumeration(Vec6f vec6f)
		{
			yield return vec6f.m_float1;
			yield return vec6f.m_float2;
			yield return vec6f.m_float3;
			yield return vec6f.m_float4;
			yield return vec6f.m_float5;
			yield return vec6f.m_float6;
			yield break;
		}
		#endregion

		#region IEnumerable
		public IEnumerator<float> GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}
		#endregion
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec8f :
		IDeepCopyable<Vec8f>,
		IEquatable<Vec8f>,
		ICloneable,
		IPseudoArray<float>
	{

		[FieldOffset(0)]
		private float m_float1;
		[FieldOffset(4)]
		private float m_float2;
		[FieldOffset(8)]
		private float m_float3;
		[FieldOffset(12)]
		private float m_float4;
		[FieldOffset(16)]
		private float m_float5;
		[FieldOffset(20)]
		private float m_float6;
		[FieldOffset(24)]
		private float m_float7;
		[FieldOffset(28)]
		private float m_float8;

		#region IPseudoArray
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0: return m_float1;
					case 1: return m_float2;
					case 2: return m_float3;
					case 3: return m_float4;
					case 4: return m_float5;
					case 5: return m_float6;
					case 6: return m_float7;
					case 7: return m_float8;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,7]");
			}
			set
			{
				switch (index)
				{
					case 0: m_float1 = value; return;
					case 1: m_float2 = value; return;
					case 2: m_float3 = value; return;
					case 3: m_float4 = value; return;
					case 4: m_float5 = value; return;
					case 5: m_float6 = value; return;
					case 6: m_float7 = value; return;
					case 7: m_float8 = value; return;
					default:
						break;
				}
				throw new IndexOutOfRangeException("index should be in range of [0,7]");
			}
		}
		int IPseudoArray<float>.Count => 8;
		#endregion

		#region Ctor

		public Vec8f(IPseudoArray<float> array) : this()
		{
			if (array is null)
			{
				return;
			}
			var count = array.Count;
			const int elementCount = 8;
			if (count > elementCount)
			{
				count = elementCount;
			}
			for (int i = 0; i < count; i++)
			{
				try
				{
					this[i] = array[i];
				}
				finally
				{
				}
			}
		}

		public Vec8f(params float[] array) : this()
		{
			if (array != null)
			{
				var length = array.Length;
				if (length > 8)
				{
					length = 8;
				}
				for (int i = 0; i < length; i++)
				{
					this[i] = array[i];
				}
			}
		}

		#endregion

		#region equality operator
		public static bool operator ==(Vec8f left, Vec8f right) => Equals(left, right);
		public static bool operator !=(Vec8f left, Vec8f right) => !Equals(left, right);
		#endregion

		#region IDeepCopyable
		public Vec8f DeepCopy()
		{
			return new Vec8f(this);
		}

		public void DeepCopyValueFrom(Vec8f obj)
		{
			m_float1 = obj.m_float1;
			m_float2 = obj.m_float2;
			m_float3 = obj.m_float3;
			m_float4 = obj.m_float4;

			m_float5 = obj.m_float5;
			m_float6 = obj.m_float6;
			m_float7 = obj.m_float7;
			m_float8 = obj.m_float8;
		}
		#endregion

		#region IEquatable
		private static bool Equals(Vec8f left, Vec8f right)
		{
			return left.m_float1 == right.m_float1
				&& left.m_float2 == right.m_float2
				&& left.m_float3 == right.m_float3
				&& left.m_float4 == right.m_float4
				&& left.m_float5 == right.m_float5
				&& left.m_float6 == right.m_float6
				&& left.m_float7 == right.m_float7
				&& left.m_float8 == right.m_float8;
		}


		public override bool Equals(object obj)
		{
			if (obj is Vec8f vec8f)
			{
				return Equals(vec8f);
			}
			return false;
		}

		public bool Equals(Vec8f other)
		{
			return Equals(this, other);
		}

		public override int GetHashCode()
		{
			return m_float1.GetHashCode()
				^ m_float2.GetHashCode()
				^ m_float3.GetHashCode()
				^ m_float4.GetHashCode()
				^ m_float5.GetHashCode()
				^ m_float6.GetHashCode()
				^ m_float7.GetHashCode()
				^ m_float8.GetHashCode();
		}

		#endregion

		#region ICloneable
		public object Clone()
		{
			return new Vec8f(this);
		}
		#endregion

		#region Enumerations
		private static IEnumerable<float> Float32Enumeration(Vec8f vec8f)
		{
			yield return vec8f.m_float1;
			yield return vec8f.m_float2;
			yield return vec8f.m_float3;
			yield return vec8f.m_float4;
			yield return vec8f.m_float5;
			yield return vec8f.m_float6;
			yield return vec8f.m_float7;
			yield return vec8f.m_float8;
			yield break;
		}
		#endregion

		#region IEnumerable
		public IEnumerator<float> GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Float32Enumeration(this).GetEnumerator();
		}
		#endregion
	}

	public enum DataStatus : byte
	{
		DataIsCurrent,
		DataIsNew
	}

	public interface IDataChangeExecutable
	{
		void SubmitDataChanged();
		bool IsDataChanged { get; }
		DataStatus DataStatus { get; }
		void ExecuteDataChanged();
	}

	public interface IValueContainer<T> 
	{
		T Value { get; set; }
	}

	public class EllipseF : IValueContainer<Vec4f>, IEquatable<EllipseF>, IDeepCopyable<EllipseF>
	{
		private Vec4f m_value;
		public Vec4f Value { get => m_value; set => m_value = value; }
		public EllipseF()
		{
			m_value = new Vec4f();
		}

		public EllipseF(float centerX, float centerY, float radiusX, float radiusY)
		{
			m_value = new Vec4f(centerX, centerY, radiusX, radiusY);
		}

		public EllipseF(Vec4f float4)
		{
			m_value = float4.DeepCopy();
		}

		public EllipseF(EllipseF ellipseF)
		{
			if (ellipseF is null)
			{
				m_value = new Vec4f();
			}
			else
			{
				m_value = ellipseF.m_value.DeepCopy();
			}
		}

		public float CenterX { get => m_value[0]; set => m_value[0] = value; }
		public float CenterY { get => m_value[1]; set => m_value[1] = value; }
		public float RadiusX { get => m_value[2]; set => m_value[2] = value; }
		public float RadiusY { get => m_value[3]; set => m_value[3] = value; }

		public object Clone()
		{
			return DeepCopy();
		}

		public EllipseF DeepCopy()
		{
			return new EllipseF(this);
		}

		public void DeepCopyValueFrom(EllipseF obj)
		{
			if (obj is null)
			{
				return;
			}

			m_value = obj.m_value.DeepCopy();
		}

		public bool Equals(EllipseF other)
		{
			if (other is null)
			{
				return false;
			}

			return m_value.Equals(other.m_value);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public override string ToString()
		{
			return $"EllipseF {{Center ({CenterX},{CenterY}); RadiusX ({RadiusX}); RadiusY ({RadiusY})}}";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as EllipseF);
		}

		public static bool operator ==(EllipseF left, EllipseF right)
		{
			if (left is null)
			{
				if (right is null)
				{
					return true;
				}
				return false;
			}

			return left.Equals(right);
		}

		public static bool operator !=(EllipseF left, EllipseF right)
		{
			return !(left == right);
		}
	}

	[Flags]
	public enum OrientationFlags : byte
	{
		Unknown,
		Horizontal,
		Vertical,
		Both = Horizontal | Vertical
	}

}
