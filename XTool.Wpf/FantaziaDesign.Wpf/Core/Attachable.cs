using System.Windows;
using FantaziaDesign.Core;

namespace FantaziaDesign.Wpf.Core
{
	public static class Attachable
	{
		public static CornerRadius GetCornerRadius(DependencyObject obj)
		{
			return (CornerRadius)obj.GetValue(CornerRadiusProperty);
		}

		public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
		{
			obj.SetValue(CornerRadiusProperty, value);
		}

		// Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CornerRadiusProperty =
			DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(Attachable),
				new FrameworkPropertyMetadata(default(CornerRadius)), new ValidateValueCallback(IsCornerRadiusValid));

		private static bool IsCornerRadiusValid(object value)
		{
			if (value is CornerRadius cr)
			{
				return DoubleUtil.IsValid(default, cr.TopLeft, cr.TopRight, cr.BottomRight, cr.BottomRight);
			}
			return false;
		}

		public static object GetHeader(DependencyObject obj)
		{
			return obj.GetValue(HeaderProperty);
		}

		public static void SetHeader(DependencyObject obj, object value)
		{
			obj.SetValue(HeaderProperty, value);
		}

		// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.RegisterAttached("Header", typeof(object), typeof(Attachable), new FrameworkPropertyMetadata("Header"));

		public static object GetContent(DependencyObject obj)
		{
			return obj.GetValue(ContentProperty);
		}

		public static void SetContent(DependencyObject obj, object value)
		{
			obj.SetValue(ContentProperty, value);
		}

		// Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.RegisterAttached("Content", typeof(object), typeof(Attachable), new FrameworkPropertyMetadata("Content"));




	}

}
