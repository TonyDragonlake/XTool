using System;
//using System.Windows.Data;
namespace FantaziaDesign.Core
{
	public interface IDeepCopyable<T> : ICloneable
	{
		T DeepCopy();
		void DeepCopyValueFrom(T obj);
	}

}

