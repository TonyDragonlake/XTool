using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace XTool.Excel.ObjectModels
{
	public sealed class XAutoFilter : ComObjectHost<AutoFilter>
	{
		private XRange m_range;

		public XAutoFilter(AutoFilter parent)
		{
			m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
			UpdateContent();
		}

		public XAutoFilter()
		{
		}

		public XRange Range => m_range;
		public IEnumerable<XFilter> Filters => FiltersEnumeration();
		public bool FilterMode => (m_parent?.FilterMode).GetValueOrDefault();

		//Sort Sort { get; }

		private IEnumerable<XFilter> FiltersEnumeration()
		{
			if (m_parent != null)
			{
				var filters = m_parent.Filters;
				foreach (Filter item in filters)
				{
					yield return new XFilter(item);
				}
			}
			yield break;
		}

		public void ApplyFilter()
		{
			m_parent?.ApplyFilter();
		}

		public void ShowAllData()
		{
			m_parent?.ShowAllData();
		}

		// // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
		~XAutoFilter()
		{
			// 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
			Dispose(disposing: false);
		}

		public override void UpdateContent()
		{
			if (HasParent)
			{
				if (m_range != null)
				{
					m_range.SetParent(m_parent.Range);
				}
				else
				{
					m_range = new XRange(m_parent.Range);
				}
			}
		}

		public override void ClearContent()
		{
			m_range?.Dispose();
			m_range = null;
		}

		protected override void ReleaseParent()
		{
			if (m_parent != null)
			{
				Marshal.ReleaseComObject(m_parent);
				m_parent = null;
			}
		}

		protected override void CleanupUnmanagedResources()
		{
			ReleaseParent();
		}

		protected override void CleanupManagedResources()
		{
		}
	}
}
