using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using FantaziaDesign.Core;
using FantaziaDesign.Input;
using FantaziaDesign.Wpf.Core;
using FantaziaDesign.Wpf.Input;

namespace FantaziaDesign.Wpf.Controls
{
	public class MessageHost : ItemsControl
	{
		private static Type s_typeOfThis = typeof(MessageHost);
		private static Dictionary<string, WeakReference> s_loadedInstances = new Dictionary<string, WeakReference>();
		internal static Dictionary<string, WeakReference> LoadedInstances => s_loadedInstances;

		protected CommonCommandSourceCollection m_commandSources;
		protected IRemoveItemCommandHost m_messageHostManager;
		protected IBindingOperationsProvider m_bindingsProvider = DefaultBindingsProvider;

		public sealed class BindingsProvider : IBindingOperationsProvider
		{
			internal BindingsProvider()
			{
			}

			public void ClearBindingOperations(DependencyObject targetObject)
			{
				if (targetObject is null)
				{
					return;
				}
				//BindingOperations.ClearBinding(targetObject, IdentifierProperty);
				BindingOperations.ClearBinding(targetObject, RemoveItemsCommandProperty);
			}

			public void SetBindingOperations(DependencyObject targetObject, object model)
			{
				var currentManager = model as IRemoveItemCommandHost;
				if (currentManager is null)
				{
					return;
				}
				//BindingOperations.SetBinding(targetObject, IdentifierProperty, new Binding($"{nameof(IMessageHostManager.Identifier)}") { Mode = BindingMode.OneWayToSource, Source = currentManager });
				BindingOperations.SetBinding(targetObject, RemoveItemsCommandProperty, new Binding($"{nameof(IRemoveItemCommandHost.RemoveItemCommand)}") { Mode = BindingMode.OneWay, Source = currentManager });

			}
		}

		protected static IBindingOperationsProvider DefaultBindingsProvider => new BindingsProvider();

		static MessageHost()
		{
			ItemsPanelTemplate itemsPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(LayersPanel)));
			itemsPanelTemplate.Seal();
			ItemsControl.ItemsPanelProperty.OverrideMetadata(s_typeOfThis, new FrameworkPropertyMetadata(itemsPanelTemplate));

			DataContextProperty.OverrideMetadata(s_typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnDataContextChanged)));
		}

		public MessageHost()
		{
			InitCommandSources();

			Loaded += MessageHost_Loaded;
			Unloaded += MessageHost_Unloaded;
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
			}
		}

		private void MessageHost_Unloaded(object sender, RoutedEventArgs e)
		{
			var identifier = Identifier;
			if (string.IsNullOrWhiteSpace(identifier))
			{
				return;
			}
			s_loadedInstances.Remove(identifier);
		}

		private void MessageHost_Loaded(object sender, RoutedEventArgs e)
		{
			var identifier = Identifier;
			if (string.IsNullOrWhiteSpace(identifier))
			{
				return;
			}
			if (!s_loadedInstances.ContainsKey(identifier))
			{
				s_loadedInstances.Add(identifier, new WeakReference(this));
			}
		}

		protected virtual void InitCommandSources()
		{
			m_commandSources = new CommonCommandSourceCollection();
			m_commandSources.ParentInputElement = this;
			m_commandSources.RegisterCommandSource(RemoveItemsCommandProperty, CommandComponentKind.Command, true);
		}

		private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as MessageHost)?.OnDataContextChanged(e);
		}

		protected virtual void OnDataContextChanged(DependencyPropertyChangedEventArgs e)
		{
			bool isLastConnected = e.OldValue is IRemoveItemCommandHost;
			bool isCurrentConnected = e.NewValue is IRemoveItemCommandHost;

			if (isLastConnected || isCurrentConnected)
			{
				BindingManager(e.NewValue as IRemoveItemCommandHost);
			}
		}

		protected virtual void BindingManager(IRemoveItemCommandHost manager)
		{
			if (m_bindingsProvider is null)
			{
				return;
			}
			if (manager is null)
			{
				m_bindingsProvider.ClearBindingOperations(this);
				m_messageHostManager = null;
			}
			else
			{
				m_messageHostManager = manager;
				m_bindingsProvider.SetBindingOperations(this, m_messageHostManager);
			}
		}

		public string Identifier
		{
			get { return (string)GetValue(IdentifierProperty); }
			set { SetValue(IdentifierProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Identifier.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IdentifierProperty =
			DependencyProperty.Register("Identifier", typeof(string), s_typeOfThis, new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnIdentifierChanged), new CoerceValueCallback(OnCoerceIdentifier)));

		private static object OnCoerceIdentifier(DependencyObject d, object baseValue)
		{
			var baseValueStr = baseValue.ToString();
			if (string.IsNullOrWhiteSpace(baseValueStr))
			{
				return string.Empty;
			}
			return baseValue;
		}

		private static void OnIdentifierChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageHost host)
			{
				var oldValue = (string)e.OldValue;
				var newValue = (string)e.NewValue;
				var isOldValueInvalid = string.IsNullOrWhiteSpace(oldValue);
				var isNewValueInvalid = string.IsNullOrWhiteSpace(newValue);

				if (isOldValueInvalid)
				{
					if (isNewValueInvalid)
					{
						return;
					}
					if (host.IsLoaded)
					{
						if (!s_loadedInstances.ContainsKey(newValue))
						{
							s_loadedInstances.Add(newValue, new WeakReference(host));
						}
					}
				}
				else
				{
					s_loadedInstances.Remove(oldValue);
					if (!isNewValueInvalid)
					{
						if (host.IsLoaded)
						{
							if (!s_loadedInstances.ContainsKey(newValue))
							{
								s_loadedInstances.Add(newValue, new WeakReference(host));
							}
						}
					}
				}
			}
		}

		public ICommand RemoveItemsCommand
		{
			get { return (ICommand)GetValue(RemoveItemsCommandProperty); }
			set { SetValue(RemoveItemsCommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for RemoveItemsCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RemoveItemsCommandProperty =
			DependencyProperty.Register("RemoveItemsCommand", typeof(ICommand), s_typeOfThis, new FrameworkPropertyMetadata(null, OnCommonCommandChanged));

		protected static void OnCommonCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageHost view)
			{
				view.m_commandSources.TrySetCommand(RemoveItemsCommandProperty, (ICommand)e.NewValue);
			}
		}

		public IInputElement RemoveItemsCommandTarget
		{
			get { return (IInputElement)GetValue(RemoveItemsCommandTargetProperty); }
			set { SetValue(RemoveItemsCommandTargetProperty, value); }
		}

		// Using a DependencyProperty as the backing store for RemoveItemsCommandTarget.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RemoveItemsCommandTargetProperty =
			DependencyProperty.Register("RemoveItemsCommandTarget", typeof(IInputElement), s_typeOfThis, new FrameworkPropertyMetadata(null, OnCommonCommandTargetChanged));

		protected static void OnCommonCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageHost view)
			{
				view.m_commandSources.TrySetCommandTarget(e.Property, (IInputElement)e.NewValue);
			}
		}

		public IBindingOperationsProvider BindingOperationsProvider
		{
			get { return (IBindingOperationsProvider)GetValue(BindingOperationsProviderProperty); }
			set { SetValue(BindingOperationsProviderProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BindingOperationsProvider.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BindingOperationsProviderProperty =
			Bindable.BindingOperationsProviderProperty.AddOwner(s_typeOfThis, new FrameworkPropertyMetadata(DefaultBindingsProvider, new PropertyChangedCallback(OnBindingOperationsProviderChanged)));

		private static void OnBindingOperationsProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageHost host)
			{
				var oldValue = e.OldValue as IBindingOperationsProvider;
				var newValue = e.NewValue as IBindingOperationsProvider;
				if (newValue is null)
				{
					if (oldValue != null)
					{
						oldValue.ClearBindingOperations(host);
					}
					host.m_bindingsProvider = null;
				}
				else
				{
					host.m_bindingsProvider = newValue;
				}
				host.BindingManager(host.m_messageHostManager);
			}
		}

		protected override DependencyObject GetContainerForItemOverride()
		{
			return new MessageContainer();
		}

		internal void TryRemoveItem(MessageContainer item)
		{
			if (item is null)
			{
				return;
			}

			var itemModel = ItemContainerGenerator.ItemFromContainer(item);
			if (itemModel != null)
			{
				m_commandSources.TrySetCommandParameter(RemoveItemsCommandProperty, itemModel, CommandComponentKind.Command);
				m_commandSources.ExecuteCommandSource(RemoveItemsCommandProperty, CommandComponentKind.Command);
			}
		}
	}
}
