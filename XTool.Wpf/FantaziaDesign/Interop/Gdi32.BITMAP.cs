using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class Gdi32
	{
		//[StructLayout(LayoutKind.Sequential)]
		//internal struct CIEXYZ
		//{
		//	internal int ciexyzX;
		//	internal int ciexyzY;
		//	internal int ciexyzZ;
		//}
		//[StructLayout(LayoutKind.Sequential)]
		//internal struct CIEXYZTRIPLE
		//{
		//	CIEXYZ ciexyzRed;
		//	CIEXYZ ciexyzGreen;
		//	CIEXYZ ciexyzBlue;
		//}
		//[StructLayout(LayoutKind.Sequential)]
		//internal struct BITMAPV5HEADER
		//{
		//	internal uint bV5Size;
		//	internal int bV5Width;
		//	internal int bV5Height;
		//	internal ushort bV5Planes;
		//	internal ushort bV5BitCount;
		//	internal uint bV5Compression;
		//	internal uint bV5SizeImage;
		//	internal int bV5XPelsPerMeter;
		//	internal int bV5YPelsPerMeter;
		//	internal uint bV5ClrUsed;
		//	internal uint bV5ClrImportant;
		//	internal uint bV5RedMask;
		//	internal uint bV5GreenMask;
		//	internal uint bV5BlueMask;
		//	internal uint bV5AlphaMask;
		//	internal uint bV5CSType;
		//	internal CIEXYZTRIPLE bV5Endpoints;
		//	internal uint bV5GammaRed;
		//	internal uint bV5GammaGreen;
		//	internal uint bV5GammaBlue;
		//	internal uint bV5Intent;
		//	internal uint bV5ProfileData;
		//	internal uint bV5ProfileSize;
		//	internal uint bV5Reserved;
		//}


		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAP
		{
			internal int bmType;
			internal int bmWidth;
			internal int bmHeight;
			internal int bmWidthBytes;
			internal ushort bmPlanes;
			internal ushort bmBitsPixel;
			internal IntPtr bmBits;
		}
	}
}
