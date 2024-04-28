using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaziaDesign.Core
{
	public static class StringEqualityComparers
	{
		public static bool EqualsCurrentCulture(this string myStr, string otherStr)
		{
			return string.Equals(myStr, otherStr, StringComparison.CurrentCulture);
		}

		public static bool EqualsCurrentCultureIgnoreCase(this string myStr, string otherStr)
		{
			return string.Equals(myStr, otherStr, StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool EqualsInvariantCulture(this string myStr, string otherStr)
		{
			return string.Equals(myStr, otherStr, StringComparison.InvariantCulture);
		}

		public static bool EqualsInvariantCultureIgnoreCase(this string myStr, string otherStr)
		{
			return string.Equals(myStr, otherStr, StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool EqualsOrdinal(this string myStr, string otherStr)
		{
			return string.Equals(myStr, otherStr, StringComparison.Ordinal);
		}

		public static bool EqualsOrdinalIgnoreCase(this string myStr, string otherStr)
		{
			return string.Equals(myStr, otherStr, StringComparison.OrdinalIgnoreCase);
		}
	}

	public sealed class StringEqualityComparer : IEqualityComparer<string>
	{
		public static readonly StringEqualityComparer CurrentCulture = new StringEqualityComparer(StringComparison.CurrentCulture);
		public static readonly StringEqualityComparer CurrentCultureIgnoreCase = new StringEqualityComparer(StringComparison.CurrentCultureIgnoreCase);
		public static readonly StringEqualityComparer InvariantCulture = new StringEqualityComparer(StringComparison.InvariantCulture);
		public static readonly StringEqualityComparer InvariantCultureIgnoreCase = new StringEqualityComparer(StringComparison.InvariantCultureIgnoreCase);
		public static readonly StringEqualityComparer Ordinal = new StringEqualityComparer(StringComparison.Ordinal);
		public static readonly StringEqualityComparer OrdinalIgnoreCase = new StringEqualityComparer(StringComparison.OrdinalIgnoreCase);

		private readonly StringComparison m_comparison;

		private StringEqualityComparer(StringComparison comparison)
		{
			m_comparison = comparison;
		}

		public bool Equals(string x, string y)
		{
			return string.Equals(x, y, m_comparison);
		}

		public int GetHashCode(string obj)
		{
			return obj.GetHashCode();
		}
	}
}
