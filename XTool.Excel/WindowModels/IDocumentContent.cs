using System;
using FantaziaDesign.Core;

namespace XTool.Excel.WindowModels
{
	public interface IDocumentContent : IDeepCopyable<IDocumentContent>, IRuntimeUnique
	{
		Type ContentType { get; }
		object ContentRoot { get; }
		bool TryGetContentRootAs<T>(out T contentRoot);
		bool TrySetContentRoot(object contentRoot);
		IDocument ParentDocument { get; }
		bool TrySetParentDocument(IDocument parentDocument);
	}
}
