using System;
using System.Windows;
using FantaziaDesign.Core;

namespace FantaziaDesign.Wpf.Core
{
	public static class ControlMetrics
	{
		public static Point NaNPoint => new Point(double.NaN, double.NaN);

		public static bool IsNaNPoint(this Point point)
		{
			return point.X == double.NaN || point.Y == double.NaN;
		}

		public static void AssignRect(ref Rect rect, Size size)
		{
			rect.X = 0;
			rect.Y = 0;
			rect.Width = size.Width;
			rect.Height = size.Height;
		}

		public static bool IsRectValid(Rect rect)
		{
			return !rect.IsEmpty && rect.Width * rect.Height >= 0;
		}

		public static bool IsRectAbsoluteValid(Rect rect)
		{
			return !rect.IsEmpty && rect.Width * rect.Height > 0;
		}

		public static RectType GetRectType(this Rect rect)
		{
			var w = rect.Width;
			var h = rect.Height;
			if (w < 0 || h < 0 || w == 0 && h == 0)
			{
				return RectType.InvisibleRect;
			}
			else if (h == 0)
			{
				if (double.IsInfinity(w))
				{
					return RectType.InfinityHorizontalLine;
				}
				return RectType.HorizontalLine;
			}
			else if (w == 0)
			{
				if (double.IsInfinity(w))
				{
					return RectType.InfinityVerticalLine;
				}
				return RectType.VerticalLine;
			}
			else if (double.IsInfinity(w) || double.IsInfinity(w))
			{
				return RectType.InfinityRect;
			}
			return RectType.NormalRect;
		}

		public static bool IsThicknessZero(Thickness thickness)
		{
			return DoubleUtil.IsZero(thickness.Left)
				&& DoubleUtil.IsZero(thickness.Top)
				&& DoubleUtil.IsZero(thickness.Right)
				&& DoubleUtil.IsZero(thickness.Bottom);
		}

		public static Thickness Max(Thickness thickness1, Thickness thickness2)
		{
			return new Thickness(
				Math.Max(thickness1.Left, thickness2.Left),
				Math.Max(thickness1.Top, thickness2.Top),
				Math.Max(thickness1.Right, thickness2.Right),
				Math.Max(thickness1.Bottom, thickness2.Bottom)
				);
		}

		public static Thickness CombineThickness(params Thickness[] thicknesses)
		{
			if (thicknesses is null || thicknesses.Length == 0)
			{
				return default;
			}
			var first = thicknesses[0];
			var result = new Thickness(first.Left, first.Top, first.Right, first.Bottom);
			for (int i = 1; i < thicknesses.Length; i++)
			{
				var cur = thicknesses[i];
				result.Left += cur.Left;
				result.Top += cur.Top;
				result.Right += cur.Right;
				result.Bottom += cur.Bottom;
			}
			return result;
		}

		public static double GetMaximumAsUniformThickness(Thickness thickness)
		{
			var first = thickness.Left;
			var data = new double[] { thickness.Top, thickness.Right, thickness.Bottom };
			foreach (var t in data)
			{
				if (t > first)
				{
					first = t;
				}
			}
			return first;
		}

		public static Size NewInflateSize(Size size, Thickness thickness)
		{
			var w = size.Width + (thickness.Left + thickness.Right);
			var h = size.Height + (thickness.Top + thickness.Bottom);
			return new Size(w, h);
		}

		public static void InflateSize(ref Size size, Thickness thickness)
		{
			size.Width = size.Width + (thickness.Left + thickness.Right);
			size.Height = size.Height + (thickness.Top + thickness.Bottom);
		}

		public static Size NewDeflateSize(Size size, Thickness thickness)
		{
			var w = Math.Max(0.0, size.Width - (thickness.Left + thickness.Right));
			var h = Math.Max(0.0, size.Height - (thickness.Top + thickness.Bottom));
			return new Size(w, h);
		}


		public static void DeflateSize(ref Size size, Thickness thickness)
		{
			size.Width = Math.Max(0.0, size.Width - (thickness.Left + thickness.Right));
			size.Height = Math.Max(0.0, size.Height - (thickness.Top + thickness.Bottom));
		}

		public static Rect NewDeflateRect(Rect rect, Thickness thickness)
		{
			if (IsThicknessZero(thickness))
			{
				return rect;
			}

			var x = rect.Left;
			var y = rect.Top;
			var w = rect.Width;
			var h = rect.Height;
			x = x + thickness.Left;
			y = y + thickness.Top;
			w = Math.Max(0.0, w - thickness.Left - thickness.Right);
			h = Math.Max(0.0, h - thickness.Top - thickness.Bottom);
			return new Rect(x, y, w, h);
		}

		public static void DeflateRect(ref Rect rect, Thickness thickness)
		{
			if (IsThicknessZero(thickness))
			{
				return;
			}
			var x = rect.Left;
			var y = rect.Top;
			var w = rect.Width;
			var h = rect.Height;
			rect.X = x + thickness.Left;
			rect.Y = y + thickness.Top;
			rect.Width = Math.Max(0.0, w - thickness.Left - thickness.Right);
			rect.Height = Math.Max(0.0, h - thickness.Top - thickness.Bottom);
		}

		public static Rect NewInflateRect(Rect rect, Thickness thickness)
		{
			if (IsThicknessZero(thickness))
			{
				return rect;
			}
			var x = rect.Left;
			var y = rect.Top;
			var w = rect.Width;
			var h = rect.Height;
			x = x - thickness.Left;
			y = y - thickness.Top;
			w = Math.Max(0.0, w + thickness.Left + thickness.Right);
			h = Math.Max(0.0, h + thickness.Top + thickness.Bottom);
			return new Rect(x, y, w, h);
		}

		public static void InflateRect(ref Rect rect, Thickness thickness)
		{
			if (IsThicknessZero(thickness))
			{
				return;
			}
			var x = rect.Left;
			var y = rect.Top;
			var w = rect.Width;
			var h = rect.Height;
			rect.X = x - thickness.Left;
			rect.Y = y - thickness.Top;
			rect.Width = Math.Max(0.0, w + thickness.Left + thickness.Right);
			rect.Height = Math.Max(0.0, h + thickness.Top + thickness.Bottom);
		}
	}

}
