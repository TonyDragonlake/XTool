using System;
using System.Collections;
using System.Collections.Generic;
using FantaziaDesign.Core;
using FantaziaDesign.Input;
using FantaziaDesign.Model;
using FantaziaDesign.Wpf.Core;
using System.Collections.Specialized;

namespace XTool.Excel.WindowModels
{
	public sealed class FieldFiltersManager : NotifiableCollectionBase<FieldItem>, IRuntimeUnique, IEquatable<FieldFiltersManager>
	{
		private static readonly CriteriaItemsContainer s_noSelectCriterias 
			= new CriteriaItemsContainer() 
			{ 
				IsTextContentActived = true, 
				ContentTemplateNameKey = "CriteriaItemsTemplate.NoSelect" 
			};

		private static readonly CriteriaItemsContainer s_multiSelectCriterias 
			= new CriteriaItemsContainer() 
			{ 
				IsTextContentActived = true, 
				ContentTemplateNameKey = "CriteriaItemsTemplate.MultiSelect" 
			};

		private static readonly CriteriaItemsContainer s_emptySingleSelectCriterias
			= new CriteriaItemsContainer()
			{
				IsTextContentActived = true,
				ContentTemplateNameKey = "CriteriaItemsTemplate.EmptySingleSelect"
			};

		private long m_uId;
		private FieldItemsContainer m_fields;
		private List<int> m_orderedFieldIndex;
		private Dictionary<int, CriteriaItemsContainer> m_filters;
		
		private RelayCommand m_addFieldForFilterCommand;
		private RelayCommand m_removeFieldForFilterCommand;
		private RelayCommand m_clearFilterFieldsCommand;

		private RelayCommand m_moveUpFilterFieldCommand;
		private RelayCommand m_moveDownFilterFieldCommand;

		private RelayCommand m_addCriteriasForFieldsCommand;
		private RelayCommand m_removeCriteriasForFieldsCommand;

		private IList m_fieldSelection;
		private IList m_filterSelection;

		private CriteriaItemsContainer m_currentCriterias;

		private SelectionResultType m_fieldSelectionResult;
		private SelectionResultType m_filterSelectionResult;
		private bool m_canAddCriteriasForFields;

		private bool m_shouldNotifyContentRootChanged = true;

		public FieldFiltersManager() : base()
		{
			m_uId = SnowflakeUId.Next();
			m_fields = new FieldItemsContainer();
			m_orderedFieldIndex = new List<int>();
			m_filters = new Dictionary<int, CriteriaItemsContainer>();
			m_addFieldForFilterCommand = new RelayCommand(ExecuteAddFieldForFilterCommand);
			m_removeFieldForFilterCommand = new RelayCommand(ExecuteRemoveFieldForFilterCommand);
			m_clearFilterFieldsCommand = new RelayCommand(ExecuteClearFilterFieldsCommand);
			m_addCriteriasForFieldsCommand = new RelayCommand(ExecuteAddCriteriasForSelectedFieldsCommand);
			m_removeCriteriasForFieldsCommand = new RelayCommand(ExecuteRemoveCriteriasForSelectedFieldsCommand);
			m_moveUpFilterFieldCommand = new RelayCommand(ExecuteMoveUpFilterFieldCommand);
			m_moveDownFilterFieldCommand = new RelayCommand(ExecuteMoveDownFilterFieldCommand);
		}

		public override int Count => m_orderedFieldIndex.Count;

		public FieldItemsContainer Fields => m_fields;

		public bool HasFields => m_fields.Count > 0;

		public bool HasFilters => m_orderedFieldIndex.Count>0 && m_filters.Count > 0;

		public CriteriaItemsContainer CurrentCriterias => GetCurrentCriterias();

		public bool CanAddCriteriasForFields => m_canAddCriteriasForFields;

		public bool HasCurrentCriterias => m_currentCriterias != null;

		public long UId => m_uId;

		public RelayCommand AddFieldForFilterCommand { get => m_addFieldForFilterCommand; }
		public RelayCommand RemoveFieldForFilterCommand { get => m_removeFieldForFilterCommand; }
		public RelayCommand ClearFilterFieldsCommand { get => m_clearFilterFieldsCommand; }

		public RelayCommand AddCriteriasForFieldsCommand { get => m_addCriteriasForFieldsCommand; }
		public RelayCommand RemoveCriteriasForFieldsCommand { get => m_removeCriteriasForFieldsCommand; }

		public RelayCommand MoveUpFilterFieldCommand => m_moveUpFilterFieldCommand;
		public RelayCommand MoveDownFilterFieldCommand => m_moveDownFilterFieldCommand;

		public IList FieldSelection { get => m_fieldSelection; set { m_fieldSelection = value; UpdateFieldSelection(); } }

		public IList FilterSelection { get => m_filterSelection; set { m_filterSelection = value; UpdateFilterSelection(); } }

		public SelectionResultType FieldSelectionResult => m_fieldSelectionResult;

		public SelectionResultType FilterSelectionResult => m_filterSelectionResult;

		private void NotifyContentRootChanged()
		{
			if (m_shouldNotifyContentRootChanged)
			{
				FilterConfigurationsManager.Current.InvalidateCurrentContentRoot();
			}
		}

		private CriteriaItemsContainer GetCurrentCriterias()
		{
			switch (m_filterSelectionResult)
			{
				case SelectionResultType.SingleSelection:
					return HasCurrentCriterias? m_currentCriterias : s_emptySingleSelectCriterias;
				case SelectionResultType.MultipleSelection:
				case SelectionResultType.AllSelection:
					return s_multiSelectCriterias;
				default:
					return s_noSelectCriterias;
			}
		}

		private void UpdateFieldSelection()
		{
			if (m_fieldSelection != null && m_fieldSelection.Count > 0)
			{
				if (m_fieldSelection.Count > 1)
				{
					this.SetPropertyIfChanged(ref m_fieldSelectionResult, SelectionResultType.MultipleSelection, nameof(FieldSelectionResult));
				}
				else
				{
					this.SetPropertyIfChanged(ref m_fieldSelectionResult, SelectionResultType.SingleSelection, nameof(FieldSelectionResult));
				}
			}
			else
			{
				this.SetPropertyIfChanged(ref m_fieldSelectionResult, SelectionResultType.NoSelection, nameof(FieldSelectionResult));
			}
		}

		private void UpdateFilterSelection()
		{
			if (HasFields && m_filterSelection != null && m_filterSelection.Count > 0)
			{
				if (m_filterSelection.Count > 1)
				{
					var canAddCriterias = CheckCanAddCriteriasForFields();
					SetFilterProperties(SelectionResultType.MultipleSelection, canAddCriterias, null);
					return;
				}
				var fieldItem = m_filterSelection[0] as FieldItem;
				if (fieldItem != null)
				{
					var indexInField = GetItemIndexInFields(fieldItem);
					if (indexInField >= 0)
					{
						var hasCriterias = m_filters.TryGetValue(indexInField, out var criterias);
						SetFilterProperties(SelectionResultType.SingleSelection, !hasCriterias, hasCriterias ? criterias : null);
						return;
					}
				}
			}
			SetFilterProperties(SelectionResultType.NoSelection, false, null);
		}

		private void SetFilterProperties(SelectionResultType selectionResult, bool canAddCriterias, CriteriaItemsContainer criteriaItems)
		{
			bool propChanged = false;
			if (m_filterSelectionResult != selectionResult)
			{
				m_filterSelectionResult = selectionResult;
				propChanged = true;
				RaisePropertyChangedEvent(nameof(FilterSelectionResult));
			}
			if (m_canAddCriteriasForFields != canAddCriterias)
			{
				m_canAddCriteriasForFields = canAddCriterias;
				propChanged = true;
				RaisePropertyChangedEvent(nameof(CanAddCriteriasForFields));
			}
			if (m_currentCriterias != criteriaItems)
			{
				m_currentCriterias = criteriaItems;
				propChanged = true;
				RaisePropertyChangedEvent(nameof(HasCurrentCriterias));
			}
			if (propChanged)
			{
				RaisePropertyChangedEvent(nameof(CurrentCriterias));
				NotifyContentRootChanged();
			}
		}

		private bool CheckCanAddCriteriasForFields()
		{
			if (m_filters.Count != m_orderedFieldIndex.Count)
			{
				foreach (var item in m_filterSelection)
				{
					if (item is FieldItem fi)
					{
						var indexInField = GetItemIndexInFields(fi);
						if (indexInField >= 0 && !m_filters.ContainsKey(indexInField))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public override bool Contains(FieldItem item)
		{
			var indexInFields = GetItemIndexInFields(item);
			if (indexInFields < 0)
			{
				return false;
			}
			return m_orderedFieldIndex.Contains(indexInFields);
		}

		public override void CopyTo(FieldItem[] array, int arrayIndex)
		{
			if (m_fields is null) return;
			var count = m_orderedFieldIndex.Count;
			if (arrayIndex + count <= array.Length)
			{
				var current = 0;
				foreach (var indexInFields in m_orderedFieldIndex)
				{
					array[arrayIndex + current] = m_fields[indexInFields];
					current++;
				}
			}
		}

		private IEnumerable<FieldItem> Enumeration()
		{
			if (m_fields is null)
			{
				yield break;
			}
			foreach (var indexInFields in m_orderedFieldIndex)
			{
				yield return m_fields[indexInFields];
			}
		}

		public override IEnumerator<FieldItem> GetEnumerator()
		{
			return Enumeration().GetEnumerator();
		}

		public override FieldItem GetItem(int index)
		{
			if (m_fields is null || !ContainsIndex(index)) return null;
			var indexInFields = m_orderedFieldIndex[index];
			return m_fields.GetItem(indexInFields);
		}

		protected override int GetItemIndex(FieldItem item)
		{
			var indexInFields = GetItemIndexInFields(item);
			if (indexInFields < 0)
			{
				return -1;
			}
			return m_orderedFieldIndex.IndexOf(indexInFields);
		}

		protected override bool InsertItem(int index, FieldItem item)
		{
			var indexInFields = GetItemIndexInFields(item);
			if (indexInFields < 0)
			{
				return false;
			}
			m_orderedFieldIndex.Insert(index, indexInFields);
			return true;
		}

		protected override bool RemoveItem(FieldItem item)
		{
			var indexInFields = GetItemIndexInFields(item);
			if (indexInFields < 0)
			{
				return false;
			}
			var index = m_orderedFieldIndex.IndexOf(indexInFields);
			if (index < 0)
			{
				return false;
			}
			return RemoveItemByIndex(index);
		}

		protected override bool RemoveItemByIndex(int index)
		{
			var indexInFields = m_orderedFieldIndex[index];
			var fieldItem = m_fields.GetItem(indexInFields);
			if (fieldItem != null)
			{
				fieldItem.IsActived = false;
			}
			m_orderedFieldIndex.RemoveAt(index);
			m_filters.Remove(indexInFields);
			return true;
		}

		protected override bool ReplaceItem(int index, FieldItem item)
		{
			var indexInFields = GetItemIndexInFields(item);
			if (indexInFields < 0)
			{
				return false;
			}
			m_orderedFieldIndex[index] = indexInFields;
			return true;
		}

		protected override bool ResetItem()
		{
			m_orderedFieldIndex.Clear();
			return true;
		}

		protected override bool MoveItem(int oldIndex, int newIndex, out FieldItem oldItem)
		{
			if (m_fields is null) { oldItem = null; return false; }
			var indexInFields = m_orderedFieldIndex[oldIndex];
			oldItem = m_fields.GetItem(indexInFields);
			m_orderedFieldIndex.RemoveAt(oldIndex);
			m_orderedFieldIndex.Insert(newIndex, indexInFields);
			return true;
		}

		private int GetItemIndexInFields(FieldItem item)
		{
			if (m_fields is null) { return -1; }
			return m_fields.IndexOf(item);
		}

		public bool Equals(FieldFiltersManager other)
		{
			return !(other is null) && m_uId == other.m_uId;
		}

		public bool SetCriterias(int index, CriteriaItemsContainer criteriaItems)
		{
			if (ContainsIndex(index))
			{
				var indexInFields = m_orderedFieldIndex[index];
				if (criteriaItems is null)
					m_filters.Remove(indexInFields);
				else if (m_filters.ContainsKey(indexInFields))
					m_filters[indexInFields] = criteriaItems;
				else
					m_filters.Add(indexInFields, criteriaItems);
				NotifyContentRootChanged();
				return true;
			}
			return false;
		}

		public bool AddCriteriasForSelectedFields()
		{
			if (HasFields && m_filterSelection != null && m_filterSelection.Count > 0)
			{
				bool actionDone = false;
				foreach (var item in m_filterSelection)
				{
					if (item is FieldItem fieldItem)
					{
						var indexInField = GetItemIndexInFields(fieldItem);
						if (!m_filters.ContainsKey(indexInField))
						{
							m_filters.Add(indexInField, new CriteriaItemsContainer());
							actionDone = true;
						}
					}
				}
				if (actionDone)
				{
					UpdateFilterSelection();
					return true;
				}
			}
			return false;
		}

		public bool RemoveCriteriasForSelectedFields()
		{
			if (HasFields && m_filterSelection != null && m_filterSelection.Count > 0)
			{
				bool actionDone = false;
				foreach (var item in m_filterSelection)
				{
					if (item is FieldItem fieldItem)
					{
						var indexInField = GetItemIndexInFields(fieldItem);
						if (m_filters.Remove(indexInField))
						{
							actionDone = true;
						}
					}
				}
				if (actionDone)
				{
					UpdateFilterSelection();
					return true;
				}
			}
			return false;
		}

		public bool AddFilterField(int indexInFields)
		{
			if (m_fields.ContainsIndex(indexInFields))
			{
				var index = m_orderedFieldIndex.IndexOf(indexInFields);
				if (index < 0)
				{
					index = m_orderedFieldIndex.Count;
					ThrowIfDuringChanging();
					m_orderedFieldIndex.Insert(index, indexInFields);
					var field = m_fields[indexInFields];
					field.IsActived = true;
					var eventArgs = CollectionNotifiers.CreateItemInsertedEventArgs(field, index);
					RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
					RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
					NotifyCollectionChangedEvent(eventArgs);
					return true;
				}
			}
			return false;
		}

		private void AddFilterFieldFromSelection(SelectionOption option, IList selection)
		{
			if (selection != null && selection.Count > 0)
			{
				switch (option)
				{
					case SelectionOption.FirstSelection:
						{
							var fieldItem = selection[0] as FieldItem;
							if (fieldItem != null)
							{
								var indexInFields = m_fields.IndexOf(fieldItem);
								AddFilterField(indexInFields);
							}
						}
						break;
					case SelectionOption.LastSelection:
						{
							var lastIndex = selection.Count - 1;
							var fieldItem = selection[lastIndex] as FieldItem;
							if (fieldItem != null)
							{
								var indexInFields = m_fields.IndexOf(fieldItem);
								AddFilterField(indexInFields);
							}
						}
						break;
					case SelectionOption.AllSelection:
						{
							foreach (var item in selection)
							{
								if (item is FieldItem fieldItem)
								{
									var indexInFields = m_fields.IndexOf(fieldItem);
									AddFilterField(indexInFields);
								}
							}
						}
						break;
					case SelectionOption.All:
						{
							for (int i = 0; i < m_fields.Count; i++)
							{
								AddFilterField(i);
							}
						}
						break;
					default:
						break;
				}
			}

		}

		public bool AddFilterField(int indexInFields, CriteriaItemsContainer criteriaItems)
		{
			if (m_fields.ContainsIndex(indexInFields))
			{
				m_shouldNotifyContentRootChanged = false;
				var index = m_orderedFieldIndex.IndexOf(indexInFields);
				if (index < 0)
				{
					index = m_orderedFieldIndex.Count;
					ThrowIfDuringChanging();
					m_orderedFieldIndex.Insert(index, indexInFields);
					var field = m_fields[indexInFields];
					field.IsActived = true;
					var eventArgs = CollectionNotifiers.CreateItemInsertedEventArgs(field, index);
					RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
					RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
					NotifyCollectionChangedEvent(eventArgs);
				}
				if (criteriaItems is null)
					m_filters.Remove(indexInFields);
				else if (m_filters.ContainsKey(indexInFields))
					m_filters[indexInFields] = criteriaItems;
				else
					m_filters.Add(indexInFields, criteriaItems);
				m_shouldNotifyContentRootChanged = true;
				NotifyContentRootChanged();
				return true;
			}
			return false;
		}

		public bool RemoveFilterField(int indexInFields)
		{
			if (m_fields.ContainsIndex(indexInFields))
			{
				var index = m_orderedFieldIndex.IndexOf(indexInFields);
				if (index >= 0)
				{
					ThrowIfDuringChanging();
					m_orderedFieldIndex.RemoveAt(index);
					m_filters.Remove(indexInFields);
					var field = m_fields[indexInFields];
					field.IsActived = false;
					var eventArgs = CollectionNotifiers.CreateItemRemovedEventArgs(field, index);
					RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
					RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
					NotifyCollectionChangedEvent(eventArgs);
					return true;
				}
			}
			return false;
		}

		private void RemoveFilterFieldFromSelection(SelectionOption option, IList selection)
		{
			if (selection != null && selection.Count > 0)
			{
				switch (option)
				{
					case SelectionOption.FirstSelection:
						{
							var fieldItem = selection[0] as FieldItem;
							if (fieldItem != null)
							{
								var indexInFields = m_fields.IndexOf(fieldItem);
								RemoveFilterField(indexInFields);
							}
						}
						break;
					case SelectionOption.LastSelection:
						{
							var lastIndex = selection.Count - 1;
							var fieldItem = selection[lastIndex] as FieldItem;
							if (fieldItem != null)
							{
								var indexInFields = m_fields.IndexOf(fieldItem);
								RemoveFilterField(indexInFields);
							}
						}
						break;
					case SelectionOption.AllSelection:
						{
							var indexes = new List<int>(selection.Count);
							foreach (var item in selection)
							{
								if (item is FieldItem fieldItem)
								{
									var index = IndexOf(fieldItem);
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
								m_shouldNotifyContentRootChanged = false;
								for (var i = length - 1; i >= 0; i--)
								{
									RemoveAt(indexes[i]);
								}
								m_shouldNotifyContentRootChanged = false;
								NotifyContentRootChanged();
							}
						}
						break;
					case SelectionOption.All:
						{
							ClearFilterFields();
						}
						break;
					default:
						break;
				}
			}
		}

		public bool ClearFilterFields()
		{
			if (m_orderedFieldIndex.Count > 0 || m_filters.Count > 0)
			{
				ThrowIfDuringChanging();
				m_filters.Clear();
				m_orderedFieldIndex.Clear();
				RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
				RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
				NotifyCollectionChangedEvent(CollectionNotifiers.ResetCollectionChanged);
				foreach (var item in m_fields)
				{
					item.IsActived = false;
				}
				return true;
			}
			return false;
		}

		private void ExecuteAddFieldForFilterCommand(object param)
		{
			if (param is FieldItem fieldItem)
			{
				var indexInFields = m_fields.IndexOf(fieldItem);
				AddFilterField(indexInFields);
			}
			else if (param is SelectionOption option)
			{
				AddFilterFieldFromSelection(option, m_fieldSelection);
			}
		}

		private void ExecuteRemoveFieldForFilterCommand(object param)
		{
			if (param is FieldItem fieldItem)
			{
				var indexInFields = m_fields.IndexOf(fieldItem);
				RemoveFilterField(indexInFields);
			}
			else if (param is SelectionOption option)
			{
				RemoveFilterFieldFromSelection(option, m_filterSelection);
			}
		}

		private void ExecuteAddCriteriasForSelectedFieldsCommand()
		{
			AddCriteriasForSelectedFields();
		}

		private void ExecuteRemoveCriteriasForSelectedFieldsCommand()
		{
			RemoveCriteriasForSelectedFields();
		}

		private void ExecuteClearFilterFieldsCommand()
		{
			ClearFilterFields();
		}

		private void ExecuteMoveUpFilterFieldCommand(object param)
		{
			if (param is FieldItem fieldItem)
			{
				var index = IndexOf(fieldItem);
				var newIndex = index - 1;
				Move(index, newIndex);
			}
		}

		private void ExecuteMoveDownFilterFieldCommand(object param)
		{
			if (param is FieldItem fieldItem)
			{
				var index = IndexOf(fieldItem);
				var newIndex = index + 1;
				Move(index, newIndex);
			}
		}

		public void SetupFromRange(IEnumerable<FieldItem> fields = null)
		{
			Cleanup();
			if (fields != null)
			{
				foreach (var field in fields)
				{
					m_fields.Add(field);
				}
			}
		}

		public void Cleanup()
		{
			if (m_fields.Count > 0)
			{
				m_fieldSelection = null;
				m_filterSelection = null;
				m_currentCriterias = null;
				m_fieldSelectionResult = SelectionResultType.NoSelection;
				m_filterSelectionResult = SelectionResultType.NoSelection;
				m_canAddCriteriasForFields = false;
				m_fields.Clear();
				ClearFilterFields();
			}
		}

		public List<FilterValuesSetting> AsFilterValuesSettings()
		{
			var settings = new List<FilterValuesSetting>();
			if (HasFilters)
			{
				foreach (var indexInFields in m_orderedFieldIndex)
				{
					if (m_filters.TryGetValue(indexInFields, out var criteriaItems))
					{
						var criterias = criteriaItems.AsStringArray();
						if (criterias.Length > 0)
						{
							var setting = new FilterValuesSetting(indexInFields + 1, criterias);
							settings.Add(setting);
						}
					}
				}
			}
			return settings;
		}

		public void SetFromFilterValuesSettings(IReadOnlyList<FilterValuesSetting> settings)
		{
			if (settings is null)
			{
				return;
			}

			ClearFilterFields();
			foreach (var item in settings)
			{
				AddFilterField(item.FieldIndex - 1, new CriteriaItemsContainer(item.Criteria));
			}
		}

		protected override void NotifyCollectionChangedEvent(NotifyCollectionChangedEventArgs eventArgs)
		{
			base.NotifyCollectionChangedEvent(eventArgs);
			NotifyContentRootChanged();
		}
	}
}
