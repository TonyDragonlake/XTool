using System;

namespace FantaziaDesign.Core
{
	public interface IThickness<T>
	{
		T Left { get; set; }
		T Top { get; set; }
		T Right { get; set; }
		T Bottom { get; set; }
		bool IsZeroThickness { get; }
		T TotalThicknessInWidth { get; }
		T TotalThicknessInHeight { get; }

		void GetThicknessRaw(out float left, out float top, out float right, out float bottom);
		void SetThickness(float left, float top, float right, float bottom);
		void SetUniformThickness(float thickness);
		void ZeroThickness();
		void MaximizeThickness(float left, float top, float right, float bottom);
		void MinimizeThickness(float left, float top, float right, float bottom);
	}

	public class ThicknessF : IValueContainer<Vec4f>, IEquatable<ThicknessF>, IDeepCopyable<ThicknessF>, IThickness<float>
	{
		private Vec4f m_value;
		public Vec4f Value { get => m_value; set => m_value = value; }
		public float Left { get => m_value[0]; set => m_value[0] = value; }
		public float Top { get => m_value[1]; set => m_value[1] = value; }
		public float Right { get => m_value[2]; set => m_value[2] = value; }
		public float Bottom { get => m_value[3]; set => m_value[3] = value; }

		public ThicknessF()
		{
			m_value = new Vec4f();
		}

		public ThicknessF(Vec4f float4)
		{
			m_value = float4.DeepCopy();
		}

		public ThicknessF(int left, int top, int right, int bottom)
		{
			m_value = new Vec4f(left, top, right, bottom);
		}

		public ThicknessF(float left, float top, float right, float bottom)
		{
			m_value = new Vec4f(left, top, right, bottom);
		}

		public ThicknessF(float thickness)
		{
			m_value = new Vec4f(thickness, thickness, thickness, thickness);
		}

		public ThicknessF(int thickness)
		{
			m_value = new Vec4f(thickness, thickness, thickness, thickness);
		}

		public ThicknessF(ThicknessF thicknessF)
		{
			m_value = thicknessF is null ? new Vec4f() : thicknessF.m_value.DeepCopy();
		}

		public bool IsZeroThickness => m_value[0] == 0f && m_value[1] == 0f && m_value[2] == 0f && m_value[3] == 0f;

		public float TotalThicknessInWidth => m_value[0] + m_value[2];

		public float TotalThicknessInHeight => m_value[1] + m_value[3];

		public override string ToString()
		{
			return $"{nameof(ThicknessF)} : {{L:{Left}, T:{Top}, R:{Right}, B:{Bottom}}}";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as RectF);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public static bool operator ==(ThicknessF left, ThicknessF right)
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

		public static bool operator !=(ThicknessF left, ThicknessF right)
		{
			return !(left == right);
		}

		public bool Equals(ThicknessF other)
		{
			if (other is null)
			{
				return false;
			}
			return m_value.Equals(other.m_value);
		}

		public ThicknessF DeepCopy()
		{
			return new ThicknessF(this);
		}

		public void DeepCopyValueFrom(ThicknessF obj)
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

		public void GetThicknessRaw(out float left, out float top, out float right, out float bottom)
		{
			left = m_value[0];
			top = m_value[1];
			right = m_value[2];
			bottom = m_value[3];
		}

		public void SetThickness(float left, float top, float right, float bottom)
		{
			m_value[0] = left;
			m_value[1] = top;
			m_value[2] = right;
			m_value[3] = bottom;
		}

		public void SetUniformThickness(float thickness)
		{
			m_value[0] = thickness;
			m_value[1] = thickness;
			m_value[2] = thickness;
			m_value[3] = thickness;
		}

		public void ZeroThickness()
		{
			m_value[0] = 0;
			m_value[1] = 0;
			m_value[2] = 0;
			m_value[3] = 0;
		}

		public void MaximizeThickness(float left, float top, float right, float bottom)
		{
			if (left > m_value[0])
			{
				m_value[0] = left;
			}
			if (top > m_value[1])
			{
				m_value[1] = top;
			}
			if (right > m_value[2])
			{
				m_value[2] = right;
			}
			if (bottom > m_value[3])
			{
				m_value[3] = bottom;
			}
		}

		public void MinimizeThickness(float left, float top, float right, float bottom)
		{
			if (left < m_value[0])
			{
				m_value[0] = left;
			}
			if (top < m_value[1])
			{
				m_value[1] = top;
			}
			if (right < m_value[2])
			{
				m_value[2] = right;
			}
			if (bottom < m_value[3])
			{
				m_value[3] = bottom;
			}
		}
	}

}
