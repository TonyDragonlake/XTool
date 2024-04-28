using System.Windows;
using System.Windows.Controls;
using FantaziaDesign.Model;

namespace FantaziaDesign.Wpf.Controls
{
	public class ContentTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is IContentTemplateHost host)
			{
				var key = host.ContentTemplateNameKey;
				if (!string.IsNullOrWhiteSpace(key))
				{
					var fe = container as FrameworkElement;
					var fce = container as FrameworkContentElement;
					object resource = null;
					if (fe != null)
					{
						resource = fe.TryFindResource(key);
					}
					else if (fce != null)
					{
						resource = fce.TryFindResource(key);
					}
					if (resource is null)
					{
						resource = Application.Current.TryFindResource(key);
					}
					return resource as DataTemplate;
				}
			}
			return base.SelectTemplate(item, container);
		}
	}
}
