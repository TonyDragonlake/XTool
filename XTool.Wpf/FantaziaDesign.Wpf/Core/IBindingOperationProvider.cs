using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FantaziaDesign.Wpf.Core
{
	public interface IBindingOperationsProvider
	{
		void SetBindingOperations(DependencyObject targetObject, object model);
		void ClearBindingOperations(DependencyObject targetObject);
	}

	public static class Bindable
	{
		public static IBindingOperationsProvider GetBindingOperationsProvider(DependencyObject obj)
		{
			return (IBindingOperationsProvider)obj.GetValue(BindingOperationsProviderProperty);
		}

		public static void SetBindingOperationsProvider(DependencyObject obj, IBindingOperationsProvider value)
		{
			obj.SetValue(BindingOperationsProviderProperty, value);
		}

		// Using a DependencyProperty as the backing store for BindingOperationsProvider.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BindingOperationsProviderProperty =
			DependencyProperty.RegisterAttached("BindingOperationsProvider", typeof(IBindingOperationsProvider), typeof(Bindable), new FrameworkPropertyMetadata(null));

	}

}
