using System;
using System.Text;
using FantaziaDesign.Model.Message;

namespace XTool.Excel.WindowModels
{
	// Fantazia Design - XTool.Excel
	// Copyright © 2023 - 2024 Fantazia Design. All rights reserved.
	public sealed class AboutInformationDialog : SimpleDialogBase
	{
		private AssemblyInfoAccessor m_infoAccessor;
		private string m_copyright;
		private string m_version;

		public AboutInformationDialog(AssemblyInfoAccessor infoAccessor) : base()
		{
			m_infoAccessor = infoAccessor ?? throw new ArgumentNullException(nameof(infoAccessor));
			ContentTemplateNameKey = "AboutDialogTemplate";
		}

		public string ProductName => m_infoAccessor.AssemblyProduct;

		public string Copyright {
			get
			{
				if (string.IsNullOrWhiteSpace(m_copyright))
				{
					var copy = m_infoAccessor.AssemblyCopyright;
					if (!string.IsNullOrWhiteSpace(copy))
					{
						var array = copy.Split('.');
						if (array.Length > 0)
						{
							bool first = true;
							var stringBuilder = new StringBuilder();
							for (int i = 0; i < array.Length; i++)
							{
								var item = array[i];
								if (!string.IsNullOrWhiteSpace(item))
								{
									if (first)
									{
										first = false;
									}
									else
									{
										stringBuilder.Append('\n');
									}
									stringBuilder.Append(item.Trim());
									stringBuilder.Append('.');
								}
							}
							m_copyright = stringBuilder.ToString();
						}
					}

				}
				return m_copyright;

			}
		}

		public string Version
		{
			get
			{
				if (string.IsNullOrWhiteSpace(m_version))
				{
					m_version = "v " + m_infoAccessor.AssemblyVersion;
				}
				return m_version;
			}
		}
	}
}
