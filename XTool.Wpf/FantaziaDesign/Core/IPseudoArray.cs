using System;
using System.Collections;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public interface IPseudoArray<T> : IEnumerable<T>, IEnumerable
	{
		T this[int index] { get; set; }
		int Count { get; }
	}

	public static class PseudoArrays
	{
		public static T GetItem<T>(IPseudoArray<T> array, int index)
		{
			if (array is null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			return array[index];
		}

		public static void SetItem<T>(IPseudoArray<T> array, int index, T value)
		{
			if (array is null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			array[index] = value;
		}

		public static int GetElementCount<T>(IPseudoArray<T> array)
		{
			if (array is null)
			{
				return 0;
			}
			return array.Count;
		}

		public static bool MinimumElementCountEquals<T>(IPseudoArray<T> array, int count)
		{
			if (array is null)
			{
				return false;
			}
			if (count <= 0)
			{
				return false;
			}
			return array.Count >= count;
		}
	}
}
