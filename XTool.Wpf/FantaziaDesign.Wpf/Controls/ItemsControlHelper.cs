using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FantaziaDesign.Core;
using FantaziaDesign.Wpf.Core;

namespace FantaziaDesign.Wpf.Controls
{
	// Ctrl + / = Ctrl + A = Select All 

	public static class ItemsControlHelper
	{
		private static RoutedCommand s_addItemCommand = new RoutedCommand(nameof(RoutedAddItemCommand),typeof(ItemsControlHelper));
		private static RoutedCommand s_removeItemsCommand = new RoutedCommand(nameof(RoutedRemoveItemsCommand), typeof(ItemsControlHelper));
		private static RoutedCommand s_clearItemsCommand = new RoutedCommand(nameof(RoutedClearItemsCommand), typeof(ItemsControlHelper));

		public static RoutedCommand RoutedAddItemCommand => s_addItemCommand;
		public static RoutedCommand RoutedRemoveItemsCommand => s_removeItemsCommand;
		public static RoutedCommand RoutedClearItemsCommand => s_clearItemsCommand;

		private static Func<ItemsControl, Panel> GetItemsHostProperty_ItemsControl;
		private static Func<ItemsControl, ScrollViewer> GetScrollHostProperty_ItemsControl;
		private static Func<ItemsPresenter, ItemsControl> GetOwnerProperty_ItemsPresenter;
		private static Func<ItemContainerGenerator, DependencyObject> GetPeer_ItemContainerGenerator;

		public static ItemsControl GetOwnerFromItemsPresenter(this ItemsPresenter itemsPresenter)
		{
			if (itemsPresenter is null)
			{
				throw new ArgumentNullException(nameof(itemsPresenter));
			}
			if (GetOwnerProperty_ItemsPresenter is null)
			{
				GetOwnerProperty_ItemsPresenter =
					ReflectionUtil.BindInstancePropertyGetterToDelegate<ItemsPresenter, ItemsControl>(
						"Owner", ReflectionUtil.NonPublicInstance, true
						);
			}
			return GetOwnerProperty_ItemsPresenter(itemsPresenter);
		}

		public static ItemContainerGenerator GetGeneratorFromItemsPresenter(this ItemsPresenter itemsPresenter)
		{
			var itemsControl = GetOwnerFromItemsPresenter(itemsPresenter);
			if (itemsControl is null)
			{
				return null;
			}
			return itemsControl.ItemContainerGenerator;
		}

		public static Panel GetItemsPanelFromItemsControl(this ItemsControl itemsControl)
		{
			if (itemsControl is null)
			{
				throw new ArgumentNullException(nameof(itemsControl));
			}
			if (GetItemsHostProperty_ItemsControl is null)
			{
				GetItemsHostProperty_ItemsControl = 
					ReflectionUtil.BindInstancePropertyGetterToDelegate<ItemsControl, Panel>(
						"ItemsHost", ReflectionUtil.NonPublicInstance, true
						);
			}
			return GetItemsHostProperty_ItemsControl(itemsControl);
		}

		public static ScrollViewer GetScrollHostFromItemsControl(this ItemsControl itemsControl)
		{
			if (itemsControl is null)
			{
				throw new ArgumentNullException(nameof(itemsControl));
			}
			if (GetScrollHostProperty_ItemsControl is null)
			{
				GetScrollHostProperty_ItemsControl =
					ReflectionUtil.BindInstancePropertyGetterToDelegate<ItemsControl, ScrollViewer>(
						"ScrollHost", ReflectionUtil.NonPublicInstance, true
						);
			}
			return GetScrollHostProperty_ItemsControl(itemsControl);
		}

		public static DependencyObject GetPeerFromItemContainerGenerator(this ItemContainerGenerator itemContainerGenerator)
		{
			if (GetPeer_ItemContainerGenerator is null)
			{
				GetPeer_ItemContainerGenerator = ReflectionUtil.BindInstancePropertyGetterToDelegate<ItemContainerGenerator, DependencyObject>("Peer", ReflectionUtil.NonPublicInstance, true);
			}
			return GetPeer_ItemContainerGenerator(itemContainerGenerator);
		}

		public static ItemsPresenter FindFirstItemsPresenter(ItemsControl itemsControl)
		{
			var visualtreeEnum = VisualTreeExtension.GetVisualTreeTraverser(itemsControl);

			while (visualtreeEnum.MoveNext())
			{
				if (visualtreeEnum.Current is ItemsPresenter presenter)
				{
					return presenter;
				}
			}
			return null;
		}

		public static ScrollViewer FindFirstScrollViewer(ItemsControl itemsControl)
		{
			var visualtreeEnum = VisualTreeExtension.GetVisualTreeTraverser(itemsControl);

			while (visualtreeEnum.MoveNext())
			{
				if (visualtreeEnum.Current is ScrollViewer scrollViewer)
				{
					return scrollViewer;
				}
			}
			return null;
		}

		public static Panel FindItemsPanelFromPresenter(ItemsPresenter itemsPresenter)
		{
			if (itemsPresenter is null)
			{
				throw new ArgumentNullException(nameof(itemsPresenter));
			}

			if (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
			{
				var panel = VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
				if (panel is null || !panel.IsItemsHost)
				{
					return null;
				}
				return panel;
			}
			return null;
		}

		public static ScrollViewer FindScrollViewerFromPresenter(ItemsPresenter itemsPresenter, ItemsControl itemsControl)
		{
			if (itemsPresenter is null)
			{
				throw new ArgumentNullException(nameof(itemsPresenter));
			}

			if (itemsControl is null)
			{
				if (GetOwnerProperty_ItemsPresenter is null)
				{
					GetOwnerProperty_ItemsPresenter =
						ReflectionUtil.BindInstancePropertyGetterToDelegate<ItemsPresenter, ItemsControl>(
							"Owner", ReflectionUtil.NonPublicInstance, true
							);
				}
				itemsControl = GetOwnerProperty_ItemsPresenter(itemsPresenter);
				if (itemsControl is null)
				{
					throw new ArgumentNullException(nameof(itemsControl));
				}
			}

			DependencyObject dependencyObject = itemsPresenter;
			while (dependencyObject != itemsControl && dependencyObject != null)
			{
				ScrollViewer scrollViewer = dependencyObject as ScrollViewer;
				if (scrollViewer != null)
				{
					return scrollViewer;
				}
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			}
			return null;
		}

		public static void GetItemsControlComponents(ItemsControl itemsControl, out Panel itemsPanel, out ItemsPresenter itemsPresenter, out ScrollViewer scrollHost)
		{
			itemsPanel = null;
			itemsPresenter = null;
			scrollHost = null;
			if (itemsControl is null)
			{
				return;
			}
			var visualtreeEnum = VisualTreeExtension.GetVisualTreeTraverser(itemsControl);
			while (visualtreeEnum.MoveNext())
			{
				if (visualtreeEnum.Current is ItemsPresenter presenter)
				{
					itemsPresenter = presenter;
					break;
				}
			}
			if (itemsPresenter != null)
			{
				if (VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
				{
					itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
					DependencyObject dependencyObject = itemsPanel;
					while (dependencyObject != itemsControl && dependencyObject != null)
					{
						ScrollViewer scrollViewer = dependencyObject as ScrollViewer;
						if (scrollViewer != null)
						{
							scrollHost = scrollViewer;
							break;
						}
						dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
					}
				}
			}
		}

	}




}
