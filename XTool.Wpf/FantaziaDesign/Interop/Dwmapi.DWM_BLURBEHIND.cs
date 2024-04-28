using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class Dwmapi
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct DWM_BLURBEHIND
		{
			public uint dwFlags;
			public int fEnable;
			public IntPtr hRgnBlur;
			public int fTransitionOnMaximized;
		}


	}


}
