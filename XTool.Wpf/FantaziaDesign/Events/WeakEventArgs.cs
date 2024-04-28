using System;

namespace FantaziaDesign.Events
{
	public class WeakEventArgs : IEventArguments<EventHandler>
	{
		protected readonly WeakReference m_sender;
		protected EventArgs m_eventArgs;
		protected bool m_handled;

		public WeakEventArgs(object sender, EventArgs eventArgs)
		{
			m_sender = new WeakReference(sender);
			m_eventArgs = eventArgs;
		}

		public object Sender => m_sender.IsAlive ? m_sender.Target : null;
		public EventArgs EventArgs { get => m_eventArgs; set => m_eventArgs = value; }
		public bool Handled { get => m_handled; set => m_handled = value; }

		public void InvokeEventHandler(EventHandler handler)
		{
			handler?.Invoke(Sender, EventArgs);
		}
	}

	public class WeakEventArgs<TEventArgs> : IEventArguments<EventHandler<TEventArgs>>
	{
		protected readonly WeakReference m_sender;
		protected TEventArgs m_eventArgs;
		protected bool m_handled;

		public WeakEventArgs(object sender, TEventArgs eventArgs)
		{
			m_sender = new WeakReference(sender);
			m_eventArgs = eventArgs;
		}

		public object Sender => m_sender.IsAlive ? m_sender.Target : null;
		public TEventArgs EventArgs { get => m_eventArgs; set => m_eventArgs = value; }
		public bool Handled { get => m_handled; set => m_handled = value; }

		public void InvokeEventHandler(EventHandler<TEventArgs> handler)
		{
			handler?.Invoke(Sender, EventArgs);
		}
	}
}
