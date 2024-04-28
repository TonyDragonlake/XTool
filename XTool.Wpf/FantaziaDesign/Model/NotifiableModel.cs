using System;
using System.Collections.Generic;
using System.Threading;
using FantaziaDesign.Core;

namespace FantaziaDesign.Model
{
	public interface INotifiableModel : IPropertyNotifier, IRuntimeUnique, IEquatable<INotifiableModel>
	{
		SynchronizationContext SynchronizationContext { get; }
	}

	public class NotifiableModel : PropertyNotifier, INotifiableModel
	{
		private long m_uId;
		public long UId => m_uId;

		public NotifiableModel() : base()
		{
			m_uId = SnowflakeUId.Next();
		}

		public bool Equals(INotifiableModel other)
		{
			return !(other is null) && m_uId == other.UId;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as INotifiableModel);
		}

		public override int GetHashCode()
		{
			return m_uId.GetHashCode();
		}
	}

	public class NotifiableModelList<TNotifiableModel> : NotifiableList<TNotifiableModel>, INotifiableModel where TNotifiableModel : INotifiableModel
	{
		private long m_uId;
		private SynchronizationContext m_syncContext;

		public NotifiableModelList()
		{
			m_uId = SnowflakeUId.Next();
			m_syncContext = SynchronizationContext.Current;
		}

		public NotifiableModelList(int capacity) : base(capacity)
		{
			m_uId = SnowflakeUId.Next();
			m_syncContext = SynchronizationContext.Current;
		}

		public NotifiableModelList(IEnumerable<TNotifiableModel> collection) : base(collection)
		{
			m_uId = SnowflakeUId.Next();
			m_syncContext = SynchronizationContext.Current;
		}

		public NotifiableModelList(IList<TNotifiableModel> items) : base(items)
		{
			m_uId = SnowflakeUId.Next();
			m_syncContext = SynchronizationContext.Current;
		}

		public long UId => m_uId;
		public SynchronizationContext SynchronizationContext { get => m_syncContext; protected set => m_syncContext = value; }

		public bool Equals(INotifiableModel other)
		{
			return !(other is null) && m_uId == other.UId;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as INotifiableModel);
		}

		public override int GetHashCode()
		{
			return m_uId.GetHashCode();
		}
	}

}
