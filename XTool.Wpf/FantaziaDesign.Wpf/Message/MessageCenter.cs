using System;
using System.Threading.Tasks;
using FantaziaDesign.Wpf.Controls;
using FantaziaDesign.Model.Message;

namespace FantaziaDesign.Wpf.Message
{
	public static class MessageCenter
	{
		public static async Task<bool> Show(string identifier, MessageModel messageModel)
		{
			if (string.IsNullOrWhiteSpace(identifier))
			{
				return false;
			}

			if (messageModel is null)
			{
				return false;
			}

			var instances = MessageHost.LoadedInstances;

			if (instances.TryGetValue(identifier, out WeakReference weakReference))
			{
				var host = weakReference.Target as MessageHost;
				if (host != null)
				{
					if (host.ItemTemplateSelector is null)
					{
						host.ItemTemplateSelector = new ContentTemplateSelector();
					}

					if (host.ItemContainerStyleSelector is null)
					{
						host.ItemContainerStyleSelector = new ContainerStyleSelector();
					}

					var model = host.DataContext as MessageHostManager;
					if (model is null)
					{
						model = new MessageHostManager();
						host.DataContext = model;
						host.ItemsSource = model;
					}
					if (!model.Contains(messageModel))
					{
						messageModel.ResetModel();
						model.Add(messageModel);
						return await messageModel.MessageTask;
					}
				}
			}
			return false;
		}
	}
}
