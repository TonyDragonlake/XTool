using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FantaziaDesign.Core;
using FantaziaDesign.Input;
using FantaziaDesign.Model;
using FantaziaDesign.Wpf.Core;

namespace XTool.Excel.WindowModels
{
	public sealed class CriteriaItemsContainer : NotifiableList<CriteriaItem>, IRuntimeUnique, IEquatable<CriteriaItemsContainer>, IContentTemplateHost
	{
		private long m_uId;
		private bool m_isTextContentActived;
		private string m_textContent;
		private bool m_isContentChanged;
		private readonly RelayCommand m_addNewItemCommand;
		private readonly RelayCommand m_insertNewItemBelowCommand;
		private readonly RelayCommand m_removeItemsCommand;
		private readonly RelayCommand m_clearItemsCommand;
		private readonly RelayCommand m_completeChangeCommand;
		private IList m_selection;
		private string m_contentTemplateName;

		public CriteriaItemsContainer() : base()
		{
			m_uId = SnowflakeUId.Next();
			m_textContent = string.Empty;
			m_addNewItemCommand = new RelayCommand(ExecuteAddNewItemCommand);
			m_insertNewItemBelowCommand = new RelayCommand(ExecuteInsertNewItemBelowCommand);
			m_removeItemsCommand = new RelayCommand(ExecuteRemoveItemsCommand);
			m_clearItemsCommand = new RelayCommand(ExecuteClearItemsCommand);
			m_completeChangeCommand = new RelayCommand(ExecuteCompleteChangeCommand);
			m_contentTemplateName = "CriteriaItemsTemplate.Common";
		}

		public CriteriaItemsContainer(IEnumerable<CriteriaItem> collection) : base(collection)
		{
			m_uId = SnowflakeUId.Next();
			m_addNewItemCommand = new RelayCommand(ExecuteAddNewItemCommand);
			m_insertNewItemBelowCommand = new RelayCommand(ExecuteInsertNewItemBelowCommand);
			m_removeItemsCommand = new RelayCommand(ExecuteRemoveItemsCommand);
			m_clearItemsCommand = new RelayCommand(ExecuteClearItemsCommand);
			m_completeChangeCommand = new RelayCommand(ExecuteCompleteChangeCommand);
			m_contentTemplateName = "CriteriaItemsTemplate.Common";
			UpdateTextContentFromCollection();
		}

		public CriteriaItemsContainer(IEnumerable<string> collection) : this(collection?.Where(str => !string.IsNullOrWhiteSpace(str)).Select(str => new CriteriaItem(str)))
		{
		}

		public long UId => m_uId;

		public override int Count => m_items.Count;

		public bool IsTextContentActived { get => m_isTextContentActived; set { if (this.SetPropertyIfChanged(ref m_isTextContentActived, value, nameof(IsTextContentActived))) OnTextContentActivityChanged(); } }

		public string TextContent { get => m_textContent; set => SetTextContent(value); }

		public bool IsContentChanged { get => m_isContentChanged; internal set { SetContentChangedInternal(value); } }

		private void SetContentChangedInternal(bool value)
		{
			m_isContentChanged = value; 
			FilterConfigurationsManager.Current.InvalidateCurrentContentRoot();
		}

		public RelayCommand AddNewItemCommand => m_addNewItemCommand;

		public RelayCommand InsertNewItemBelowCommand => m_insertNewItemBelowCommand;

		public RelayCommand RemoveItemsCommand => m_removeItemsCommand;

		public RelayCommand ClearItemsCommand => m_clearItemsCommand;

		public RelayCommand CompleteChangeCommand => m_completeChangeCommand;

		public IList Selection { get => m_selection; set => m_selection = value; }

		public string ContentTemplateNameKey { get => m_contentTemplateName; set => m_contentTemplateName = value; }

		protected override IList<CriteriaItem> CopyItemsFromEnumerable(IEnumerable<CriteriaItem> collection)
		{
			if (collection is null)
			{
				return new List<CriteriaItem>();
			}

			var ciCol = collection as ICollection<CriteriaItem>;
			List<CriteriaItem> items;
			if (ciCol is null)
			{
				items = new List<CriteriaItem>();
			}
			else
			{
				int count = ciCol.Count;
				if (count == 0)
				{
					return new List<CriteriaItem>();
				}
				items = new List<CriteriaItem>(count);
			}
			foreach (var item in collection)
			{
				items.Add(item);
				item.InternalParent = this;
			}
			return items;
		}

		public bool Equals(CriteriaItemsContainer other)
		{
			return !(other is null) && m_uId == other.m_uId;
		}

		protected override bool InsertItem(int index, CriteriaItem item)
		{
			if (!m_items.Contains(item))
			{
				if (item.InternalParent != null)
				{
					throw new InvalidOperationException("Cannot insert item that has parent");
				}
				item.InternalParent = this;
			}
			else
			{
				item = new CriteriaItem() { Value = item.Value, InternalParent = this };
			}
			m_items.Insert(index, item);
			SetContentChangedInternal(true);
			return true;
		}

		protected override bool RemoveItem(CriteriaItem item)
		{
			var parent = item.InternalParent;
			if (parent is null || parent.m_uId != m_uId)
			{
				throw new InvalidOperationException("Cannot remove item from null or other parent");
			}
			item.InternalParent = null;
			var index = m_items.IndexOf(item);
			if (index >= 0)
			{
				m_items.RemoveAt(index);
				SetContentChangedInternal(true);
				return true;
			}
			return false;
		}

		protected override bool RemoveItemByIndex(int index)
		{
			var item = m_items[index];
			item.InternalParent = null;
			m_items.RemoveAt(index);
			SetContentChangedInternal(true);
			return true;
		}

		protected override bool ReplaceItem(int index, CriteriaItem item)
		{
			var itemIndex = m_items.IndexOf(item);
			if (itemIndex == index)
			{
				return true;
			}
			if (itemIndex < 0)
			{
				if (item.InternalParent != null)
				{
					throw new InvalidOperationException("Cannot insert item that has parent");
				}
				item.InternalParent = this;
			}
			else
			{
				item = new CriteriaItem() { Value = item.Value, InternalParent = this };
			}
			m_items[index] = item;
			SetContentChangedInternal(true);
			return true;
		}

		protected override bool ResetItem()
		{
			foreach (var item in m_items)
			{
				item.InternalParent = null;
			}
			m_items.Clear();
			SetContentChangedInternal(true);
			return true;
		}

		private void ExecuteAddNewItemCommand()
		{
			Add(new CriteriaItem());
		}

		private void ExecuteInsertNewItemBelowCommand(object param)
		{
			if (param is CriteriaItem criteriaItem)
			{
				var index = IndexOf(criteriaItem);
				Insert(index + 1, new CriteriaItem());
			}
			else if (param is SelectionOption option && m_selection != null && m_selection.Count > 0)
			{
				switch (option)
				{
					case SelectionOption.FirstSelection:
						{
							if (m_selection[0] is CriteriaItem selectedItem)
							{
								var index = IndexOf(selectedItem);
								Insert(index + 1, new CriteriaItem());
							}
						}
						break;
					case SelectionOption.LastSelection:
						{
							if (m_selection[m_selection.Count - 1] is CriteriaItem selectedItem)
							{
								var index = IndexOf(selectedItem);
								Insert(index + 1, new CriteriaItem());
							}
						}
						break;
					default:
						break;
				}
			}
		}

		private void ExecuteRemoveItemsCommand(object param)
		{
			if (param is CriteriaItem criteriaItem)
			{
				Remove(criteriaItem);
			}
			else if (param is SelectionOption option && m_selection != null && m_selection.Count > 0)
			{
				switch (option)
				{
					case SelectionOption.FirstSelection:
						{
							if (m_selection[0] is CriteriaItem selectedItem)
							{
								Remove(selectedItem);
							}
						}
						break;
					case SelectionOption.LastSelection:
						{
							if (m_selection[m_selection.Count - 1] is CriteriaItem selectedItem)
							{
								Remove(selectedItem);
							}
						}
						break;
					case SelectionOption.AllSelection:
						{
							var indexes = new List<int>(m_selection.Count);
							foreach (var item in m_selection)
							{
								if (item is CriteriaItem ci)
								{
									var index = IndexOf(ci);
									if (index >= 0)
									{
										indexes.Add(index);
									}
								}
							}
							var length = indexes.Count;
							if (length > 0)
							{
								indexes.Sort();
								for (var i = length - 1; i >= 0; i--)
								{
									RemoveAt(indexes[i]);
								}
							}
						}
						break;
					case SelectionOption.All:
						{
							Clear();
						}
						break;
					default:
						break;
				}
			}
		}

		private void ExecuteClearItemsCommand()
		{
			Clear();
		}

		private void ExecuteCompleteChangeCommand()
		{
			if (m_isContentChanged)
			{
				m_isContentChanged = false;
				if (m_isTextContentActived)
				{
					UpdateCollectionFromTextContent();
				}
				else
				{
					UpdateTextContentFromCollection();
					RaisePropertyChangedEvent(nameof(TextContent));
				}
				RaisePropertyChangedEvent(nameof(IsContentChanged));
			}
		}

		private void UpdateCollectionFromTextContent()
		{
			Clear();
			if (!string.IsNullOrWhiteSpace(m_textContent))
			{
				var hashSet = new HashSet<string>(
					Regex.Replace(m_textContent, @"[\r\n]", "\n")
					.Split('\n')
					.Where(item => !string.IsNullOrWhiteSpace(item)));
				foreach (var item in hashSet)
				{
					Add(new CriteriaItem() { Value = item });
				}
			}
		}

		private void OnTextContentActivityChanged()
		{
			if (m_isContentChanged)
			{
				m_isContentChanged = false;
				if (m_isTextContentActived)
				{
					UpdateTextContentFromCollection();
					RaisePropertyChangedEvent(nameof(TextContent));
				}
				else
				{
					UpdateCollectionFromTextContent();
				}
				RaisePropertyChangedEvent(nameof(IsContentChanged));
			}
		}

		private void UpdateTextContentFromCollection()
		{
			if (m_items.Count == 0)
			{
				m_textContent = string.Empty;
			}
			else
			{
				var hashSet = new HashSet<string>(m_items.Select(item => item.Value).Where(item => !string.IsNullOrWhiteSpace(item)));
				m_textContent = string.Join("\r\n", hashSet);
			}
		}

		private void SetTextContent(string value)
		{
			if (!m_isTextContentActived)
			{
				throw new InvalidOperationException("TextContent should be actived before assigning value to TextContent");
			}
			m_textContent = value;
			SetContentChangedInternal(true);
			RaisePropertyChangedEvent(nameof(TextContent));
		}

		public string[] AsStringArray()
		{
			ExecuteCompleteChangeCommand();
			var strCol = m_items.Select(item => item.Value).Where(item => !string.IsNullOrWhiteSpace(item));
			var hashSet = new HashSet<string>(strCol);
			if (hashSet.Count > 0)
			{
				return hashSet.ToArray();
			}
			return Array.Empty<string>();
		}

	}
}
