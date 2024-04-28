using FantaziaDesign.Core;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
		public struct MONITORINFOEX
		{
			internal int cbSize;
			public Vec4i rcMonitor;
			public Vec4i rcWork;
			internal int dwFlags;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			internal char[] szDevice;

			public static void Init(ref MONITORINFOEX info)
			{
				info.szDevice = new char[32];
				info.cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
			}

			public static bool IsPrimary(ref MONITORINFOEX info)
			{
				return info.dwFlags == 1;
			}

			public static string GetDeviceName(ref MONITORINFOEX info)
			{
				var array = info.szDevice;
				if (array != null)
				{
					return new string(array);
				}
				return new string('\0', 32);
			}
		}
	}
}
