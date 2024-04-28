using System;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public static class StringParsers
	{
		private static Dictionary<Type, object> s_parsers = new Dictionary<Type, object>();

		public static IStringParser<T> GetDefaultStringParser<T>()
		{
			if (s_parsers.Count == 0)
			{
				InitBuiltInTypeParsers();
			}
			var type = typeof(T);
			if (s_parsers.TryGetValue(type, out var obj))
			{
				var parser = obj as IStringParser<T>;
				if (parser is null)
				{
					if (type.IsEnum)
					{
						obj = Activator.CreateInstance(typeof(EnumToStringParser<>).MakeGenericType(type));
						parser = obj as IStringParser<T>;
						if (parser != null)
						{
							s_parsers[type] = parser;
							return parser;
						}
					}
				}
				else
				{
					return parser;
				}
			}
			return null;
		}

		private static void InitBuiltInTypeParsers()
		{
			RegisterDefaultStringParser(new BooleanToStringParser());
			RegisterDefaultStringParser(new CharToStringParser());
			RegisterDefaultStringParser(new SByteToStringParser());
			RegisterDefaultStringParser(new ByteToStringParser());
			RegisterDefaultStringParser(new Int16ToStringParser());
			RegisterDefaultStringParser(new UInt16ToStringParser());
			RegisterDefaultStringParser(new Int32ToStringParser());
			RegisterDefaultStringParser(new UInt32ToStringParser());
			RegisterDefaultStringParser(new Int64ToStringParser());
			RegisterDefaultStringParser(new UInt64ToStringParser());
			RegisterDefaultStringParser(new SingleToStringParser());
			RegisterDefaultStringParser(new DoubleToStringParser());
			RegisterDefaultStringParser(new DecimalToStringParser());
			RegisterDefaultStringParser(new DateTimeToStringParser());
			RegisterDefaultStringParser(new StringToStringParser());
			RegisterDefaultStringParser(new GuidToStringParser());
		}

		public static bool RegisterDefaultStringParser<T>(IStringParser<T> stringParser)
		{
			var type = typeof(T);
			if (s_parsers.TryGetValue(type, out var obj))
			{
				var parser = obj as IStringParser<T>;
				if (parser is null)
				{
					s_parsers[type] = stringParser;
					return true;
				}
				return false;
			}
			else
			{
				s_parsers.Add(type, stringParser);
				return true;
			}
		}

		public static object GetDefaultStringParser(Type type)
		{
			if (s_parsers.Count == 0)
			{
				InitBuiltInTypeParsers();
			}
			if (s_parsers.TryGetValue(type, out var obj))
			{
				var parserType = typeof(IStringParser<>).MakeGenericType(type);
				if (parserType.IsAssignableFrom(obj.GetType()))
				{
					return obj;
				}
			}
			return null;
		}
	}

	public interface IStringParser<T>
	{
		bool TryParseString(string str, out T result);
	}

	public sealed class BooleanToStringParser : IStringParser<bool>
	{
		public bool TryParseString(string str, out bool result)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				result = false;
				return true;
			}
			if (bool.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class CharToStringParser : IStringParser<char>
	{
		public bool TryParseString(string str, out char result)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				result = (char)0;
				return false;
			}
			if (!char.TryParse(str, out result))
			{
				result = str[0];
			}
			return true;
		}
	}

	public sealed class SByteToStringParser : IStringParser<sbyte>
	{
		public bool TryParseString(string str, out sbyte result)
		{
			if (sbyte.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class ByteToStringParser : IStringParser<byte>
	{
		public bool TryParseString(string str, out byte result)
		{
			if (byte.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class Int16ToStringParser : IStringParser<Int16>
	{
		public bool TryParseString(string str, out Int16 result)
		{
			if (Int16.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class UInt16ToStringParser : IStringParser<UInt16>
	{
		public bool TryParseString(string str, out UInt16 result)
		{
			if (UInt16.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class Int32ToStringParser : IStringParser<Int32>
	{
		public bool TryParseString(string str, out Int32 result)
		{
			if (Int32.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class UInt32ToStringParser : IStringParser<UInt32>
	{
		public bool TryParseString(string str, out UInt32 result)
		{
			if (UInt32.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class Int64ToStringParser : IStringParser<Int64>
	{
		public bool TryParseString(string str, out Int64 result)
		{
			if (Int64.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class UInt64ToStringParser : IStringParser<UInt64>
	{
		public bool TryParseString(string str, out UInt64 result)
		{
			if (UInt64.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class SingleToStringParser : IStringParser<Single>
	{
		public bool TryParseString(string str, out Single result)
		{
			if (Single.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class DoubleToStringParser : IStringParser<Double>
	{
		public bool TryParseString(string str, out Double result)
		{
			if (Double.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class DecimalToStringParser : IStringParser<Decimal>
	{
		public bool TryParseString(string str, out Decimal result)
		{
			if (Decimal.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class DateTimeToStringParser : IStringParser<DateTime>
	{
		public bool TryParseString(string str, out DateTime result)
		{
			if (DateTime.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class StringToStringParser : IStringParser<string>
	{
		public bool TryParseString(string str, out string result)
		{
			result = str;
			return true;
		}
	}

	public sealed class GuidToStringParser : IStringParser<Guid>
	{
		public bool TryParseString(string str, out Guid result)
		{
			if (Guid.TryParse(str, out result))
			{
				return true;
			}
			return false;
		}
	}

	public sealed class EnumToStringParser<T> : IStringParser<T> where T : struct, Enum
	{
		public bool TryParseString(string str, out T result)
		{
			return Enum.TryParse(str, out result);
		}
	}

}
