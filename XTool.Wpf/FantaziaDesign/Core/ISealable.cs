namespace FantaziaDesign.Core
{
	public interface ISealable
	{
		bool IsSealed { get; }
		void Seal();
		void Unseal();
	}


	//[Serializable]
	//public class QuadrilateralF : StructPack<Float8>, IEquatable<QuadrilateralF>, IDeepCopyable<QuadrilateralF>
	//{
	//	public float X1 { get => _inner.val1; set => _inner.val1 = value; }
	//	public float Y1 { get => _inner.val2; set => _inner.val2 = value; }
	//	public float X2 { get => _inner.val3; set => _inner.val3 = value; }
	//	public float Y2 { get => _inner.val4; set => _inner.val4 = value; }
	//	public float X3 { get => _inner.val5; set => _inner.val5 = value; }
	//	public float Y3 { get => _inner.val6; set => _inner.val6 = value; }
	//	public float X4 { get => _inner.val7; set => _inner.val7 = value; }
	//	public float Y4 { get => _inner.val8; set => _inner.val8 = value; }

	//	public QuadrilateralF()
	//	{
	//		_inner = new Float8();
	//	}

	//	public QuadrilateralF(Float8 float8)
	//	{
	//		_inner = float8 is null ? new Float8() : float8.DeepCopy();
	//	}

	//	public QuadrilateralF(int left, int top, int right, int bottom)
	//	{
	//		_inner = new Float4(left, top, right, bottom);
	//	}

	//	public QuadrilateralF(float left, float top, float right, float bottom)
	//	{
	//		_inner = new Float4(left, top, right, bottom);
	//	}

	//	public QuadrilateralF(RectF rectFSrc)
	//	{
	//		_inner = rectFSrc is null ? new Float4() : rectFSrc._inner.DeepCopy();
	//	}

	//	protected QuadrilateralF(SerializationInfo info, StreamingContext context) : base(info, context)
	//	{
	//	}

	//	public static RectF FromPointAndSize(float x, float y, float w, float h)
	//	{
	//		if (w < 0)
	//		{
	//			w = 0;
	//		}
	//		if (h < 0)
	//		{
	//			h = 0;
	//		}
	//		return new RectF(x, y, x + w, y + h);
	//	}

	//	public void SetRect(float left, float top, float right, float bottom)
	//	{
	//		_inner.val1 = left;
	//		_inner.val2 = top;
	//		_inner.val3 = right;
	//		_inner.val4 = bottom;
	//	}

	//	public void SetFromPointAndSize(float x, float y, float w, float h)
	//	{
	//		if (w < 0)
	//		{
	//			w = 0;
	//		}
	//		if (h < 0)
	//		{
	//			h = 0;
	//		}

	//		_inner.val1 = x;
	//		_inner.val2 = y;
	//		_inner.val3 = x + w;
	//		_inner.val4 = y + h;
	//	}

	//	public static bool InLocationRectFRegion(ref float rx, ref float ry, ref float rw, ref float rh, ref float px, ref float py)
	//	{
	//		var right = rx + rw;
	//		var bottom = ry + rh;
	//		return InRectFRegion(ref rx, ref ry, ref right, ref bottom, ref px, ref py);
	//	}

	//	public static bool InRectFRegion(ref float left, ref float top, ref float right, ref float bottom, ref float px, ref float py)
	//	{
	//		if (IsEmptyRect(ref left, ref top, ref right, ref bottom))
	//		{
	//			return false;
	//		}
	//		return px >= left
	//			&& px <= right
	//			&& py >= top
	//			&& py <= bottom;
	//	}

	//	public static RectF FromPointAndSize(PointF pointF, SizeF sizeF)
	//	{
	//		return new RectF(pointF.X, pointF.Y, pointF.X + sizeF.Width, pointF.Y + sizeF.Height);
	//	}

	//	public override string ToString()
	//	{
	//		return $"{nameof(QuadrilateralF)} : {{X1:{X1}, Y1:{Y1} | X2:{X2}, Y2:{Y2} | X3:{X3}, Y3:{Y3} | X4:{X4}, Y4:{Y4}}}";
	//	}

	//	public override bool Equals(object obj)
	//	{
	//		return Equals(obj as RectF);
	//	}

	//	public override int GetHashCode()
	//	{
	//		return _inner.GetHashCode();
	//	}

	//	public static bool operator ==(QuadrilateralF left, QuadrilateralF right)
	//	{
	//		if (left is null)
	//		{
	//			if (right is null)
	//			{
	//				return true;
	//			}
	//			return false;
	//		}
	//		return left.Equals(right);
	//	}

	//	public static bool operator !=(QuadrilateralF left, QuadrilateralF right)
	//	{
	//		return !(left == right);
	//	}

	//	public bool Contains(PointF point)
	//	{
	//		return Contains(point.X, point.Y);
	//	}

	//	public bool Contains(float x, float y)
	//	{
	//		return !IsEmpty
	//			&& ContainsInternal(x, y);
	//	}

	//	public bool Contains(RectF rect)
	//	{
	//		return !IsEmptyRect(this)
	//			&& !IsEmptyRect(rect)
	//			&& Left <= rect.Left
	//			&& Top <= rect.Top
	//			&& Right >= rect.Right
	//			&& Bottom >= rect.Bottom;
	//	}

	//	public static bool IsEmptyRect(RectF rect)
	//	{
	//		if (rect is null)
	//		{
	//			return true;
	//		}
	//		return rect.Left >= rect.Right || rect.Top >= rect.Bottom;
	//	}

	//	public static bool IsEmptyRect(ref float left, ref float top, ref float right, ref float bottom)
	//	{
	//		return left >= right || top >= bottom;
	//	}

	//	private bool ContainsInternal(float x, float y)
	//	{
	//		return x >= Left
	//			&& x <= Right
	//			&& y >= Top
	//			&& y <= Bottom;
	//	}

	//	public void Offset(float x, float y)
	//	{
	//		_inner.val1 += x;
	//		_inner.val2 += y;
	//		_inner.val3 += x;
	//		_inner.val4 += y;
	//	}

	//	public static void Offset(Float4 float4, int x, int y)
	//	{
	//		if (float4 is null)
	//		{
	//			throw new ArgumentNullException(nameof(float4));
	//		}

	//		float4.val1 += x;
	//		float4.val2 += y;
	//		float4.val3 += x;
	//		float4.val4 += y;
	//	}

	//	public bool Equals(RectF other)
	//	{
	//		if (other is null)
	//		{
	//			return false;
	//		}
	//		return _inner.Equals(other._inner);
	//	}

	//	public QuadrilateralF DeepCopy()
	//	{
	//		return new QuadrilateralF(this);
	//	}

	//	public void DeepCopyValueFrom(QuadrilateralF obj)
	//	{
	//		if (obj is null)
	//		{
	//			return;
	//		}

	//		_inner = obj._inner.DeepCopy();
	//	}

	//	public object Clone()
	//	{
	//		return DeepCopy();
	//	}

	//	public static bool GetRectFInfo(Float4 float4, out float left, out float top, out float width, out float height)
	//	{
	//		left = 0;
	//		top = 0;
	//		width = 0;
	//		height = 0;

	//		if (float4 is null)
	//		{
	//			return false;
	//		}

	//		left = float4.val1;
	//		top = float4.val2;
	//		width = Math.Abs(float4.val3 - left);
	//		height = Math.Abs(float4.val4 - top);
	//		return true;
	//	}

	//	public void ExtendFromRect(RectF rectSrc, ThicknessF thicknessF)
	//	{
	//		if (rectSrc is null)
	//		{
	//			throw new ArgumentNullException(nameof(rectSrc));
	//		}

	//		if (thicknessF is null)
	//		{
	//			throw new ArgumentNullException(nameof(thicknessF));
	//		}

	//		_inner.val1 = rectSrc._inner.val1 - thicknessF._inner.val1;
	//		_inner.val2 = rectSrc._inner.val2 - thicknessF._inner.val2;
	//		_inner.val3 = rectSrc._inner.val3 + thicknessF._inner.val3;
	//		_inner.val4 = rectSrc._inner.val4 + thicknessF._inner.val4;
	//	}

	//	public void ExtendSelf(ThicknessF thicknessF)
	//	{
	//		if (thicknessF is null)
	//		{
	//			throw new ArgumentNullException(nameof(thicknessF));
	//		}

	//		_inner.val1 -= thicknessF._inner.val1;
	//		_inner.val2 -= thicknessF._inner.val2;
	//		_inner.val3 += thicknessF._inner.val3;
	//		_inner.val4 += thicknessF._inner.val4;
	//	}

	//	public static void ExtendRect(RectF rectF, RectF rectSrc, ThicknessF thicknessF)
	//	{
	//		if (rectF is null)
	//		{
	//			throw new ArgumentNullException(nameof(rectF));
	//		}
	//		rectF.ExtendFromRect(rectSrc, thicknessF);
	//	}

	//	public static RectF ExtendedRect(RectF rectSrc, ThicknessF thicknessF)
	//	{
	//		if (rectSrc is null)
	//		{
	//			throw new ArgumentNullException(nameof(rectSrc));
	//		}

	//		if (thicknessF is null)
	//		{
	//			throw new ArgumentNullException(nameof(thicknessF));
	//		}
	//		return new RectF(
	//			rectSrc._inner.val1 - thicknessF._inner.val1,
	//			rectSrc._inner.val2 - thicknessF._inner.val2,
	//			rectSrc._inner.val3 + thicknessF._inner.val3,
	//			rectSrc._inner.val4 + thicknessF._inner.val4
	//			);
	//	}

	//	public void ShrinkFromRect(RectF rectSrc, ThicknessF thicknessF)
	//	{
	//		if (rectSrc is null)
	//		{
	//			throw new ArgumentNullException(nameof(rectSrc));
	//		}

	//		if (thicknessF is null)
	//		{
	//			throw new ArgumentNullException(nameof(thicknessF));
	//		}

	//		_inner.val1 = rectSrc._inner.val1 + thicknessF._inner.val1;
	//		_inner.val2 = rectSrc._inner.val2 + thicknessF._inner.val2;
	//		_inner.val3 = rectSrc._inner.val3 - thicknessF._inner.val3;
	//		_inner.val4 = rectSrc._inner.val4 - thicknessF._inner.val4;
	//	}

	//	public void ShrinkSelf(ThicknessF thicknessF)
	//	{
	//		if (thicknessF is null)
	//		{
	//			throw new ArgumentNullException(nameof(thicknessF));
	//		}

	//		_inner.val1 += thicknessF._inner.val1;
	//		_inner.val2 += thicknessF._inner.val2;
	//		_inner.val3 -= thicknessF._inner.val3;
	//		_inner.val4 -= thicknessF._inner.val4;
	//	}

	//	public static void ShrinkRect(RectF rectF, RectF rectSrc, ThicknessF thicknessF)
	//	{
	//		if (rectF is null)
	//		{
	//			throw new ArgumentNullException(nameof(rectF));
	//		}
	//		rectF.ShrinkFromRect(rectSrc, thicknessF);
	//	}

	//	public static RectF ShrunkRect(RectF rectSrc, ThicknessF thicknessF)
	//	{
	//		if (rectSrc is null)
	//		{
	//			throw new ArgumentNullException(nameof(rectSrc));
	//		}

	//		if (thicknessF is null)
	//		{
	//			throw new ArgumentNullException(nameof(thicknessF));
	//		}
	//		return new RectF(
	//			rectSrc._inner.val1 + thicknessF._inner.val1,
	//			rectSrc._inner.val2 + thicknessF._inner.val2,
	//			rectSrc._inner.val3 - thicknessF._inner.val3,
	//			rectSrc._inner.val4 - thicknessF._inner.val4
	//			);
	//	}

	//	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	//	{
	//		if (info is null)
	//		{
	//			throw new ArgumentNullException(nameof(info));
	//		}
	//		info.AddValue(nameof(Left), Left);
	//		info.AddValue(nameof(Top), Top);
	//		info.AddValue(nameof(Right), Right);
	//		info.AddValue(nameof(Bottom), Bottom);
	//	}

	//	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	//	{
	//		if (info is null)
	//		{
	//			throw new ArgumentNullException(nameof(info));
	//		}
	//		Left = info.GetInt32(nameof(Left));
	//		Top = info.GetInt32(nameof(Top));
	//		Right = info.GetInt32(nameof(Right));
	//		Bottom = info.GetInt32(nameof(Bottom));
	//	}

	//	protected override void InitInnerStruct()
	//	{
	//		_inner = new Float4();
	//	}

	//	public void GetLocationAndSizeRaw(out float x, out float y, out float w, out float h)
	//	{
	//		x = Left;
	//		y = Top;
	//		w = Width;
	//		h = Height;
	//	}

	//	public void GetRectSizeRaw(out float width, out float height)
	//	{
	//		width = Width;
	//		height = Height;
	//	}

	//	public void ZeroRect()
	//	{
	//		_inner.val1 = 0;
	//		_inner.val2 = 0;
	//		_inner.val3 = 0;
	//		_inner.val4 = 0;
	//	}

	//}

}
