using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using FantaziaDesign.Resourcable;
using XTool.Excel.Properties;
using XTool.Excel.ResourceModels;

namespace XTool.Excel
{
	public static class XResourceManager
	{
		public static readonly LanguagePackageXmlStreamParser LanguagePackageStreamParser = new LanguagePackageXmlStreamParser();

		public static readonly LanguagePackageXmlStringParser LanguagePackageStringParser = new LanguagePackageXmlStringParser();

		public static string GetResourceUIText(string resourceName)
		{
			return LanguagePackageManager.Current.ProvideResource(resourceName);
		}

		public static Bitmap GetResourceBitmap(string resourceName)
		{
			return Resources.ResourceManager.GetObject(resourceName, null) as Bitmap;
		}

		public static string GetAddInUIResource(string resourceName)
		{
			var asm = Assembly.GetExecutingAssembly();
			var resourceNames = asm.GetManifestResourceNames();
			foreach (var name in resourceNames)
			{
				if (string.Equals(resourceName, name, StringComparison.OrdinalIgnoreCase))
				{
					using (var sr = new StreamReader(asm.GetManifestResourceStream(resourceName)))
					{
						if (sr != null)
						{
							return sr.ReadToEnd();
						}
					}
				}
			}
			return null;
		}
	}
}
