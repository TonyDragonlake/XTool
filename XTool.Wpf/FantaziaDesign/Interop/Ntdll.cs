using System.Runtime.InteropServices;

namespace FantaziaDesign.Interop
{
	public static class Ntdll
	{
		[DllImport("NTDLL.dll", EntryPoint = "RtlGetNtVersionNumbers", SetLastError = true)]
		public static extern bool GetNtVersionNumbers(ref int dwMajorVer, ref int dwMinorVer, ref int dwBuildNumber);
	}
}
