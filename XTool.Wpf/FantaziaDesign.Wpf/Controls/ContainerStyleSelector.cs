using System.Windows;
using System.Windows.Controls;
using FantaziaDesign.Model;

namespace FantaziaDesign.Wpf.Controls
{
	public class ContainerStyleSelector : StyleSelector
	{
		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is IStyleHost host)
			{
				string nameKey = host.StyleNameKey;
				if (string.IsNullOrWhiteSpace(nameKey))
				{
					return FindByTypeKey(container);
				}
				else
				{
					return FindByNameKey(nameKey, container);
				}
			}
			return base.SelectStyle(item, container);
		}

		private Style FindByNameKey(string nameKey, DependencyObject container)
		{
			object resource = null;
			var fe = container as FrameworkElement;
			var fce = container as FrameworkContentElement;
			if (fe != null)
			{
				resource = fe.TryFindResource(nameKey);
			}
			else if (fce != null)
			{
				resource = fce.TryFindResource(nameKey);
			}
			var style = resource as Style;
			if (style is null)
			{
				var typeKey = container.GetType();
				if (fe != null)
				{
					resource = fe.TryFindResource(typeKey);
				}
				else if (fce != null)
				{
					resource = fce.TryFindResource(typeKey);
				}
				style = resource as Style;
			}

			return style;
		}

		private Style FindByTypeKey(DependencyObject container)
		{
			object resource = null;
			var fe = container as FrameworkElement;
			var fce = container as FrameworkContentElement;
			var typeKey = container.GetType();
			if (fe != null)
			{
				resource = fe.TryFindResource(typeKey);
			}
			else if (fce != null)
			{
				resource = fce.TryFindResource(typeKey);
			}
			return resource as Style;
		}
	}
}
