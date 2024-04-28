using System;

namespace XTool.Excel.ObjectModels
{
	public interface IComObjectHost<T> : IDisposable
	{
		bool HasParent { get; }
		T Parent { get; }
		void SetParent(T parent);
		void UpdateContent();
	}
}
