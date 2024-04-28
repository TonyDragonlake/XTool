using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class Gdi32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct RGBQUAD
		{
			internal byte rgbBlue;
			internal byte rgbGreen;
			internal byte rgbRed;
			internal byte rgbReserved;
		}
	}
}
