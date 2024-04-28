using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
using FantaziaDesign.Resourcable;
using XTool.Excel.WindowModels;
using XTool.Excel.ResourceModels;

namespace XTool.Excel
{
	[ComVisible(true)]
	public class AddInUI : IRibbonExtensibility
	{
		private IRibbonUI m_ui;

		private FilterAssistantManager m_filterAssistantManager;

		public AddInUI()
		{
		}

		public string GetCustomUI(string ribbonID)
		{
			InitializeLanguageSettings();
			ResourceNotifierFactories.RegisterFactory(UITextResource.Factory);
			return XResourceManager.GetAddInUIResource("XTool.Excel.AddInUI.xml");
		}

		private static void InitializeLanguageSettings()
		{
			var langPkgMgr = LanguagePackageManager.Current;
			langPkgMgr.TryLoadLanguagePackage(
				XResourceManager.GetAddInUIResource("XTool.Excel.Assets.Language.lang_en-US.xml"), 
				XResourceManager.LanguagePackageStringParser);
			langPkgMgr.TryLoadLanguagePackage(
				XResourceManager.GetAddInUIResource("XTool.Excel.Assets.Language.lang_zh-CN.xml"), 
				XResourceManager.LanguagePackageStringParser);
			var lcid = Globals.ThisAddIn.Application.LanguageSettings.LanguageID[MsoAppLanguageID.msoLanguageIDUI];
			langPkgMgr.TrySelectLanguage(lcid);
		}

		public void OnAddInUILoad(IRibbonUI ribbonUI)
		{
			m_ui = ribbonUI;
		}

		public string GetLabel(IRibbonControl control)
		{
			var controlId = control.Id;
			var resKey = controlId.Split('.').Last();
			return XResourceManager.GetResourceUIText($"Label_{resKey}");
		}

		public void OnAction(IRibbonControl control)
		{
			if (m_filterAssistantManager is null)
			{
				m_filterAssistantManager = new FilterAssistantManager(Globals.ThisAddIn.Application);
			}
			m_filterAssistantManager.LaunchWindowView();
		}
	}



}
