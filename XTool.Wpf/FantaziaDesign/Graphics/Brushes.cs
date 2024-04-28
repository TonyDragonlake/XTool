using System;
using System.Collections.Generic;
using FantaziaDesign.Core;

namespace FantaziaDesign.Graphics
{
	public enum BrushType
	{
		NullBrush,
		SimpleColorBrush,
		LinearGradientColorBrush,
		RadialGradientColorBrush,
		UnsupportedBrush,
	}

	public abstract class GenericBrush : IEquatable<GenericBrush>
	{
		private static readonly NullBrush s_Empty = new NullBrush();
		public static NullBrush Empty => s_Empty;

		public static bool IsNullOrEmpty(GenericBrush genericBrush)
		{
			if (genericBrush is null)
			{
				return true;
			}
			return genericBrush.m_brushType == BrushType.NullBrush;
		}

		public static bool IsEmpty(GenericBrush genericBrush)
		{
			if (genericBrush is null)
			{
				return false;
			}
			return genericBrush.m_brushType == BrushType.NullBrush;
		}

		private BrushType m_brushType;
		protected float m_opacity = 1f;


		public virtual float Opacity { get => m_opacity; set => m_opacity = value; }

		protected GenericBrush()
		{
			Initialize();
		}

		public BrushType BrushType { get => m_brushType; protected set => m_brushType = value; }

		protected abstract void Initialize();

		public abstract bool Equals(GenericBrush other);

	}

	public sealed class NullBrush : GenericBrush
	{
		internal NullBrush() : base()
		{
		}

		public override float Opacity { get => m_opacity; set { } }

		public override bool Equals(GenericBrush other)
		{
			return IsEmpty(other);
		}

		public override string ToString()
		{
			return "Null Brush";
		}

		protected override void Initialize()
		{
			BrushType = BrushType.NullBrush;
		}
	}

	public abstract class ColorBrushBase : GenericBrush
	{
		protected ColorBrushBase()
		{
		}

		public override float Opacity { get => m_opacity; set => m_opacity = value; }
	}

	public class SimpleColorBrush : ColorBrushBase
	{
		private ColorF4 _color;

		public SimpleColorBrush() : base()
		{
		}

		public SimpleColorBrush(uint colorUInt)
		{
			BrushType = BrushType.SimpleColorBrush;
			_color = new ColorF4(colorUInt);
		}

		public SimpleColorBrush(float r, float g, float b, float a = 1.0f)
		{
			BrushType = BrushType.SimpleColorBrush;
			_color = new ColorF4(r, g, b, a);
		}

		public SimpleColorBrush(ColorF4 color)
		{
			BrushType = BrushType.SimpleColorBrush;
			_color = color;
		}

		public ColorF4 Color { get => _color; set => SetColor(value); }

		public void SetColor(ColorF4 colorF4)
		{
			if (_color is null)
			{
				_color = new ColorF4(colorF4);
			}
			else
			{
				_color.DeepCopyValueFrom(colorF4);
			}
		}

		public void SetColor(ColorB4 colorB4)
		{
			if (_color is null)
			{
				_color = new ColorF4(colorB4);
			}
			else
			{
				_color.DeepCopyValueFrom(colorB4);
			}
		}

		public void SetColorDirectly(ColorF4 colorF4)
		{
			if (colorF4 is null)
			{
				return;
			}
			_color = colorF4;
		}

		protected override void Initialize()
		{
			BrushType = BrushType.SimpleColorBrush;
			_color = new ColorF4();
		}

		public override bool Equals(GenericBrush other)
		{
			if (other.BrushType == BrushType.SimpleColorBrush)
			{
				var colorBrush = other as SimpleColorBrush;
				if (colorBrush is null)
				{
					return false;
				}
				return _color.Equals(colorBrush._color);
			}
			return false;
		}
	}

	public sealed class ColorGradient : IEquatable<ColorGradient>
	{
		private float _offset;
		private ColorF4 _color;

		public float Offset { get => _offset; set => _offset = value; }
		public ColorF4 Color { get => _color; set => SetColor(value); }

		public ColorGradient(float offset, ColorF4 color)
		{
			_offset = offset;
			_color = color is null ? new ColorF4() : color;
		}

		public ColorGradient() : this(0f, null)
		{
		}

		public ColorGradient(float offset, float r, float g, float b, float a = 1f)
		{
			_offset = offset;
			_color = new ColorF4(r, g, b, a);
		}

		public void SetColor(ColorF4 color)
		{
			if (color is null)
			{
				return;
			}
			_color.DeepCopyValueFrom(color);
		}

		public void SetColorDirectly(ColorF4 color)
		{
			if (color is null)
			{
				return;
			}
			_color = color;
		}

		public override string ToString()
		{
			return $"{{{_offset}, {_color}}}";
		}

		public bool Equals(ColorGradient other)
		{
			if (other is null)
			{
				return false;
			}
			return _offset == other._offset
				&& _color.Equals(other._color);
		}
	}

	public abstract class GradientColorBrush : ColorBrushBase
	{
		protected List<ColorGradient> colorGradients;

		protected GradientColorBrush() : base()
		{
		}

		public List<ColorGradient> ColorGradients { get => colorGradients; set => colorGradients = value; }

		protected override void Initialize()
		{
			colorGradients = new List<ColorGradient>();
		}

		protected static bool ColorGradientsEquals(List<ColorGradient> gradients1, List<ColorGradient> gradients2)
		{
			if (gradients1 is null)
			{
				return false;
			}

			if (gradients2 is null)
			{
				return false;
			}
			var len = gradients1.Count;
			if (len == gradients2.Count)
			{
				for (int i = 0; i < len; i++)
				{
					if (!gradients1[i].Equals(gradients2[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

	}

	public class LinearGradientColorBrush : GradientColorBrush
	{
		private VectorF m_gradientVector = new VectorF();

		public LinearGradientColorBrush() : base()
		{
		}

		public VectorF GradientVector { get => m_gradientVector; set => m_gradientVector = value; }

		public void SetGradientVector(VectorF vectorF)
		{
			m_gradientVector.DeepCopyValueFrom(vectorF);
		}

		protected override void Initialize()
		{
			base.Initialize();
			BrushType = BrushType.LinearGradientColorBrush;
			m_gradientVector = new VectorF();
		}

		public override bool Equals(GenericBrush other)
		{
			if (other.BrushType == BrushType.LinearGradientColorBrush)
			{
				var linearBrush = other as LinearGradientColorBrush;
				if (linearBrush is null)
				{
					return false;
				}
				return m_gradientVector.Equals(linearBrush.m_gradientVector)
					&& ColorGradientsEquals(colorGradients, linearBrush.colorGradients);
			}
			return false;
		}
	}

	public class RadialGradientColorBrush : GradientColorBrush
	{
		private PointF _originOffset;
		private EllipseF _gradientEllipse;

		public RadialGradientColorBrush()
		{
		}

		public PointF OriginOffset { get => _originOffset; set => _originOffset = value; }

		public EllipseF GradientEllipse { get => _gradientEllipse; set => _gradientEllipse = value; }

		public override bool Equals(GenericBrush other)
		{
			if (other.BrushType == BrushType.RadialGradientColorBrush)
			{
				var radialBrush = other as RadialGradientColorBrush;
				if (radialBrush is null)
				{
					return false;
				}
				return _originOffset.Equals(radialBrush._originOffset)
					&& _gradientEllipse.Equals(radialBrush._gradientEllipse)
					&& ColorGradientsEquals(colorGradients, radialBrush.colorGradients);
			}
			return false;
		}

		protected override void Initialize()
		{
			base.Initialize();
			BrushType = BrushType.RadialGradientColorBrush;
			_originOffset = new PointF();
			_gradientEllipse = new EllipseF();
		}
	}

}
