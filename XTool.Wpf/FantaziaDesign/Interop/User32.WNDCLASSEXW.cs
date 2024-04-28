using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct WNDCLASSEXW
		{
			internal uint cbSize;
			/* Win 3.x */
			internal CS style;
			internal WNDPROC lpfnWndProc;
			internal int cbClsExtra;
			internal int cbWndExtra;
			internal IntPtr hInstance;
			internal IntPtr hIcon;
			internal IntPtr hCursor;
			internal IntPtr hbrBackground;
			[MarshalAs(UnmanagedType.LPWStr)]
			internal string lpszMenuName;
			[MarshalAs(UnmanagedType.LPWStr)]
			internal string lpszClassName;	
			/* Win 4.0 */
			internal IntPtr hIconSm;
		}
	}
}
