using System;
using System.Windows;

namespace XTool.Windows
{
	public partial class XToolApp : Application
	{
		private static bool s_hadApp;

		public static void StartApplication()
		{
			if (Current is null)
			{
				if (s_hadApp)
				{
					throw new InvalidOperationException("Application cannot start twice.");
				}

				new XToolApp 
					{ ShutdownMode = ShutdownMode.OnExplicitShutdown }
					.InitializeComponent();
				s_hadApp = true;
			}
		}

		public static void ShutdownApplication()
		{
			Current?.Shutdown();
		}


		public XToolApp()
		{
		}
	}
}
