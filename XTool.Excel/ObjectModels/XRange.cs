using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using FantaziaDesign.Core;

namespace XTool.Excel.ObjectModels
{
	public sealed class XRange : ComObjectHost<Range>, IEnumerable<XRange>
	{
		public struct Snapshot
		{
			private readonly XAddress m_first;
			private readonly int m_rowCount;
			private readonly int m_columnCount;
			private readonly string m_source;

			internal Snapshot(XAddress first, int rowCount, int columnCount, string source)
			{
				m_first = first;
				m_rowCount = rowCount;
				m_columnCount = columnCount;
				m_source = source;
			}

			public XAddress First => m_first;
			public int RowCount => m_rowCount;
			public int ColumnCount => m_columnCount;
			public string Source => m_source;
		}

		private XAddress m_first;
		private XAddress m_last;
		private string m_worksheetName = string.Empty;
		private string m_workbookName = string.Empty;
		private bool m_isSingleCell;

		public XRange()
		{
		}

		public XRange(Range parent)
		{
			m_parent = parent ?? throw new ArgumentNullException(nameof(parent));
			UpdateContent();
		}

		public XAddress First => m_first;
		public XAddress Last => m_isSingleCell ? m_first : m_last;

		public int RowCount => Last.Row.Index - First.Row.Index + 1;
		public int ColumnCount => Last.Column.Index - First.Column.Index + 1;

		public string WorksheetName => m_worksheetName;
		public string WorkbookName => m_workbookName;
		public bool IsSingleCell => m_isSingleCell;

		public string FullAddress => GetFullAddress(ref m_isSingleCell, ref m_first, ref m_last, ref m_workbookName, ref m_worksheetName);

		public string Address => m_isSingleCell ? m_first.ToString() : $"{m_first}:{m_last}";

		private static string GetFullAddress(ref bool isSingleCell, ref XAddress first, ref XAddress last, ref string workbookName, ref string worksheetName)
		{
			var sb = new StringBuilder();
			GetRangeSource(workbookName, worksheetName, sb);
			if (isSingleCell)
			{
				sb.Append(first);
			}
			else
			{
				sb.Append($"{first}:{last}");
			}
			return sb.ToString();
		}

		private static StringBuilder GetRangeSource(string workbookName, string worksheetName, StringBuilder stringBuilder)
		{
			var sep = '\'';
			var indexOfWb = workbookName.IndexOf(sep);
			var indexOfWs = worksheetName.IndexOf(sep);
			if (indexOfWb > 0 || indexOfWs > 0)
			{
				stringBuilder.Append($"'[{workbookName.Replace("'", "''")}]{worksheetName.Replace("'", "''")}'!");
			}
			else
			{
				stringBuilder.Append($"[{workbookName}]{worksheetName}!");
			}
			return stringBuilder;
		}

		public XCellValue CellValue => GetCellValue(1, 1);

		public bool IsCellInRange(XAddress cellAddress)
		{
			return TryGetRelativeAddressInRange(cellAddress, out var _, out var _);
		}

		public bool TryGetRelativeAddressInRange(XAddress cellAddress, out int rRowIndex, out int rColumnIndex)
		{
			rRowIndex = cellAddress.Row.GetRelativeIndex(m_first.Row.Index);
			rColumnIndex = cellAddress.Column.GetRelativeIndex(m_first.Column.Index);
			return 0 < rRowIndex && rRowIndex <= RowCount && 0 < rColumnIndex && rColumnIndex <= ColumnCount;
		}

		public bool TryGetRelativeAddressInRange(XAddress cellAddress, out XAddress relativeAddress)
		{
			var rRowIndex = cellAddress.Row.GetRelativeIndex(m_first.Row.Index);
			var rColumnIndex = cellAddress.Column.GetRelativeIndex(m_first.Column.Index);
			relativeAddress = new XAddress(rRowIndex, false, rColumnIndex, false);
			return 0 < rRowIndex && rRowIndex <= RowCount && 0 < rColumnIndex && rColumnIndex <= ColumnCount;
		}

		public XCellValue GetCellValue(XAddress cellAddress)
		{
			int rRowIndex, rColumnIndex;
			if (TryGetRelativeAddressInRange(cellAddress, out rRowIndex, out rColumnIndex))
			{
				if (m_parent != null)
				{
					var value = GetCellRawValue(ref rRowIndex, ref rColumnIndex);
					return new XCellValue(value);
				}
			}
			return null;
		}

		private object GetCellRawValue(ref int rRowIndex, ref int rColumnIndex)
		{
			object value = m_parent.Value[XlRangeValueDataType.xlRangeValueDefault];
			if (value != null)
			{
				Type type = value.GetType();
				if (type.IsArray)
				{
					var rawValues = value as object[,];
					value = rawValues[rRowIndex, rColumnIndex];
				}
			}

			return value;
		}

		public XCellValue GetCellValue(int rRowIndex, int rColumnIndex)
		{
			if (0 < rRowIndex && rRowIndex <= RowCount && 0 < rColumnIndex && rColumnIndex <= ColumnCount)
			{
				if (m_parent != null)
				{
					var value = GetCellRawValue(ref rRowIndex, ref rColumnIndex);
					return new XCellValue(value);
				}
			}
			return null;
		}

		public Snapshot GetSnapshot()
		{
			return new Snapshot(m_first,RowCount,ColumnCount,GetRangeSource(m_workbookName,m_worksheetName, new StringBuilder()).ToString());
		}

		public IEnumerator<XRange> GetEnumerator()
		{
			return EnumerateCells(OrientationFlags.Horizontal).GetEnumerator();
		}

		public IEnumerator<XRange> GetEnumerator(OrientationFlags flags)
		{
			return EnumerateCells(flags).GetEnumerator();
		}

		public IEnumerable<XRange> EnumerateCells(OrientationFlags flags)
		{
			if (m_parent != null)
			{
				var rowCount = RowCount;
				var colCount = ColumnCount;
				if (rowCount == 1 && colCount == 1)
				{
					yield return this;
					yield break;
				}
				else
				{
					if (flags == OrientationFlags.Vertical)
					{
						for (int j = 1; j <= colCount; j++)
						{
							for (int i = 1; i <= rowCount; i++)
							{
								yield return new XRange(m_parent[i, j]);
							}
						}
					}
					else
					{
						for (int i = 1; i <= rowCount; i++)
						{
							for (int j = 1; j <= colCount; j++)
							{
								yield return new XRange(m_parent[i, j]);
							}
						}
					}
				}
			}
			else
			{
				yield break;
			}

		}

		public IEnumerable<XRange> AtRow(XIndex rowIndex)
		{
			if (m_parent != null)
			{
				var rowCount = RowCount;
				var colCount = ColumnCount;
				int rRowIndex = rowIndex.GetRelativeIndex(m_first.Row.Index);
				if (rRowIndex < 1 || rRowIndex > rowCount)
				{
					yield break;
				}
				else if (rowCount == 1 && colCount == 1)
				{
					yield return this;
					yield break;
				}
				else
				{
					for (int j = 1; j <= colCount; j++)
					{
						yield return new XRange(m_parent[rRowIndex, j]);
					}
				}
			}
			else
			{
				yield break;
			}

		}

		public IEnumerable<XRange> AtColumn(XIndex colIndex)
		{
			if (m_parent != null)
			{
				var rowCount = RowCount;
				var colCount = ColumnCount;
				int rColIndex = colIndex.GetRelativeIndex(m_first.Column.Index);
				if (rColIndex < 1 || rColIndex > colCount)
				{
					yield break;
				}
				else if (rowCount == 1 && colCount == 1)
				{
					yield return this;
					yield break;
				}
				else
				{
					for (int i = 1; i <= rowCount; i++)
					{
						yield return new XRange(m_parent[i, rColIndex]);
					}
				}
			}
			else
			{
				yield break;
			}

		}

		public override string ToString()
		{
			return FullAddress;
		}

		public override void UpdateContent()
		{
			if (HasParent)
			{
				var addrStr = m_parent.Address[true, true].Split(':');
				var len = addrStr.Length;
				if (len == 2)
				{
					m_first = new XAddress(addrStr[0]);
					m_last = new XAddress(addrStr[1]);
					m_isSingleCell = false;
				}
				else if (len == 1)
				{
					m_first = new XAddress(addrStr[0]);
					m_isSingleCell = true;
				}
				else
				{
					return;
				}
				UpdateRangeSourceInfo();
			}
		}

		public void UpdateRangeSourceInfo()
		{
			var parentWorksheet = m_parent?.Parent as Worksheet;
			if (parentWorksheet != null)
			{
				m_worksheetName = parentWorksheet.Name;
				var parentWorkbook = parentWorksheet.Parent as Workbook;
				if (parentWorkbook != null)
				{
					m_workbookName = parentWorkbook.Name;
					Marshal.ReleaseComObject(parentWorkbook);
				}
				Marshal.ReleaseComObject(parentWorksheet);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void ClearContent()
		{
			m_first.ZeroAddress();
			m_last.ZeroAddress();
			m_worksheetName = string.Empty;
			m_workbookName = string.Empty;
			m_isSingleCell = false;
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
			ClearContent();
		}

		// // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
		~XRange()
		{
			// 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
			Dispose(disposing: false);
		}

	}
}
