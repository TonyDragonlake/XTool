//using System.Windows.Data;
namespace FantaziaDesign.Core
{
	public interface IUnitConverter<TValue> : IValueConverter<TValue, TValue>
	{
		double Ratio { get; }

		void SetRatio(double ratio);
	}

}

