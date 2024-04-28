using System.Windows.Input;
using FantaziaDesign.Input;

namespace FantaziaDesign.Model.Message
{
	public class MessageHostManager : CollectionControlModel<MessageModel>, IRemoveItemCommandHost
	{
		private readonly RelayCommand m_removeItemCommand;

		public MessageHostManager() : base()
		{
			m_removeItemCommand = new RelayCommand(ExcuteRemoveItemCommand);
		}

		public ICommand RemoveItemCommand => m_removeItemCommand;

		private void ExcuteRemoveItemCommand(object parameter)
		{
			if (parameter is MessageModel item)
			{
				item.CompletionSource.TrySetResult(true);
				Remove(item);
			}
		}
	}

}
