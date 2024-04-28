using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FantaziaDesign.Model.Message
{
	public interface IMessageModel
	{
		MessageStatus MessageStatus { get; set; }
		object MessageResult { get; set; }
		bool CloseOnTimeout { get; set; }
		bool CloseOnClickAway { get; set; }
		TimeSpan MessageTimeout { get; set; }
		ICommand ClosingCommand { get; }
		Task<bool> MessageTask { get; }
		double MaskOpacity { get; set; }
	}

}
