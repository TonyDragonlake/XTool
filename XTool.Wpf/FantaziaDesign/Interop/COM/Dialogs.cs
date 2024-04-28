using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop.COM
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B4DB1657-70D7-485E-8E3E-6FCB5A5C1802")]
	[ComImport]
	public interface IModalWindow
	{
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		int Show([In] IntPtr parent);
	}

	[Guid("00000114-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IOleWindow
	{
		[PreserveSig]
		int GetWindow(out IntPtr hwnd);
		void ContextSensitiveHelp(int fEnterMode);
	}
}
