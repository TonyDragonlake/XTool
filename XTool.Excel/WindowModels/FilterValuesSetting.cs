using System;
using FantaziaDesign.Core;

namespace XTool.Excel.WindowModels
{
	public sealed class FilterValuesSetting : IFilterValuesSetting
	{
		public FilterValuesSetting()
		{
		}

		public FilterValuesSetting(int fieldIndex, string[] criteria)
		{
			FieldIndex = fieldIndex;
			Criteria = criteria;
		}

		public FilterValuesSetting(IFilterValuesSetting setting)
		{
			if (setting is null)
			{
				throw new ArgumentNullException(nameof(setting));
			}
			FieldIndex = setting.FieldIndex;
			Criteria = setting.Criteria.CopyAsArray();
		}

		public int FieldIndex { get; set; }
		public string[] Criteria { get; set; }
	}
}
