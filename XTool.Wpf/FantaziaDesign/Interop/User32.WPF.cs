using System;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		[Flags]
		public enum WPF : uint
		{
			SETMINPOSITION = 0x0001,
			RESTORETOMAXIMIZED = 0x0002,
			ASYNCWINDOWPLACEMENT = 0x0004,
		}
	}
}
