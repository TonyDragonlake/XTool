using FantaziaDesign.Core;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			internal Vec2i ptReserved;
			public Vec2i ptMaxSize;
			public Vec2i ptMaxPosition;
			public Vec2i ptMinTrackSize;
			public Vec2i ptMaxTrackSize;
		}
	}
}
