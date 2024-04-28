using System;
using System.Collections.Generic;
using System.Threading;
using FantaziaDesign.Core;

namespace FantaziaDesign.Events
{
	public sealed class WeakEvent<TEventHandler> where TEventHandler : Delegate
	{
		private readonly object m_lockObj = new object();
		private readonly List<WeakDelegate> m_multiDelegates = new List<WeakDelegate>();

		public void AddHandler(TEventHandler eventHandler)
		{
			if (eventHandler is null)
			{
				return;
			}
			if (m_multiDelegates.Count != 0)
			{
				if (TryFindIndex(eventHandler, out var _))
				{
					return;
				}
			}
			var weakDelegate = WeakDelegate.FromDelegate(eventHandler);
			if (weakDelegate != null)
			{
				AddItemSafe(weakDelegate);
			}
		}

		public void RemoveHandler(TEventHandler eventHandler)
		{
			if (eventHandler is null)
			{
				return;
			}
			if (TryFindIndex(eventHandler, out var index))
			{
				RemoveItemSafe(index);
			}
		}

		private bool TryFindIndex(TEventHandler eventHandler, out int index)
		{
			index = 0;
			var len = m_multiDelegates.Count;
			for (; index < len; index++)
			{
				var item = m_multiDelegates[index];
				if (item.TargetEquals(eventHandler))
				{
					return true;
				}
			}
			return false;
		}

		private void AddItemSafe(WeakDelegate weakDelegate)
		{
			if (Monitor.TryEnter(m_lockObj))
			{
				try
				{
					m_multiDelegates.Add(weakDelegate);
				}
				finally
				{
					Monitor.Exit(m_lockObj);
				}
			}
		}

		private void RemoveItemSafe(int index)
		{
			if (Monitor.TryEnter(m_lockObj))
			{
				try
				{
					m_multiDelegates.RemoveAt(index);
				}
				finally
				{
					Monitor.Exit(m_lockObj);
				}
			}
		}

		public void RaiseEvent(IEventArguments<TEventHandler> arguments)
		{
			if (arguments is null || arguments.Handled)
			{
				return;
			}

			var length = m_multiDelegates.Count;
			if (Monitor.TryEnter(m_lockObj))
			{
				try
				{
					var rmStack = new Stack<int>(length);
					for (int i = 0; i < length; i++)
					{
						var handler = m_multiDelegates[i].GetDelegateAs<TEventHandler>();
						if (handler is null)
						{
							rmStack.Push(i);
						}
						else
						{
							arguments.InvokeEventHandler(handler);
							if (arguments.Handled)
							{
								break;
							}
						}
					}
					if (rmStack.Count > 0)
					{
						foreach (var index in rmStack)
						{
							m_multiDelegates.RemoveAt(index);
						}
					}
				}
				finally
				{
					Monitor.Exit(m_lockObj);
				}
			}

		}

	}
}
