using System;
using System.Windows;
using FantaziaDesign.Core;

namespace FantaziaDesign.Wpf.Controls
{
	public static class FrameworkUtil
	{
		private static Type s_typeofFrameworkElement = typeof(FrameworkElement);

		private static Func<FrameworkElement, string, DependencyObject> GetTemplateChild_FrameworkElement;


		public static Rect GetRelativeRenderRect(this FrameworkElement frameworkElement, FrameworkElement relativeTo)
		{
			if (frameworkElement != null && relativeTo != null)
			{
				return new Rect(frameworkElement.TranslatePoint(new Point(), relativeTo), frameworkElement.RenderSize);
			}
			return default(Rect);
		}

		public static double GetArea(this Rect rect)
		{
			if (rect.IsEmpty)
			{
				return 0;
			}
			return rect.Height * rect.Width;
		}

		public static double GetArea(this Size size)
		{
			if (size.IsEmpty)
			{
				return 0;
			}
			return size.Height * size.Width;
		}

		public static Size GetActualSize(this FrameworkElement frameworkElement)
		{
			return new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);
		}

		public static DependencyObject GetTemplateChild(FrameworkElement frameworkElement, string name)
		{
			if (GetTemplateChild_FrameworkElement is null)
			{
				GetTemplateChild_FrameworkElement = ReflectionUtil.BindMethodToDelegate<Func<FrameworkElement, string, DependencyObject>>(
					s_typeofFrameworkElement, "GetTemplateChild", ReflectionUtil.NonPublicInstance
					);
			}
			return GetTemplateChild_FrameworkElement(frameworkElement, name);
		}

		public static bool GetTemplateChild<T>(this FrameworkElement frameworkElement, string name, out T control) where T : DependencyObject
		{
			control = GetTemplateChild(frameworkElement,name) as T;
			return control != null;
		}
	}
}
