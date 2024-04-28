using System;

namespace FantaziaDesign.Core
{
	public interface IVector<T>
	{
		T StartPointX { get; set; }
		T StartPointY { get; set; }
		T EndPointX { get; set; }
		T EndPointY { get; set; }

		void GetVectorRaw(out T spx, out T spy, out T epx, out T epy);
		void GetStartPointRaw(out T spx, out T spy);
		void GetEndPointRaw(out T epx, out T epy);
		void SetVector(T spx, T spy, T epx, T epy);
		void SetStartPoint(T spx, T spy);
		void SetEndPoint(T epx, T epy);
		void Offset(T offsetX, T offsetY);
		void ZeroStartPoint();
		void ZeroEndPoint();
		void ZeroVector();
	}

	public class VectorF : IValueContainer<Vec4f>, IEquatable<VectorF>, IDeepCopyable<VectorF>, IVector<float>
	{
		private Vec4f m_value;

		public Vec4f Value { get => m_value; set => m_value = value; }

		public float StartPointX { get => m_value[0]; set => m_value[0] = value; }
		public float StartPointY { get => m_value[1]; set => m_value[1] = value; }
		public float EndPointX { get => m_value[2]; set => m_value[2] = value; }
		public float EndPointY { get => m_value[3]; set => m_value[3] = value; }

		public VectorF()
		{
			m_value = new Vec4f();
		}

		public VectorF(float startPointX, float startPointY, float endPointX, float endPointY)
		{
			m_value = new Vec4f(startPointX, startPointY, endPointX, endPointY);
		}

		public VectorF(Vec4f float4)
		{
			m_value = float4.DeepCopy();
		}

		public VectorF(VectorF vectorF)
		{
			if (vectorF is null)
			{
				m_value = new Vec4f();
			}
			else
			{
				m_value = vectorF.m_value.DeepCopy();
			}
		}

		public object Clone()
		{
			return DeepCopy();
		}

		public VectorF DeepCopy()
		{
			return new VectorF(this);
		}

		public void DeepCopyValueFrom(VectorF obj)
		{
			if (obj is null)
			{
				return;
			}

			m_value = obj.m_value.DeepCopy();
		}

		public bool Equals(VectorF other)
		{
			if (other is null)
			{
				return false;
			}

			return m_value.Equals(other.m_value);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public override string ToString()
		{
			return $"VectorF {{StartPoint ({StartPointX},{StartPointY}); EndPoint ({EndPointX},{EndPointY})}}";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as VectorF);
		}

		public void GetVectorRaw(out float spx, out float spy, out float epx, out float epy)
		{
			spx = StartPointX; spy = StartPointY;
			epx = EndPointX; epy = EndPointY;
		}

		public void GetStartPointRaw(out float spx, out float spy)
		{
			spx = StartPointX; spy = StartPointY;
		}

		public void GetEndPointRaw(out float epx, out float epy)
		{
			epx = EndPointX; epy = EndPointY;
		}

		public void SetVector(float spx, float spy, float epx, float epy)
		{
			StartPointX = spx; StartPointY = spy;
			EndPointX = epx; EndPointY = epy;
		}

		public void SetStartPoint(float spx, float spy)
		{
			StartPointX = spx; StartPointY = spy;
		}

		public void SetEndPoint(float epx, float epy)
		{
			EndPointX = epx; EndPointY = epy;
		}

		public void Offset(float offsetX, float offsetY)
		{
			StartPointX += offsetX; StartPointY += offsetY;
			EndPointX += offsetX; EndPointY += offsetY;
		}

		public void ZeroStartPoint()
		{
			StartPointX = 0; StartPointY = 0;
		}

		public void ZeroEndPoint()
		{
			EndPointX = 0; EndPointY = 0;
		}

		public void ZeroVector()
		{
			StartPointX = 0; StartPointY = 0;
			EndPointX = 0; EndPointY = 0;
		}

		public static bool operator ==(VectorF left, VectorF right)
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

		public static bool operator !=(VectorF left, VectorF right)
		{
			return !(left == right);
		}
	}

}
