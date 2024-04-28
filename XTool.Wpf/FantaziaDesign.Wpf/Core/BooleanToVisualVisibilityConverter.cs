using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FantaziaDesign.Wpf.Core
{
	public interface IBooleanLikeValueConverter<TTarget> : IValueConverter
	{
		TTarget TrueTarget { get; set; }
		TTarget FalseTarget { get; set; }
	}

	public class BooleanToTargetObjectConverter : IBooleanLikeValueConverter<object>
	{
		public object TrueTarget { get; set; }
		public object FalseTarget { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool boolean)
			{
				return boolean ? TrueTarget : FalseTarget;
			}
			var nullableBoolean = value as bool?;
			return nullableBoolean.GetValueOrDefault() ? TrueTarget : FalseTarget;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value.Equals(TrueTarget);
		}
	}

	public class BooleanToVisualVisibilityConverter : IBooleanLikeValueConverter<Visibility>
	{
		public Visibility TrueTarget { get; set; } = Visibility.Visible;

		public Visibility FalseTarget { get; set; } = Visibility.Collapsed;

		public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool boolean)
			{
				return boolean ? TrueTarget : FalseTarget;
			}
			var nullableBoolean = value as bool?;
			return nullableBoolean.GetValueOrDefault() ? TrueTarget : FalseTarget;
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility visibility)
			{
				return visibility == TrueTarget;
			}
			return false;
		}
	}

	public class EqualityToVisualVisibilityConverter<TSource> : BooleanToVisualVisibilityConverter
	{
		public TSource EqualitySource { get; set; }

		public TSource DefaultSource { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is TSource source)
			{
				var equals = EqualityComparer<TSource>.Default.Equals(source, EqualitySource);
				return equals ? TrueTarget : FalseTarget;
			}
			return FalseTarget;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility visibility)
			{
				if (visibility == TrueTarget)
				{
					return EqualitySource;
				}
			}
			return DefaultSource;
		}
	}

	public sealed class SelectionResultTypeToVisualVisibilityConverter : EqualityToVisualVisibilityConverter<SelectionResultType>
	{
	}

}
