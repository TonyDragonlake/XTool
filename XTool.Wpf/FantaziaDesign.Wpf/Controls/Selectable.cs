using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using FantaziaDesign.Core;
using FantaziaDesign.Wpf.Core;

namespace FantaziaDesign.Wpf.Controls
{
	public static class Selectable
	{
		private class __Perceptor
		{
			public __Perceptor() { }

			public bool IsChangingSelectedItems { get; set; }

			public bool IsOnHandleSelectionChanged { get; set; }

			public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
			{
				if (IsChangingSelectedItems)
				{
					return;
				}
				IsOnHandleSelectionChanged = true;
				var selector = sender as Selector;
				if (s_GetSelectedItemsImpl_Selector is null)
				{
					s_GetSelectedItemsImpl_Selector =
						ReflectionUtil.BindPropertyGetterToDelegate<Func<Selector, IList>>(
							typeof(Selector), "SelectedItemsImpl", ReflectionUtil.NonPublicInstance, true);
				}
				var selectedItems = s_GetSelectedItemsImpl_Selector.Invoke(selector);
				SetMultipleSelection(selector, selectedItems);
				IsOnHandleSelectionChanged = false;
			}


		}

		private static Func<Selector, IList> s_GetSelectedItemsImpl_Selector;
		private static Func<Selector, IEnumerable, bool> s_SetSelectedItemsImpl_Selector;
		private static readonly Dictionary<Selector, __Perceptor> s_perceptors = new Dictionary<Selector, __Perceptor>();

		public static bool GetIsSelectionPerceptionEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsSelectionPerceptionEnabledProperty);
		}

		public static void SetIsSelectionPerceptionEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(IsSelectionPerceptionEnabledProperty, value);
		}

		// Using a DependencyProperty as the backing store for IsSelectionPerceptionEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsSelectionPerceptionEnabledProperty =
			DependencyProperty.RegisterAttached("IsSelectionPerceptionEnabled", typeof(bool), typeof(Selectable), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectionPerceptionEnabledChanged)));

		private static void OnIsSelectionPerceptionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var selector = d as Selector;
			if (selector != null)
			{
				var isSelectionPerceptionEnabled = (bool)e.NewValue;
				if (isSelectionPerceptionEnabled)
				{
					if (!s_perceptors.ContainsKey(selector))
					{
						var perceptor = new __Perceptor();
						selector.SelectionChanged += perceptor.OnSelectionChanged;
						s_perceptors.Add(selector, perceptor);
					}
				}
				else
				{
					if (s_perceptors.TryGetValue(selector, out var perceptor))
					{
						selector.SelectionChanged -= perceptor.OnSelectionChanged;
						s_perceptors.Remove(selector);
					}
				}
			}
		}


		public static IList GetMultipleSelection(DependencyObject obj)
		{
			return (IList)obj.GetValue(MultipleSelectionProperty);
		}

		public static void SetMultipleSelection(DependencyObject obj, IList value)
		{
			obj.SetValue(MultipleSelectionProperty, value);
		}

		// Using a DependencyProperty as the backing store for MultipleSelection.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MultipleSelectionProperty =
			DependencyProperty.RegisterAttached("MultipleSelection", typeof(IList), typeof(Selectable), new PropertyMetadata(null, new PropertyChangedCallback(OnMultipleSelectionChanged)));

		private static void OnMultipleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var selector = d as Selector;
			if (selector != null && GetIsSelectionPerceptionEnabled(selector))
			{
				if (s_perceptors.TryGetValue(selector, out var perceptor))
				{
					if (perceptor.IsOnHandleSelectionChanged)
					{
						return;
					}

					perceptor.IsChangingSelectedItems = true;
					if (s_SetSelectedItemsImpl_Selector is null)
					{
						s_SetSelectedItemsImpl_Selector =
							ReflectionUtil.BindMethodToDelegate<Func<Selector, IEnumerable, bool>>(
								typeof(Selector), "SetSelectedItemsImpl", ReflectionUtil.NonPublicInstance, typeof(IEnumerable));

					}
					s_SetSelectedItemsImpl_Selector.Invoke(selector, (IList)e.NewValue);
					perceptor.IsChangingSelectedItems = false;
				}
			}
		}

	}

}
