using FantaziaDesign.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace FantaziaDesign.Model
{
	public static class CollectionNotifiers
	{
		public static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");

		public static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");

		public static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

		public static CollectionChangedInfo<T> CreateItemInsertedInfo<T>(Action<CollectionChange<T>> action)
		{
			return new CollectionChangedInfo<T>(0, action);
		}

		public static CollectionChangedInfo<T> CreateItemRemovedInfo<T>(Action<CollectionChange<T>> action)
		{
			return new CollectionChangedInfo<T>(1, action);
		}

		public static CollectionChangedInfo<T> CreateItemReplacedInfo<T>(Action<CollectionChange<T>> action)
		{
			return new CollectionChangedInfo<T>(2, action);
		}

		public static CollectionChangedInfo<T> CreateItemMovedInfo<T>(Action<CollectionChange<T>> action)
		{
			return new CollectionChangedInfo<T>(3, action);
		}

		public static CollectionChangedInfo<T> CreateItemResetInfo<T>(Action<CollectionChange<T>> action)
		{
			return new CollectionChangedInfo<T>(4, action);
		}

		public static NotifyCollectionChangedEventArgs CreateItemInsertedEventArgs(object item, int index)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
		}

		public static bool TryGetItemInsertedEventArgsParameters<T>(NotifyCollectionChangedEventArgs args, out T item, out int index)
		{
			if (!(args is null) && args.Action == NotifyCollectionChangedAction.Add)
			{
				var newItems = args.NewItems;
				if (newItems != null && newItems.Count > 0 && newItems[0] is T tItem)
				{
					item = tItem;
					index = args.NewStartingIndex;
					return true;
				}
			}
			item = default(T);
			index = -1;
			return false;
		}

		public static NotifyCollectionChangedEventArgs CreateItemsInsertedEventArgs(IList item, int index)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
		}

		public static NotifyCollectionChangedEventArgs CreateItemRemovedEventArgs(object item, int index)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
		}

		public static NotifyCollectionChangedEventArgs CreateItemRemovedEventArgs(object item)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item);
		}

		public static bool TryGetItemRemovedEventArgsParameters<T>(NotifyCollectionChangedEventArgs args, out T item, out int index)
		{
			if (!(args is null) && args.Action == NotifyCollectionChangedAction.Remove)
			{
				var oldItems = args.OldItems;
				if (oldItems != null && oldItems.Count > 0 && oldItems[0] is T tItem)
				{
					item = tItem;
					index = args.OldStartingIndex;
					return true;
				}
			}
			item = default(T);
			index = -1;
			return false;
		}

		public static NotifyCollectionChangedEventArgs CreateItemsRemovedEventArgs(IList item, int index)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index);
		}

		public static NotifyCollectionChangedEventArgs CreateItemReplacedEventArgs(object oldItem, object newItem, int index)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
		}

		public static bool TryGetItemReplacedEventArgsParameters<T>(NotifyCollectionChangedEventArgs args, out T oldItem, out T newItem, out int index)
		{
			if (!(args is null) && args.Action == NotifyCollectionChangedAction.Replace)
			{
				var oldItems = args.OldItems;
				var newItems = args.NewItems;
				if (oldItems != null && oldItems.Count > 0 && oldItems[0] is T oldtItem && newItems != null && newItems.Count > 0 && newItems[0] is T newtItem)
				{
					oldItem = oldtItem;
					newItem = newtItem;
					index = args.OldStartingIndex;
					return true;
				}
			}
			oldItem = default(T);
			newItem = default(T);
			index = -1;
			return false;
		}

		public static NotifyCollectionChangedEventArgs CreateItemsReplacedEventArgs(IList oldItem, IList newItem, int startIndex)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, startIndex);
		}

		public static NotifyCollectionChangedEventArgs CreateItemMovedEventArgs(object item, int newIndex, int oldIndex)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
		}

		public static bool TryGetItemMovedEventArgsParameters<T>(NotifyCollectionChangedEventArgs args, out T item, out int oldIndex, out int newIndex)
		{
			if (!(args is null) && args.Action == NotifyCollectionChangedAction.Move)
			{
				var oldItems = args.OldItems;
				if (oldItems != null && oldItems.Count > 0 && oldItems[0] is T oldtItem)
				{
					item = oldtItem;
					oldIndex = args.OldStartingIndex;
					newIndex = args.NewStartingIndex;
					return true;
				}
			}
			item = default(T);
			oldIndex = -1;
			newIndex = -1;
			return false;
		}

		public static NotifyCollectionChangedEventArgs CreateItemsMovedEventArgs(IList item, int newIndex, int oldIndex)
		{
			return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
		}
	}

	public class CollectionChangedInfo<T>
	{
		private int m_actionCode;
		private Action<CollectionChange<T>> m_action;

		internal int? m_oldIndex;
		internal int? m_newIndex;
		internal T[] m_oldItems;
		internal T[] m_newItems;
		internal bool m_cancelChange;
		internal CollectionChangedInfo(int actionCode, Action<CollectionChange<T>> action)
		{
			if (actionCode < 0 || actionCode > 4)
			{
				throw new ArgumentOutOfRangeException("actionCode should be in range of [0,4]");
			}
			m_actionCode = actionCode;
			m_action = action;
		}

		public int ActionCode => m_actionCode;

		public int OldIndex { get => m_oldIndex.GetValueOrDefault(-1); }

		public int NewIndex { get => m_newIndex.GetValueOrDefault(-1); }

		public IReadOnlyList<T> OldItems => m_oldItems;

		public IReadOnlyList<T> NewItems => m_newItems;

		public T FirstOldItem => m_oldItems is null || m_oldItems.Length <= 0 ? default(T) : m_oldItems[0];
		public T FirstNewItem => m_newItems is null || m_newItems.Length <= 0 ? default(T) : m_newItems[0];

		public bool ExecuteAction()
		{
			if (m_action != null)
			{
				using (var change = new CollectionChange<T>(this))
				{
					m_action.Invoke(change);
				}
				return !m_cancelChange;
			}
			return false;
		}
	}

	public class CollectionChange<T> : IDisposable
	{
		private CollectionChangedInfo<T> m_changeInfo;

		internal CollectionChange(CollectionChangedInfo<T> parent)
		{
			m_changeInfo = parent ?? throw new ArgumentNullException(nameof(parent));
		}

		private void ThrowIfActionCodeNotMatched(int code)
		{
			if (m_changeInfo.ActionCode != code)
			{
				throw new InvalidOperationException("Cannot change CollectionChange.ActionCode");
			}
		}

		public void InsertItem(int index, T item)
		{
			ThrowIfActionCodeNotMatched(0);
			m_changeInfo.m_newIndex = index;
			m_changeInfo.m_newItems = new T[] { item };
		}

		public void InsertItems(int startIndex, IList<T> items)
		{
			ThrowIfActionCodeNotMatched(0);
			m_changeInfo.m_newIndex = startIndex;
			m_changeInfo.m_newItems = items.CopyAsArray();
		}

		public void InsertItems(int startIndex, params T[] items)
		{
			ThrowIfActionCodeNotMatched(0);
			m_changeInfo.m_newIndex = startIndex;
			m_changeInfo.m_newItems = items;
		}

		public void RemoveItem(int index, T item)
		{
			ThrowIfActionCodeNotMatched(1);
			m_changeInfo.m_oldIndex = index;
			m_changeInfo.m_oldItems = new T[] { item };
		}

		public void RemoveItem(int index)
		{
			ThrowIfActionCodeNotMatched(1);
			m_changeInfo.m_oldIndex = index;
		}

		public void RemoveItem(T item)
		{
			ThrowIfActionCodeNotMatched(1);
			m_changeInfo.m_oldItems = new T[] { item };
		}

		public void RemoveItems(int index, IList<T> items)
		{
			ThrowIfActionCodeNotMatched(1);
			m_changeInfo.m_oldIndex = index;
			m_changeInfo.m_oldItems = items.CopyAsArray();
		}

		public void RemoveItems(int index, params T[] items)
		{
			ThrowIfActionCodeNotMatched(1);
			m_changeInfo.m_oldIndex = index;
			m_changeInfo.m_oldItems = items;
		}

		public void ReplaceItem(int index, T oldItem, T newItem)
		{
			ThrowIfActionCodeNotMatched(2);
			m_changeInfo.m_oldIndex = index;
			m_changeInfo.m_newIndex = index;
			m_changeInfo.m_oldItems = new T[] { oldItem };
			m_changeInfo.m_newItems = new T[] { newItem };
		}

		public void ReplaceItems(int index, IList<T> oldItem, IList<T> newItem)
		{
			ThrowIfActionCodeNotMatched(2);
			m_changeInfo.m_oldIndex = index;
			m_changeInfo.m_newIndex = index;
			m_changeInfo.m_oldItems = oldItem.CopyAsArray();
			m_changeInfo.m_newItems = newItem.CopyAsArray();
		}

		public void MoveItem(int oldIndex, int newIndex, T item)
		{
			ThrowIfActionCodeNotMatched(3);
			m_changeInfo.m_oldIndex = oldIndex;
			m_changeInfo.m_newIndex = newIndex;
			var array = new T[] { item };
			m_changeInfo.m_oldItems = array;
			m_changeInfo.m_newItems = array;
		}

		public void MoveItems(int oldIndex, int newIndex, IList<T> items)
		{
			ThrowIfActionCodeNotMatched(3);
			m_changeInfo.m_oldIndex = oldIndex;
			m_changeInfo.m_newIndex = newIndex;
			var array = items.CopyAsArray();
			m_changeInfo.m_oldItems = array;
			m_changeInfo.m_newItems = array;
		}

		public void MoveItems(int oldIndex, int newIndex, params T[] items)
		{
			ThrowIfActionCodeNotMatched(3);
			m_changeInfo.m_oldIndex = oldIndex;
			m_changeInfo.m_newIndex = newIndex;
			m_changeInfo.m_oldItems = items;
			m_changeInfo.m_newItems = items;
		}

		public void ResetItems()
		{
			ThrowIfActionCodeNotMatched(4);
		}

		public void CancelChange()
		{
			m_changeInfo.m_cancelChange = true;
		}

		public void ActiveChange()
		{
			m_changeInfo.m_cancelChange = false;
		}

		public void Dispose()
		{

		}
	}

	public interface ICollectionNotifier<T> : INotifyCollectionChanged, IPropertyNotifier
	{
		void RaiseCollectionChangedEvent(NotifyCollectionChangedEventArgs eventArgs);
		void PushCollectionChangedInfo(CollectionChangedInfo<T> changedInfo);
		CollectionChangedInfo<T> PeekNextCollectionChangedInfo();
		void ExecuteCollectionChangeByInfo(CollectionChangedInfo<T> info);
	}

	public sealed class PseudoNotifyCollectionChangedEvent : IPseudoEvent
	{
		private readonly NotifyCollectionChangedEventHandler m_handler;
		private readonly NotifyCollectionChangedEventArgs m_args;
		private readonly object m_sender;

		public PseudoNotifyCollectionChangedEvent(NotifyCollectionChangedEventHandler handler, NotifyCollectionChangedEventArgs args, object sender = null)
		{
			m_sender = sender;
			m_handler = handler;
			m_args = args;
		}

		public object Handler => m_handler;

		public object EventArgs => m_args;

		public object Sender => m_sender;

		public void InvokeHandler()
		{
			m_handler?.Invoke(m_sender, m_args);
		}
	}

	public abstract class CollectionNotifier<T> : PropertyNotifier, ICollectionNotifier<T>
	{
		private int m_changingFlag;
		protected Queue<CollectionChangedInfo<T>> m_changedInfoQueue;
		protected NotifyCollectionChangedEventHandler m_collectionChangedHandler;

		private bool m_isCollectionNotifierSuspended;

		protected CollectionNotifier() : base()
		{
		}

		public bool IsCollectionNotifierSuspended => m_isCollectionNotifierSuspended;

		public void SuspendCollectionNotifier()
		{
			m_isCollectionNotifierSuspended = true;
		}

		public void ResumeCollectionNotifier()
		{
			m_isCollectionNotifierSuspended = true;
		}

		protected bool IsChanging => m_changingFlag > 0;

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				var thisHandler = m_collectionChangedHandler;
				NotifyCollectionChangedEventHandler tempHandler;
				do
				{
					tempHandler = thisHandler;
					var newHandler = (NotifyCollectionChangedEventHandler)Delegate.Combine(tempHandler, value);
					thisHandler = Interlocked.CompareExchange(ref m_collectionChangedHandler, newHandler, tempHandler);
				}
				while (thisHandler != tempHandler);
			}
			remove
			{
				var thisHandler = m_collectionChangedHandler;
				NotifyCollectionChangedEventHandler tempHandler;
				do
				{
					tempHandler = thisHandler;
					var newHandler = (NotifyCollectionChangedEventHandler)Delegate.Remove(tempHandler, value);
					thisHandler = Interlocked.CompareExchange(ref m_collectionChangedHandler, newHandler, tempHandler);
				}
				while (thisHandler != tempHandler);
			}
		}

		protected virtual void NotifyCollectionChangedEvent(NotifyCollectionChangedEventArgs eventArgs)
		{
			if (IsChanging)
			{
				return;
			}
			Interlocked.Exchange(ref m_changingFlag, 1);
			RaiseCollectionChangedEvent(eventArgs);
			Interlocked.Exchange(ref m_changingFlag, 0);
			while (m_changedInfoQueue != null && m_changedInfoQueue.Count > 0)
			{
				var info = m_changedInfoQueue.Dequeue();
				if (!info.ExecuteAction())
				{
					continue;
				}
				ExecuteCollectionChangeByInfo(info);
			}
			m_changedInfoQueue = null;
		}

		public void RaiseCollectionChangedEvent(NotifyCollectionChangedEventArgs eventArgs)
		{
			if (m_isCollectionNotifierSuspended)
			{
				return;
			}
			if (eventArgs is null)
			{
				throw new ArgumentNullException(nameof(eventArgs));
			}
			var e = new PseudoNotifyCollectionChangedEvent(m_collectionChangedHandler, eventArgs, this);
			if (IsNullOrCurrentContext)
			{
				InvokeEventCore(e);
			}
			else
			{
				m_synchronizationContext.Send(new SendOrPostCallback(InvokeEventCore), e);
			}
		}

		public abstract void ExecuteCollectionChangeByInfo(CollectionChangedInfo<T> info);

		public virtual void PushCollectionChangedInfo(CollectionChangedInfo<T> changedInfo)
		{
			if (changedInfo is null)
			{
				return;
			}
			if (m_changedInfoQueue is null)
			{
				m_changedInfoQueue = new Queue<CollectionChangedInfo<T>>();
			}
			m_changedInfoQueue.Enqueue(changedInfo);
		}

		public virtual CollectionChangedInfo<T>  PeekNextCollectionChangedInfo()
		{
			if (m_changedInfoQueue is null)
			{
				return null;
			}
			if (m_changedInfoQueue.Count > 0)
			{
				return m_changedInfoQueue.Peek();
			}
			return null;
		}
	}

	public abstract class NotifiableCollectionBase<T> : CollectionNotifier<T>, IList<T>, IReadOnlyList<T>, IList, ICollection
	{
		#region Indexer and Count
		public abstract int Count { get; }
		public T this[int index] { get => GetItem(index); set => SetItem(index, value); }
		#endregion

		#region Abstract Methods
		protected abstract bool InsertItem(int index, T item);
		protected abstract bool RemoveItemByIndex(int index);
		protected abstract bool RemoveItem(T item);
		protected abstract bool ReplaceItem(int index, T item);
		protected abstract bool MoveItem(int oldIndex, int newIndex, out T oldItem);
		protected abstract bool ResetItem();
		protected abstract int GetItemIndex(T item);
		public abstract T GetItem(int index);
		public abstract bool Contains(T item);
		public abstract void CopyTo(T[] array, int arrayIndex);
		public abstract IEnumerator<T> GetEnumerator();
		#endregion

		#region Override Methods
		public override void ExecuteCollectionChangeByInfo(CollectionChangedInfo<T> info)
		{
			var oldIndex = info.OldIndex;
			var oldItem = info.FirstOldItem;

			var newIndex = info.NewIndex;
			var newItem = info.FirstNewItem;
			switch (info.ActionCode)
			{
				case 0:
					{
						Insert(newIndex, newItem);
					}
					break;
				case 1:
					{
						if (oldIndex >= 0)
						{
							RemoveAt(oldIndex);
						}
						else
						{
							Remove(oldItem);
						}
					}
					break;
				case 2:
					{
						SetItem(newIndex, newItem);
					}
					break;
				case 3:
					{
						Move(oldIndex, newIndex);
					}
					break;
				case 4:
					{
						Clear();
					}
					break;
				default:
					break;
			}
		}
		#endregion

		#region Private Methods
		protected void ThrowIfDuringChanging()
		{
			if (IsChanging)
			{
				throw new InvalidOperationException(" Cannot change CollectionNotifier during a CollectionChanged event, use CollectionNotifier.PushCollectionChangedInfo instead.");
			}
		}
		#endregion

		#region Public Methods
		public int IndexOf(T item) => GetItemIndex(item);

		public void Add(T item)
		{
			Insert(Count, item);
		}

		public virtual void SetItem(int index, T item)
		{
			if (ContainsIndex(index))
			{
				ThrowIfDuringChanging();
				if (!ReplaceItem(index, item)) return;
				var eventArgs = CollectionNotifiers.CreateItemReplacedEventArgs(GetItem(index), item, index);
				RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
				NotifyCollectionChangedEvent(eventArgs);
			}
		}

		public virtual void Insert(int index, T item)
		{
			ThrowIfDuringChanging();
			if(!InsertItem(index, item)) return;
			var eventArgs = CollectionNotifiers.CreateItemInsertedEventArgs(item, index);
			RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
			RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
			NotifyCollectionChangedEvent(eventArgs);
		}

		public virtual void Move(int oldIndex, int newIndex)
		{
			if (ContainsIndex(oldIndex) && ContainsIndex(newIndex))
			{
				ThrowIfDuringChanging();
				if(!MoveItem(oldIndex,newIndex, out var oldItem)) return;
				var eventArgs = CollectionNotifiers.CreateItemMovedEventArgs(oldItem, newIndex, oldIndex);
				RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
				NotifyCollectionChangedEvent(eventArgs);
			}
		}

		public virtual bool Remove(T item)
		{
			var index = GetItemIndex(item);
			if (index < 0)
			{
				return false;
			}
			ThrowIfDuringChanging();
			if(!RemoveItem(item)) return false;
			var eventArgs = CollectionNotifiers.CreateItemRemovedEventArgs(item, index);
			RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
			RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
			NotifyCollectionChangedEvent(eventArgs);
			return true;
		}

		public virtual void RemoveAt(int index)
		{
			if (ContainsIndex(index))
			{
				ThrowIfDuringChanging();
				var item = GetItem(index);
				if(!RemoveItemByIndex(index)) return;
				var eventArgs = CollectionNotifiers.CreateItemRemovedEventArgs(item, index);
				RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
				RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
				NotifyCollectionChangedEvent(eventArgs);
			}
		}

		public virtual void Clear()
		{
			ThrowIfDuringChanging();
			if(!ResetItem()) return;
			RaisePropertyChangedEvent(CollectionNotifiers.CountPropertyChanged);
			RaisePropertyChangedEvent(CollectionNotifiers.IndexerPropertyChanged);
			NotifyCollectionChangedEvent(CollectionNotifiers.ResetCollectionChanged);
		}

		public virtual bool ContainsIndex(int index)
		{
			return 0 <= index && index < Count;
		}

		#endregion

		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		#region ICollection
		protected object m_syncRoot = new object();

		protected NotifiableCollectionBase() : base()
		{
		}

		object ICollection.SyncRoot => m_syncRoot;
		public virtual bool IsReadOnly => false;
		bool ICollection.IsSynchronized => false;
		#endregion

		#region IList

		bool IList.IsFixedSize => false;
		object IList.this[int index] { get => GetItem(index); set => SetItem(index, (T)value); }

		int IList.Add(object value)
		{
			if (value != null && value is T item)
			{
				Add(item);
			}
			return Count - 1;
		}

		bool IList.Contains(object value)
		{
			if (value != null && value is T item)
			{
				return Contains(item);
			}
			return false;
		}

		int IList.IndexOf(object value)
		{
			if (value != null && value is T item)
			{
				return GetItemIndex(item);
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			if (value != null && value is T item)
			{
				Insert(index, item);
			}
		}

		void IList.Remove(object value)
		{
			throw new NotImplementedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException($"Rank {array.Rank} is invalid");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("Argument Non Zero Lower Bound");
			}
			if (index < 0)
			{
				throw new IndexOutOfRangeException("NeedNonNegNum");
			}
			if (array.Length - index < this.Count)
			{
				throw new IndexOutOfRangeException("ArrayPlusOffTooSmall");
			}
			T[] array2 = array as T[];
			if (array2 != null)
			{
				CopyTo(array2, index);
				return;
			}
			Type elementType = array.GetType().GetElementType();
			Type typeFromHandle = typeof(T);
			if (!elementType.IsAssignableFrom(typeFromHandle) && !typeFromHandle.IsAssignableFrom(elementType))
			{
				throw new ArgumentException("Argument_InvalidArrayType");
			}
			object[] array3 = array as object[];
			if (array3 == null)
			{
				throw new ArgumentException("Argument_InvalidArrayType");
			}
			int count = Count;
			try
			{
				for (int i = 0; i < count; i++)
				{
					array3[index++] = GetItem(i);
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Argument_InvalidArrayType");
			}
		}
		#endregion
	}

	public class NotifiableList<T> : NotifiableCollectionBase<T>
	{
		protected IList<T> m_items;

		public NotifiableList() : base()
		{
			m_items = new List<T>();
		}

		public NotifiableList(int capacity) : base()
		{
			m_items = new List<T>(capacity);
		}

		public NotifiableList(IEnumerable<T> collection) : base()
		{
			m_items = CopyItemsFromEnumerable(collection);
		}

		public NotifiableList(IList<T> items, bool useSourceReference = false) : base()
		{
			m_items = items is null 
				? new List<T>() 
				: useSourceReference 
					? items 
					: CopyItemsFromEnumerable(items);
		}

		protected virtual IList<T> CopyItemsFromEnumerable(IEnumerable<T> collection)
		{
			return new List<T>(collection);
		}

		public override int Count => m_items.Count;

		public override bool Contains(T item)
		{
			return m_items.Contains(item);
		}

		public override void CopyTo(T[] array, int arrayIndex)
		{
			m_items.CopyTo(array, arrayIndex);
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return m_items.GetEnumerator();
		}

		public override T GetItem(int index)
		{
			return ContainsIndex(index) ? m_items[index] : default(T);
		}

		protected override int GetItemIndex(T item)
		{
			return m_items.IndexOf(item);
		}

		protected override bool InsertItem(int index, T item)
		{
			m_items.Insert(index, item);
			return true;
		}

		protected override bool MoveItem(int oldIndex, int newIndex, out T oldItem)
		{
			oldItem = m_items[oldIndex];
			m_items.RemoveAt(oldIndex);
			m_items.Insert(newIndex, oldItem);
			return true;
		}

		protected override bool RemoveItem(T item)
		{
			m_items.Remove(item);
			return true;
		}

		protected override bool RemoveItemByIndex(int index)
		{
			m_items.RemoveAt(index);
			return true;
		}

		protected override bool ReplaceItem(int index, T item)
		{
			m_items[index] = item;
			return true;
		}

		protected override bool ResetItem()
		{
			m_items.Clear();
			return true;
		}
	}
}
