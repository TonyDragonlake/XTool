using System;
using System.Runtime.InteropServices;
using FantaziaDesign.Core;

namespace FantaziaDesign.Interop
{
	public static partial class Dwmapi
	{
		[DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);
		
		[DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true, PreserveSig = true)]
		public static extern void DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

		[DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true, PreserveSig = true)]
		public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, Vec4i pMarInset);

		[DllImport("dwmapi.dll", CharSet = CharSet.Auto, SetLastError = true, PreserveSig = true)]
		public static extern void DwmIsCompositionEnabled(ref bool pfEnabled);
	}


}
