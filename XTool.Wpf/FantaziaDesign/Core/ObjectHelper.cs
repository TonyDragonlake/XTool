using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaziaDesign.Core
{
	public static class ObjectHelper
	{
		public static bool IsNull(this object obj)
		{
			return obj is null;
		}

		public static bool IsNotNull(this object obj)
		{
			return !(obj is null);
		}
	}

	public sealed class EqualityComparerWrapper<T> : IEqualityComparer<T>
	{
		private IComparer<T> _comparer;

		public EqualityComparerWrapper(IComparer<T> comparer)
		{
			_comparer = comparer;
		}

		public bool Equals(T x, T y)
		{
			return _comparer.Compare(x, y) == 0;
		}

		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}
	}


}
