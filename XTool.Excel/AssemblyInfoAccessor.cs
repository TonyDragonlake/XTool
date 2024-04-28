using System;
using System.Reflection;
using System.Runtime.InteropServices;
using XTool.Excel.ObjectModels;

namespace XTool.Excel
{
	public sealed class AssemblyInfoAccessor
	{
		private readonly Assembly m_assembly;
		private readonly string[] m_infos;
		private bool m_shouldUpdateInfo;
		public AssemblyInfoAccessor(Assembly assembly)
		{
			m_assembly = assembly;
			m_infos = new string[12];
			m_shouldUpdateInfo = m_assembly != null;
		}

		public AssemblyInfoAccessor(Assembly assembly, bool preload)
		{
			m_assembly = assembly;
			m_infos = new string[12];
			if (preload)
			{
				m_shouldUpdateInfo = m_assembly != null;
				GetAssemblyInfos(0);
			}
		}

		private void SetAttribute(int index, string value)
		{
			m_infos[index] = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
		}

		private T GetAssemblyAttribute<T>() where T : Attribute
		{
			object[] attributes = m_assembly.GetCustomAttributes(typeof(T), false);
			if (attributes.Length > 0)
			{
				return attributes[0] as T;
			}
			return default(T);
		}

		private string GetAssemblyInfos(int index)
		{
			// [assembly: AssemblyTitle("XTool.Excel")]
			// [assembly: AssemblyDescription("")]
			// [assembly: AssemblyConfiguration("")]
			// [assembly: AssemblyCompany("Fantazia Design")]
			// [assembly: AssemblyProduct("Fantazia Design - XTool.Excel")]
			// [assembly: AssemblyCopyright("Copyright © 2023 - 2024 Fantazia Design. All rights reserved.")]
			// [assembly: AssemblyTrademark("")]
			// [assembly: AssemblyCulture("")]
			// [assembly: ComVisible(true)]
			// [assembly: Guid("7336f025-d05d-4f50-aa57-8c26696b8776")]
			// [assembly: AssemblyVersion("0.0.1.0")]
			// [assembly: AssemblyFileVersion("0.0.1.0")]

			if (m_shouldUpdateInfo)
			{
				SetAttribute(0, GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title);
				SetAttribute(1, GetAssemblyAttribute<AssemblyDescriptionAttribute>()?.Description);
				SetAttribute(2, GetAssemblyAttribute<AssemblyConfigurationAttribute>()?.Configuration);
				SetAttribute(3, GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company);
				SetAttribute(4, GetAssemblyAttribute<AssemblyProductAttribute>()?.Product);
				SetAttribute(5, GetAssemblyAttribute<AssemblyCopyrightAttribute>()?.Copyright);
				SetAttribute(6, GetAssemblyAttribute<AssemblyTrademarkAttribute>()?.Trademark);
				SetAttribute(7, GetAssemblyAttribute<AssemblyCultureAttribute>()?.Culture);
				SetAttribute(8, GetAssemblyAttribute<ComVisibleAttribute>()?.Value.ToString());
				SetAttribute(9, GetAssemblyAttribute<GuidAttribute>()?.Value);
				SetAttribute(10, m_assembly.GetName().Version.ToString());
				SetAttribute(11, GetAssemblyAttribute<AssemblyFileVersionAttribute>()?.Version);
				m_shouldUpdateInfo = false;
			}
			return m_infos[index];
		}

		public string AssemblyTitle => GetAssemblyInfos(0);
		public string AssemblyDescription => GetAssemblyInfos(1);
		public string AssemblyConfiguration => GetAssemblyInfos(2);
		public string AssemblyCompany => GetAssemblyInfos(3);
		public string AssemblyProduct => GetAssemblyInfos(4);
		public string AssemblyCopyright =>GetAssemblyInfos(5);
		public string AssemblyTrademark => GetAssemblyInfos(6);
		public string AssemblyCulture => GetAssemblyInfos(7);
		public string ComVisible => GetAssemblyInfos(8);
		public string Guid => GetAssemblyInfos(9);
		public string AssemblyVersion => GetAssemblyInfos(10);
		public string AssemblyFileVersion => GetAssemblyInfos(11);

	}
}
