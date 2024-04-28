using System;
using System.Text;

namespace FantaziaDesign.Core
{
	public enum HorizontalLocation
	{
		Left,
		Right,
		Center
	}

	public enum VerticalLocation
	{
		Top,
		Bottom,
		Middle
	}

	public enum CornerLocation
	{
		Undefined,
		TopLeft = 1,
		TopRight = 17,
		BottomLeft = 65,
		BottomRight = 81,
	}

	public struct Location : IEquatable<Location>, IDeepCopyable<Location>, ISealable
	{
		byte _bits;
		private bool _isSealed;

		public Location(byte bits)
		{
			_isSealed = false;
			_bits = bits;
		}

		private bool GetBit(byte mask)
		{
			return (_bits & mask) == mask;
		}

		private void SetBit(byte mask, bool actived)
		{
			if (_isSealed)
			{
				return;
			}
			if (actived)
			{
				_bits |= mask;
			}
			else
			{
				_bits &= (byte)~mask;
			}
		}

		public bool Equals(Location other)
		{
			return _bits == other._bits;
		}

		public Location DeepCopy()
		{
			return new Location(_bits);
		}

		public void DeepCopyValueFrom(Location obj)
		{
			_bits = obj._bits;
		}

		public object Clone()
		{
			return new Location(_bits);
		}

		public override bool Equals(object obj)
		{
			if (obj is Location location)
			{
				return Equals(location);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _bits.GetHashCode();
		}

		public override string ToString()
		{
			if (Undefined)
			{
				return "Undefined = 0";
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (Fixed)
			{
				stringBuilder.Append("Fixed | At : ");
				//stringBuilder.Append("Fixed");
			}
			else
			{
				if (RelativeFloating)
				{
					stringBuilder.Append("Floating | Relative To : ");
					//stringBuilder.Append("RelativeFloating");

				}
				else
				{
					stringBuilder.Append($"Floating | Absolute = 3");
					return stringBuilder.ToString();
					//stringBuilder.Append("AbsoluteFloating");
				}
			}

			if (Middle)
			{
				stringBuilder.Append("Middle");
			}
			else
			{
				if (Bottom)
				{
					stringBuilder.Append("Bottom");
				}
				else
				{
					stringBuilder.Append("Top");
				}
			}

			if (Center)
			{
				stringBuilder.Append("Center");
			}
			else
			{
				if (Right)
				{
					stringBuilder.Append("Right");
				}
				else
				{
					stringBuilder.Append("Left");
				}
			}
			////Convert.ToString(_bits, 2)
			stringBuilder.Append($" = {_bits}");
			return stringBuilder.ToString();
		}

		public void Seal()
		{
			if (_isSealed)
			{
				return;
			}
			_isSealed = true;
		}

		public void Unseal()
		{
			if (_isSealed)
			{
				_isSealed = false;
			}
		}

		public bool Undefined
		{
			get => !GetBit(0b01);
			set
			{
				if (value)
				{
					_bits = 0;
				}
			}
		}

		public bool Fixed
		{
			get => !GetBit(0b011);
			set
			{
				if (value)
				{
					SetBit(0b010, false);
				}
			}
		}

		public bool AbsoluteFloating
		{
			get => GetBit(0b011);
			set 
			{
				if (value)
				{
					SetBit(0b011, true);
					SetBit(0b100, false);
				}
			}
		}

		public bool RelativeFloating
		{
			get => GetBit(0b111);
			set
			{
				if (value)
				{
					SetBit(0b111, true);
				}
			}
		}

		public bool Center
		{
			get => GetBit(0b1001);
			set 
			{
				if (value)
				{
					SetBit(0b1001, true);
				}
				else
				{
					SetBit(0b1000, false);
				}
			}
		}

		public bool Left
		{
			get
			{
				if (Center)
				{
					return false;
				}
				return !GetBit(0b10001);
			}
			set
			{
				if (value)
				{
					SetBit(0b10000, false);
					Center = false;
				}
			}
		}

		public bool Right
		{
			get
			{
				if (Center)
				{
					return false;
				}
				return GetBit(0b10001);
			}
			set
			{
				if (value)
				{
					SetBit(0b10001, true);
					Center = false;
				}
			}
		}

		public bool Middle
		{
			get => GetBit(0b100001);
			set 
			{
				if (value)
				{
					SetBit(0b100001, true);

				}
				else
				{
					SetBit(0b100000, false);
				}
			}
		}

		public bool Top
		{
			get 
			{
				if (Middle)
				{
					return false;
				}
				return !GetBit(0b1000001);
			}
			set 
			{
				if (value)
				{
					SetBit(0b1000000, false);
					Middle = false;
				}
			}
		}

		public bool Bottom
		{
			get
			{
				if (Middle)
				{
					return false;
				}
				return GetBit(0b1000001);
			}
			set
			{
				if (value)
				{
					SetBit(0b1000001, true);
					Middle = false;
				}
			}
		}

		public bool TopLeft
		{
			get
			{
				return Top && Left;
			}
			set
			{
				if (value)
				{
					Top = true;
					Left = true;
				}
			}
		}

		public bool MiddleLeft
		{
			get
			{
				return Left && Middle;
			}
			set
			{
				if (value)
				{
					Middle = true;
					Left = true;
				}
			}
		}

		public bool BottomLeft
		{
			get
			{
				return Bottom && Left;
			}
			set
			{
				if (value)
				{
					Bottom = true;
					Left = true;
				}
			}
		}

		public bool TopCenter
		{
			get
			{
				return Center && Top;
			}
			set
			{
				if (value)
				{
					Top = true;
					Center = true;
				}
			}
		}

		public bool MiddleCenter
		{
			get
			{
				return Middle && Center;
			}
			set
			{
				if (value)
				{
					Middle = true;
					Center = true;
				}
			}
		}

		public bool BottomCenter
		{
			get
			{
				return Center && Bottom;
			}
			set
			{
				if (value)
				{
					Bottom = true;
					Center = true;
				}
			}
		}

		public bool TopRight
		{
			get
			{
				return Top && Right;
			}
			set
			{
				if (value)
				{
					Top = true;
					Right = true;
				}
			}
		}

		public bool MiddleRight
		{
			get
			{
				return Middle && Right;
			}
			set
			{
				if (value)
				{
					Middle = true;
					Right = true;
				}
			}
		}

		public bool BottomRight
		{
			get
			{
				return Bottom && Right;
			}
			set
			{
				if (value)
				{
					Bottom = true;
					Right = true;
				}
			}
		}

		public byte Bits { get => _bits; internal set => _bits = value; }

		public bool IsSealed => _isSealed;

		public static bool operator ==(Location lhs, Location rhs)
		{
			return lhs._bits == rhs._bits;
		}

		public static bool operator !=(Location lhs, Location rhs)
		{
			return lhs._bits != rhs._bits;
		}
	}
}
