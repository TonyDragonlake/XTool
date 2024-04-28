using System;
using System.Collections;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public class UniqueSet<TFirstKey, TSecondKey> :
		ICollection<IReadOnlyPair<TFirstKey, TSecondKey>>,
		IEnumerable<IReadOnlyPair<TFirstKey, TSecondKey>>,
		IReadOnlyCollection<IReadOnlyPair<TFirstKey, TSecondKey>>
	{
		private HashSet<PairRef<TFirstKey, TSecondKey>> m_firstKeySet;
		private HashSet<PairRef<TFirstKey, TSecondKey>> m_secondKeySet;
		private IEqualityComparer<TFirstKey> m_firstKeyComparer;
		private IEqualityComparer<TSecondKey> m_secondKeyComparer;
		private PairRef<TFirstKey, TSecondKey> m_cache;
		private int m_count;

		public UniqueSet() : this(null, null)
		{
		}

		public UniqueSet(IEqualityComparer<TFirstKey> firstKeyComparer, IEqualityComparer<TSecondKey> secondKeyComparer)
		{
			if (firstKeyComparer is null)
			{
				firstKeyComparer = EqualityComparer<TFirstKey>.Default;
			}

			if (secondKeyComparer is null)
			{
				secondKeyComparer = EqualityComparer<TSecondKey>.Default;
			}
			m_firstKeyComparer = firstKeyComparer;
			m_secondKeyComparer = secondKeyComparer;
			m_firstKeySet = new HashSet<PairRef<TFirstKey, TSecondKey>>(
				new FunctionalEqualityComparer<PairRef<TFirstKey, TSecondKey>>(
					EqualityCompareItem1
					)
				);
			m_secondKeySet = new HashSet<PairRef<TFirstKey, TSecondKey>>(
				new FunctionalEqualityComparer<PairRef<TFirstKey, TSecondKey>>(
					EqualityCompareItem2
					)
				);
			m_cache = new PairRef<TFirstKey, TSecondKey>();
		}

		public int Count => m_count;

		public bool IsReadOnly => false;

		public bool Add(IReadOnlyPair<TFirstKey, TSecondKey> item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}
			return AddCore(new PairRef<TFirstKey, TSecondKey>(item));
		}

		public bool Add(TFirstKey firstKey, TSecondKey secondKey)
		{
			return AddCore(new PairRef<TFirstKey, TSecondKey>(firstKey, secondKey));
		}

		private bool AddCore(PairRef<TFirstKey, TSecondKey> pair)
		{
			if (m_firstKeySet.Contains(pair) || m_secondKeySet.Contains(pair))
			{
				return false;
			}
			m_firstKeySet.Add(pair);
			m_secondKeySet.Add(pair);
			m_count++;
			return true;
		}

		public void Clear()
		{
			m_firstKeySet.Clear();
			m_secondKeySet.Clear();
		}

		public bool Contains(IReadOnlyPair<TFirstKey, TSecondKey> item)
		{
			m_cache.SetPair(item);
			return m_firstKeySet.Contains(m_cache) && m_secondKeySet.Contains(m_cache);
		}

		public bool ContainsFirstKey(TFirstKey firstKey)
		{
			m_cache.First = firstKey;
			return m_firstKeySet.Contains(m_cache);
		}

		public bool ContainsSecondKey(TSecondKey secondKey)
		{
			m_cache.Second = secondKey;
			return m_secondKeySet.Contains(m_cache);
		}

		void ICollection<IReadOnlyPair<TFirstKey, TSecondKey>>.CopyTo(IReadOnlyPair<TFirstKey, TSecondKey>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<IReadOnlyPair<TFirstKey, TSecondKey>> GetEnumerator()
		{
			return m_firstKeySet.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection<IReadOnlyPair<TFirstKey, TSecondKey>>.Add(IReadOnlyPair<TFirstKey, TSecondKey> item)
		{
			Add(item);
		}

		public bool Remove(IReadOnlyPair<TFirstKey, TSecondKey> keyPair)
		{
			m_cache.SetPair(keyPair);
			var contains1 = m_firstKeySet.Contains(m_cache);
			var contains2 = m_secondKeySet.Contains(m_cache);
			if (contains1)
			{
				m_firstKeySet.Remove(m_cache);
			}
			if (contains2)
			{
				m_firstKeySet.Remove(m_cache);
			}
			if (!contains1 && !contains2) return false;
			m_count--;
			return true;
		}

		public bool RemoveByFirstKey(TFirstKey firstKey)
		{
			m_cache.First = firstKey;
			if (m_firstKeySet.TryGetValue(m_cache, out var actualValue))
			{
				m_firstKeySet.Remove(actualValue);
				m_secondKeySet.Remove(actualValue);
				m_count--;
				return true;
			}
			return false;
		}

		public bool RemoveBySecondKey(TSecondKey secondKey)
		{
			m_cache.Second = secondKey;
			if (m_secondKeySet.TryGetValue(m_cache, out var actualValue))
			{
				m_firstKeySet.Remove(actualValue);
				m_secondKeySet.Remove(actualValue);
				m_count--;
				return true;
			}
			return false;
		}

		public bool ReplaceFirstKey(TFirstKey oldItem, TFirstKey newItem)
		{
			m_cache.First = oldItem;
			if (m_firstKeySet.TryGetValue(m_cache, out var actualValue))
			{
				if (!m_secondKeySet.Contains(actualValue))
				{
					m_secondKeySet.Add(actualValue);
				}
				m_firstKeySet.Remove(actualValue);
				actualValue.First = newItem;
				m_firstKeySet.Add(actualValue);
				return true;
			}
			return false;
		}

		public bool ReplaceSecondKey(TSecondKey oldItem, TSecondKey newItem)
		{
			m_cache.Second = oldItem;
			if (m_secondKeySet.TryGetValue(m_cache, out var actualValue))
			{
				if (!m_firstKeySet.Contains(actualValue))
				{
					m_firstKeySet.Add(actualValue);
				}
				m_secondKeySet.Remove(actualValue);
				actualValue.Second = newItem;
				m_secondKeySet.Add(actualValue);
				return true;
			}
			return false;
		}

		public bool TryGetSecondKey(TFirstKey firstKey, out TSecondKey secondKey)
		{
			m_cache.First = firstKey;
			if (m_firstKeySet.TryGetValue(m_cache, out var actualValue))
			{
				if (!m_secondKeySet.Contains(actualValue))
				{
					m_firstKeySet.Remove(actualValue);
				}
				secondKey = actualValue.Second;
				return true;
			}
			secondKey = default(TSecondKey);
			return false;
		}

		public bool TryGetFirstKey(TSecondKey secondKey, out TFirstKey firstKey)
		{
			m_cache.Second = secondKey;
			if (m_secondKeySet.TryGetValue(m_cache, out var actualValue))
			{
				if (!m_firstKeySet.Contains(actualValue))
				{
					m_secondKeySet.Remove(actualValue);
				}
				firstKey = actualValue.First;
				return true;
			}
			firstKey = default(TFirstKey);
			return false;
		}

		public bool TryGetSecondKeyRef(TFirstKey firstKey, out TypedRef<TSecondKey> secondKeyRef)
		{
			m_cache.First = firstKey;
			if (m_firstKeySet.TryGetValue(m_cache, out var actualValue))
			{
				if (!m_secondKeySet.Contains(actualValue))
				{
					m_firstKeySet.Remove(actualValue);
				}
				secondKeyRef = actualValue.SecondRef;
				return true;
			}
			secondKeyRef = null;
			return false;
		}

		public bool TryGetFirstKeyRef(TSecondKey secondKey, out TypedRef<TFirstKey> firstKeyRef)
		{
			m_cache.Second = secondKey;
			if (m_secondKeySet.TryGetValue(m_cache, out var actualValue))
			{
				if (!m_firstKeySet.Contains(actualValue))
				{
					m_secondKeySet.Remove(actualValue);
				}
				firstKeyRef = actualValue.FirstRef;
				return true;
			}
			firstKeyRef = null;
			return false;
		}

		private bool EqualityCompareItem1(PairRef<TFirstKey, TSecondKey> left, PairRef<TFirstKey, TSecondKey> right)
		{
			return m_firstKeyComparer.Equals(left.First, right.First);
		}

		private bool EqualityCompareItem2(PairRef<TFirstKey, TSecondKey> left, PairRef<TFirstKey, TSecondKey> right)
		{
			return m_secondKeyComparer.Equals(left.Second, right.Second);
		}
	}

}
