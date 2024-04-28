using System;
using System.Collections.Generic;
using FantaziaDesign.Core;
using FantaziaDesign.Model;

namespace XTool.Excel.WindowModels
{
	public sealed class FieldItemsContainer : NotifiableList<FieldItem>, IRuntimeUnique, IEquatable<FieldItemsContainer>
	{
		private long m_uId;
		private int m_selectedIndex;

		public FieldItemsContainer() : base()
		{
			m_uId = SnowflakeUId.Next();
		}

		public FieldItemsContainer(IEnumerable<FieldItem> collection) : base(collection)
		{
			m_uId = SnowflakeUId.Next();
		}

		public long UId => m_uId;

		public int SelectedIndex { get => m_selectedIndex; set { if (this.SetPropertyIfChanged(ref m_selectedIndex, value, nameof(SelectedIndex))) RaisePropertyChangedEvent(nameof(SelectedItem)); } }

		public FieldItem SelectedItem { get { if (m_selectedIndex >= 0 && m_selectedIndex < Count) return m_items[m_selectedIndex]; return null; } }

		public bool Equals(FieldItemsContainer other)
		{
			return !(other is null) && m_uId == other.m_uId;
		}
	}
}
