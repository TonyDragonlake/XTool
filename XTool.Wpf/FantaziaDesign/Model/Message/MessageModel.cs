using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FantaziaDesign.Input;

namespace FantaziaDesign.Model.Message
{
	public class MessageModel : ControlModel, IMessageModel
	{
		protected RelayAsyncCommand m_closingAsyncCommand;
		private TaskCompletionSource<bool> m_completionSource;
		private object m_messageResult;
		private bool m_closeOnTimeout;
		private bool m_closeOnClickAway;
		private TimeSpan m_messageTimeout = TimeSpan.Zero;
		private MessageStatus m_messageStatus;
		private double m_maskOpacity = 0.5;

		public object MessageResult
		{
			get => m_messageResult;
			set => this.SetPropertyIfChanged(ref m_messageResult, value, nameof(MessageResult));
		}

		public bool CloseOnTimeout
		{
			get => m_closeOnTimeout;
			set => this.SetPropertyIfChanged(ref m_closeOnTimeout, value, nameof(CloseOnTimeout));
		}

		public bool CloseOnClickAway
		{
			get => m_closeOnClickAway;
			set => this.SetPropertyIfChanged(ref m_closeOnClickAway, value, nameof(CloseOnClickAway));
		}

		public TimeSpan MessageTimeout
		{
			get => m_messageTimeout;
			set => this.SetPropertyIfChanged(ref m_messageTimeout, value, nameof(MessageTimeout));
		}

		public ICommand ClosingCommand
		{
			get
			{
				if (m_closingAsyncCommand is null)
				{
					m_closingAsyncCommand =
						new RelayAsyncCommand(CanExecuteClosing, ExecuteClosing);
				}
				return m_closingAsyncCommand;
			}
		}

		internal TaskCompletionSource<bool> CompletionSource
		{
			get
			{
				if (m_completionSource is null)
				{
					m_completionSource = new TaskCompletionSource<bool>();
				}
				return m_completionSource;
			}
		}

		public Task<bool> MessageTask => CompletionSource.Task;

		public MessageStatus MessageStatus
		{
			get => m_messageStatus;
			set
			{
				//System.Diagnostics.Debug.WriteLine(value);
				this.SetPropertyIfChanged(ref m_messageStatus, value, nameof(MessageStatus));
			}
		}

		public double MaskOpacity
		{
			get => m_maskOpacity;
			set => this.SetPropertyIfChanged(ref m_maskOpacity, value, nameof(MaskOpacity));
		}

		protected virtual bool CanExecuteClosing(object parameter)
		{
			return true;
		}

		protected virtual Task ExecuteClosing(CancellationToken cancellationToken, object parameter)
		{
			return Task.CompletedTask;
		}

		public void ClearMessageResult(bool allowNotifyUI = false)
		{
			m_messageResult = null;

			if (allowNotifyUI)
			{
				RaisePropertyChangedEvent(nameof(MessageResult));
			}
		}

		internal void CriticalInvalidateMessageTask()
		{
			if (m_completionSource is null)
			{
				return;
			}
			if (!MessageTask.IsCompleted)
			{
				m_completionSource.TrySetCanceled();
			}
			m_completionSource = null;
		}

		protected virtual void OnResettingModel()
		{

		}

		public void ResetModel()
		{
			m_messageResult = null;
			m_messageStatus = MessageStatus.RequestOpen;
			OnResettingModel();
			CriticalInvalidateMessageTask();
		}
	}

}
