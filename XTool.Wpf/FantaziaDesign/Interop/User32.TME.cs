using System;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		[Flags]
		public enum TME : uint
		{
			CANCEL = 0x80000000,
			HOVER = 0x00000001,
			LEAVE = 0x00000002,
			NONCLIENT = 0x00000010,
			QUERY = 0x40000000,
		}
	}
}
