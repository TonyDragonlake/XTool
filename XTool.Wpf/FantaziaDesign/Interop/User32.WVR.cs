﻿namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		/// <summary>
		/// WM_NCCALCSIZE "window valid rect" return values
		/// </summary>
		public enum WVR
		{
			ALIGNTOP = 0x0010,
			ALIGNLEFT = 0x0020,
			ALIGNBOTTOM = 0x0040,
			ALIGNRIGHT = 0x0080,
			HREDRAW = 0x0100,
			VREDRAW = 0x0200,
			REDRAW = HREDRAW | VREDRAW,
			VALIDRECTS = 0x0400
		}

	}
}
