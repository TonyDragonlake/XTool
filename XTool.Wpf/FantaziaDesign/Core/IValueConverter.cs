//using System.Windows.Data;
namespace FantaziaDesign.Core
{
	public interface IValueConverter<Tsource, Ttarget> 
	{
		Ttarget ConvertTo(Tsource source);
		Tsource ConvertBack(Ttarget target);
	}

}

