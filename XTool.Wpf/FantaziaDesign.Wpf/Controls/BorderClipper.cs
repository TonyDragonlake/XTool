using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FantaziaDesign.Core;
using FantaziaDesign.Wpf.Core;

namespace FantaziaDesign.Wpf.Controls
{
	public class BorderClipper : RectangleClipper
	{
		protected StreamGeometry m_backgroundGeometry;
		protected StreamGeometry m_borderGeometry;
		protected byte m_drawingFlags;

		protected SolidColorBrush m_cacheLightnessBrush;

		public Brush Background
		{
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		public static readonly DependencyProperty BackgroundProperty = 
			Border.BackgroundProperty.AddOwner(typeof(BorderClipper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public Brush BorderBrush
		{
			get { return (Brush)GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}

		public static readonly DependencyProperty BorderBrushProperty = 
			Border.BorderBrushProperty.AddOwner(typeof(BorderClipper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public double BackgroundOpacity
		{
			get { return (double)GetValue(BackgroundOpacityProperty); }
			set { SetValue(BackgroundOpacityProperty, value); }
		}

		public static readonly DependencyProperty BackgroundOpacityProperty = 
			Layerable.BackgroundOpacityProperty.AddOwner(typeof(BorderClipper));

		public double LayerOpacity
		{
			get { return (double)GetValue(LayerOpacityProperty); }
			set { SetValue(LayerOpacityProperty, value); }
		}

		public static readonly DependencyProperty LayerOpacityProperty = 
			Layerable.LayerOpacityProperty.AddOwner(typeof(BorderClipper));

		public Brush LayerBrush
		{
			get { return (Brush)GetValue(LayerBrushProperty); }
			set { SetValue(LayerBrushProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LayerBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LayerBrushProperty = 
			Layerable.LayerBrushProperty.AddOwner(typeof(BorderClipper));

		public double LayerLightness
		{
			get { return (double)GetValue(LayerLightnessProperty); }
			set { SetValue(LayerLightnessProperty, value); }
		}

		public static readonly DependencyProperty LayerLightnessProperty =
			Layerable.LayerLightnessProperty.AddOwner(typeof(BorderClipper));

		private void PrepareRenderGeometry()
		{
			m_drawingFlags = 0;

			if (m_backgroundGeometry is null)
			{
				m_backgroundGeometry = new StreamGeometry();
			}
			using (var context = m_backgroundGeometry.Open())
			{
				BorderLikeControlUtil.GenerateRoundedRectangleGeometry(context, m_innerBound, m_innerRadii);
			}
			m_drawingFlags |= 1;

			if (!ControlMetrics.IsThicknessZero(BorderThickness))
			{
				if (m_borderGeometry is null)
				{
					m_borderGeometry = new StreamGeometry();
				}
				using (var context = m_borderGeometry.Open())
				{
					BorderLikeControlUtil.GenerateRoundedRectangleGeometry(context, m_outerBound, m_outerRadii);
					BorderLikeControlUtil.GenerateRoundedRectangleGeometry(context, m_innerBound, m_innerRadii);
				}
				m_drawingFlags |= 2;
			}
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			var result = base.ArrangeOverride(arrangeSize);
			PrepareRenderGeometry();
			return result;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (BitUtil.GetBitStatus(ref m_drawingFlags, 1))
			{
				var bgOpacity = BackgroundOpacity;
				var background = Background;
				var lOpacity = LayerOpacity;
				var layerBrush = LayerBrush;
				if (!ShouldCullingBackground(lOpacity, layerBrush))
				{
					RenderLayer(drawingContext, m_backgroundGeometry, bgOpacity, background);
				}
				RenderLayer(drawingContext, m_backgroundGeometry, lOpacity, layerBrush);

				var lightness = (int)(LayerLightness * lOpacity * 255);
				if (lightness != 0)
				{
					Color color;
					bool lighten = lightness > 0;
					var a = (byte)(lighten ? lightness : -lightness);
					color = lighten 
						? Color.FromArgb(a, 255, 255, 255) 
						: Color.FromArgb(a, 0, 0, 0);
					if (m_cacheLightnessBrush is null)
					{
						m_cacheLightnessBrush = new SolidColorBrush(color);
					}
					else
					{
						m_cacheLightnessBrush.Color = color;
					}
					drawingContext.DrawGeometry(m_cacheLightnessBrush, null, m_backgroundGeometry);
				}
			}

			if (BitUtil.GetBitStatus(ref m_drawingFlags, 2))
			{
				var borderBrush = BorderBrush;
				if (borderBrush is null)
				{
					return;
				}
				drawingContext.DrawGeometry(borderBrush, null, m_borderGeometry);
			}
		}

		private static bool ShouldCullingBackground(double opacity, Brush brush)
		{
			if (opacity < 1)
			{
				return false;
			}
			var scb = brush as SolidColorBrush;
			if (scb is null)
			{
				return false;
			}
			return scb.Color.A == byte.MaxValue;
		}

		private static void RenderLayer(DrawingContext drawingContext, Geometry geometry, double opacity, Brush brush)
		{
			if (opacity > 0)
			{
				if (brush is null)
				{
					return;
				}
				if (opacity >= 1)
				{
					drawingContext.DrawGeometry(brush, null, geometry);
				}
				else
				{
					drawingContext.PushOpacity(opacity);
					drawingContext.DrawGeometry(brush, null, geometry);
					drawingContext.Pop();
				}
			}
		}
	}
}
