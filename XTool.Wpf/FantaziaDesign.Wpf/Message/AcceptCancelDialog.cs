using FantaziaDesign.Model.Message;
using FantaziaDesign.Input;
using System.Windows.Input;

namespace FantaziaDesign.Wpf.Message
{
	public class AcceptCancelDialog : SimpleDialogBase
	{
		private RelayCommand m_acceptCommand;

		public AcceptCancelDialog() : base()
		{
			ContentTemplateNameKey = "AcceptCancelDialogTemplate";
		}

		public string AcceptKeyword { get; set; }

		public ICommand AcceptCommand
		{
			get
			{
				if (m_acceptCommand is null)
				{
					m_acceptCommand = new RelayCommand(CanExecuteAcceptCommand, ExecuteAcceptCommand);
				}
				return m_acceptCommand;
			}
		}

		protected virtual void ExecuteAcceptCommand(object parameter)
		{
			MessageResult = MessageResults.Accept;
			MessageStatus = MessageStatus.RequestClose;
		}

		protected virtual bool CanExecuteAcceptCommand(object parameter)
		{
			return true;
		}

		public override string ToString()
		{
			return $"{nameof(AcceptCancelDialog)} {{{DialogTitle}, {MessageResult}}}";
		}

	}

}
