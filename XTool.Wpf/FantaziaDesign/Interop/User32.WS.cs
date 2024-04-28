﻿using System;

namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		//[Flags]
		//public enum WS : uint
		//{
		//	OVERLAPPED = 0u,
		//	POPUP = 2147483648u,
		//	CHILD = 1073741824u,
		//	MINIMIZE = 536870912u,
		//	VISIBLE = 268435456u,
		//	DISABLED = 134217728u,
		//	CLIPSIBLINGS = 67108864u,
		//	CLIPCHILDREN = 33554432u,
		//	MAXIMIZE = 16777216u,
		//	BORDER = 8388608u,
		//	DLGFRAME = 4194304u,
		//	VSCROLL = 2097152u,
		//	HSCROLL = 1048576u,
		//	SYSMENU = 524288u,
		//	THICKFRAME = 262144u,
		//	GROUP = 131072u,
		//	TABSTOP = 65536u,
		//	MINIMIZEBOX = 131072u,
		//	MAXIMIZEBOX = 65536u,
		//	CAPTION = 12582912u,
		//	TILED = 0u,
		//	ICONIC = 536870912u,
		//	SIZEBOX = 262144u,
		//	TILEDWINDOW = 13565952u,
		//	OVERLAPPEDWINDOW = 13565952u,
		//	POPUPWINDOW = 2156396544u,
		//	CHILDWINDOW = 1073741824u
		//}
		[Flags]
		public enum WS : uint
		{
			OVERLAPPED = 0x00000000,
			POPUP = 0x80000000,
			CHILD = 0x40000000,
			MINIMIZE = 0x20000000,
			VISIBLE = 0x10000000,
			DISABLED = 0x08000000,
			CLIPSIBLINGS = 0x04000000,
			CLIPCHILDREN = 0x02000000,
			MAXIMIZE = 0x01000000,
			CAPTION = 0x00C00000,    // WS_BORDER | WS_DLGFRAME
			BORDER = 0x00800000,
			DLGFRAME = 0x00400000,
			VSCROLL = 0x00200000,
			HSCROLL = 0x00100000,
			SYSMENU = 0x00080000,
			THICKFRAME = 0x00040000,
			GROUP = 0x00020000,
			TABSTOP = 0x00010000,
			MINIMIZEBOX = 0x00020000,
			MAXIMIZEBOX = 0x00010000,
			TILED = OVERLAPPED,
			ICONIC = MINIMIZE,
			SIZEBOX = THICKFRAME,
			OVERLAPPEDWINDOW = (OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX),
			TILEDWINDOW = OVERLAPPEDWINDOW,
			POPUPWINDOW = (POPUP | BORDER | SYSMENU),
			CHILDWINDOW = (CHILD),
		}
	}
}
