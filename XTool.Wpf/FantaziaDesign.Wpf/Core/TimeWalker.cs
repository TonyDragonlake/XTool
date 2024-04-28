using System;
using System.Windows.Threading;
using FantaziaDesign.Events;

namespace FantaziaDesign.Wpf.Core
{
	public enum TimerStatus
	{
		Stopped,
		Paused,
		Running
	}

	public abstract class TimeWalker : IDisposable
	{
		private DispatcherTimer m_timer;
		protected long m_targetTick;
		protected long m_interval;
		protected long m_lastTick;
		private TimerStatus m_status;
		//private EventHandler m_timeOutEventHandler;
		private WeakEvent<EventHandler> m_timeOutEventHandler = new WeakEvent<EventHandler>();

		public TimeSpan Interval { get => TimeSpan.FromTicks(m_interval); set => SetInterval(value.Ticks); }

		private void SetInterval(long ticks)
		{
			if (ticks != m_interval)
			{
				if (m_status != TimerStatus.Stopped)
				{
					m_targetTick += ticks - m_interval;
				}
				m_interval = ticks;
			}
		}

		public TimeSpan RemainingTime => GetRemainingTime();

		public bool IsTimeOut => m_targetTick - DateTime.Now.Ticks <= 0L;

		public TimerStatus TimerStatus => m_status;

		protected virtual TimeSpan GetRemainingTime()
		{
			var delta = m_targetTick - DateTime.Now.Ticks;
			if (delta <= 0L)
			{
				return TimeSpan.Zero;
			}
			return TimeSpan.FromTicks(delta);
		}

		public event EventHandler TimeOut
		{
			add
			{
				AddHandler(value);
			}
			remove
			{
				RemoveHandler(value);
			}
		}

		protected virtual void RemoveHandler(EventHandler handler)
		{
			m_timeOutEventHandler.RemoveHandler(handler);
			//var thisHandler = m_timeOutEventHandler;
			//EventHandler tempHandler;
			//do
			//{
			//	tempHandler = thisHandler;
			//	var newHandler = (EventHandler)Delegate.Remove(tempHandler, handler);
			//	thisHandler = Interlocked.CompareExchange(ref m_timeOutEventHandler, newHandler, tempHandler);
			//}
			//while (thisHandler != tempHandler);
		}

		protected virtual void AddHandler(EventHandler handler)
		{
			m_timeOutEventHandler.AddHandler(handler);
			//var thisHandler = m_timeOutEventHandler;
			//EventHandler tempHandler;
			//do
			//{
			//	tempHandler = thisHandler;
			//	var newHandler = (EventHandler)Delegate.Combine(tempHandler, handler);
			//	thisHandler = Interlocked.CompareExchange(ref m_timeOutEventHandler, newHandler, tempHandler);
			//}
			//while (thisHandler != tempHandler);
		}

		protected TimeWalker()
		{
			InitializeTimer();
		}

		protected virtual void InitializeTimer()
		{
			m_timer = new DispatcherTimer();
			m_timer.Interval = TimeSpan.FromMilliseconds(1);
			m_timer.Tick += OnTimerTick;
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			if (IsTimeOut)
			{
				RaiseTimeOutEvent();
			}
		}

		protected void RaiseTimeOutEventCore()
		{
			m_timeOutEventHandler.RaiseEvent(new WeakEventArgs(this, EventArgs.Empty));
			//m_timeOutEventHandler?.Invoke(this, EventArgs.Empty);
		}

		protected abstract void RaiseTimeOutEvent();
		protected abstract void OnChangingTimerStatus(TimerStatus currentStatus, TimerStatus nextStatus);

		public void Start()
		{
			var nextStatus = TimerStatus.Running;
			if (m_status != nextStatus)
			{
				OnChangingTimerStatus(m_status, nextStatus);
				m_status = nextStatus;
				m_timer.Start();
			}
		}

		public void Pause()
		{
			if (m_status == TimerStatus.Running)
			{
				OnChangingTimerStatus(m_status, TimerStatus.Paused);
				m_status = TimerStatus.Paused;
				m_timer.Stop();
			}
		}

		public void Stop()
		{
			if (m_status != TimerStatus.Stopped)
			{
				OnChangingTimerStatus(m_status, TimerStatus.Stopped);
				m_status = TimerStatus.Stopped;
				m_timer.Stop();
			}
		}

		public virtual void Dispose()
		{
			if (m_timer.IsEnabled)
			{
				m_timer.Stop();
			}
			m_timer.Tick -= OnTimerTick;
		}
	}

	public sealed class InfiniteTimeWalker : TimeWalker
	{
		public InfiniteTimeWalker() :base()
		{
		}

		protected override void RaiseTimeOutEvent()
		{
			RaiseTimeOutEventCore();
			m_targetTick = DateTime.Now.Ticks + m_interval;
		}

		protected override void OnChangingTimerStatus(TimerStatus currentStatus, TimerStatus nextStatus)
		{
			switch (nextStatus)
			{
				case TimerStatus.Paused:
					{
						m_lastTick = DateTime.Now.Ticks;
					}
					break;
				case TimerStatus.Running:
					{
						if (currentStatus == TimerStatus.Stopped)
						{
							m_targetTick = DateTime.Now.Ticks + m_interval;
						}
						else
						{
							m_targetTick += DateTime.Now.Ticks - m_lastTick;
							m_lastTick = 0;
						}
					}
					break;
				default:
					break;
			}
		}
	}

	public sealed class FiniteTimeWalker : TimeWalker
	{
		private int m_currentLoop;
		private int m_loopCount;

		public FiniteTimeWalker(int finiteLoop = 1) : base()
		{
			if (finiteLoop < 1)
			{
				finiteLoop = 1;
			}
			m_loopCount = finiteLoop;
		}

		protected override void RaiseTimeOutEvent()
		{
			if (m_currentLoop < m_loopCount)
			{
				m_currentLoop++;
			}
			else
			{
				Stop();
			}
			RaiseTimeOutEventCore();
			m_targetTick = DateTime.Now.Ticks + m_interval;
		}

		protected override void OnChangingTimerStatus(TimerStatus currentStatus, TimerStatus nextStatus)
		{
			switch (nextStatus)
			{
				case TimerStatus.Paused:
					{
						m_lastTick = DateTime.Now.Ticks;
					}
					break;
				case TimerStatus.Running:
					{
						if (currentStatus == TimerStatus.Stopped)
						{
							m_targetTick = DateTime.Now.Ticks + m_interval;
							m_currentLoop = 0;
						}
						else
						{
							m_targetTick += DateTime.Now.Ticks - m_lastTick;
							m_lastTick = 0;
						}
					}
					break;
				default:
					break;
			}
		}
	}

}
