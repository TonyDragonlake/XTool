using FantaziaDesign.Model.Message;
using FantaziaDesign.Input;
using System.Windows.Input;

namespace FantaziaDesign.Wpf.Message
{
	public class YesNoCancelDialog : SimpleDialogBase
	{
		private RelayCommand m_yesCommand;
		private RelayCommand m_noCommand;

		public YesNoCancelDialog() : base()
		{
			ContentTemplateNameKey = "YesNoCancelDialogTemplate";
		}

		public string YesKeyword { get; set; }
		public string NoKeyword { get; set; }

		public ICommand YesCommand
		{
			get
			{
				if (m_yesCommand is null)
				{
					m_yesCommand = new RelayCommand(CanExecuteYesCommand, ExecuteYesCommand);
				}
				return m_yesCommand;
			}
		}

		private void ExecuteYesCommand(object parameter)
		{
			MessageResult = MessageResults.Accept;
			MessageStatus = MessageStatus.RequestClose;
		}

		private bool CanExecuteYesCommand(object parameter)
		{
			return true;
		}

		public ICommand NoCommand
		{
			get
			{
				if (m_noCommand is null)
				{
					m_noCommand = new RelayCommand(CanExecuteNoCommand, ExecuteNoCommand);
				}
				return m_noCommand;
			}
		}

		private void ExecuteNoCommand(object parameter)
		{
			MessageResult = MessageResults.No;
			MessageStatus = MessageStatus.RequestClose;
		}

		private bool CanExecuteNoCommand(object parameter)
		{
			return true;
		}

		public override string ToString()
		{
			return $"{nameof(YesNoCancelDialog)} {{{DialogTitle}, {MessageResult}}}";
		}

	}


}
