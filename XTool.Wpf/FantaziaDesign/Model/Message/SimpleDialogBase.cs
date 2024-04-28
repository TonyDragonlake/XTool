using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FantaziaDesign.Events;
using FantaziaDesign.Input;

namespace FantaziaDesign.Model.Message
{
	public enum MessageResults : short
	{
		No = -2,
		Cancel = -1,
		NoResult,
		Accept,
		Yes = Accept
	}

	public enum InformationSeverity : byte
	{
		Informational,
		Success,
		Warning,
		Error
	}

	public class SimpleDialogBase : MessageModel
	{
		protected string m_dialogTitle;
		protected object m_dialogContent;
		protected string m_infoContent;
		protected InformationSeverity m_infoSeverity;
		private readonly RelayCommand m_cancelCommand;

		public string DialogTitle
		{
			get => m_dialogTitle;
			set => this.SetPropertyIfChanged(ref m_dialogTitle, value, nameof(DialogTitle));
		}

		public object DialogContent
		{
			get => m_dialogContent;
			set => this.SetPropertyIfChanged(ref m_dialogContent, value, nameof(DialogContent));
		}

		public string InformationContent
		{
			get => m_infoContent;
			set => this.SetPropertyIfChanged(ref m_infoContent, value, nameof(InformationContent));
		}

		public InformationSeverity InformationSeverity
		{
			get => m_infoSeverity;
			set => this.SetPropertyIfChanged(ref m_infoSeverity, value, nameof(InformationSeverity));
		}

		public ICommand CancelCommand => m_cancelCommand;

		public string CancelKeyword { get; set; }

		public event AsyncEventHandler<bool> DialogClosing;

		public MessageResults DefinedMessageResult
		{
			get
			{
				var result = MessageResult;
				if (result is null)
				{
					return MessageResults.NoResult;
				}
				if (result is MessageResults mres)
				{
					return mres;
				}
				return MessageResults.NoResult;
			}
		}

		public SimpleDialogBase()
		{
			m_cancelCommand = new RelayCommand(ExecuteCancelCommand);
		}
		protected virtual void ExecuteCancelCommand(object parameter)
		{
			MessageResult = MessageResults.Cancel;
			MessageStatus = MessageStatus.RequestClose;
		}

		protected override Task ExecuteClosing(CancellationToken cancellationToken, object parameter)
		{
			if (DialogClosing is null)
			{
				return Task.FromResult(true);
			}
			return DialogClosing.Invoke(this, EventArgs.Empty).ContinueWith<bool>(AfterExecuteClosing, cancellationToken);
		}

		protected virtual bool AfterExecuteClosing(Task<bool> closingTask)
		{
			var result = closingTask.Result;
			if (!result)
			{
				MessageStatus = MessageStatus.RequestCancelClose;
			}
			return result;
		}

		public override string ToString()
		{
			return $"Dialog {{{m_dialogTitle}, {MessageResult}}}";
		}

		protected override void OnResettingModel()
		{
			m_infoContent = null;
			m_infoSeverity = InformationSeverity.Informational;
		}

	}

}
