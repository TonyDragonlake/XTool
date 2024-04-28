using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using FantaziaDesign.Wpf.Core;

namespace FantaziaDesign.Wpf.Controls
{
	public enum SlotHoleCorner
	{
		None,
		Rounded
	}

	public sealed class SlotHole : Shape
	{
		private Rect _rect = Rect.Empty;

		public SlotHoleCorner SlotCorner
		{
			get { return (SlotHoleCorner)GetValue(SlotCornerProperty); }
			set { SetValue(SlotCornerProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SlotCorner.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SlotCornerProperty =
			DependencyProperty.Register("SlotCorner", typeof(SlotHoleCorner), typeof(SlotHole), new FrameworkPropertyMetadata(SlotHoleCorner.None, FrameworkPropertyMetadataOptions.AffectsRender));

		public SlotHole()
		{
		}

		static SlotHole()
		{
			StretchProperty.OverrideMetadata(typeof(SlotHole), new FrameworkPropertyMetadata(Stretch.Fill));
		}

		public override Geometry RenderedGeometry
		{
			get
			{
				if (SlotCorner == SlotHoleCorner.None)
				{
					return new RectangleGeometry(_rect);
				}
				double radius = Math.Min(_rect.Width, _rect.Height) / 2;
				return new RectangleGeometry(_rect, radius, radius);
			}
		}

		public override Transform GeometryTransform
		{
			get
			{
				return Transform.Identity;
			}
		}

		internal bool IsPenNoOp
		{
			get
			{
				double strokeThickness = StrokeThickness;
				return Stroke == null || double.IsNaN(strokeThickness) || strokeThickness == 0.0;
			}
		}

		internal double GetStrokeThickness()
		{
			if (IsPenNoOp)
			{
				return 0.0;
			}
			return Math.Abs(StrokeThickness);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			CacheDefiningGeometry();
			if (Stretch != Stretch.UniformToFill)
			{
				return GetNaturalSize();
			}
			double num = constraint.Width;
			double height = constraint.Height;
			if (double.IsInfinity(num) && double.IsInfinity(height))
			{
				return GetNaturalSize();
			}
			if (double.IsInfinity(num) || double.IsInfinity(height))
			{
				num = Math.Min(num, height);
			}
			else
			{
				num = Math.Max(num, height);
			}
			return new Size(num, num);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			double strokeThickness = GetStrokeThickness();
			double num = strokeThickness / 2.0;
			_rect = new Rect(num, num, Math.Max(0.0, finalSize.Width - strokeThickness), Math.Max(0.0, finalSize.Height - strokeThickness));
			switch (Stretch)
			{
				case Stretch.None:
					_rect.Width = _rect.Height = 0.0;
					break;
				case Stretch.Uniform:
					if (_rect.Width > _rect.Height)
					{
						_rect.Width = _rect.Height;
					}
					else
					{
						_rect.Height = _rect.Width;
					}
					break;
				case Stretch.UniformToFill:
					if (_rect.Width < _rect.Height)
					{
						_rect.Width = _rect.Height;
					}
					else
					{
						_rect.Height = _rect.Width;
					}
					break;
			}
			this.ResetRenderedGeometryInShape();
			return finalSize;
		}

		protected override Geometry DefiningGeometry
		{
			get
			{
				if (SlotCorner == SlotHoleCorner.None)
				{
					return new RectangleGeometry(_rect);
				}
				double radius = Math.Min(_rect.Width, _rect.Height) / 2;
				return new RectangleGeometry(_rect, radius, radius);
			}
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			Pen pen = this.GetPenInShape();
			if (SlotCorner == SlotHoleCorner.None)
			{
				drawingContext.DrawRectangle(Fill, pen, _rect);
			}
			double radius = Math.Min(_rect.Width, _rect.Height) / 2;
			drawingContext.DrawRoundedRectangle(Fill, pen, this._rect, radius, radius);
		}

		internal void CacheDefiningGeometry()
		{
			double num = GetStrokeThickness() / 2.0;
			_rect = new Rect(num, num, 0.0, 0.0);
		}

		internal Size GetNaturalSize()
		{
			double strokeThickness = GetStrokeThickness();
			return new Size(strokeThickness, strokeThickness);
		}
	}
}
