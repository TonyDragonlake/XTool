using System;
using System.Linq;
using FantaziaDesign.Resourcable;
using FantaziaDesign.Core;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace XTool.Excel.ResourceModels
{
	public sealed class LanguagePackageXmlStreamParser : IValueConverter<Stream, LanguagePackage>
	{
		public Stream ConvertBack(LanguagePackage target)
		{
			throw new NotImplementedException();
		}

		public LanguagePackage ConvertTo(Stream source)
		{
			if (source is null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (source.CanRead)
			{
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(source);
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
			}
			return null;
		}
	}


}
