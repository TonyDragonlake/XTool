using System.Collections.Generic;
using FantaziaDesign.Core;

namespace FantaziaDesign.Resourcable
{
	public interface IResourceProvider<Tkey, TResource> : IRuntimeUnique, IEnumerable<KeyValuePair<string, string>>
	{
		string ProviderName { get; }
		TResource ProvideResource(Tkey key, TResource defaultResource = default(TResource));
		bool CombineWith(IResourceProvider<Tkey, TResource> provider);
	}
}
