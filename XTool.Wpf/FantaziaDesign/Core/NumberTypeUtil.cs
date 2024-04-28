using System;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public static class BitUtil
	{
		private static readonly int[] s_bitMaskList = new int[]
		{
			1,
			2,
			4,
			8,
			16,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048,
			4096,
			8192,
			16384,
			32768,
			65536,
			131072,
			262144,
			524288,
			1048576,
			2097152,
			4194304,
			8388608,
			16777216,
			33554432,
			67108864,
			134217728,
			268435456,
			536870912,
			1073741824,
			-2147483648
		};

		public static int GetMask32(int index)
		{
			if (index >= 0 && index < 32)
			{
				return s_bitMaskList[index];
			}
			return 0;
		}

		public static bool TryGetTrueMaskIndex32(int data, out int[] indexCollection)
		{
			return TryGetTrueMaskIndex32(data, 32, out indexCollection);
		}

		public static bool TryGetTrueMaskIndex32(int data, int limitedLength, out int[] indexCollection)
		{
			indexCollection = null;
			if (data == 0)
			{
				return false;
			}
			if (limitedLength > 0 && limitedLength <= 32)
			{
				List<int> indexColList = new List<int>(limitedLength);
				for (int i = 0; i < limitedLength; i++)
				{
					if ((data & 1) == 1)
					{
						indexColList.Add(i);
					}
					data >>= 1;
				}
				if (indexColList.Count > 0)
				{
					indexCollection = indexColList.ToArray();
					return true;
				}
			}
			return false;
		}

		public static bool GetBit32Status(ref int targetBits, int mask)
		{
			return (targetBits & mask) == mask;
		}

		public static void SetBit32Status(ref int targetBits, int mask, bool actived)
		{
			if (actived)
			{
				targetBits |= mask;
			}
			else
			{
				targetBits &= ~mask;
			}
		}

		public static bool GetBitStatus(ref byte targetBits, byte mask)
		{
			return (targetBits & mask) == mask;
		}

		public static void SetBitStatus(ref byte targetBits, byte mask, bool actived)
		{
			if (actived)
			{
				targetBits |= mask;
			}
			else
			{
				targetBits &= (byte)~mask;
			}
		}

	}

	public static class TimeUtil
	{
		private static RefAction<TimeSpan, long> SetTicks_TimeSpanRef;

		public static bool SetTimeTicks(ref TimeSpan timeSpan, long tick)
		{
			if (timeSpan.Ticks != tick)
			{
				if (SetTicks_TimeSpanRef is null)
				{
					SetTicks_TimeSpanRef = ReflectionUtil.BindInstanceFieldSetterToDelegateByRef<TimeSpan, long>(
						"_ticks",
						ReflectionUtil.NonPublicInstance,
						true
						);
				}
				SetTicks_TimeSpanRef(ref timeSpan, ref tick);
				return true;
			}
			return false;
		}

		public static bool SetTotalMilliseconds(ref TimeSpan timeSpan, double ms)
		{
			return SetTimeTicks(ref timeSpan, GetTicksFromMilliseconds(ms));
		}

		public static long GetTicksFromMilliseconds(double ms)
		{
			if (DoubleUtil.IsNaN(ms))
			{
				throw new ArgumentException("Argument Cannot Be NaN");
			}
			double ticks_d = ms * TimeSpan.TicksPerMillisecond;
			if (ticks_d > long.MaxValue || ticks_d < long.MinValue)
			{
				throw new OverflowException("Overflow : TimeSpanTooLong");
			}
			return Convert.ToInt64(ticks_d);
		}

	}

	public static class NumberUtil
	{
		public static readonly char SpaceChar = ' ';

		public static readonly double Tau = 6.283185307179586476925286766559;

		public static readonly float Tau32 = 6.283185307179586476925286766559f;

		public static bool IsNumber(object obj)
		{
			if (obj is null)
			{
				return false;
			}
			else
			{
				TypeCode tCode = Type.GetTypeCode(obj.GetType());
				if (tCode >= TypeCode.Char && tCode <= TypeCode.Decimal)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNumberType(Type type)
		{
			if (type is null)
			{
				return false;
			}
			else
			{
				TypeCode tCode = Type.GetTypeCode(type);
				if (tCode >= TypeCode.Char && tCode <= TypeCode.Decimal)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNumberType(Type type, out TypeCode typeCode)
		{
			if (type is null)
			{
				typeCode = TypeCode.Empty;
				return false;
			}
			else
			{
				typeCode = Type.GetTypeCode(type);
				if (typeCode >= TypeCode.Char && typeCode <= TypeCode.Decimal)
				{
					return true;
				}
			}
			return false;
		}

		public static T Addition<T>(T num1, T num2)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						resBox = (char)num1Box + (char)num2Box;
					}
					break;
				case TypeCode.SByte:
					{
						resBox = (sbyte)num1Box + (sbyte)num2Box;
					}
					break;
				case TypeCode.Byte:
					{
						resBox = (byte)num1Box + (byte)num2Box;
					}
					break;
				case TypeCode.Int16:
					{
						resBox = (short)num1Box + (short)num2Box;
					}
					break;
				case TypeCode.UInt16:
					{
						resBox = (ushort)num1Box + (ushort)num2Box;
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box + (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box + (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box + (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box + (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box + (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box + (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box + (decimal)num2Box;
					}
					break;
				default:
					break;
			}
			return (T)resBox;
		}

		public static bool Addition<T>(T num1, T num2, out T num3)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						resBox = (char)num1Box + (char)num2Box;
					}
					break;
				case TypeCode.SByte:
					{
						resBox = (sbyte)num1Box + (sbyte)num2Box;
					}
					break;
				case TypeCode.Byte:
					{
						resBox = (byte)num1Box + (byte)num2Box;
					}
					break;
				case TypeCode.Int16:
					{
						resBox = (short)num1Box + (short)num2Box;
					}
					break;
				case TypeCode.UInt16:
					{
						resBox = (ushort)num1Box + (ushort)num2Box;
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box + (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box + (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box + (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box + (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box + (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box + (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box + (decimal)num2Box;
					}
					break;
				default:
					{
						num3 = default(T);
						return false;
					}
			}
			num3 = (T)resBox;
			return true;
		}

		public static T Subtraction<T>(T num1, T num2)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						resBox = (char)num1Box - (char)num2Box;
					}
					break;
				case TypeCode.SByte:
					{
						resBox = (sbyte)num1Box - (sbyte)num2Box;
					}
					break;
				case TypeCode.Byte:
					{
						resBox = (byte)num1Box - (byte)num2Box;
					}
					break;
				case TypeCode.Int16:
					{
						resBox = (short)num1Box - (short)num2Box;
					}
					break;
				case TypeCode.UInt16:
					{
						resBox = (ushort)num1Box - (ushort)num2Box;
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box - (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box - (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box - (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box - (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box - (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box - (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box - (decimal)num2Box;
					}
					break;
				default:
					break;
			}
			return (T)resBox;
		}

		public static bool Subtraction<T>(T num1, T num2, out T num3)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						resBox = (char)num1Box - (char)num2Box;
					}
					break;
				case TypeCode.SByte:
					{
						resBox = (sbyte)num1Box - (sbyte)num2Box;
					}
					break;
				case TypeCode.Byte:
					{
						resBox = (byte)num1Box - (byte)num2Box;
					}
					break;
				case TypeCode.Int16:
					{
						resBox = (short)num1Box - (short)num2Box;
					}
					break;
				case TypeCode.UInt16:
					{
						resBox = (ushort)num1Box - (ushort)num2Box;
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box - (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box - (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box - (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box - (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box - (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box - (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box - (decimal)num2Box;
					}
					break;
				default:
					{
						num3 = default(T);
						return false;
					}
			}
			num3 = (T)resBox;
			return true;
		}

		public static T Multiplication<T>(T num1, T num2)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						resBox = (char)num1Box * (char)num2Box;
					}
					break;
				case TypeCode.SByte:
					{
						resBox = (sbyte)num1Box * (sbyte)num2Box;
					}
					break;
				case TypeCode.Byte:
					{
						resBox = (byte)num1Box * (byte)num2Box;
					}
					break;
				case TypeCode.Int16:
					{
						resBox = (short)num1Box * (short)num2Box;
					}
					break;
				case TypeCode.UInt16:
					{
						resBox = (ushort)num1Box * (ushort)num2Box;
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box * (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box * (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box * (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box * (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box * (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box * (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box * (decimal)num2Box;
					}
					break;
				default:
					break;
			}
			return (T)resBox;
		}

		public static bool Multiplication<T>(T num1, T num2, out T num3)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						resBox = (char)num1Box * (char)num2Box;
					}
					break;
				case TypeCode.SByte:
					{
						resBox = (sbyte)num1Box * (sbyte)num2Box;
					}
					break;
				case TypeCode.Byte:
					{
						resBox = (byte)num1Box * (byte)num2Box;
					}
					break;
				case TypeCode.Int16:
					{
						resBox = (short)num1Box * (short)num2Box;
					}
					break;
				case TypeCode.UInt16:
					{
						resBox = (ushort)num1Box * (ushort)num2Box;
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box * (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box * (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box * (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box * (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box * (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box * (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box * (decimal)num2Box;
					}
					break;
				default:
					{
						num3 = default(T);
						return false;
					}
			}
			num3 = (T)resBox;
			return true;
		}

		public static T Division<T>(T num1, T num2)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						var res = (char)num1Box / (char)num2Box;
						resBox = (char)(res);
					}
					break;
				case TypeCode.SByte:
					{
						var res = (sbyte)num1Box / (sbyte)num2Box;
						resBox = (sbyte)(res);
					}
					break;
				case TypeCode.Byte:
					{
						var res = (byte)num1Box / (byte)num2Box;
						resBox = (byte)(res);
					}
					break;
				case TypeCode.Int16:
					{
						var res = (short)num1Box / (short)num2Box;
						resBox = (short)(res);
					}
					break;
				case TypeCode.UInt16:
					{
						var res = (ushort)num1Box / (ushort)num2Box;
						resBox = (ushort)(res);
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box / (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box / (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box / (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box / (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box / (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box / (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box / (decimal)num2Box;
					}
					break;
				default:
					break;
			}
			return (T)resBox;
		}

		public static bool Division<T>(T num1, T num2, out T num3)
		{
			TypeCode tCode = Type.GetTypeCode(typeof(T));
			object num1Box = num1;
			object num2Box = num2;
			object resBox = default(T);
			switch (tCode)
			{
				case TypeCode.Char:
					{
						var res = (char)num1Box / (char)num2Box;
						resBox = (char)(res);
					}
					break;
				case TypeCode.SByte:
					{
						var res = (sbyte)num1Box / (sbyte)num2Box;
						resBox = (sbyte)(res);
					}
					break;
				case TypeCode.Byte:
					{
						var res = (byte)num1Box / (byte)num2Box;
						resBox = (byte)(res);
					}
					break;
				case TypeCode.Int16:
					{
						var res = (short)num1Box / (short)num2Box;
						resBox = (short)(res);
					}
					break;
				case TypeCode.UInt16:
					{
						var res = (ushort)num1Box / (ushort)num2Box;
						resBox = (ushort)(res);
					}
					break;
				case TypeCode.Int32:
					{
						resBox = (int)num1Box / (int)num2Box;
					}
					break;
				case TypeCode.UInt32:
					{
						resBox = (uint)num1Box / (uint)num2Box;
					}
					break;
				case TypeCode.Int64:
					{
						resBox = (long)num1Box / (long)num2Box;
					}
					break;
				case TypeCode.UInt64:
					{
						resBox = (ulong)num1Box / (ulong)num2Box;
					}
					break;
				case TypeCode.Single:
					{
						resBox = (float)num1Box / (float)num2Box;
					}
					break;
				case TypeCode.Double:
					{
						resBox = (double)num1Box / (double)num2Box;
					}
					break;
				case TypeCode.Decimal:
					{
						resBox = (decimal)num1Box / (decimal)num2Box;
					}
					break;
				default:
					{
						num3 = default(T);
						return false;
					}
			}
			num3 = (T)resBox;
			return true;
		}
	}

	public enum ResultOp
	{
		AND,
		OR,
		NOT,
		NAND,
		NOR,
		XOR
	}

	public static class DoubleUtil
	{
		public enum RestrictionRule
		{
			AllowNone,
			AllowNegative = 0b0001,
			AllowNaN = 0b0010,
			AllowPositiveInfinity = 0b0100,
			AllowNegativeInfinity = 0b1000,
			AllowAny = 0b1111
		}

		public static bool AreClose(double value1, double value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
			double delta = value1 - value2;
			return -eps < delta && eps > delta;
		}

		public static bool LessThan(double value1, double value2)
		{
			return value1 < value2 && !AreClose(value1, value2);
		}

		public static bool GreaterThan(double value1, double value2)
		{
			return value1 > value2 && !AreClose(value1, value2);
		}

		public static bool LessThanOrClose(double value1, double value2)
		{
			return value1 < value2 || AreClose(value1, value2);
		}

		public static bool GreaterThanOrClose(double value1, double value2)
		{
			return value1 > value2 || AreClose(value1, value2);
		}

		public static bool IsOne(double value)
		{
			return Math.Abs(value - 1.0) < 2.2204460492503131E-15;
		}

		public static bool IsZero(double value)
		{
			return Math.Abs(value) < 2.2204460492503131E-15;
		}

		public static bool IsBetweenZeroAndOne(double val)
		{
			return GreaterThanOrClose(val, 0.0) && LessThanOrClose(val, 1.0);
		}

		public static int DoubleToInt(double val)
		{
			return (0 < val) ? (int)(val + 0.5) : (int)(val - 0.5);
		}

		public static bool IsNaN(double value)
		{
			CompatibleInt64 t = value;
			var exp = t.UValue & 0xfff0000000000000;
			var man = t.UValue & 0x000fffffffffffff;
			return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
		}

		public static void SetIfNonnegative(ref double target, double value, double defaultValue = 0.0)
		{
			if (value < 0.0)
			{
				value = defaultValue;
			}
			if (target != value)
			{
				target = value;
			}
		}

		internal static double RoundLayoutValue(double value, double dpiScale)
		{
			double newValue;

			// If DPI == 1, don't use DPI-aware rounding.
			if (!DoubleUtil.AreClose(dpiScale, 1.0))
			{
				newValue = Math.Round(value * dpiScale) / dpiScale;
				// If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
				if (DoubleUtil.IsNaN(newValue) ||
					Double.IsInfinity(newValue) ||
					DoubleUtil.AreClose(newValue, Double.MaxValue))
				{
					newValue = value;
				}
			}
			else
			{
				newValue = Math.Round(value);
			}

			return newValue;
		}

		public static bool IsFinite(double d)
		{
			long num = ((CompatibleInt64)d).Value;
			return (num & long.MaxValue) < 9218868437227405312L;
		}

		//internal const double DBL_EPSILON = 2.2204460492503131E-16;

		//internal const float FLT_MIN = 1.17549435E-38f;

		public static bool IsValid(RestrictionRule rule = RestrictionRule.AllowNone, params double[] array)
		{
			if (rule >= RestrictionRule.AllowAny)
			{
				return true;
			}
			if (array is null)
			{
				return false;
			}
			var len = array.Length;
			var ruleVal = (int)rule;

			int i;
			if (!BitUtil.GetBit32Status(ref ruleVal, (int)RestrictionRule.AllowNegative))
			{
				i = 0;
				while (i < len)
				{
					if (array[i] < 0)
					{
						return false;
					}
					i++;
				}
			}
			if (!BitUtil.GetBit32Status(ref ruleVal, (int)RestrictionRule.AllowNaN))
			{
				i = 0;
				while (i < len)
				{
					if (DoubleUtil.IsNaN(array[i]))
					{
						return false;
					}
					i++;
				}
			}
			if (!BitUtil.GetBit32Status(ref ruleVal, (int)RestrictionRule.AllowPositiveInfinity))
			{
				i = 0;
				while (i < len)
				{
					if (double.IsPositiveInfinity(array[i]))
					{
						return false;
					}
					i++;
				}
			}
			if (!BitUtil.GetBit32Status(ref ruleVal, (int)RestrictionRule.AllowNegativeInfinity))
			{
				i = 0;
				while (i < len)
				{
					if (double.IsNegativeInfinity(array[i]))
					{
						return false;
					}
					i++;
				}
			}
			return true;
		}

	}

	public static class FloatUtil
	{
		public static bool AreClose(float value1, float value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			float num = (Math.Abs(value1) + Math.Abs(value2) + 10f) * 1.1920929E-07f;
			float num2 = value1 - value2;
			return -num < num2 && num > num2;
		}

		public static bool LessThan(float value1, float value2)
		{
			return value1 < value2 && !AreClose(value1, value2);
		}

		public static bool GreaterThan(float value1, float value2)
		{
			return value1 > value2 && !AreClose(value1, value2);
		}

		public static bool LessThanOrClose(float value1, float value2)
		{
			return value1 < value2 || AreClose(value1, value2);
		}

		public static bool GreaterThanOrClose(float value1, float value2)
		{
			return value1 > value2 || AreClose(value1, value2);
		}

		public static bool IsOne(float value)
		{
			return Math.Abs(value - 1.0f) < 1.1920929E-07f;
		}

		public static bool IsZero(float value)
		{
			return Math.Abs(value) < 1.1920929E-07f;
		}

		public static bool IsBetweenZeroAndOne(float val)
		{
			return GreaterThanOrClose(val, 0.0f) && LessThanOrClose(val, 1.0f);
		}

		public static int FloatToInt(float val)
		{
			if (0.0f >= val)
			{
				return (int)(val - 0.5f);
			}
			return (int)(val + 0.5f);
		}

		internal static float RoundLayoutValue(float value, float dpiScale)
		{
			return Convert.ToSingle(DoubleUtil.RoundLayoutValue(value, dpiScale));
		}

		public static void SetIfNonnegative(ref float target, float value, float defaultValue = 0f)
		{
			if (value < 0f)
			{
				value = defaultValue;
			}
			if (target != value)
			{
				target = value;
			}
		}

		public static bool IsApproximatelyGreaterOrEquals(float val1, float val2, float tolerance = 1E-05f)
		{
			return val1 + tolerance >= val2 || val1 - tolerance >= val2;
		}

		public static bool IsApproximatelyLessOrEquals(float val1, float val2, float tolerance = 1E-05f)
		{
			return val1 + tolerance <= val2 || val1 - tolerance <= val2;
		}

		public static bool IsApproximatelyGreater(float val1, float val2, float tolerance = 1E-05f)
		{
			return val1 + tolerance > val2 || val1 - tolerance > val2;
		}

		public static bool IsApproximatelyLess(float val1, float val2, float tolerance = 1E-05f)
		{
			return val1 + tolerance < val2 || val1 - tolerance < val2;
		}

		public static bool IsApproximatelyEquals(float val1, float val2, float tolerance = 1E-05f)
		{
			return Math.Abs(val1 - val2) <= tolerance;
		}

	}
}
