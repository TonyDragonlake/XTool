using System;

namespace FantaziaDesign.Core
{
	public interface IPoint<T>
	{
		T X { get; set; }
		T Y { get; set; }
		void GetPointRaw(out T px, out T py);
		void SetPoint(T px, T py);
		void Offset(T offsetX, T offsetY);
		void ZeroPoint();
	}

	public abstract class PointBase<T> : IPoint<T>
	{
		public abstract T X { get; set; }
		public abstract T Y { get; set; }

		public void GetPointRaw(out T px, out T py)
		{
			px = X;
			py = Y;
		}

		public abstract void Offset(T offsetX, T offsetY);

		public void SetPoint(T px, T py)
		{
			X = px;
			Y = py;
		}

		public virtual void ZeroPoint()
		{
			X = default(T);
			Y = default(T);
		}
	}

	public class PointInt : PointBase<int>, IValueContainer<Vec2i>, IEquatable<PointInt>, IDeepCopyable<PointInt>, IPoint<int>
	{
		private Vec2i m_value;
		public Vec2i Value { get => m_value; set => m_value = value; }
		public override int X { get => m_value[0]; set => m_value[0] = value; }
		public override int Y { get => m_value[1]; set => m_value[1] = value; }

		public PointInt(int x, int y)
		{
			m_value = new Vec2i(x, y);
		}

		public PointInt()
		{
			m_value = new Vec2i();
		}

		public PointInt(Vec2i integer2)
		{
			m_value = integer2.DeepCopy();
		}

		public PointInt(PointInt pointInt)
		{
			m_value = pointInt is null ? new Vec2i() : pointInt.m_value.DeepCopy();
		}

		public static PointInt Zero => new PointInt();

		public override string ToString()
		{
			return $"{nameof(PointInt)}:{{X:{m_value[0]}, Y:{m_value[1]}}}";
		}

		public bool Equals(PointInt other)
		{
			return m_value.Equals(other.m_value);
		}

		public PointInt DeepCopy()
		{
			return new PointInt(this);
		}

		public void DeepCopyValueFrom(PointInt obj)
		{
			if (obj is null)
			{
				return;
			}

			m_value = obj.m_value.DeepCopy();
		}

		public object Clone()
		{
			return DeepCopy();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as PointInt);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public static bool operator ==(PointInt left, PointInt right)
		{
			if (left is null)
			{
				if (right is null)
				{
					return true;
				}
				return false;
			}
			return left.Equals(right);
		}

		public static bool operator !=(PointInt left, PointInt right)
		{
			return !(left == right);
		}

		public override void Offset(int offsetX, int offsetY)
		{
			X += offsetX;
			Y += offsetY;
		}

	}

	public class PointF : PointBase<float>, IValueContainer<Vec2f>, IEquatable<PointF>, IDeepCopyable<PointF>, IPoint<float>
	{
		private Vec2f m_value;

		public Vec2f Value { get => m_value; set => m_value = value; }

		public override float X { get => m_value[0]; set => m_value[0] = value; }
		public override float Y { get => m_value[1]; set => m_value[1] = value; }

		public PointF()
		{
			m_value = new Vec2f();
		}

		public PointF(float x, float y)
		{
			m_value = new Vec2f(x, y);
		}

		public PointF(Vec2f float2)
		{
			m_value = float2.DeepCopy();
		}

		public PointF(PointF pointF)
		{
			m_value = pointF is null ? new Vec2f() : pointF.m_value.DeepCopy();
		}

		public static PointF Zero
		{
			get => new PointF();
		}

		public override string ToString()
		{
			return $"{nameof(PointF)}:{{X:{m_value[0]}, Y:{m_value[1]}}}";
		}

		public bool Equals(PointF other)
		{
			return m_value.Equals(other.m_value);
		}

		public PointF DeepCopy()
		{
			return new PointF(this);
		}

		public void DeepCopyValueFrom(PointF obj)
		{
			if (obj is null)
			{
				return;
			}

			m_value = obj.m_value.DeepCopy();
		}

		public object Clone()
		{
			return DeepCopy();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as PointF);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public static bool operator ==(PointF left, PointF right)
		{
			if (left is null)
			{
				if (right is null)
				{
					return true;
				}
				return false;
			}
			return left.Equals(right);
		}

		public static bool operator !=(PointF left, PointF right)
		{
			return !(left == right);
		}

		public override void Offset(float offsetX, float offsetY)
		{
			X += offsetX;
			Y += offsetY;
		}

	}

}
