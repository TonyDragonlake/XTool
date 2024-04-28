using System;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace FantaziaDesign.Wpf.Controls
{
	public class LayersPanel : ScrollablePanel
	{
		public double BackgroundOpacity
		{
			get { return (double)GetValue(BackgroundOpacityProperty); }
			set { SetValue(BackgroundOpacityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BackgroundOpacity.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BackgroundOpacityProperty = Layerable.BackgroundOpacityProperty.AddOwner(typeof(LayersPanel));
			// DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(LayersPanel), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(OnCoerceOpacity)));

		public double LayerOpacity
		{
			get { return (double)GetValue(LayerOpacityProperty); }
			set { SetValue(LayerOpacityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LayerOpacity.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LayerOpacityProperty = Layerable.LayerOpacityProperty.AddOwner(typeof(LayersPanel));
			// DependencyProperty.Register("LayerOpacity", typeof(double), typeof(LayersPanel), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(OnCoerceOpacity)));

		public Brush LayerBrush
		{
			get { return (Brush)GetValue(LayerBrushProperty); }
			set { SetValue(LayerBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LayerBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LayerBrushProperty = Layerable.LayerBrushProperty.AddOwner(typeof(LayersPanel));
			// DependencyProperty.Register("LayerBrush", typeof(Brush), typeof(LayersPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public LayersPanel()
		{
			ScrollLogicManager.AllowScroll = false;
		}

		public override void LineDown()
		{

		}

		public override void LineLeft()
		{

		}

		public override void LineRight()
		{

		}

		public override void LineUp()
		{

		}

		public override void MouseWheelDown()
		{

		}

		public override void MouseWheelLeft()
		{

		}

		public override void MouseWheelRight()
		{

		}

		public override void MouseWheelUp()
		{

		}

		public override void PageDown()
		{

		}

		public override void PageLeft()
		{

		}

		public override void PageRight()
		{

		}

		public override void PageUp()
		{

		}

		protected override Size ArrangeChildren(Size arrangeSize, Vector computedOffset)
		{
			foreach (UIElement item in InternalChildren)
			{
				item.Arrange(new Rect(arrangeSize));
			}
			return arrangeSize;
		}

		protected override Size MeasureChildren(Size constraint, Size childrenConstraint)
		{
			Size result = default(Size);
			foreach (UIElement item in InternalChildren)
			{
				item.Measure(constraint);
				result.Width = Math.Max(result.Width, item.DesiredSize.Width);
				result.Height = Math.Max(result.Height, item.DesiredSize.Height);
			}
			return result;
		}

		protected override void OnRender(DrawingContext dc)
		{
			var renderSize = RenderSize;
			if (renderSize.Width <= 0 || renderSize.Height <= 0)
			{
				return;
			}
			DrawLayer(dc, renderSize.Width, renderSize.Height, BackgroundOpacity, Background);
			DrawLayer(dc, renderSize.Width, renderSize.Height, LayerOpacity, LayerBrush);
		}

		private static void DrawLayer(DrawingContext dc, double width, double height, double opacity, Brush brush)
		{
			if (opacity > 0)
			{
				if (brush is null)
				{
					return;
				}
				if (opacity >= 1)
				{
					dc.DrawRectangle(brush, null, new Rect(0.0, 0.0, width, height));
				}
				else
				{
					dc.PushOpacity(opacity);
					dc.DrawRectangle(brush, null, new Rect(0.0, 0.0, width, height));
					dc.Pop();
				}
			}
		}
	}
}
