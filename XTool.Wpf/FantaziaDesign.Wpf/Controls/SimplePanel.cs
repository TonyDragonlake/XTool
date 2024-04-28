using System;
using System.Windows.Controls;
using System.Windows;

namespace FantaziaDesign.Wpf.Controls
{
	public class SimplePanel : Panel
	{
		protected override Size MeasureOverride(Size constraint)
		{
			var maxSize = new Size();

			foreach (UIElement child in InternalChildren)
			{
				if (child != null)
				{
					child.Measure(constraint);
					maxSize.Width = Math.Max(maxSize.Width, child.DesiredSize.Width);
					maxSize.Height = Math.Max(maxSize.Height, child.DesiredSize.Height);
				}
			}

			return maxSize;
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			foreach (UIElement child in InternalChildren)
			{
				child?.Arrange(new Rect(arrangeSize));
			}

			return arrangeSize;
		}
	}
}
