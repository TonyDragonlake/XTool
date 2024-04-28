using System.Windows;
using System.Windows.Media;

namespace FantaziaDesign.Wpf.Controls
{
	public static class Layerable
	{
		private static object OnCoerceOpacity(DependencyObject d, object baseValue)
		{
			if (baseValue is double val)
			{
				if (val > 1)
				{
					val = 1;
				}
				if (val < 0)
				{
					val = 0;
				}
				return val;
			}
			return baseValue;
		}

		private static object OnCoerceLightness(DependencyObject d, object baseValue)
		{
			if (baseValue is double val)
			{
				if (val > 1)
				{
					val = 1;
				}
				if (val < -1)
				{
					val = -1;
				}
				return val;
			}
			return baseValue;
		}


		public static double GetBackgroundOpacity(DependencyObject obj)
		{
			return (double)obj.GetValue(BackgroundOpacityProperty);
		}

		public static void SetBackgroundOpacity(DependencyObject obj, double value)
		{
			obj.SetValue(BackgroundOpacityProperty, value);
		}

		// Using a DependencyProperty as the backing store for BackgroundOpacity.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BackgroundOpacityProperty =
			DependencyProperty.RegisterAttached("BackgroundOpacity", typeof(double), typeof(Layerable), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(OnCoerceOpacity)));

		public static double GetLayerOpacity(DependencyObject obj)
		{
			return (double)obj.GetValue(LayerOpacityProperty);
		}

		public static void SetLayerOpacity(DependencyObject obj, double value)
		{
			obj.SetValue(LayerOpacityProperty, value);
		}

		// Using a DependencyProperty as the backing store for LayerOpacity.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LayerOpacityProperty =
			DependencyProperty.RegisterAttached("LayerOpacity", typeof(double), typeof(Layerable), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(OnCoerceOpacity)));

		public static Brush GetLayerBrush(DependencyObject obj)
		{
			return (Brush)obj.GetValue(LayerBrushProperty);
		}

		public static void SetLayerBrush(DependencyObject obj, Brush value)
		{
			obj.SetValue(LayerBrushProperty, value);
		}

		// Using a DependencyProperty as the backing store for LayerBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LayerBrushProperty =
			DependencyProperty.RegisterAttached("LayerBrush", typeof(Brush), typeof(Layerable), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public static double GetLayerLightness(DependencyObject obj)
		{
			return (double)obj.GetValue(LayerLightnessProperty);
		}

		public static void SetLayerLightness(DependencyObject obj, double value)
		{
			obj.SetValue(LayerLightnessProperty, value);
		}

		// Using a DependencyProperty as the backing store for LayerLightness.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LayerLightnessProperty =
			DependencyProperty.RegisterAttached("LayerLightness", typeof(double), typeof(Layerable), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, null, new CoerceValueCallback(OnCoerceLightness)));

	}
}
