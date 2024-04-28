using System.Windows;
using System.Windows.Controls;
using FantaziaDesign.Model;

namespace FantaziaDesign.Wpf.Controls
{
	public class ContainerTemplateSelector : ItemContainerTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
		{
			if (item is IContainerTemplateHost host)
			{
				var key = host.ContainerTemplateNameKey;
				if (!string.IsNullOrWhiteSpace(key))
				{
					object resource = parentItemsControl.TryFindResource(key);
					//if (resource is null)
					//{
					//	resource = Application.Current.TryFindResource(key);
					//}
					return resource as DataTemplate;
				}
			}
			return base.SelectTemplate(item, parentItemsControl);
		}

		//public T GetContainerControl<T>(object item, ItemsControl parentItemsControl) where T : DependencyObject
		//{
		//	DataTemplate dataTemplate = SelectTemplate(item, parentItemsControl);
		//	if (dataTemplate != null)
		//	{
		//		var result = dataTemplate.LoadContent() as T;
		//		if (result != null)
		//		{
		//			return result;
		//		}
		//	}
		//	return null;
		//}

	}
}
