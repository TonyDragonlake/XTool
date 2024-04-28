using System;
using FantaziaDesign.Core;
using FantaziaDesign.Model;

namespace XTool.Excel.WindowModels
{
	public sealed class CriteriaItem : PropertyNotifier, IRuntimeUnique, IEquatable<CriteriaItem>
	{
		private long m_uId;
		private CriteriaItemsContainer m_parent;
		private string m_value;

		public CriteriaItem() : base()
		{
			m_uId = SnowflakeUId.Next();
		}

		public CriteriaItem(string value) : this()
		{
			m_value = value;
		}

		public long UId => m_uId;

		public string Value
		{
			get => m_value;
			set
			{
				if (this.SetPropertyIfChanged(ref m_value, value, nameof(Value)))
				{
					OnCriteriaValueChanged();
				}
			}
		}

		internal CriteriaItemsContainer InternalParent { get => m_parent; set => m_parent = value; }

		public bool Equals(CriteriaItem other)
		{
			return !(other is null) && m_uId == other.m_uId;
		}

		private void OnCriteriaValueChanged()
		{
			if (m_parent != null)
			{
				m_parent.IsContentChanged = true;
			}
		}
	}
}
