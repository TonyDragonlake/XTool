//using System.Windows.Data;
namespace FantaziaDesign.Core
{
	public interface IValueContextProvider<T>
	{
		IValueContext<T> AttachedValueContext { get; }
		void AttachValueContext(IValueContext<T> valueContext);
		bool SynchronizeToContext();
	}

}

