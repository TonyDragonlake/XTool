using System;
using FantaziaDesign.Events;

namespace FantaziaDesign.Resourcable
{
	public class ResourceChangedEventArgs<TKey, TResource> : EventArgs, IResourceContainer<TKey, TResource>
	{
		private class __ResourceContainer : IResourceContainer<TKey, TResource>
		{
			private readonly TKey m_key;
			private TResource m_resource;
			public __ResourceContainer(TKey key) => m_key = key;
			public TKey Key => m_key;
			public TResource Resource => m_resource;
			public void SetResource(TResource resource) => m_resource = resource;
		}

		protected IResourceContainer<TKey, TResource> m_innerArgs;

		public TKey Key => m_innerArgs.Key;

		public TResource Resource => m_innerArgs.Resource;

		public ResourceChangedEventArgs(TKey key) => m_innerArgs = new __ResourceContainer(key);
		public ResourceChangedEventArgs(TKey key, TResource resource) : this(key) => m_innerArgs.SetResource(resource);
		public ResourceChangedEventArgs(IResourceContainer<TKey, TResource> resourceContainer)
		{
			if (resourceContainer is null)
			{
				throw new ArgumentNullException(nameof(resourceContainer));
			}
			m_innerArgs = resourceContainer;
		}
		public virtual void SetResource(TResource resource) => m_innerArgs.SetResource(resource);
	}

	public class WeakResourceChangedEventArgs<TKey, TResource> : ResourceChangedEventArgs<TKey, TResource>, IEventArguments<ResourceChangedEventHandler<TKey, TResource>>
	{
		protected readonly WeakReference m_sender;
		protected bool m_handled;

		public WeakResourceChangedEventArgs(object sender, TKey key) : base(key) 
		{
			m_sender = new WeakReference(sender);
		}

		public WeakResourceChangedEventArgs(object sender, TKey key, TResource resource) : this(sender, key) => m_innerArgs.SetResource(resource);
		
		public WeakResourceChangedEventArgs(object sender, IResourceContainer<TKey, TResource> resourceContainer) : base(resourceContainer) 
		{
			m_sender = new WeakReference(sender);
		}

		public object Sender => m_sender.IsAlive ? m_sender.Target : null;
		public bool Handled { get => m_handled; set => m_handled = value; }

		public void InvokeEventHandler(ResourceChangedEventHandler<TKey, TResource> handler)
		{
			handler?.Invoke(Sender, m_innerArgs);
		}
	}

}
