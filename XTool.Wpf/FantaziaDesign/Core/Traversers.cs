using System;
using System.Collections;
using System.Collections.Generic;

namespace FantaziaDesign.Core
{
	public enum TraversalMethod : byte
	{
		DeepFirst,
		BreathFirst
	}

	public interface IReadOnlyChildrenContainer<out T>
	{
		IReadOnlyCollection<T> Children { get; }
	}

	public delegate IReadOnlyCollection<T> GetChildrenCallback<T>(T node);

	public abstract class TraverserBase<T> : TraversalIterator<T> where T : IReadOnlyChildrenContainer<T>
	{
		protected TraverserBase(T root) : base(root)
		{
		}
	}

	public abstract class TraversalIterator<T> : IEnumerator<T>
	{
		protected T m_root;
		protected T m_current;
		protected Predicate<T> m_skip;

		protected TraversalIterator(T root)
		{
			if (!typeof(T).IsValueType)
			{
				if (root.IsNull())
				{
					throw new ArgumentNullException(nameof(root));
				}
			}
			m_root = root;
		}

		public T Current => m_current;
		object IEnumerator.Current => m_current;
		public Predicate<T> SkippingCondition { get => m_skip; set => m_skip = value; }
		public T Root => m_root;
		public abstract TraversalMethod TraversalMethod { get; }

		public abstract void Dispose();
		public abstract bool MoveNext();
		public abstract void Reset();

		protected bool CanAdd(T item)
		{
			if (m_skip is null)
			{
				return true;
			}
			return !m_skip.Invoke(item);
		}
	}

	public abstract class GenericTraverserBase<T> : TraversalIterator<T>
	{
		protected GetChildrenCallback<T> GetChildrenNodes;

		protected GenericTraverserBase(T root, GetChildrenCallback<T> childrenNodesGetter = null) : base(root)
		{
			if (childrenNodesGetter is null)
			{
				if (typeof(T).IsAssignableFrom(typeof(IReadOnlyChildrenContainer<T>)))
				{
					childrenNodesGetter = CollectionHelper.ChildrenItems;
				}
				else
				{
					throw new ArgumentNullException(nameof(childrenNodesGetter));
				}
			}
			GetChildrenNodes = childrenNodesGetter;
		}
	}

	public class DeepFirstTraverser<T> : TraverserBase<T> where T : IReadOnlyChildrenContainer<T>
	{
		private T[] _stack;
		private int _eleCount;

		public DeepFirstTraverser(T root) : base(root)
		{
			_stack = new T[8];
			_stack[0] = m_root;
			_eleCount = 1;
		}

		public override sealed TraversalMethod TraversalMethod => TraversalMethod.DeepFirst;

		public override void Dispose()
		{
			m_root = default(T);
			m_current = default(T);
			Array.Clear(_stack, 0, _stack.Length);
			_stack = null;
			_eleCount = 0;
		}

		public override bool MoveNext()
		{
			if (_eleCount > 0)
			{
				_eleCount--;
				m_current = _stack[_eleCount];
				var children = m_current.Children;
				if (children is null)
				{
					return true;
				}
				var stackCapacity = _stack.Length;
				var targetCount = _eleCount + children.Count;
				if (CollectionHelper.TryExtendCapacity(ref stackCapacity, targetCount))
				{
					Array.Resize(ref _stack, stackCapacity);
					//T[] array = new T[stackCapacity];
					//if (_eleCount > 0)
					//{
					//	Array.Copy(_stack, 0, array, 0, _eleCount);
					//}
					//_stack = array;
				}
				using (var enumerator = children.GetEnumerator())
				{
					int i = targetCount - 1;
					while (i >= _eleCount)
					{
						if (enumerator.MoveNext())
						{
							var item = enumerator.Current;
							// to do : add skip condition 
							if (CanAdd(item))
							{
								_stack[i] = enumerator.Current;
							}
						}
						else
						{
							break;
						}
						i--;
					}
					i++;
					var skipped = i - _eleCount;
					// skipped logic
					if (skipped > 0)
					{
						Array.Copy(_stack, i, _stack, _eleCount, targetCount - i);
						targetCount -= skipped;
					}
				}
				_eleCount = targetCount;
				return true;
			}
			//if (_stackHelper.Count > 0)
			//{
			//	_current = _stackHelper.Pop();
			//	var children = _current.Children;
			//	for (int i = children.Count - 1; i >= 0; i--)
			//	{
			//		_stackHelper.Push(children[i]);
			//	}
			//	return true;
			//}
			return false;
		}

		public override void Reset()
		{
			m_current = default(T);
			Array.Clear(_stack, 0, _stack.Length);
			_stack[0] = m_root;
			_eleCount = 0;
		}
	}

	public class BreathFirstTraverser<T> : TraverserBase<T> where T : IReadOnlyChildrenContainer<T>
	{
		private Queue<T> _queueHelper;

		public override sealed TraversalMethod TraversalMethod => TraversalMethod.BreathFirst;

		public BreathFirstTraverser(T root) : base(root)
		{
			_queueHelper = new Queue<T>();
			_queueHelper.Enqueue(m_root);
		}

		public override void Dispose()
		{
			m_root = default(T);
			m_current = default(T);
			_queueHelper = null;
		}

		public override bool MoveNext()
		{
			if (_queueHelper.Count > 0)
			{
				m_current = _queueHelper.Dequeue();
				var children = m_current.Children;
				if (children is null)
				{
					return true;
				}
				var enumerator = children.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current;
					if (CanAdd(item))
					{
						_queueHelper.Enqueue(item);
					}
				}
				return true;
			}
			return false;
		}

		public override void Reset()
		{
			m_current = default(T);
			_queueHelper.Clear();
			_queueHelper.Enqueue(m_root);
		}
	}

	public class TraversalBacktracker<T> : IEnumerator<T> where T : IReadOnlyChildrenContainer<T>
	{
		private List<T> _backtracker;
		private T _current;
		private int _currentIndex;

		public TraversalBacktracker(T root, TraversalMethod traversalMethod, Predicate<T> skippingCondition = null)
		{
			_backtracker = new List<T>(32);
			TraverserBase<T> traverser;
			if (traversalMethod == TraversalMethod.BreathFirst)
			{
				traverser = new BreathFirstTraverser<T>(root);
			}
			else
			{
				traverser = new DeepFirstTraverser<T>(root);
			}
			if (skippingCondition != null)
			{
				traverser.SkippingCondition = skippingCondition;
			}
			using (traverser)
			{
				while (traverser.MoveNext())
				{
					_backtracker.Add(traverser.Current);
				}
			}
			_currentIndex = _backtracker.Count;
		}

		public T Current => _current;

		public IReadOnlyList<T> Backtracker { get => _backtracker; }

		object IEnumerator.Current => _current;

		public void Dispose()
		{
			_backtracker.Clear();
			_current = default(T);
			_currentIndex = 0;
		}

		public bool MoveNext()
		{
			var index = _currentIndex - 1;
			if (index >= 0 && index < _backtracker.Count)
			{
				_current = _backtracker[index];
				_currentIndex = index;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			_current = default(T);
			_currentIndex = _backtracker.Count;
		}
	}

	public class GenericDeepFirstTraverser<T> : GenericTraverserBase<T>
	{
		private T[] _stack;
		private int _eleCount;

		public override sealed TraversalMethod TraversalMethod => TraversalMethod.DeepFirst;

		public GenericDeepFirstTraverser(T root, GetChildrenCallback<T> childrenNodesGetter = null)
			: base(root, childrenNodesGetter)
		{
			_stack = new T[8];
			_stack[0] = m_root;
			_eleCount = 1;
		}


		public override void Dispose()
		{
			m_root = default(T);
			m_current = default(T);
			Array.Clear(_stack, 0, _stack.Length);
			_stack = null;
			GetChildrenNodes = null;
			_eleCount = 0;
		}

		public override bool MoveNext()
		{
			if (_eleCount > 0)
			{
				_eleCount--;
				m_current = _stack[_eleCount];
				var children = GetChildrenNodes?.Invoke(m_current);
				if (children is null)
				{
					return true;
				}
				var stackCapacity = _stack.Length;
				var targetCount = _eleCount + children.Count;
				if (CollectionHelper.TryExtendCapacity(ref stackCapacity, targetCount))
				{
					Array.Resize(ref _stack, stackCapacity);
					//T[] array = new T[stackCapacity];
					//if (_eleCount > 0)
					//{
					//	Array.Copy(_stack, 0, array, 0, _eleCount);
					//}
					//_stack = array;
				}
				using (var enumerator = children.GetEnumerator())
				{
					int i = targetCount - 1;
					while (i >= _eleCount)
					{
						if (enumerator.MoveNext())
						{
							var item = enumerator.Current;
							if (CanAdd(item))
							{
								_stack[i] = enumerator.Current;
							}
						}
						else
						{
							break;
						}
						i--;
					}
					i++;
					var skipped = i - _eleCount;
					// skipped logic
					if (skipped > 0)
					{
						Array.Copy(_stack, i, _stack, _eleCount, targetCount - i);
						targetCount -= skipped;
					}
				}
				_eleCount = targetCount;
				return true;
			}
			//if (_stackHelper.Count > 0)
			//{
			//	_current = _stackHelper.Pop();
			//	var children = _current.Children;
			//	for (int i = children.Count - 1; i >= 0; i--)
			//	{
			//		_stackHelper.Push(children[i]);
			//	}
			//	return true;
			//}
			return false;
		}

		public override void Reset()
		{
			m_current = default(T);
			Array.Clear(_stack, 0, _stack.Length);
			_stack[0] = m_root;
			_eleCount = 0;
		}
	}

	public class GenericBreathFirstTraverser<T> : GenericTraverserBase<T>
	{
		private Queue<T> _queueHelper;

		public override sealed TraversalMethod TraversalMethod => TraversalMethod.BreathFirst;

		public GenericBreathFirstTraverser(T root, GetChildrenCallback<T> childrenNodesGetter = null)
			: base(root, childrenNodesGetter)
		{
			_queueHelper = new Queue<T>();
			_queueHelper.Enqueue(m_root);
		}

		public override void Dispose()
		{
			m_root = default(T);
			m_current = default(T);
			_queueHelper = null;
		}

		public override bool MoveNext()
		{
			if (_queueHelper.Count > 0)
			{
				m_current = _queueHelper.Dequeue();
				var children = GetChildrenNodes?.Invoke(m_current);
				if (children is null)
				{
					return true;
				}
				var enumerator = children.GetEnumerator();
				while (enumerator.MoveNext())
				{
					var item = enumerator.Current;
					if (CanAdd(item))
					{
						_queueHelper.Enqueue(item);
					}
				}
				return true;
			}
			return false;
		}

		public override void Reset()
		{
			m_current = default(T);
			_queueHelper.Clear();
			_queueHelper.Enqueue(m_root);
		}
	}

	public class GenericTraversalBacktracker<T> : IEnumerator<T>
	{
		private List<T> _backtracker;
		private T _current;
		private int _currentIndex;

		public GenericTraversalBacktracker(T root, TraversalMethod traversalMethod, GetChildrenCallback<T> childrenNodesGetter = null, Predicate<T> skippingCondition = null)
		{
			_backtracker = new List<T>(32);
			GenericTraverserBase<T> traverser;
			if (traversalMethod == TraversalMethod.BreathFirst)
			{
				traverser = new GenericBreathFirstTraverser<T>(root, childrenNodesGetter);
			}
			else
			{
				traverser = new GenericDeepFirstTraverser<T>(root, childrenNodesGetter);
			}
			if (skippingCondition != null)
			{
				traverser.SkippingCondition = skippingCondition;
			}
			using (traverser)
			{
				while (traverser.MoveNext())
				{
					_backtracker.Add(traverser.Current);
				}
			}
			_currentIndex = _backtracker.Count;
		}

		public T Current => _current;

		public IReadOnlyList<T> Backtracker { get => _backtracker; }

		object IEnumerator.Current => _current;

		public void Dispose()
		{
			_backtracker.Clear();
			_current = default(T);
			_currentIndex = 0;
		}

		public bool MoveNext()
		{
			var index = _currentIndex - 1;
			if (index >= 0 && index < _backtracker.Count)
			{
				_current = _backtracker[index];
				_currentIndex = index;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			_current = default(T);
			_currentIndex = _backtracker.Count;
		}
	}

	public sealed class SingleItemCollection<T> : IEnumerable<T>
	{
		private T _item;

		public T Item { get => _item; set => _item = value; }

		public static implicit operator T(SingleItemCollection<T> collection)
		{
			if (collection is null)
			{
				throw new ArgumentNullException(nameof(collection));
			}
			return collection._item;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public class Enumerator : IEnumerator<T>
		{
			private T _item;
			private bool canMoveNext;
			internal Enumerator(SingleItemCollection<T> parent)
			{
				if (parent is null)
				{
					return;
				}
				_item = parent._item;
				canMoveNext = true;
			}

			public T Current => _item;

			object IEnumerator.Current => _item;

			public void Dispose()
			{

			}

			public bool MoveNext()
			{
				if (canMoveNext)
				{
					canMoveNext = false;
					return true;
				}
				return false;
			}

			public void Reset()
			{
				canMoveNext = true;
			}
		}
	}
}
