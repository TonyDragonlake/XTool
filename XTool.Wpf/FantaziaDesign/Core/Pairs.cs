using System;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public interface IReadOnlyPair<TFirst, TSecond>
	{
		TFirst First { get; }
		TSecond Second { get; }
	}

	public class Pair<TFirst, TSecond> : IReadOnlyPair<TFirst, TSecond>, IEquatable<Pair<TFirst, TSecond>>
	{
		protected TFirst m_first;
		protected TSecond m_second;

		public Pair() : this(default, default)
		{
		}

		public Pair(TFirst first, TSecond second)
		{
			m_first = first;
			m_second = second;
		}

		public Pair(IReadOnlyPair<TFirst, TSecond> pairItems) : this(pairItems.First, pairItems.Second)
		{
		}

		public TFirst First { get => m_first; set => m_first = value; }

		public TSecond Second { get => m_second; set => m_second = value; }

		public void SetPair(TFirst item1, TSecond item2)
		{
			m_first = item1;
			m_second = item2;
		}

		public void SetPair(IReadOnlyPair<TFirst, TSecond> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			SetPair(items.First, items.Second);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Pair<TFirst, TSecond>);
		}

		public virtual bool Equals(Pair<TFirst, TSecond> other)
		{
			return !(other is null) &&
				   EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) &&
				   EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second);
		}

		public override int GetHashCode()
		{
			return m_first.GetHashCode() ^ m_second.GetHashCode();
		}
	}

	public class PairRef<TFirst, TSecond> : IReadOnlyPair<TFirst, TSecond>, IEquatable<PairRef<TFirst, TSecond>>
	{
		protected TypedRef<TFirst> m_first;
		protected TypedRef<TSecond> m_second;

		public PairRef() : this (default, default)
		{
		}

		public PairRef(TFirst first, TSecond second)
		{
			m_first = first;
			m_second = second;
		}

		public PairRef(IReadOnlyPair<TFirst, TSecond> pairItems) : this(pairItems.First, pairItems.Second)
		{
		}

		public TFirst First { get => m_first; set => TypedRef<TFirst>.CriticalSetItem(m_first, value); }

		public TSecond Second { get => m_second; set => TypedRef<TSecond>.CriticalSetItem(m_second, value); }

		public TypedRef<TFirst> FirstRef => m_first;

		public TypedRef<TSecond> SecondRef => m_second;

		public void SetPair(TFirst item1, TSecond item2)
		{
			TypedRef<TFirst>.CriticalSetItem(m_first, item1);
			TypedRef<TSecond>.CriticalSetItem(m_second, item2);
		}

		public void SetPair(IReadOnlyPair<TFirst, TSecond> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			SetPair(items.First, items.Second);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as PairRef<TFirst, TSecond>);
		}

		public virtual bool Equals(PairRef<TFirst, TSecond> other)
		{
			return !(other is null) &&
				   EqualityComparer<TFirst>.Default.Equals(m_first, other.m_first) &&
				   EqualityComparer<TSecond>.Default.Equals(m_second, other.m_second);
		}

		public override int GetHashCode()
		{
			return m_first.GetHashCode() ^ m_second.GetHashCode();
		}
	}

}
