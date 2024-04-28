using FantaziaDesign.Core;
using FantaziaDesign.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FantaziaDesign.Wpf.Graphics
{
	public static class WpfGraphicsUtil
	{
		public static Color ToColor(this ColorF4 colorF4)
		{
			if (colorF4 is null)
			{
				return new Color();
			}
			ColorsUtility.ColorBytes(colorF4.A,colorF4.R,colorF4.G, colorF4.B, out byte a, out byte r, out byte g, out byte b);
			return Color.FromArgb(a, r, g, b);
		}

		public static bool TryMakeBrush(this GenericBrush genericBrush, out Brush brush)
		{
			brush = null;
			if (!GenericBrush.IsNullOrEmpty(genericBrush))
			{
				switch (genericBrush.BrushType)
				{
					case BrushType.SimpleColorBrush:
						{
							var scb = genericBrush as SimpleColorBrush;
							if (scb != null)
							{
								brush = new SolidColorBrush(scb.Color.ToColor());
								return true;
							}
						}
						break;
					case BrushType.LinearGradientColorBrush:
						{
							var lgcb = genericBrush as LinearGradientColorBrush;
							if (lgcb != null)
							{
								GradientStopCollection gradientStops = null;
								var gradientsCount = lgcb.ColorGradients.Count;
								if (gradientsCount > 0)
								{
									gradientStops = new GradientStopCollection(lgcb.ColorGradients.Count);
									foreach (var cgd in lgcb.ColorGradients)
									{
										gradientStops.Add(new GradientStop(cgd.Color.ToColor(), cgd.Offset));
									}
								}
								var gv = lgcb.GradientVector;
								brush = new LinearGradientBrush(gradientStops, new Point(gv.StartPointX, gv.StartPointY), new Point(gv.EndPointX, gv.EndPointY));
								return true;
							}
						}
						break;
					case BrushType.RadialGradientColorBrush:
						{
							var rgcb = genericBrush as RadialGradientColorBrush;
							if (rgcb != null)
							{
								GradientStopCollection gradientStops = null;
								var gradientsCount = rgcb.ColorGradients.Count;
								if (gradientsCount > 0)
								{
									gradientStops = new GradientStopCollection(rgcb.ColorGradients.Count);
									foreach (var cgd in rgcb.ColorGradients)
									{
										gradientStops.Add(new GradientStop(cgd.Color.ToColor(), cgd.Offset));
									}
								}
								var ooffset = rgcb.OriginOffset;
								var gellipse = rgcb.GradientEllipse;
								brush = new RadialGradientBrush(gradientStops)
								{
									GradientOrigin = new Point(ooffset.X + gellipse.CenterX, ooffset.Y + gellipse.CenterY),
									Center = new Point(gellipse.CenterX, gellipse.CenterY),
									RadiusX = gellipse.RadiusX,
									RadiusY = gellipse.RadiusY
								};
								return true;
							}
						}
						break;
					default:
						break;
				}
			}
			return false;
		}
	}
}
