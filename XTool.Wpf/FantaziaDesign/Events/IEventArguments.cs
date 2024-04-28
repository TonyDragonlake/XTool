using System;

namespace FantaziaDesign.Events
{
	public interface IEventArguments<TEventHandler> where TEventHandler : Delegate
	{
		bool Handled { get; set; }
		void InvokeEventHandler(TEventHandler handler);
	}
}
