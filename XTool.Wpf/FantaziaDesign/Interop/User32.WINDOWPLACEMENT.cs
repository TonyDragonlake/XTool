using FantaziaDesign.Core;
using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPLACEMENT
		{
			internal int length;
			public uint flags;
			public int showCmd;
			public Vec2i ptMinPosition;
			public Vec2i ptMaxPosition;
			public Vec4i rcNormalPosition;
			//internal Vec4i rcDevice;

			public static void Init(ref WINDOWPLACEMENT wndPlace)
			{
				wndPlace.length = Marshal.SizeOf<WINDOWPLACEMENT>();
			}

		}
	}
}
