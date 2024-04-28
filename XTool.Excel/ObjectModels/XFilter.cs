using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace XTool.Excel.ObjectModels
{
	public sealed class XFilter : ComObjectHost<Filter>
	{
		public XFilter(Filter parent)
		{
			m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}
		public bool On => (m_parent?.On).GetValueOrDefault();
		public object Criteria1 => m_parent?.Criteria1;
		public object Criteria2 => m_parent?.Criteria2;
		public XlAutoFilterOperator? Operator => m_parent?.Operator;

		// // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
		~XFilter()
		{
			// 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
			Dispose(disposing: false);
		}

		public override void UpdateContent() { }

		public override void ClearContent() { }

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

		protected override void CleanupManagedResources() { }
	}
}
