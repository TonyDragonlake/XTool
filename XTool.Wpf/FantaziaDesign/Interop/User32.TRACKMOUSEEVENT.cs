using System;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct TRACKMOUSEEVENT
		{
			internal uint cbSize;
			public TME dwFlags;
			public IntPtr hwndTrack;
			public uint dwHoverTime;

			public static void Init(ref  TRACKMOUSEEVENT tme)
			{
				tme.cbSize = (uint)Marshal.SizeOf<TRACKMOUSEEVENT>();
			}
		}
	}
}
