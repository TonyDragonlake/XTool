using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class Gdi32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAPINFO
		{
			public BITMAPINFOHEADER bmiHeader;
			public RGBQUAD bmiColors;
		}
	}
}
