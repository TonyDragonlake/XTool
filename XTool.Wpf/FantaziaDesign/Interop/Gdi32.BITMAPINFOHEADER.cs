using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class Gdi32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAPINFOHEADER
		{
			internal uint biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public uint biCompression;
			public uint biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;

			public static void Init(ref BITMAPINFOHEADER pbi)
			{
				pbi.biSize = (uint) Marshal.SizeOf<BITMAPINFOHEADER>();
			}

		}
	}
}
