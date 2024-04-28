using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FantaziaDesign.Wpf.Controls
{
	public class ClippedContentControl : ContentControl
	{
		static ClippedContentControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ClippedContentControl), new FrameworkPropertyMetadata(typeof(ClippedContentControl)));
		}

		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(ClippedContentControl));

		public bool IsBorderClipperEnabled
		{
			get { return (bool)GetValue(IsBorderClipperEnabledProperty); }
			set { SetValue(IsBorderClipperEnabledProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsBorderClipperEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsBorderClipperEnabledProperty =
			DependencyProperty.Register("IsBorderClipperEnabled", typeof(bool), typeof(ClippedContentControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		public Brush LayerBrush
		{
			get { return (Brush)GetValue(LayerBrushProperty); }
			set { SetValue(LayerBrushProperty, value);}
		}

		public static readonly DependencyProperty LayerBrushProperty = LayersPanel.LayerBrushProperty.AddOwner(typeof(ClippedContentControl));
	
		public double LayerOpacity
		{
			get { return (double) GetValue(LayerOpacityProperty); }
			set { SetValue(LayerOpacityProperty, value);}
		}

		public static readonly DependencyProperty LayerOpacityProperty = LayersPanel.LayerOpacityProperty.AddOwner(typeof (ClippedContentControl));

	}
}
