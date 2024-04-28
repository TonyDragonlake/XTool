using System;
using Microsoft.Office.Core;
using XTool.Windows;

namespace XTool.Excel
{
	public partial class ThisAddIn
	{
		private void ThisAddIn_Startup(object sender, EventArgs e)
		{
			XToolApp.StartApplication();
		}

		private void ThisAddIn_Shutdown(object sender, EventArgs e)
		{
			XToolApp.ShutdownApplication();
		}

		private void InternalStartup()
		{
			Startup += new EventHandler(ThisAddIn_Startup);
			Shutdown += new EventHandler(ThisAddIn_Shutdown);
		}

		protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
		{
			return new AddInUI();
		}
	}
}
