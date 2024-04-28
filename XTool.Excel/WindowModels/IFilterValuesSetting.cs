namespace XTool.Excel.WindowModels
{
	public interface IFilterValuesSetting
	{
		int FieldIndex { get; set; }
		string[] Criteria { get; set; }
	}
}
