using System;

namespace FantaziaDesign.Core
{
	[Flags]
	public enum ArgbChannel
	{
		None = 0,
		Alpha = 1,
		Red = 2,
		Green = 4,
		Blue = 8,
		All = Alpha | Red | Green | Blue
	}

	public interface IArgbColor<T>
	{
		T A { get; set; }
		T R { get; set; }
		T G { get; set; }
		T B { get; set; }
		void SetColorFromInt32Argb(int colorInt);
		void SetColorFromBytesArgb(byte r, byte g, byte b, byte a = 255);
		void SetColorFromFloatArgb(float r, float g, float b, float a = 1.0f);
		void ZeroColor();
	}

	public interface IArgbColorContext<T> : IArgbColor<T>
	{
		IArgbColor<T> ArgbColor { get; }
		ArgbChannel ChangedArgbChannel { get; }
		void ResetChangedChannel();
	}

	[Flags]
	public enum AhsvChannel
	{
		None = 0,
		Alpha = 1,
		Hue = 2,
		Saturation = 4,
		Value = 8,
		All = Alpha | Hue | Saturation | Value
	}

	public interface IAhsvColor<T>
	{
		T Alpha { get; set; }
		T Hue { get; set; }
		T Saturation { get; set; }
		T Value { get; set; }
		void SetColorFromFloatAhsv(float h, float s, float v, float a = 1.0f);
	}

	public interface IAhsvColorContext<T>
	{
		IAhsvColor<T> AhsvColor { get; }
		AhsvChannel ChangedAhsvChannel { get; }
		void ResetChangedChannel();
	}

	public interface IWritableAhsvColorContext<T> : IAhsvColorContext<T>
	{
		void AppendChangedAhsvChannel(AhsvChannel channel);
		void RemoveChangedAhsvChannel(AhsvChannel channel);
		void SetChangedAhsvChannel(AhsvChannel channel);
	}

	public interface IColorChannels : IArgbColor<byte>, IAhsvColor<float>, IArgbColorContext<byte>, IAhsvColorContext<float>
	{

	}

	public interface IChildPositionInfo<T> : IPseudoArray<T>
	{
		T ContainerWidth { get; set; }
		T ContainerHeight { get; set; }
		T ChildPositionX { get; set; }
		T ChildPositionY { get; set; }
		void SetContainerSize(T width, T height);
		void SetChildPosition(T positionX, T positionY);
	}

	public static class ColorsUtility
	{
		// bytes -> int 
		public static int ColorInt(byte a, byte r, byte g, byte b)
		{
			var bytes = new CompatibleInt32();
			bytes[0] = b;
			bytes[1] = g;
			bytes[2] = r;
			bytes[3] = a;
			return bytes;
		}
		// floats -> int 
		public static int ColorInt(float af, float rf, float gf, float bf)
		{
			var bytes = new CompatibleInt32();
			bytes[0] = ColorByte(bf);
			bytes[1] = ColorByte(gf);
			bytes[2] = ColorByte(rf);
			bytes[3] = ColorByte(af);
			return bytes;
		}
		// int -> floats
		public static void ColorFloats(int colorInt, out float af, out float rf, out float gf, out float bf)
		{
			var bytes = new CompatibleInt32(colorInt);
			bf = ColorFloat(bytes[0]);
			gf = ColorFloat(bytes[1]);
			rf = ColorFloat(bytes[2]);
			af = ColorFloat(bytes[3]);
		}
		// uint -> floats
		public static void ColorFloats(uint colorInt, out float af, out float rf, out float gf, out float bf)
		{
			var bytes = new CompatibleInt32(colorInt);
			bf = ColorFloat(bytes[0]);
			gf = ColorFloat(bytes[1]);
			rf = ColorFloat(bytes[2]);
			af = ColorFloat(bytes[3]);
		}
		// uint -> bytes
		public static void ColorBytes(int colorInt, out byte a, out byte r, out byte g, out byte b)
		{
			var bytes = new CompatibleInt32(colorInt);
			b = bytes[0];
			g = bytes[1];
			r = bytes[2];
			a = bytes[3];
		}
		// floats -> bytes
		public static void ColorBytes(float af, float rf, float gf, float bf, out byte a, out byte r, out byte g, out byte b)
		{
			a = ColorByte(af);
			r = ColorByte(rf);
			g = ColorByte(gf);
			b = ColorByte(bf);
		}
		// bytes -> floats
		public static void ColorFloats(byte a, byte r, byte g, byte b, out float af, out float rf, out float gf, out float bf)
		{
			af = ColorFloat(a);
			rf = ColorFloat(r);
			gf = ColorFloat(g);
			bf = ColorFloat(b);
		}

		public static byte ColorByte(float f)
		{
			var i = Convert.ToInt32(f * 255.0f);
			return (byte)(i & 0xFF);
		}

		public static float ColorFloat(float value)
		{
			if (value > 1f)
			{
				value = 1f;
			}
			if (value < 0f)
			{
				value = 0f;
			}
			return value;
		}

		public static float ColorFloat(byte value)
		{
			return value / 255.0f;
		}

		public static void ConvertArgbB4ToAhsvF4(byte a, byte r, byte g, byte b, out float af, out float hf, out float sf, out float vf)
		{
			af = ColorFloat(a);
			ConvertRgbB3ToHsvF3(r, g, b, out hf, out sf, out vf);
		}

		public static void ConvertRgbB3ToHsvF3(byte r, byte g, byte b, out float hf, out float sf, out float vf)
		{
			GetMinMaxByte(out byte min, out byte max, r, g, b);
			float delta = max - min;
			sf = max == 0 ? 0 : delta / max;
			float h = 0;
			if (sf != 0)
			{
				if (r == max)
				{
					h = g - b;
				}
				else if (g == max)
				{
					h = (b - r) + 2 * delta;
				}
				else if (b == max)
				{
					h = (r - g) + 4 * delta;
				}
				h /= delta * 6;
				if (h < 0.0)
				{
					h += 1;
				}
			}
			hf = h;
			vf = max / 255.0f;
		}

		private static void GetMinMaxByte(out byte min, out byte max, params byte[] bytes)
		{
			if (bytes is null || bytes.Length <= 0)
			{
				min = max = 0;
				return;
			}
			min = max = bytes[0];
			for (int i = 1; i < bytes.Length; i++)
			{
				var b = bytes[i];
				if (min > b)
				{
					min = b;
				}
				if (max < b)
				{
					max = b;
				}
			}
		}

		public static void ConvertAhsvF4ToArgbB4(float af, float hf, float sf, float vf, out byte a, out byte r, out byte g, out byte b)
		{
			a = ColorByte(af);
			ConvertHsvF3ToRgbB3(hf, sf, vf, out r, out g, out b);
		}

		public static void ConvertHsvF3ToRgbB3(float hf, float sf, float vf, out byte r, out byte g, out byte b)
		{
			if (sf == 0)
			{
				var temp = ColorByte(vf);
				r = temp;
				g = temp;
				b = temp;
				return;
			}
			EnsureHSVRange(ref hf, ref sf, ref vf);
			float f, pf, qf, tf;
			if (hf == 1)
			{
				hf = 0;
			}
			else
			{
				hf *= 6;
			}

			int i = (int)Math.Truncate(hf);
			f = hf - i;

			pf = vf * (1 - sf);
			qf = vf * (1 - (sf * f));
			tf = vf * (1 - (sf * (1 - f)));

			switch (i)
			{
				case 0:
					{
						r = ColorByte(vf);
						g = ColorByte(tf);
						b = ColorByte(pf);
						break;
					}
				case 1:
					{
						r = ColorByte(qf);
						g = ColorByte(vf);
						b = ColorByte(pf);
						break;
					}
				case 2:
					{
						r = ColorByte(pf);
						g = ColorByte(vf);
						b = ColorByte(tf);
						break;
					}
				case 3:
					{
						r = ColorByte(pf);
						g = ColorByte(qf);
						b = ColorByte(vf);
						break;
					}
				case 4:
					{
						r = ColorByte(tf);
						g = ColorByte(pf);
						b = ColorByte(vf);
						break;
					}
				default:
					{
						r = ColorByte(vf);
						g = ColorByte(pf);
						b = ColorByte(qf);
						break;
					}
			}
		}

		private static void EnsureHSVRange(ref float h, ref float s, ref float v)
		{
			if (h < 0 || h > 1)
			{
				h -= (float)Math.Truncate(h);
			}
			if (s < 0)
			{
				s = 0;
			}
			else if (s > 1)
			{
				s = 1;
			}
			if (v < 0)
			{
				v = 0;
			}
			else if (v > 1)
			{
				v = 1;
			}
		}

		public static byte ConvertRgbB3ToGrayscaleByte(byte r, byte g, byte b)
		{
			return (byte)(((r * 19595 + g * 38469 + b * 7472) >> 16) & 0xFF);
		}


	}
	// in order of BGRA
	public class ColorB4 : IValueContainer<Vec4b>, IEquatable<ColorB4>, IDeepCopyable<ColorB4>, IEquatable<ColorF4>, IDeepCopyable<ColorF4>, IArgbColor<byte>
	{
		private Vec4b m_value;
		public Vec4b Value { get => m_value; set => m_value = value; }

		public ColorB4()
		{
			m_value = new Vec4b();
		}

		public ColorB4(int colorInt)
		{
			m_value = new Vec4b(colorInt);
		}

		public ColorB4(uint colorUInt)
		{
			m_value = new Vec4b(colorUInt);
		}

		public ColorB4(byte b, byte g, byte r, byte a = byte.MaxValue)
		{
			m_value = new Vec4b(b, g, r, a);
		}

		public ColorB4(Vec4b byte4)
		{
			m_value = byte4.DeepCopy();
		}

		public ColorB4(ColorB4 colorB4)
		{
			m_value = colorB4 is null ? new Vec4b() : colorB4.m_value.DeepCopy();
		}

		public ColorB4(Vec4f float4)
		{
			m_value = new Vec4b(ColorsUtility.ColorInt(float4[0], float4[1], float4[2], float4[3]));
		}

		public ColorB4(ColorF4 colorF4)
		{
			m_value = colorF4 is null
				? new Vec4b()
				: new Vec4b(ColorsUtility.ColorInt(colorF4.A, colorF4.R, colorF4.G, colorF4.B));
		}

		public ColorB4(float r, float g, float b, float a = 1.0f)
		{
			m_value = new Vec4b(ColorsUtility.ColorInt(a, r, g, b));
		}

		public byte A { get => m_value[3]; set => m_value[3] = value; }
		public byte R { get => m_value[2]; set => m_value[2] = value; }
		public byte G { get => m_value[1]; set => m_value[1] = value; }
		public byte B { get => m_value[0]; set => m_value[0] = value; }

		public object Clone()
		{
			return DeepCopy();
		}

		public ColorB4 DeepCopy()
		{
			return new ColorB4(this);
		}

		public void DeepCopyValueFrom(ColorB4 obj)
		{
			if (obj is null)
			{
				return;
			}

			m_value = obj.m_value.DeepCopy();
		}

		public bool Equals(ColorB4 other)
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
			return string.Format("#{0:X8}", PseudoArrays.GetItem<int>(m_value, 0));
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ColorB4);
		}

		public bool Equals(ColorF4 other)
		{
			if (other is null)
			{
				return false;
			}
			return PseudoArrays.GetItem<int>(m_value, 0) == ColorsUtility.ColorInt(other.A, other.R, other.G, other.B);
		}

		ColorF4 IDeepCopyable<ColorF4>.DeepCopy()
		{
			return new ColorF4(this);
		}

		public void DeepCopyValueFrom(ColorF4 obj)
		{
			if (obj is null)
			{
				return;
			}
			PseudoArrays.SetItem<int>(m_value, 0, ColorsUtility.ColorInt(obj.A, obj.R, obj.G, obj.B));
		}

		public void SetColorFromInt32Argb(int colorInt)
		{
			PseudoArrays.SetItem<int>(m_value, 0, colorInt);
		}

		public void SetColorFromFloatArgb(float r, float g, float b, float a = 1.0f)
		{
			SetColorFromInt32Argb(ColorsUtility.ColorInt(a, r, g, b));
		}

		public void ZeroColor()
		{
			PseudoArrays.SetItem<int>(m_value, 0, 0);
		}

		public void SetColorFromBytesArgb(byte r, byte g, byte b, byte a = 255)
		{
			m_value[0] = b;
			m_value[1] = g;
			m_value[2] = r;
			m_value[3] = a;
		}

		public static bool operator ==(ColorB4 left, ColorB4 right)
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

		public static bool operator !=(ColorB4 left, ColorB4 right)
		{
			return !(left == right);
		}
	}

	// in order of RGBA<float>, same with D2D::ColorF

	/*
	#ifndef D3DCOLORVALUE_DEFINED
	typedef struct _D3DCOLORVALUE {
		float r;
		float g;
		float b;
		float a;
	} D3DCOLORVALUE;

	#define D3DCOLORVALUE_DEFINED
	#endif

	typedef D3DCOLORVALUE DXGI_RGBA;
	 */
	public class ColorF4 : IValueContainer<Vec4f>, IEquatable<ColorF4>, IDeepCopyable<ColorF4>, IEquatable<ColorB4>, IDeepCopyable<ColorB4>, IArgbColor<float>
	{
		private Vec4f m_value;
		public Vec4f Value { get => m_value; set => m_value = value; }
		public float R { get => m_value[0]; set => m_value[0] = ColorsUtility.ColorFloat(value); }
		public float G { get => m_value[1]; set => m_value[1] = ColorsUtility.ColorFloat(value); }
		public float B { get => m_value[2]; set => m_value[2] = ColorsUtility.ColorFloat(value); }
		public float A { get => m_value[3]; set => m_value[3] = ColorsUtility.ColorFloat(value); }

		public ColorF4()
		{
			m_value = new Vec4f();
		}

		public ColorF4(Vec4f float4)
		{
			m_value = new Vec4f(ColorsUtility.ColorFloat(float4[0]), ColorsUtility.ColorFloat(float4[1]), ColorsUtility.ColorFloat(float4[2]), ColorsUtility.ColorFloat(float4[3]));
		}

		public ColorF4(float r, float g, float b, float a = 1.0f)
		{
			m_value = new Vec4f(ColorsUtility.ColorFloat(r), ColorsUtility.ColorFloat(g), ColorsUtility.ColorFloat(b), ColorsUtility.ColorFloat(a));
		}

		public ColorF4(byte b, byte g, byte r, byte a = byte.MaxValue)
		{
			m_value = new Vec4f(ColorsUtility.ColorFloat(r), ColorsUtility.ColorFloat(g), ColorsUtility.ColorFloat(b), ColorsUtility.ColorFloat(a));
		}

		public ColorF4(ColorF4 colorF4)
		{
			m_value = colorF4 is null
				? new Vec4f()
				: colorF4.m_value.DeepCopy();
		}

		public ColorF4(ColorB4 colorB4)
		{
			if (colorB4 is null)
			{
				m_value = new Vec4f();
			}
			else
			{
				ColorsUtility.ColorFloats(colorB4.Value, out var af, out var rf, out var gf, out var bf);
				m_value = new Vec4f(rf, gf, bf, af);
			}
		}

		public ColorF4(Vec4b byte4)
		{
			ColorsUtility.ColorFloats(byte4, out var af, out var rf, out var gf, out var bf);
			m_value = new Vec4f(rf, gf, bf, af);
		}

		public ColorF4(int colorInt)
		{
			if (colorInt == 0)
			{
				m_value = new Vec4f();
			}
			else
			{
				ColorsUtility.ColorFloats(colorInt, out var af, out var rf, out var gf, out var bf);
				m_value = new Vec4f(rf, gf, bf, af);
			}
		}

		public ColorF4(uint colorUInt)
		{
			if (colorUInt == 0u)
			{
				m_value = new Vec4f();
			}
			else
			{
				ColorsUtility.ColorFloats(colorUInt, out var af, out var rf, out var gf, out var bf);
				m_value = new Vec4f(rf, gf, bf, af);
			}
		}

		public object Clone()
		{
			return DeepCopy();
		}

		public ColorF4 DeepCopy()
		{
			return new ColorF4(this);
		}

		public void DeepCopyValueFrom(ColorF4 obj)
		{
			if (obj is null)
			{
				return;
			}

			m_value = obj.m_value.DeepCopy();
		}

		public bool Equals(ColorF4 other)
		{
			if (other is null)
			{
				return false;
			}

			return m_value.Equals(other.m_value);
		}

		public override string ToString()
		{
			return string.Format("rgba({0:f2},{1:f2},{2:f2},{3:f2})", m_value[0], m_value[1], m_value[2], m_value[3]);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ColorF4);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public bool Equals(ColorB4 other)
		{
			if (other is null)
			{
				return false;
			}
			return other.Equals(this);
		}

		ColorB4 IDeepCopyable<ColorB4>.DeepCopy()
		{
			return new ColorB4(this);
		}

		public void DeepCopyValueFrom(ColorB4 obj)
		{
			if (obj is null)
			{
				return;
			}
			SetColorFromInt32Argb(obj.Value);
		}

		public void SetColorFromInt32Argb(int colorInt)
		{
			if (colorInt == 0)
			{
				m_value[0] = 0f;
				m_value[1] = 0f;
				m_value[2] = 0f;
				m_value[3] = 0f;
			}
			else
			{
				ColorsUtility.ColorFloats(colorInt, out float af, out float rf, out float gf, out float bf);
				m_value[0] = rf;
				m_value[1] = gf;
				m_value[2] = bf;
				m_value[3] = af;
			}
		}

		public void SetColorFromFloatArgb(float r, float g, float b, float a = 1.0f)
		{
			m_value[0] = ColorsUtility.ColorFloat(r);
			m_value[1] = ColorsUtility.ColorFloat(g);
			m_value[2] = ColorsUtility.ColorFloat(b);
			m_value[3] = ColorsUtility.ColorFloat(a);
		}

		public void ZeroColor()
		{
			m_value[0] = 0f;
			m_value[1] = 0f;
			m_value[2] = 0f;
			m_value[3] = 0f;
		}

		public void SetColorFromBytesArgb(byte r, byte g, byte b, byte a = 255)
		{
			ColorsUtility.ColorFloats(a, r, g, b, out float af, out float rf, out float gf, out float bf);
			m_value[0] = rf;
			m_value[1] = gf;
			m_value[2] = bf;
			m_value[3] = af;
		}

		public static bool operator ==(ColorF4 left, ColorF4 right)
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

		public static bool operator !=(ColorF4 left, ColorF4 right)
		{
			return !(left == right);
		}
	}

}
