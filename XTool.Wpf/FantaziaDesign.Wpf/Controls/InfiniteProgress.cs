using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FantaziaDesign.Wpf.Controls
{
	[TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
	[TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
	public class InfiniteProgress : Control
	{
		static InfiniteProgress()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(InfiniteProgress), new FrameworkPropertyMetadata(typeof(InfiniteProgress)));
		}

		public bool IsActived
		{
			get { return (bool)GetValue(IsActivedProperty); }
			set { SetValue(IsActivedProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsActivedProperty =
			DependencyProperty.Register("IsActived", typeof(bool), typeof(InfiniteProgress), new PropertyMetadata(false, new PropertyChangedCallback(OnActiveStateChanged)));

		private static void OnActiveStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var infiniteProgress = d as InfiniteProgress;
			if (infiniteProgress is null)
			{
				return;
			}
			if ((bool)e.NewValue)
			{
				VisualStateManager.GoToState(infiniteProgress, "Active", true);
			}
			else
			{
				VisualStateManager.GoToState(infiniteProgress, "Inactive", true);
			}
		}

	}
}
