namespace FantaziaDesign.Resourcable
{
	public delegate void ResourceChangedEventHandler<TKey, TResource>(object sender, IResourceContainer<TKey, TResource> args);

	public interface INotifyResourceChanged<TKey, TResource>
	{
		event ResourceChangedEventHandler<TKey, TResource> ResourceChanged;
	}
}
