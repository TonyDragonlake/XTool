using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FantaziaDesign.Core
{
	internal sealed class ReadOnlyCollectionImpl<T> : IReadOnlyCollection<T>
	{
		private ICollection<T> m_source;

		public ReadOnlyCollectionImpl(ICollection<T> source)
		{
			m_source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public int Count => m_source.Count;

		public IEnumerator<T> GetEnumerator()
		{
			return m_source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	internal sealed class ReadOnlyListImpl<T> : IReadOnlyList<T>
	{
		private IList<T> m_source;

		public ReadOnlyListImpl(IList<T> source)
		{
			m_source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public T this[int index] => m_source[index];

		public int Count => m_source.Count;

		public IEnumerator<T> GetEnumerator()
		{
			return m_source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public static class CollectionHelper
	{
		internal static bool TryExtendCapacity(ref int currentCapacity, int minCapacity)
		{
			if (currentCapacity < minCapacity)
			{
				int newCapacity = currentCapacity + (currentCapacity >> 1);
				if (newCapacity < minCapacity)
				{
					currentCapacity = minCapacity;
				}
				else
				{
					currentCapacity = newCapacity;
				}
				return true;
			}
			return false;
		}

		internal static IReadOnlyCollection<T> ChildrenItems<T>(T node)
		{
			var treeNode = node as IReadOnlyChildrenContainer<T>;
			return treeNode?.Children;
		}

		public static IEnumerable<T> Range<T>(this IReadOnlyList<T> list, int fromIndex, int toIndex)
		{
			var len = list.Count;
			if (toIndex >= fromIndex)
			{
				if (fromIndex < 0)
				{
					fromIndex = 0;
				}
				if (toIndex < 0)
				{
					toIndex = len - 1;
				}
				if (toIndex < len)
				{
					return EnumerationForward(list, fromIndex, toIndex);
				}
			}
			else
			{
				if (toIndex < 0)
				{
					toIndex = 0;
				}
				if (fromIndex < 0)
				{
					fromIndex = len - 1;
				}
				if (fromIndex < len)
				{
					return EnumerationReverse(list, fromIndex, toIndex);
				}
			}
			return EnumerationEmpty<T>();
		}

		public static IEnumerable<T> Sequence<T>(T startElement, int count, Func<int, T, T> getNextItem)
		{
			if (getNextItem is null || count <= 0)
			{
				yield break;
			}
			T current = startElement;
			for (int i = 0; i < count; i++)
			{
				yield return current;
				current = getNextItem.Invoke(i, current);
			}
		}

		public static IEnumerable<T> EnumerationEmpty<T>()
		{
			yield break;
		}

		public static IEnumerable<T> EnumerationReverse<T>(IReadOnlyList<T> list, int fromIndex, int toIndex)
		{
			for (int i = fromIndex; i >= toIndex; i--)
			{
				yield return list[i];
			}
		}

		public static IEnumerable<T> EnumerationForward<T>(IReadOnlyList<T> list, int fromIndex, int toIndex)
		{
			var length = toIndex + 1;
			for (int i = fromIndex; i < length; i++)
			{
				yield return list[i];
			}
		}

		public static T[] CopyAsArray<T>(this IList<T> list)
		{
			if (list is null)
			{
				return null;
			}
			var count = list.Count;
			if (count > 0)
			{
				var items = new T[count];
				list.CopyTo(items, 0);
				return items;
			}
			return null;
		}

		public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this ICollection<T> collection)
		{
			return new ReadOnlyCollectionImpl<T>(collection);
		}

		public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> list)
		{
			return new ReadOnlyListImpl<T>(list);
		}

		public static IEnumerable<T> EnumerationCombine<T>(T first, IEnumerable<T> rest)
		{
			yield return first;
			if (rest != null)
			{
				using (var emumerator = rest.GetEnumerator())
				{
					while (emumerator.MoveNext())
					{
						yield return emumerator.Current;
					}
				}
			}
		}

		public static IEnumerable<T> EnumerationCombine<T>(IEnumerable<T> first, T rest)
		{
			if (rest != null)
			{
				using (var emumerator = first.GetEnumerator())
				{
					while (emumerator.MoveNext())
					{
						yield return emumerator.Current;
					}
				}
			}
			yield return rest;
		}

		public static IEnumerable<T> EnumerationCombine<T>(params IEnumerable<T>[] any)
		{
			if (any is null || any.Length == 0)
			{
				yield break;
			}
			for (int i = 0; i < any.Length; i++)
			{
				var items = any[i];
				if (items != null)
				{
					using (var emumerator = items.GetEnumerator())
					{
						while (emumerator.MoveNext())
						{
							yield return emumerator.Current;
						}
					}
				}
			}

		}

	}

}
