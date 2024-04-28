using System;
using System.Runtime.InteropServices;
using FantaziaDesign.Core;

namespace FantaziaDesign.Interop
{
	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct MSG
		{
			internal IntPtr hwnd;
			internal uint message;
			internal IntPtr wParam;
			internal IntPtr lParam;
			internal int time;
			internal Vec2i pt;
		}
	}
}
