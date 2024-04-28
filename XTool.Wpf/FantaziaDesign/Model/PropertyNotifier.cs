using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace FantaziaDesign.Model
{
	public interface IPropertyNotifier : INotifyPropertyChanged
	{
		void RaisePropertyChangedEvent(string propName);
	}

	public interface IPseudoEvent
	{
		object Handler { get; }
		object EventArgs { get; }
		object Sender { get; }
		void InvokeHandler();
	}

	public sealed class PseudoPropertyChangedEvent : IPseudoEvent
	{
		private readonly PropertyChangedEventHandler m_handler;
		private readonly PropertyChangedEventArgs m_args;
		private readonly object m_sender;

		public PseudoPropertyChangedEvent(PropertyChangedEventHandler handler, PropertyChangedEventArgs args, object sender = null)
		{
			m_handler = handler;
			m_args = args;
			m_sender = sender;
		}

		public object Handler => m_handler;

		public object EventArgs => m_args;

		public object Sender => m_sender;

		public void InvokeHandler()
		{
			m_handler?.Invoke(m_sender, m_args);
		}
	}

	public abstract class PropertyNotifier : IPropertyNotifier
	{
		protected SynchronizationContext m_synchronizationContext;

		private bool m_isPropertyNotifierSuspended;

		public bool IsPropertyNotifierSuspended => m_isPropertyNotifierSuspended;

		public SynchronizationContext SynchronizationContext => m_synchronizationContext;

		public bool IsNullOrCurrentContext => m_synchronizationContext is null || m_synchronizationContext == SynchronizationContext.Current;

		public void SuspendPropertyNotifier()
		{
			m_isPropertyNotifierSuspended = true;
		}

		public void ResumePropertyNotifier()
		{
			m_isPropertyNotifierSuspended = true;
		}

		protected PropertyChangedEventHandler m_propertyChangedHandler;

		protected PropertyNotifier(SynchronizationContext synchronizationContext)
		{
			m_synchronizationContext = synchronizationContext;
		}

		protected PropertyNotifier() : this(SynchronizationContext.Current)
		{
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				var thisHandler = m_propertyChangedHandler;
				PropertyChangedEventHandler tempHandler;
				do
				{
					tempHandler = thisHandler;
					var newHandler = (PropertyChangedEventHandler)Delegate.Combine(tempHandler, value);
					thisHandler = Interlocked.CompareExchange(ref m_propertyChangedHandler, newHandler, tempHandler);
				}
				while (thisHandler != tempHandler);
			}
			remove
			{
				var thisHandler = m_propertyChangedHandler;
				PropertyChangedEventHandler tempHandler;
				do
				{
					tempHandler = thisHandler;
					var newHandler = (PropertyChangedEventHandler)Delegate.Remove(tempHandler, value);
					thisHandler = Interlocked.CompareExchange(ref m_propertyChangedHandler, newHandler, tempHandler);
				}
				while (thisHandler != tempHandler);
			}
		}

		protected void InvokeEventCore(object state)
		{
			var pesudoEvent = state as IPseudoEvent;
			if (pesudoEvent != null)
			{
				pesudoEvent.InvokeHandler();
			}
		}

		protected virtual void RaisePropertyChangedEvent(PropertyChangedEventArgs eventArgs)
		{
			if (m_isPropertyNotifierSuspended)
			{
				return;
			}
			var e = new PseudoPropertyChangedEvent(m_propertyChangedHandler, eventArgs, this);
			if (IsNullOrCurrentContext)
			{
				InvokeEventCore(e);
			}
			else
			{
				m_synchronizationContext.Send(new SendOrPostCallback(InvokeEventCore), e);
			}
		}

		public void RaisePropertyChangedEvent(string propName)
		{
			if (string.IsNullOrWhiteSpace(propName))
			{
				throw new ArgumentException($"{nameof(propName)} cannot be null or WhiteSpace", nameof(propName));
			}
			RaisePropertyChangedEvent(new PropertyChangedEventArgs(propName));
		}
	}

	public static class NotifyPropertyUtil
	{
		public static bool SetPropertyIfChanged<TProperty>(this IPropertyNotifier notifier, ref TProperty targetProp, TProperty value, string propName = null)
		{
			return SetPropertyIfChanged(notifier, ref targetProp, value, null, null, propName);
		}

		public static bool SetPropertyIfChanged<TProperty>(this IPropertyNotifier notifier, ref TProperty targetProp, TProperty value, IEqualityComparer<TProperty> comparer, string propName = null)
		{
			return SetPropertyIfChanged(notifier, ref targetProp, value, null, comparer, propName);
		}

		public static bool SetPropertyIfChanged<TProperty>(this IPropertyNotifier notifier, ref TProperty targetProp, TProperty value, Func<TProperty, TProperty> setter, string propName = null)
		{
			return SetPropertyIfChanged(notifier, ref targetProp, value, setter, null, propName);
		}

		public static bool SetPropertyIfChanged<TProperty>(this IPropertyNotifier notifier, ref TProperty targetProp, TProperty value, Func<TProperty, TProperty> setter, IEqualityComparer<TProperty> comparer, string propName = null)
		{
			IEqualityComparer<TProperty> m_comparer = comparer is null ? EqualityComparer<TProperty>.Default : comparer;
			var finalValue = setter is null ? value : setter.Invoke(value);
			if (!m_comparer.Equals(targetProp, finalValue))
			{
				targetProp = finalValue;
				if (!string.IsNullOrWhiteSpace(propName))
				{
					notifier.RaisePropertyChangedEvent(propName);
				}
				return true;
			}
			return false;
		}

		public static void RaisePropertyChangedEvent(this IPropertyNotifier notifier, PropertyChangedEventHandler eventHandler, string propName)
		{
			if (string.IsNullOrWhiteSpace(propName))
			{
				throw new ArgumentException($"{nameof(propName)} is null or WhiteSpace", nameof(propName));
			}
			eventHandler?.Invoke(notifier, new PropertyChangedEventArgs(propName));
		}
	}

}
