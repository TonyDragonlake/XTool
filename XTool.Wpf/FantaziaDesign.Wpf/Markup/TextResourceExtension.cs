using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using FantaziaDesign.Resourcable;

namespace FantaziaDesign.Wpf.Markup
{
	public class TextResourceExtension : MarkupExtension
	{
		[ConstructorArgument("key")]
		public string Key { get; set; }

		[ConstructorArgument("groupname")]
		public string GroupName { get; set; }

		public string DefaultValue { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var factory = ResourceNotifierFactories.TryGetFactory<string, string>(GroupName);
			if (factory is null)
			{
				var dVal = DefaultValue;
				if (string.IsNullOrWhiteSpace(dVal))
				{
					return string.Empty;
				}
				return dVal;
			}
			var source = factory.CreateFromKey(Key);
			var binding = new Binding($"{nameof(source.Resource)}")
			{
				Mode = BindingMode.OneWay,
				Source = source
			};

			var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (target.TargetObject is Setter)
			{
				return binding;
			}
			else
			{
				return binding.ProvideValue(serviceProvider);
			}
		}
	}
}
