using System;
using System.Windows;
using System.Windows.Media;

namespace FantaziaDesign.Wpf.Controls
{
	public class ProgressElement : ClippedContentControl
	{
		private double _minimum, _maximum, _offset, _percent;

		public double MinimumValue
		{
			get { return (double)GetValue(MinimumValueProperty); }
			set { SetValue(MinimumValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MinimumValue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MinimumValueProperty =
			DependencyProperty.Register("MinimumValue", typeof(double), typeof(ProgressElement), new PropertyMetadata(0.0, OnMinimumValueChanged));

		public double MaximumValue
		{
			get { return (double)GetValue(MaximumValueProperty); }
			set { SetValue(MaximumValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MaximumValue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaximumValueProperty =
			DependencyProperty.Register("MaximumValue", typeof(double), typeof(ProgressElement), new PropertyMetadata(0.0,OnMaximumValueChanged));

		public double RangeOffset
		{
			get { return (double)GetValue(RangeOffsetProperty); }
			set { SetValue(RangeOffsetProperty, value); }
		}

		// Using a DependencyProperty as the backing store for RangeOffset.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RangeOffsetProperty =
			DependencyProperty.Register("RangeOffset", typeof(double), typeof(ProgressElement), new PropertyMetadata(0.0,OnRangeOffsetChanged));

		public double CurrentProgress
		{
			get { return (double)GetValue(CurrentProgressProperty); }
			set { SetValue(CurrentProgressProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentProgress.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentProgressProperty =
			DependencyProperty.Register("CurrentProgress", typeof(double), typeof(ProgressElement), new PropertyMetadata(0.0));

		public double CurrentPercentage
		{
			get { return (double)GetValue(CurrentPercentageProperty); }
			set { SetValue(CurrentPercentageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentPercentage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentPercentageProperty =
			DependencyProperty.Register("CurrentPercentage", typeof(double), typeof(ProgressElement), new PropertyMetadata(0.0, OnCurrentPercentageChanged));

		private static void OnMinimumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ProgressElement agent)
			{
				agent._minimum = (double)e.NewValue;
				agent.UpdateCurrentProgress();
			}
		}

		private static void OnMaximumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ProgressElement agent)
			{
				agent._maximum = (double)e.NewValue;
				agent.UpdateCurrentProgress();
			}
		}

		private static void OnRangeOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ProgressElement agent)
			{
				agent._offset = (double)e.NewValue;
				agent.UpdateCurrentProgress();
			}
		}

		private static void OnCurrentPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ProgressElement agent)
			{
				agent._percent = (double)e.NewValue;
				agent.UpdateCurrentProgress();
			}
		}

		private void UpdateCurrentProgress()
		{
			bool isPercentile = _minimum == 0 && _maximum == 100;
			double current = isPercentile ? _percent : _minimum + _percent / 100 * (_maximum - _minimum);
			//System.Diagnostics.Debug.WriteLine("UpdateCurrentProgress");
			if (_offset != 0)
			{
				current += _offset;
			}
			SetCurrentValue(CurrentProgressProperty, current);
		}
	}
}
