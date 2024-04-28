using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace FantaziaDesign.Wpf.Markup
{
	public class DynamicResourceBinding : DynamicResourceExtension
	{
		public DynamicResourceBinding()
		{
		}

		public DynamicResourceBinding(string resourceKey) : base(resourceKey)
		{
		}

		public IValueConverter Converter { get; set; }

		public object ConverterParameter { get; set; }

		public CultureInfo ConverterCulture { get; set; }

		public string StringFormat { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			object obj = base.ProvideValue(serviceProvider);
			if (Converter == null && StringFormat == null)
			{
				return obj;
			}
			DynamicResourceBindingSource dynamicResourceBindingSource = new DynamicResourceBindingSource
			{
				ResourceReferenceExpression = obj
			};
			var targetobj = ((IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))).TargetObject;
			FrameworkElement frameworkElement = targetobj as FrameworkElement;
			if (frameworkElement != null)
			{
				frameworkElement.Resources.Add(dynamicResourceBindingSource, dynamicResourceBindingSource);
			}
			Binding binding = new Binding
			{
				Path = new PropertyPath(DynamicResourceBindingSource.ResourceReferenceExpressionProperty),
				Source = dynamicResourceBindingSource,
				Converter = Converter,
				ConverterParameter = ConverterParameter,
				ConverterCulture = ConverterCulture,
				StringFormat = StringFormat,
				Mode = BindingMode.OneWay
			};
			//if (frameworkElement == null)
			//{
			//	return binding;
			//}
			return binding.ProvideValue(serviceProvider);
		}

		private class DynamicResourceBindingSource : Freezable
		{
			public object ResourceReferenceExpression
			{
				get
				{
					return GetValue(ResourceReferenceExpressionProperty);
				}
				set
				{
					SetValue(ResourceReferenceExpressionProperty, value);
				}
			}

			protected override Freezable CreateInstanceCore()
			{
				return new DynamicResourceBindingSource();
			}

			public static readonly DependencyProperty ResourceReferenceExpressionProperty = DependencyProperty.Register("ResourceReferenceExpression", typeof(object), typeof(DynamicResourceBindingSource), new FrameworkPropertyMetadata());
		}
	}
}
