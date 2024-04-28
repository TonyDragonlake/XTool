using System;
using FantaziaDesign.Core;
using FantaziaDesign.Model;
using XTool.Excel.ObjectModels;

namespace XTool.Excel.WindowModels
{
	public sealed class FieldItem : PropertyNotifier, IRuntimeUnique, IEquatable<FieldItem>
	{
		private long m_uId;
		private XAddress m_address;
		private string m_text;
		private bool m_isEnabled;
		private bool m_isActived;

		public FieldItem()
		{
			m_uId = SnowflakeUId.Next();
		}

		public FieldItem(XAddress address, string text) : this()
		{
			m_address = address;
			m_text = text;
		}

		public FieldItem(XRange range, XRange cell) : this()
		{
			SetFieldItemInternal(range, cell);
		}

		private void SetFieldItemInternal(XRange range, XRange cell)
		{
			if (range is null)
			{
				throw new ArgumentNullException(nameof(range));
			}

			if (cell is null)
			{
				throw new ArgumentNullException(nameof(cell));
			}
			if (range.TryGetRelativeAddressInRange(cell.First, out m_address))
			{
				m_text = cell.CellValue.AsString();
				m_isEnabled = true;
			}
			else
			{
				m_text = null;
				m_isEnabled = false;
			}
		}

		public void SetFieldItem(XRange range, XRange cell)
		{
			SetFieldItemInternal(range, cell);
			RaisePropertyChangedEvent(nameof(PresentingText));
			RaisePropertyChangedEvent(nameof(IsEnabled));
		}

		public bool Equals(FieldItem other)
		{
			return !(other is null) && m_uId == other.m_uId;
		}

		public long UId => m_uId;
		public XAddress RelativeAddress => m_address;
		public string PresentingText => m_text;
		public bool IsEnabled => m_isEnabled;
		public bool IsActived { get => m_isActived; set => this.SetPropertyIfChanged(ref m_isActived, value, nameof(IsActived)); }

	}
}
