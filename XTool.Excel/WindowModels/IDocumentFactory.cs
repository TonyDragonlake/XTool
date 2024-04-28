using FantaziaDesign.Model.Document;

namespace XTool.Excel.WindowModels
{
	public interface IDocumentFactory<TDocument> where TDocument : IDocument
	{
		TDocument CreateDocument();
		bool TryGetExistedDocument(string filePath, out TDocument document);
		DocumentType DocumentType { get; }
	}
}
