namespace FantaziaDesign.Resourcable
{
	public interface IResourceNotifierFactory<TKey, TResource>
	{
		string FactoryName { get; }
		IResourceNotifier<TKey, TResource> CreateFromKey(TKey key);
	}

}
