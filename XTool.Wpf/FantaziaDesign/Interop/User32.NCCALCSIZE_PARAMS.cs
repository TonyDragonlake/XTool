using FantaziaDesign.Core;
using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS
		{
			public Vec4i rgrc0;
			public Vec4i rgrc1;
			public Vec4i rgrc2;
			public IntPtr lppos;
		}
	}
}
