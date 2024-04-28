using System.Collections.Generic;

namespace FantaziaDesign.Resourcable
{
	public static class ResourceNotifierFactories
	{
		private static readonly Dictionary<string, object> s_factories = new Dictionary<string, object>();

		public static IResourceNotifierFactory<TKey, TResource> TryGetFactory<TKey, TResource>(string factoryName)
		{
			if (string.IsNullOrWhiteSpace(factoryName))
			{
				return null;
			}
			if (s_factories.TryGetValue(factoryName, out var factory))
			{
				return factory as IResourceNotifierFactory<TKey, TResource>;
			}
			return null;
		}

		public static bool RegisterFactory<TKey, TResource>(IResourceNotifierFactory<TKey, TResource> factory, bool allowOverride = false)
		{
			if (factory is null)
			{
				return false;
			}
			var factoryName = factory.FactoryName;
			var contains = s_factories.ContainsKey(factoryName);
			if (contains)
			{
				if (allowOverride)
				{
					s_factories[factoryName] = factory;
					return true;
				}
				return false;
			}
			s_factories.Add(factoryName, factory);
			return true;
		}
	}

}
