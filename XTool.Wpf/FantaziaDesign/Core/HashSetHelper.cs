using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace FantaziaDesign.Core
{
#if NETSTANDARD2_1_OR_GREATER

#else

	internal abstract class HashSetValueGetter
	{
		protected Type m_elementType;
		protected HashSetValueGetter(Type elementType)
		{
			m_elementType = elementType;
		}
		public Type ElementType => m_elementType;
		public abstract HashSetValueGetter<T> GetGetter<T>();
	}

	internal sealed class HashSetValueGetter<T> : HashSetValueGetter
	{
		Func<HashSet<T>, int[]> GetBuckets;
		Func<HashSet<T>, T, int> InternalIndexOf;
		SingleRefAction<HashSet<T>, int, T> RefGetValueBySlotIndex;

		public HashSetValueGetter() : base(typeof(T))
		{
			BuildMethod();
		}

		private void BuildMethod()
		{
			// flags
			var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
			// type init
			var classType = typeof(HashSet<T>);
			var slotsField = classType.GetField("m_slots", bindingFlags);
			var slotType = slotsField.FieldType.GetElementType();
			var bucketsType = typeof(int[]);
			// input params
			var instanceExpr = Expression.Parameter(classType, "instance");
			var bucketsExpr = Expression.Variable(bucketsType, "buckets");
			var indexExpr = Expression.Parameter(typeof(int), "index");
			var actualValueExpr = Expression.Parameter(typeof(T).MakeByRefType(), "actualValue");
			// Expr : instance.m_buckets 
			var getBucketsExpr = Expression.Lambda<Func<HashSet<T>, int[]>>(
				Expression.Block(
					new ParameterExpression[] { bucketsExpr },
					 Expression.Assign(
						 bucketsExpr,
						 Expression.Field(
							 instanceExpr,
							 classType.GetField("m_buckets", bindingFlags)
							 )
						 )
					), 
				instanceExpr
				);
			GetBuckets = getBucketsExpr.Compile();
			// Expr : ref actualValue = instance.m_slots[index].value;
			var getSlotValueExpr = Expression.Lambda<SingleRefAction<HashSet<T>, int, T>>(
				Expression.Block(
					Expression.Assign(
						actualValueExpr,
						Expression.Field(
							Expression.ArrayAccess(
								Expression.Field(instanceExpr, slotsField),
								indexExpr
								),
							slotType.GetField("value", bindingFlags)
							)
						)
					),
				instanceExpr,
				indexExpr,
				actualValueExpr
				);
			RefGetValueBySlotIndex = getSlotValueExpr.Compile();
			// method binding : InternalIndexOf
			var method = classType.GetMethod("InternalIndexOf", bindingFlags);
			InternalIndexOf = (Func<HashSet<T>, T, int>)Delegate.CreateDelegate(typeof(Func<HashSet<T>, T, int>), method);
		}

		public bool TryGetValue(HashSet<T> thisSet, T equalValue, out T actualValue)
		{
			var m_buckets = GetBuckets(thisSet);
			actualValue = default(T);
			if (m_buckets != null)
			{
				int num = InternalIndexOf(thisSet, equalValue);
				if (num >= 0)
				{
					RefGetValueBySlotIndex(thisSet,num, ref actualValue);
					return true;
				}
			}
			return false;
		}

		public sealed override HashSetValueGetter<TValue> GetGetter<TValue>()
		{
			return this as HashSetValueGetter<TValue>;
		}
	}

	public static class HashSetHelper
	{
		private static object s_lockObj = new object();
		private static Dictionary<Type, HashSetValueGetter> getterDict;
		internal static Dictionary<Type, HashSetValueGetter> GetterDict 
		{
			get 
			{
				lock (s_lockObj)
				{
					if (getterDict is null)
					{
						getterDict = new Dictionary<Type, HashSetValueGetter>();
					}
					return getterDict;
				}
			}
		}

		/// <summary>
		/// Searches the set for a given value and returns the equal value it finds, if any.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="thisSet"> Target Set </param>
		/// <param name="equalValue">The value to search for.</param>
		/// <param name="actualValue">The value from the set that the search found, or the default value of T when the search yielded no match.</param>
		/// <returns></returns>
		public static bool TryGetValue<T>(this HashSet<T> thisSet, T equalValue, out T actualValue)
		{
			var dict = GetterDict;
			var currentType = typeof(T);
			HashSetValueGetter<T> realGetter;
			if (dict.TryGetValue(currentType, out HashSetValueGetter valueGetter))
			{
				realGetter = valueGetter.GetGetter<T>();
			}
			else
			{
				realGetter = new HashSetValueGetter<T>();
				dict.Add(currentType, realGetter);
			}
			return realGetter.TryGetValue(thisSet, equalValue, out actualValue);
		}
	}

#endif


}
