using FantaziaDesign.Model;

namespace FantaziaDesign.Resourcable
{
	public abstract class ResourceNotifier<TKey, TResource> : PropertyNotifier, IResourceNotifier<TKey, TResource>
	{
		protected readonly TKey m_key;

		protected ResourceNotifier(TKey key) : base()
		{
			m_key = key;
		}

		public TKey Key => m_key;
		public abstract TResource Resource { get; }
		public abstract void SetResource(TResource resource);
	}
}
