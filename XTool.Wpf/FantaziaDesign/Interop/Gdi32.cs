using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class Gdi32
	{
		[DllImport("GDI32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, ROP_CODE rop);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, DIB_USAGE usage, out IntPtr ppvBits, IntPtr hSection, uint offset);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hGdiObj);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		public static extern int GetObject(IntPtr hGdiObj, int c, IntPtr pv);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, uint start, uint cLines, IntPtr lpvBits, ref BITMAPINFO lpbmi, DIB_USAGE usage);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto)]
		public static extern bool DeleteObject(IntPtr ho);

		[DllImport("GDI32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetDeviceCaps(IntPtr hdc, int index);


	}
}
