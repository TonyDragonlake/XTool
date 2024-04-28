using System;
using System.Linq;
using FantaziaDesign.Resourcable;
using FantaziaDesign.Core;
using System.Collections.Generic;
using System.Xml;

namespace XTool.Excel.ResourceModels
{
	public sealed class LanguagePackageXmlStringParser : IValueConverter<string, LanguagePackage>
	{
		public string ConvertBack(LanguagePackage target)
		{
			throw new NotImplementedException();
		}

		public LanguagePackage ConvertTo(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw new ArgumentNullException(nameof(source));
			}
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(source.Trim());
			var docEle = xmlDoc.DocumentElement;
			if (docEle.Name == LanguagePackage.ClassName)
			{
				var k = docEle.GetAttribute(LanguagePackage.KeyProperty);
				if (!string.IsNullOrWhiteSpace(k) && docEle.HasChildNodes)
				{
					var list = docEle.ChildNodes.OfType<XmlElement>();
					var dict = new Dictionary<StringRef, StringRef>(docEle.ChildNodes.Count);
					foreach (var item in list)
					{
						var txtKey = item.GetAttribute(LanguagePackage.ItemKeyProperty);
						if (dict.ContainsKey(txtKey))
						{
							dict[txtKey] = item.InnerText;
						}
						else
						{
							dict.Add(txtKey, item.InnerText);
						}
					}
					using (var creator = new LanguagePackage.Creator(k, dict))
					{
						return creator.PackageInstance;
					}
				}
			}
			return null;
		}
	}


}
