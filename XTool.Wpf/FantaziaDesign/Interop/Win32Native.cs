using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using FantaziaDesign.Core;

namespace FantaziaDesign.Interop
{
	public static class Win32Native
	{
		public static IntPtr SetWindowLongPtr(IntPtr hWnd, User32.GWL nIndex, IntPtr dwNewLong)
		{
			if (8 == IntPtr.Size)
			{
				return User32.SetWindowLongPtr(hWnd, (int)nIndex, dwNewLong);
			}
			return new IntPtr(User32.SetWindowLong(hWnd, (int)nIndex, dwNewLong.ToInt32()));
		}

		public static bool TranslateLParamPointToClient(IntPtr hWnd, ref IntPtr lParam)
		{
			UnzipToWords(lParam, out var pt);
			if (User32.ScreenToClient(hWnd, ref pt))
			{
				lParam = new IntPtr((pt[1] << 16) | (pt[0] & 65535));
				return true;
			}
			return false;
		}

		public static bool TranslateLParamPointToClient(IntPtr hWnd, IntPtr lParam, out int x, out int y)
		{
			UnzipToWords(lParam, out var pt);
			if (User32.ScreenToClient(hWnd, ref pt))
			{
				x = pt[0]; y = pt[1];
				return true;
			}
			x = 0; y = 0;
			return false;
		}

		public static RectInt GetWindowRectInt(IntPtr hwnd)
		{
			if (User32.GetWindowRect(hwnd, out var windowRect))
			{
				return new RectInt(windowRect);
			}
			else
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		public static bool GetClientRectInScreenCoord(IntPtr hwnd, out Vec4i client)
		{
			if (User32.GetClientRect(hwnd, out client))
			{
				var pt = new Vec2i();
				if (User32.ScreenToClient(hwnd, ref pt))
				{
					Rects.OffsetRect(ref client, -pt[0], -pt[1]);
					return true;
				}
			}
			return false;
		}

		public static void UnzipToWords(IntPtr lparam, ref int loWord, ref int hiWord)
		{
			var ptrValue = lparam.ToInt32();
			loWord = (short)(ptrValue & 65535);
			hiWord = (short)(ptrValue >> 16);
		}

		public static void UnzipToWords(IntPtr lparam, out Vec2i pt)
		{
			var ptrValue = lparam.ToInt32();
			pt = new Vec2i((short)(ptrValue & 65535), (short)(ptrValue >> 16));
		}

		public static void ZipToInt(int loWord, int hiWord, out int res)
		{
			res = hiWord << 16;
			res |= loWord;
		}

		public static int GET_X_LPARAM(IntPtr lParam)
		{
			return LOWORD(lParam.ToInt32());
		}

		public static int GET_Y_LPARAM(IntPtr lParam)
		{
			return HIWORD(lParam.ToInt32());
		}

		public static int HIWORD(int i)
		{
			// ((WORD)((((DWORD_PTR)(_dw)) >> 16) & 0xffff))
			return (short)(i >> 16);
		}

		public static int LOWORD(int i)
		{
			// ((WORD)(((DWORD_PTR)(_dw)) & 0xffff))
			return (short)(i & 65535);
		}

		public static int HIWORD(IntPtr ptr)
		{
			// ((WORD)((((DWORD_PTR)(_dw)) >> 16) & 0xffff))
			return (short)(ptr.ToInt32() >> 16);
		}

		public static int LOWORD(IntPtr ptr)
		{
			// ((WORD)(((DWORD_PTR)(_dw)) & 0xffff))
			return (short)(ptr.ToInt32() & 65535);
		}

	}
}
