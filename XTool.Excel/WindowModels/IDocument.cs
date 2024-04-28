using System.IO;
using FantaziaDesign.Core;
using FantaziaDesign.Model.Document;

namespace XTool.Excel.WindowModels
{
	public interface IDocument : IRuntimeUnique
	{
		FileInfo FileInfo { get; }
		bool OverrideFileInfo(string filePath);
		DocumentType DocumentType { get; }
		IDocumentContent Content { get; }
		bool OverrideContent(IDocumentContent documentContent);
		bool IsAnonymous { get; }
		bool IsLoaded { get; }
		bool IsSaved { get; }
		bool IsChangingDocumentContent { get; }
		bool Load();
		bool Unload();
		bool Save();
		bool TrySaveAs(string filePath, out IDocument document);
		void OnContentRootChanged();
		bool FilePathEquals(string filePath);
	}
}
