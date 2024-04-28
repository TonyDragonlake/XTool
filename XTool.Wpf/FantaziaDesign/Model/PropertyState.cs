using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaziaDesign.Model
{
	public interface IDataProperty<T>
	{
		string PropertyName { get; }
		T PropertyValue { get; }
		void SetPropertyValue(T propValue);
	}

	public interface IDataProperty : IDataProperty<object>
	{
		bool TryGetPropertyValueAs<T>(out T propValue);
	}

	public interface IStateHost
	{
		void SetPropertyData(IDataProperty property);
	}

	public interface IPropertyState
	{
		IStateHost StateHost { get; }
		IDataProperty PropertyData { get; }
		bool IsVirtual { get; set; }
	}

	public static class DataPropertiesUtil
	{
		public static bool IsAnonymous<T>(this IDataProperty<T> dataProperty)
		{
			return string.IsNullOrWhiteSpace(dataProperty.PropertyName);
		}
	}

	public class DataProperty : IDataProperty
	{
		private string m_propName;
		protected object m_propValue;

		public DataProperty(string propName)
		{
			m_propName = propName;
		}

		public DataProperty(string propName, object propValue) : this(propName)
		{
			m_propValue = propValue;
		}

		public string PropertyName => m_propName;
		public object PropertyValue => m_propValue;


		public void SetPropertyValue(object propValue)
		{
			m_propValue = propValue;
		}

		public bool TryGetPropertyValueAs<T>(out T propValue)
		{
			if (!(m_propValue is null))
			{
				if (m_propValue.GetType() == typeof(T))
				{
					propValue = (T)m_propValue;
					return true;
				}
			}
			propValue = default(T);
			return false;
		}
	}

	public class PropertyState : IPropertyState
	{
		private IStateHost m_stateHost;
		private DataProperty m_prop;
		private bool m_isVirtual;
		private bool m_isEmpty;

		public static readonly PropertyState Empty = new PropertyState(true);

		private PropertyState(bool isEmpty)
		{
			m_isEmpty = isEmpty;
		}

		public static PropertyState New(IStateHost host, string propName, object propValue)
		{
			return new PropertyState(host, propName, propValue);
		}

		public PropertyState(IStateHost host, string propName, object propValue)
		{
			m_stateHost = host;
			m_prop = new DataProperty(propName, propValue);
		}

		public PropertyState()
		{
		}

		public IStateHost StateHost => m_stateHost;

		public IDataProperty PropertyData
		{
			get
			{
				if (m_isEmpty)
				{
					return null;
				}
				return m_prop;
			}
		}

		public bool IsVirtual { get => m_isVirtual; set => m_isVirtual = value; }
		public bool IsEmpty => m_isEmpty;
	}

	public sealed class Operation : IList<IPropertyState>
	{
		private string m_opName;
		private List<IPropertyState> m_states = new List<IPropertyState>();

		private Operation(string opName)
		{
			m_opName = opName;
		}

		public static Operation New(string opName)
		{
			return new Operation(opName);
		}

		public IPropertyState this[int index] { get => m_states[index]; set => m_states[index] = value; }

		public int Count => m_states.Count;

		public bool IsReadOnly => true;

		public string Name { get => m_opName; }

		public void Add(IPropertyState item)
		{
			m_states.Add(item);
		}

		public void Clear()
		{
			m_states.Clear();
		}

		public bool Contains(IPropertyState item)
		{
			return m_states.Contains(item);
		}

		public void CopyTo(IPropertyState[] array, int arrayIndex)
		{
			m_states.CopyTo(array, arrayIndex);
		}

		public IEnumerator<IPropertyState> GetEnumerator()
		{
			return m_states.GetEnumerator();
		}

		public int IndexOf(IPropertyState item)
		{
			return m_states.IndexOf(item);
		}

		public void Insert(int index, IPropertyState item)
		{
			m_states.Insert(index, item);
		}

		public bool Remove(IPropertyState item)
		{
			return m_states.Remove(item);
		}

		public void RemoveAt(int index)
		{
			m_states.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Operation AppendState(IPropertyState item)
		{
			m_states.Add(item);
			return this;
		}

		public Operation RemoveState(IPropertyState item)
		{
			m_states.Remove(item);
			return this;
		}

		public Operation RemoveStateAt(int index)
		{
			m_states.RemoveAt(index);
			return this;
		}

		public Operation ClearState()
		{
			m_states.Clear();
			return this;
		}

		public bool TryExecute()
		{
			if (m_states.Count > 0)
			{
				try
				{
					foreach (var item in m_states)
					{
						item.StateHost.SetPropertyData(item.PropertyData);
					}
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
			return false;
		}

	}

	public class OperationRecorder : IReadOnlyCollection<Operation>, IEnumerable<Operation>
	{
		private int m_capability;

		private LinkedList<Operation> m_history = new LinkedList<Operation>();

		private LinkedListNode<Operation> m_currentOpNode;

		public OperationRecorder(int capability = 32)
		{
			m_capability = capability;
		}

		public int Capability => m_capability;

		public int Count => m_history.Count;

		public void RecordOperation(Operation newOperation)
		{
			m_currentOpNode = m_history.AddLast(newOperation);
			while (m_history.Count > m_capability)
			{
				m_history.RemoveFirst();
			}
		}

		// Undo
		public bool TryExecuteLastState(Func<Operation, bool> execution)
		{
			if (m_currentOpNode is null)
			{
				return false;
			}
			var lastNode = m_currentOpNode.Previous;
			if (lastNode is null)
			{
				return false;
			}
			var executed = (execution?.Invoke(lastNode.Value)).GetValueOrDefault();
			if (executed)
			{
				m_currentOpNode = lastNode;
			}
			return executed;
		}

		public bool TryExecuteLastState()
		{
			if (m_currentOpNode is null)
			{
				return false;
			}
			var lastNode = m_currentOpNode.Previous;
			if (lastNode is null)
			{
				return false;
			}
			var executed = lastNode.Value.TryExecute();
			if (executed)
			{
				m_currentOpNode = lastNode;
			}
			return executed;
		}


		//Redo
		public bool TryExecuteNextState(Func<Operation, bool> execution)
		{
			if (m_currentOpNode is null)
			{
				return false;
			}
			var nextNode = m_currentOpNode.Next;
			if (nextNode is null)
			{
				return false;
			}
			var executed = (execution?.Invoke(nextNode.Value)).GetValueOrDefault();
			if (executed)
			{
				m_currentOpNode = nextNode;
			}
			return executed;
		}

		public bool TryExecuteNextState()
		{
			if (m_currentOpNode is null)
			{
				return false;
			}
			var nextNode = m_currentOpNode.Next;
			if (nextNode is null)
			{
				return false;
			}
			var executed = nextNode.Value.TryExecute();
			if (executed)
			{
				m_currentOpNode = nextNode;
			}
			return executed;
		}

		public void ClearHistory()
		{
			m_history.Clear();
			m_currentOpNode = null;
		}

		public IEnumerator<Operation> GetEnumerator()
		{
			return m_history.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
