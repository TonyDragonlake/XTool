using System;

namespace XTool.Excel.ObjectModels
{
	public enum XCellTypes
	{
		Null,
		Boolean,
		Number,
		Error,
		SharedString,
		String,
		InlineString,
		Date
	}

	public sealed class XCellValue
	{
		private object m_value;
		private XCellTypes m_cellType;

		public XCellValue(object value)
		{
			if (value is null)
			{
				m_cellType = XCellTypes.Null;
			}
			else
			{
				m_value = value;
				var typeCode = Type.GetTypeCode(value.GetType());
				m_cellType = GetCellTypeFromTypeCode(typeCode);
			}
		}

		private static XCellTypes GetCellTypeFromTypeCode(TypeCode typeCode)
		{
			XCellTypes cellType = default(XCellTypes);
			switch (typeCode)
			{
				case TypeCode.Boolean:
					cellType = XCellTypes.Boolean;
					break;
				case TypeCode.Int32:
				case TypeCode.UInt32:
					cellType = XCellTypes.Error;
					break;
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					cellType = XCellTypes.Number;
					break;
				case TypeCode.DateTime:
					cellType = XCellTypes.Date;
					break;
				default:
					cellType = XCellTypes.String;
					break;
			}
			return cellType;
		}

		public object RawValue => m_value;
		public XCellTypes CellType => m_cellType;

		public bool? AsBoolean()
		{
			if (m_cellType == XCellTypes.Boolean)
			{
				return (bool)m_value;
			}
			return null;
		}

		public double? AsNumber()
		{
			if (m_cellType == XCellTypes.Number)
			{
				return (double)m_value;
			}
			return null;
		}

		public int? AsError()
		{
			if (m_cellType == XCellTypes.Error)
			{
				return (int)m_value;
			}
			return null;
		}

		public string AsString()
		{
			return ToString();
		}

		public DateTime? AsDateTime()
		{
			if (m_cellType == XCellTypes.Date)
			{
				return (DateTime)m_value;
			}
			return null;
		}

		public override string ToString()
		{
			return m_value?.ToString();
		}
	}
}
