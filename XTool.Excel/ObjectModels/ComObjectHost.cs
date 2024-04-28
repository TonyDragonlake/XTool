using System;
using FantaziaDesign.Core;

namespace XTool.Excel.ObjectModels
{
	public abstract class ComObjectHost<T> : IComObjectHost<T>
	{
		protected T m_parent;

		protected bool m_disposedValue;

		public bool HasParent => m_parent != null;

		public virtual T Parent => m_parent;

		public virtual void SetParent(T parent)
		{
			if (parent.IsNull())
			{
				ReleaseParent();
				ClearContent();
			}
			else
			{
				if (m_disposedValue)
				{
					throw new ObjectDisposedException(GetType().Name);
				}
				ReleaseParent();
				m_parent = parent;
				UpdateContent();
			}
		}

		public abstract void UpdateContent();
		public abstract void ClearContent();
		protected abstract void ReleaseParent();

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposedValue)
			{
				if (disposing)
				{
					CleanupManagedResources();
				}
				CleanupUnmanagedResources();
				m_disposedValue = true;
			}
		}

		protected abstract void CleanupUnmanagedResources();

		protected abstract void CleanupManagedResources();

		// // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
		// ~ComObjectHost()
		// {
		//     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
