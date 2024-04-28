using System;
using System.ComponentModel;

namespace FantaziaDesign.Events
{
	public class WeakCancelEventArgs : IEventArguments<CancelEventHandler>
	{
		protected readonly WeakReference m_sender;
		protected CancelEventArgs m_eventArgs;
		protected bool m_handled;

		public WeakCancelEventArgs(object sender, CancelEventArgs eventArgs)
		{
			m_sender = new WeakReference(sender);
			m_eventArgs = eventArgs;
		}

		public object Sender => m_sender.IsAlive ? m_sender.Target : null;
		public CancelEventArgs EventArgs { get => m_eventArgs; set => m_eventArgs = value; }
		public bool Handled { get => m_handled; set => m_handled = value; }

		public void InvokeEventHandler(CancelEventHandler handler)
		{
			handler?.Invoke(Sender, EventArgs);
		}
	}
}
