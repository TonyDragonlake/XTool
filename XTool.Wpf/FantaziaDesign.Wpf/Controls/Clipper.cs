using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FantaziaDesign.Core;
using FantaziaDesign.Wpf.Core;

namespace FantaziaDesign.Wpf.Controls
{
	public struct Radii : IDeepCopyable<Radii>
	{
		private double[] m_radii;

		public Radii(double uniformCornerRadius)
		{
			m_radii = new double[8];
			if (uniformCornerRadius > 0)
			{
				for (int i = 0; i < 8; i++)
				{
					m_radii[i] = uniformCornerRadius;
				}
			}
		}

		public Radii(CornerRadius cornerRadius)
		{
			m_radii = new double[8];
			LeftTop = Math.Max(0.0, cornerRadius.TopLeft);
			TopLeft = Math.Max(0.0, cornerRadius.TopLeft);
			TopRight = Math.Max(0.0, cornerRadius.TopRight);
			RightTop = Math.Max(0.0, cornerRadius.TopRight);
			RightBottom = Math.Max(0.0, cornerRadius.BottomRight);
			BottomRight = Math.Max(0.0, cornerRadius.BottomRight);
			BottomLeft = Math.Max(0.0, cornerRadius.BottomLeft);
			LeftBottom = Math.Max(0.0, cornerRadius.BottomLeft);
		}

		public Radii(Radii radii)
		{
			if (radii.m_radii is null)
			{
				m_radii = null;
			}
			else
			{
				m_radii = new double[8];
				Array.Copy(radii.m_radii, m_radii, 8);
			}
		}

		private Radii(double[] radii)
		{
			m_radii = radii;
		}

		public double LeftTop { get => Get(0); set => Set(0, value); }
		public double TopLeft { get => Get(1); set => Set(1, value); }
		public double TopRight { get => Get(2); set => Set(2, value); }
		public double RightTop { get => Get(3); set => Set(3, value); }
		public double RightBottom { get => Get(4); set => Set(4, value); }
		public double BottomRight { get => Get(5); set => Set(5, value); }
		public double BottomLeft { get => Get(6); set => Set(6, value); }
		public double LeftBottom { get => Get(7); set => Set(7, value); }

		private void Set(int index, double value)
		{
			InitIfNull();
			m_radii[index] = value;
		}

		private double Get(int index)
		{
			InitIfNull();
			return m_radii[index];
		}

		private void InitIfNull()
		{
			if (m_radii is null)
			{
				m_radii = new double[8];
			}
		}

		public bool IsZero
		{
			get
			{
				if (m_radii is null)
				{
					return true;
				}
				for (int i = 0; i < 8; i++)
				{
					if (m_radii[i] != 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public void ToZero()
		{
			if (m_radii is null)
			{
				return;
			}
			Array.Clear(m_radii, 0, 8);
		}

		public void ToUniform(double uniformCornerRadius)
		{
			if (uniformCornerRadius > 0)
			{
				InitIfNull();
				for (int i = 0; i < 8; i++)
				{
					m_radii[i] = uniformCornerRadius;
				}
			}
			else
			{
				ToZero();
			}
		}

		public void Deflate(Thickness thickness)
		{
			double l = thickness.Left;
			double t = thickness.Top;
			double r = thickness.Right;
			double b = thickness.Bottom;
			LeftTop = Math.Max(0.0, LeftTop - l);
			TopLeft = Math.Max(0.0, TopLeft - t);
			TopRight = Math.Max(0.0, TopRight - t);
			RightTop = Math.Max(0.0, RightTop - r);
			RightBottom = Math.Max(0.0, RightBottom - r);
			BottomRight = Math.Max(0.0, BottomRight - b);
			BottomLeft = Math.Max(0.0, BottomLeft - b);
			LeftBottom = Math.Max(0.0, LeftBottom - l);
		}

		public void Inflate(Thickness thickness)
		{
			double l = thickness.Left;
			double t = thickness.Top;
			double r = thickness.Right;
			double b = thickness.Bottom;
			if (LeftTop == 0 || TopLeft == 0)
			{
				LeftTop = TopLeft = 0.0;
			}
			else
			{
				LeftTop += l;
				TopLeft += t;
			}
			if (TopRight == 0 || RightTop == 0)
			{
				TopRight = RightTop = 0.0;
			}
			else
			{
				TopRight += t;
				RightTop += r;
			}
			if (RightBottom == 0 || BottomRight == 0)
			{
				RightBottom = BottomRight = 0.0;
			}
			else
			{
				RightBottom += r;
				BottomRight += b;
			}
			if (BottomLeft == 0 || LeftBottom == 0)
			{
				BottomLeft = LeftBottom = 0.0;
			}
			else
			{
				BottomLeft += b;
				LeftBottom += l;
			}

		}

		public Radii DeepCopy()
		{
			if (m_radii is null)
			{
				return new Radii();
			}
			double[] copy = new double[8];
			Array.Copy(m_radii, copy, 8);
			return new Radii(m_radii);
		}

		public void DeepCopyValueFrom(Radii obj)
		{
			if (obj.m_radii is null)
			{
				m_radii = null;
			}
			else
			{
				InitIfNull();
				Array.Copy(obj.m_radii, m_radii, 8);
			}
		}

		public object Clone()
		{
			return DeepCopy();
		}
	}

	public static class BorderLikeControlUtil
	{
		public static void GenerateRoundedRectangleGeometry(StreamGeometryContext ctx, Rect rect, Radii radii)
		{
			//
			//  compute the coordinates of the key points
			//

			Point topLeft = new Point(radii.LeftTop, 0);
			Point topRight = new Point(rect.Width - radii.RightTop, 0);
			Point rightTop = new Point(rect.Width, radii.TopRight);
			Point rightBottom = new Point(rect.Width, rect.Height - radii.BottomRight);
			Point bottomRight = new Point(rect.Width - radii.RightBottom, rect.Height);
			Point bottomLeft = new Point(radii.LeftBottom, rect.Height);
			Point leftBottom = new Point(0, rect.Height - radii.BottomLeft);
			Point leftTop = new Point(0, radii.TopLeft);

			//
			//  check keypoints for overlap and resolve by partitioning radii according to
			//  the percentage of each one.  
			//

			//  top edge is handled here
			if (topLeft.X > topRight.X)
			{
				double v = (radii.LeftTop) / (radii.LeftTop + radii.RightTop) * rect.Width;
				topLeft.X = v;
				topRight.X = v;
			}

			//  right edge
			if (rightTop.Y > rightBottom.Y)
			{
				double v = (radii.TopRight) / (radii.TopRight + radii.BottomRight) * rect.Height;
				rightTop.Y = v;
				rightBottom.Y = v;
			}

			//  bottom edge
			if (bottomRight.X < bottomLeft.X)
			{
				double v = (radii.LeftBottom) / (radii.LeftBottom + radii.RightBottom) * rect.Width;
				bottomRight.X = v;
				bottomLeft.X = v;
			}

			// left edge
			if (leftBottom.Y < leftTop.Y)
			{
				double v = (radii.TopLeft) / (radii.TopLeft + radii.BottomLeft) * rect.Height;
				leftBottom.Y = v;
				leftTop.Y = v;
			}

			//
			//  add on offsets
			//

			Vector offset = new Vector(rect.TopLeft.X, rect.TopLeft.Y);
			topLeft += offset;
			topRight += offset;
			rightTop += offset;
			rightBottom += offset;
			bottomRight += offset;
			bottomLeft += offset;
			leftBottom += offset;
			leftTop += offset;

			//
			//  create the border geometry
			//
			ctx.BeginFigure(topLeft, true /* is filled */, true /* is closed */);

			// Top line
			ctx.LineTo(topRight, true /* is stroked */, false /* is smooth join */);

			// Upper-right corner
			double radiusX = rect.TopRight.X - topRight.X;
			double radiusY = rightTop.Y - rect.TopRight.Y;
			if (!DoubleUtil.IsZero(radiusX)
				|| !DoubleUtil.IsZero(radiusY))
			{
				ctx.ArcTo(rightTop, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
			}

			// Right line
			ctx.LineTo(rightBottom, true /* is stroked */, false /* is smooth join */);

			// Lower-right corner
			radiusX = rect.BottomRight.X - bottomRight.X;
			radiusY = rect.BottomRight.Y - rightBottom.Y;
			if (!DoubleUtil.IsZero(radiusX)
				|| !DoubleUtil.IsZero(radiusY))
			{
				ctx.ArcTo(bottomRight, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
			}

			// Bottom line
			ctx.LineTo(bottomLeft, true /* is stroked */, false /* is smooth join */);

			// Lower-left corner
			radiusX = bottomLeft.X - rect.BottomLeft.X;
			radiusY = rect.BottomLeft.Y - leftBottom.Y;
			if (!DoubleUtil.IsZero(radiusX)
				|| !DoubleUtil.IsZero(radiusY))
			{
				ctx.ArcTo(leftBottom, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
			}

			// Left line
			ctx.LineTo(leftTop, true /* is stroked */, false /* is smooth join */);

			// Upper-left corner
			radiusX = topLeft.X - rect.TopLeft.X;
			radiusY = leftTop.Y - rect.TopLeft.Y;
			if (!DoubleUtil.IsZero(radiusX)
				|| !DoubleUtil.IsZero(radiusY))
			{
				ctx.ArcTo(topLeft, new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true, false);
			}
		}

		public static bool IsThicknessValid(object value)
		{
			if (value is Thickness thickness)
			{
				return DoubleUtil.IsValid(default, thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
			}
			return false;
		}

		public static void GenerateSimpleBoundaryGeometry(StreamGeometryContext ctx, Rect rect, Thickness thickness)
		{
			//
			//  create the border geometry
			//
			ctx.BeginFigure(rect.TopLeft, true /* is filled */, true /* is closed */);
			// Top line
			ctx.LineTo(rect.TopRight, true /* is stroked */, false /* is smooth join */);
			// Right line
			ctx.LineTo(rect.BottomRight, true /* is stroked */, false /* is smooth join */);
			// Bottom line
			ctx.LineTo(rect.BottomLeft, true /* is stroked */, false /* is smooth join */);
			// Left line
			ctx.LineTo(rect.TopLeft, true /* is stroked */, false /* is smooth join */);
			ControlMetrics.DeflateRect(ref rect, thickness);
			ctx.BeginFigure(rect.TopLeft, true /* is filled */, true /* is closed */);
			// Top line
			ctx.LineTo(rect.TopRight, true /* is stroked */, false /* is smooth join */);
			// Right line
			ctx.LineTo(rect.BottomRight, true /* is stroked */, false /* is smooth join */);
			// Bottom line
			ctx.LineTo(rect.BottomLeft, true /* is stroked */, false /* is smooth join */);
			// Left line
			ctx.LineTo(rect.TopLeft, true /* is stroked */, false /* is smooth join */);

		}
	}

	public abstract class Clipper : Decorator
	{
		protected Rect m_outerBound;
		protected bool m_isClippingEnabled;
		protected Geometry m_clippingGeometry;

		public Thickness Padding
		{
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Padding.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PaddingProperty =
			DependencyProperty.Register(
				"Padding",
				typeof(Thickness),
				typeof(Clipper),
				new FrameworkPropertyMetadata(
					default(Thickness),
					FrameworkPropertyMetadataOptions.AffectsMeasure
					| FrameworkPropertyMetadataOptions.AffectsArrange),
				new ValidateValueCallback(BorderLikeControlUtil.IsThicknessValid));

		public bool IsChildClippingEnabled
		{
			get { return (bool)GetValue(IsChildClippingEnabledProperty); }
			set { SetValue(IsChildClippingEnabledProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsChildClippingEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsChildClippingEnabledProperty =
			DependencyProperty.Register("IsChildClippingEnabled", typeof(bool), typeof(Clipper), new PropertyMetadata(false, OnAffectChildClipping));

		protected static void OnAffectChildClipping(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is Clipper clipper)
			{
				var newVal = e.NewValue;
				if (newVal is bool isClippingEnabled)
				{
					clipper.m_isClippingEnabled = isClippingEnabled;
				}
				clipper.OnAffectChildClipping(true);
			}
		}

		protected void OnAffectChildClipping(bool affectClippingGeometry = false)
		{
			if (m_isClippingEnabled)
			{
				if (ControlMetrics.IsRectValid(m_outerBound))
				{
					if (affectClippingGeometry)
					{
						bool isHandled = false;
						var geometry = OnClippingChild(ref isHandled);
						if (isHandled && geometry != null)
						{
							m_clippingGeometry = geometry;
						}
					}
					var child = Child;
					if (child != null)
					{
						child.SetCurrentValue(UIElement.ClipProperty, m_clippingGeometry);
					}
				}
			}
			else
			{
				m_clippingGeometry = null;
				var child = Child;
				if (child != null)
				{
					child.SetCurrentValue(UIElement.ClipProperty, null);
				}
			}

		}

		protected abstract Geometry OnClippingChild(ref bool isHandled);

		protected override Size MeasureOverride(Size constraint)
		{
			var padding = Padding;
			UIElement child = Child;
			Size result = default(Size);
			if (child != null)
			{
				child.Measure(ControlMetrics.NewDeflateSize(constraint, padding));
				result = ControlMetrics.NewInflateSize(child.DesiredSize, padding);
			}
			else
			{
				ControlMetrics.InflateSize(ref result, padding);
			}
			return result;
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			m_outerBound = new Rect(arrangeSize);

			UIElement child = Child;
			if (child != null)
			{
				child.Arrange(ControlMetrics.NewDeflateRect(m_outerBound, Padding));
			}
			OnAffectChildClipping(true);
			return arrangeSize;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			OnAffectChildClipping();
		}
	}

	public class RectangleClipper : Clipper
	{
		protected Rect m_innerBound;
		protected Radii m_outerRadii;
		protected Radii m_innerRadii;

		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		public static readonly DependencyProperty CornerRadiusProperty =
			Border.CornerRadiusProperty.AddOwner(typeof(RectangleClipper), new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnCornerRadiusChanged));

		private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is RectangleClipper clipper)
			{
				clipper.UpdateRadii();
			}
		}

		public Thickness BorderThickness
		{
			get { return (Thickness)GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}

		public static readonly DependencyProperty BorderThicknessProperty =
			Border.BorderThicknessProperty.AddOwner(typeof(RectangleClipper), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnBorderThicknessChanged));

		private static void OnBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is RectangleClipper clipper)
			{
				clipper.UpdateRadii();
			}
		}

		private void UpdateRadii()
		{
			var thickness = BorderThickness;
			var halfThichness = new Thickness(thickness.Left / 2, thickness.Top / 2, thickness.Right / 2, thickness.Bottom / 2);
			m_outerRadii = new Radii(CornerRadius);
			m_outerRadii.Inflate(halfThichness);
			m_innerRadii = new Radii(CornerRadius);
			m_innerRadii.Deflate(halfThichness);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var thickness = ControlMetrics.CombineThickness(Padding, BorderThickness);
			UIElement child = Child;
			Size result = default(Size);
			if (child != null)
			{
				child.Measure(ControlMetrics.NewDeflateSize(constraint, thickness));
				result = ControlMetrics.NewInflateSize(child.DesiredSize, thickness);
			}
			else
			{
				ControlMetrics.InflateSize(ref result, thickness);
			}
			return result;
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			m_outerBound = new Rect(arrangeSize);
			m_innerBound = ControlMetrics.NewDeflateRect(m_outerBound, BorderThickness);
			var child = Child;
			if (child != null)
			{
				child.Arrange(ControlMetrics.NewDeflateRect(m_innerBound, Padding));
			}
			OnAffectChildClipping(true);
			return arrangeSize;
		}

		protected override Geometry OnClippingChild(ref bool isHandled)
		{
			var geometry = new StreamGeometry();
			using (var context = geometry.Open())
			{
				BorderLikeControlUtil.GenerateRoundedRectangleGeometry(context, new Rect(m_innerBound.Size), m_innerRadii);
				isHandled = true;
			}
			return geometry;
		}
	}

	public enum SizeReference
	{
		NoReference,
		// DesignedSize,
		PaddedChildSize,
		// TargetSize
	}

	public class SizeBoxClipper : RectangleClipper
	{
		public double WidthRatio
		{
			get { return (double)GetValue(WidthRatioProperty); }
			set { SetValue(WidthRatioProperty, value); }
		}

		// Using a DependencyProperty as the backing store for WidthRatio.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WidthRatioProperty =
			DependencyProperty.Register("WidthRatio", typeof(double), typeof(SizeBoxClipper), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public double HeightRatio
		{
			get { return (double)GetValue(HeightRatioProperty); }
			set { SetValue(HeightRatioProperty, value); }
		}

		// Using a DependencyProperty as the backing store for HeightRatio.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HeightRatioProperty =
			DependencyProperty.Register("HeightRatio", typeof(double), typeof(SizeBoxClipper), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));


		public SizeReference SizeReference
		{
			get { return (SizeReference)GetValue(SizeReferenceProperty); }
			set { SetValue(SizeReferenceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SizeReference.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SizeReferenceProperty =
			DependencyProperty.Register("SizeReference", typeof(SizeReference), typeof(SizeBoxClipper), new FrameworkPropertyMetadata(SizeReference.NoReference, FrameworkPropertyMetadataOptions.AffectsMeasure));



		public bool AffectVisualChildSize
		{
			get { return (bool)GetValue(AffectVisualChildSizeProperty); }
			set { SetValue(AffectVisualChildSizeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for AffectVisualChildSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AffectVisualChildSizeProperty =
			DependencyProperty.Register("AffectVisualChildSize", typeof(bool), typeof(SizeBoxClipper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));


		protected override Size MeasureOverride(Size constraint)
		{
			//switch (SizeReference)
			//{
			//	case SizeReference.DesignedSize:
			//		return MeasureByDesignedSize(Child, constraint, Padding, WidthRatio, HeightRatio);
			//	case SizeReference.PaddedChildSize:
			//		return MeasureByPaddedChildSize(Child, constraint, Padding, WidthRatio, HeightRatio);
			//	//case SizeReference.TargetSize:
			//	//	break;
			//	default:
			//		return MeasureByDefault(Child, constraint, Padding, WidthRatio, HeightRatio);
			//}

			switch (SizeReference)
			{
				//case SizeReference.DesignedSize:
				//	return MeasureBySpecialSize(Child, constraint, Padding, WidthRatio, HeightRatio);
				case SizeReference.PaddedChildSize:
					return MeasureByPaddedChildSize(Child, constraint, Padding, WidthRatio, HeightRatio);
				//case SizeReference.TargetSize:
				//	break;
				default:
					return MeasureByDefault(Child, constraint, Padding, WidthRatio, HeightRatio);
			}

		}

		private static Size MeasureByDefault(UIElement child, Size constraint, Thickness padding, double widthRatio, double heightRatio)
		{
			Size result = default(Size);
			if (child != null)
			{
				child.Measure(ControlMetrics.NewDeflateSize(constraint, padding));
				result = ControlMetrics.NewInflateSize(child.DesiredSize, padding);
			}
			else
			{
				ControlMetrics.InflateSize(ref result, padding);
			}
			return result;
		}

		private static Size MeasureByDesignedSize(UIElement child, Size constraint, Thickness padding, double widthRatio, double heightRatio)
		{
			Size result = constraint;
			if (child != null)
			{
				child.Measure(ControlMetrics.NewDeflateSize(constraint, padding));
				result = ControlMetrics.NewInflateSize(child.DesiredSize, padding);
			}
			if (double.IsNaN(result.Width))
			{
				result.Width = double.PositiveInfinity;
			}
			if (double.IsNaN(result.Height))
			{
				result.Height = double.PositiveInfinity;
			}
			if (CanMultiplyRatio(widthRatio))
			{
				result.Width *= widthRatio;
			}
			if (CanMultiplyRatio(heightRatio))
			{
				result.Height *= heightRatio;
			}
			return result;
		}

		private static Size MeasureByPaddedChildSize(UIElement child, Size constraint, Thickness padding, double widthRatio, double heightRatio)
		{
			Size result = default(Size);
			if (child != null)
			{
				child.Measure(ControlMetrics.NewDeflateSize(constraint, padding));
				result = ControlMetrics.NewInflateSize(child.DesiredSize, padding);
			}
			else
			{
				ControlMetrics.InflateSize(ref result, padding);
				return result;
			}
			if (CanMultiplyRatio(widthRatio))
			{
				result.Width *= widthRatio;
			}
			if (CanMultiplyRatio(heightRatio))
			{
				result.Height *= heightRatio;
			}
			return result;
		}


		private static bool CanMultiplyRatio(double ratio)
		{
			return !double.IsNaN(ratio) && ratio != 1.0;
		}

		private static void MultiplySizeRatio(ref Rect rect, double widthRatio, double heightRatio)
		{
			if (CanMultiplyRatio(widthRatio))
			{
				rect.Width *= widthRatio;
			}
			if (CanMultiplyRatio(heightRatio))
			{
				rect.Height *= heightRatio;
			}
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			double widthRatio = WidthRatio;
			double heightRatio = HeightRatio;
			var padding = Padding;
			UIElement child = Child;
			if (child != null)
			{
				var sizeRef = SizeReference;
				if (sizeRef == SizeReference.PaddedChildSize)
				{
					m_outerBound = new Rect(ControlMetrics.NewInflateSize(child.DesiredSize, padding));
					if (!CanMultiplyRatio(widthRatio))
					{
						m_outerBound.Width = arrangeSize.Width;
					}
					if (!CanMultiplyRatio(heightRatio))
					{
						m_outerBound.Height = arrangeSize.Height;
					}
				}
				else
				{
					m_outerBound = new Rect(arrangeSize);
				}
				if (AffectVisualChildSize)
				{
					if (sizeRef != SizeReference.NoReference)
					{
						MultiplySizeRatio(ref m_outerBound, widthRatio, heightRatio);
					}
					//switch (sizeRef)
					//{
					//	case SizeReference.DesignedSize:
					//		{
					//			TryMultiplySizeRatio(ref clipperRect, widthRatio, heightRatio);
					//		}
					//		break;
					//	case SizeReference.PaddedChildSize:
					//		{
					//			TryMultiplySizeRatio(ref clipperRect, widthRatio, heightRatio);
					//		}
					//		break;
					//	default:
					//		break;
					//}
					child.Arrange(ControlMetrics.NewDeflateRect(m_outerBound, padding));
				}
				else
				{
					child.Arrange(ControlMetrics.NewDeflateRect(m_outerBound, padding));
					if (sizeRef != SizeReference.NoReference)
					{
						MultiplySizeRatio(ref m_outerBound, widthRatio, heightRatio);
					}
					//switch (sizeRef)
					//{
					//	case SizeReference.DesignedSize:
					//		{
					//			TryMultiplySizeRatio(ref clipperRect, widthRatio, heightRatio);
					//		}
					//		break;
					//	case SizeReference.PaddedChildSize:
					//		{
					//			TryMultiplySizeRatio(ref clipperRect, widthRatio, heightRatio);
					//		}
					//		break;
					//	default:
					//		break;
					//}
				}
				//if (!AffectVisualChildSize && SizeReference == SizeReference.PaddedChildSize)
				//{
				//	var paddedChildBound = new Rect(child.DesiredSize);
				//	ControlExtension.InflateRect(ref paddedChildBound, Padding);
				//	paddedChildBound.X = 0;
				//	paddedChildBound.Y = 0;
				//	child.Arrange(ControlExtension.NewDeflateRect(paddedChildBound, Padding));
				//}
				//else
				//{
				//	child.Arrange(ControlExtension.NewDeflateRect(clipperRect, Padding));
				//}
				////double widthRatio = WidthRatio;
				////double heightRatio = HeightRatio;
				////if (CanMultiplyRatio(widthRatio))
				////{
				////	clipperRect.Width *= widthRatio;
				////}
				////if (CanMultiplyRatio(heightRatio))
				////{
				////	clipperRect.Height *= heightRatio;
				////}
				OnAffectChildClipping(true);
				return m_outerBound.Size;
			}
			return arrangeSize;
		}


		//protected override void OnRender(DrawingContext drawingContext)
		//{
		//	var size = RenderSize;
		//	drawingContext.DrawRectangle(Brushes.Red, null, clipperRect);
		//}
	}
}
