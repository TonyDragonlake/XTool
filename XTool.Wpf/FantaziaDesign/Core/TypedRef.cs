using System;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public class TypedRef<T> : IEquatable<T>
	{
		public static void CriticalSetItem(TypedRef<T> tref, T item) => tref.m_item = item;

		protected T m_item;
		protected TypedRef(T item)
		{
			Initialize(ref item);
		}

		public T Item => m_item;

		protected virtual void Initialize(ref T item)
		{
			m_item = item;
		}

		public virtual bool Equals(T other)
		{
			return EqualityComparer<T>.Default.Equals(m_item, other);
		}

		public override bool Equals(object obj)
		{
			if (obj is TypedRef<T> tref)
			{
				return Equals(tref.m_item);
			}
			if (obj is T t)
			{
				return Equals(t);
			}
			return false;
		}

		public override int GetHashCode()
		{
			object o = m_item;
			return o.GetHashCode();
		}

		public static implicit operator T(TypedRef<T> tref) => tref is null ? default(T) : tref.m_item;

		public static implicit operator TypedRef<T>(T item) => new TypedRef<T>(item);

	}

}
