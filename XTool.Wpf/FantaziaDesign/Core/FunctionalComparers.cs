using System;
using System.Collections.Generic;
using System.Text;

namespace FantaziaDesign.Core
{
	public interface IFunctionalComparer<T> : IComparer<T>
	{
		Func<T, T, int> CompareFunction { get; set; }
	}

	public interface IFunctionalEqualityComparer<T> : IEqualityComparer<T>
	{
		Func<T, T, bool> EqualityFunction { get; set; }
		Func<T, int> HashFunction { get; set; }
	}


	public class FunctionalComparer<T> : IFunctionalComparer<T>
	{
		private Func<T, T, int> m_compareFunc;

		public FunctionalComparer()
		{
		}

		public FunctionalComparer(Func<T, T, int> comparer)
		{
			m_compareFunc = comparer;
		}

		public Func<T, T, int> CompareFunction { get => m_compareFunc; set => m_compareFunc = value; }

		public int Compare(T x, T y)
		{
			return m_compareFunc(x, y);
		}
	}

	public class FunctionalEqualityComparer<T> : IFunctionalEqualityComparer<T>
	{
		private Func<T, T, bool> m_equalityFunction;
		private Func<T, int> m_hashFunction;

		public FunctionalEqualityComparer()
		{
		}

		public FunctionalEqualityComparer(Func<T, T, bool> equalityComparer, Func<T, int> hashFunction = null)
		{
			m_equalityFunction = equalityComparer;
			m_hashFunction = hashFunction;
		}

		public FunctionalEqualityComparer(IFunctionalComparer<T> functionalComparer, Func<T, int> hashFunction = null)
		{
			var compareFunction = functionalComparer?.CompareFunction;
			if (compareFunction != null)
			{
				m_equalityFunction = (left, right) => { return compareFunction(left, right) == 0; };
			}
			m_hashFunction = hashFunction;
		}

		public Func<T, T, bool> EqualityFunction { get => m_equalityFunction; set => m_equalityFunction = value; }
		public Func<T, int> HashFunction { get => m_hashFunction; set => m_hashFunction = value; }

		public bool Equals(T x, T y)
		{
			if (m_equalityFunction is null)
			{
				return EqualityComparer<T>.Default.Equals(x, y);
			}
			return m_equalityFunction(x, y);
		}

		public int GetHashCode(T obj)
		{
			if (m_hashFunction is null)
			{
				return obj.GetHashCode();
			}
			return m_hashFunction(obj);
		}
	}
}
