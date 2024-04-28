using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public uint flags;
		}
	}
}
