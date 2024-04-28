using FantaziaDesign.Model;

namespace FantaziaDesign.Resourcable
{
	public interface IResourceNotifier<TKey, TResource> : IResourceContainer<TKey, TResource>, IPropertyNotifier { }
}
