using System;
using System.Diagnostics;
using System.IO;
using FantaziaDesign.Interop;
using Microsoft.Win32;

namespace FantaziaDesign.Wpf.Windows
{
	public static class ApplicationEnvironment
	{
		public static Process Process => Process.GetCurrentProcess();
		public static int ProcessId => Process.Id;
		public static string MainModuleFilePath => Process.MainModule.FileName;
		public static string MainModuleDirectory => Path.GetDirectoryName(MainModuleFilePath);
		public static readonly Version OSVersion = GetWindowsNtVersion();
		private static Version GetWindowsNtVersion()
		{
			int major = 0, minor = 0, build = 0;
			if (Ntdll.GetNtVersionNumbers(ref major, ref minor, ref build))
			{
				build &= 0x0ffff;
				return new Version(major, minor, build);
			}
			return Environment.OSVersion.Version;
		}
		private static byte s_runModeCode;
		public static bool IsRunInCompatibleMode => GetIsRunInCompatibleMode();
		private static bool GetIsRunInCompatibleMode()
		{
			if (s_runModeCode == 0)
			{
				var AppCompatFlags = "SoftWare\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers";
				var rkey = Registry.CurrentUser.OpenSubKey(AppCompatFlags, false);
				if (rkey != null)
				{
					var rvalue = rkey.GetValue(MainModuleFilePath);
					if (rvalue != null)
					{
						s_runModeCode = 2;
						s_runCommand = rvalue as string;
					}
				}
				s_runModeCode = 1;
			}
			return s_runModeCode == 2;
		}
		private static string s_runCommand;
		public static string RunCommand => s_runCommand;
	}
}
