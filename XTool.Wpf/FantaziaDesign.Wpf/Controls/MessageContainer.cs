using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using FantaziaDesign.Core;
using FantaziaDesign.Model.Message;
using FantaziaDesign.Wpf.Core;
using FantaziaDesign.Wpf.Input;

namespace FantaziaDesign.Wpf.Controls
{
	public class MessageContainer : ContentControl
	{
		private static Type s_typeOfThis = typeof(MessageContainer);
		private FiniteTimeWalker m_timeWalker;
		private bool m_closeOnClickAway;
		protected IBindingOperationsProvider m_bindingsProvider = DefaultBindingsProvider;
		protected CommonCommandSourceCollection m_commandSources = new CommonCommandSourceCollection();
		protected IMessageModel messageContainerManager;
		private ContentPresenter m_contentPresenter;
		private bool m_shouldRecoverTimeWalker;


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
				BindingOperations.ClearBinding(targetObject, MessageStatusProperty);
				BindingOperations.ClearBinding(targetObject, MessageResultProperty);
				BindingOperations.ClearBinding(targetObject, CloseOnTimeoutProperty);
				BindingOperations.ClearBinding(targetObject, CloseOnClickAwayProperty);
				BindingOperations.ClearBinding(targetObject, MessageTimeoutProperty);
				BindingOperations.ClearBinding(targetObject, ClosingCommandProperty);
				BindingOperations.ClearBinding(targetObject, MaskOpacityProperty);
			}

			public void SetBindingOperations(DependencyObject targetObject, object model)
			{
				var currentManager = model as IMessageModel;
				if (currentManager is null)
				{
					return;
				}
				/*
				public interface IMessageContainerManager
				{
					MessageStatus MessageStatus { get; set; }
					object MessageResult { get; set; }
					bool CloseOnTimeout { get; set; }
					bool CloseOnClickAway { get; set; }
					TimeSpan MessageTimeout { get; set; }
					ICommand ClosingCommand { get; }
					Task<bool> MessageTask { get; }
					double MaskOpacity { get; set; }
				}
				 */
				BindingOperations.SetBinding(targetObject, MessageStatusProperty, new Binding($"{nameof(IMessageModel.MessageStatus)}") { Mode = BindingMode.TwoWay, Source = currentManager });
				BindingOperations.SetBinding(targetObject, MessageResultProperty, new Binding($"{nameof(IMessageModel.MessageResult)}") { Mode = BindingMode.TwoWay, Source = currentManager });
				BindingOperations.SetBinding(targetObject, CloseOnTimeoutProperty, new Binding($"{nameof(IMessageModel.CloseOnTimeout)}") { Mode = BindingMode.OneWay, Source = currentManager });
				BindingOperations.SetBinding(targetObject, CloseOnClickAwayProperty, new Binding($"{nameof(IMessageModel.CloseOnClickAway)}") { Mode = BindingMode.OneWay, Source = currentManager });
				BindingOperations.SetBinding(targetObject, MessageTimeoutProperty, new Binding($"{nameof(IMessageModel.MessageTimeout)}") { Mode = BindingMode.OneWay, Source = currentManager });
				BindingOperations.SetBinding(targetObject, ClosingCommandProperty, new Binding($"{nameof(IMessageModel.ClosingCommand)}") { Mode = BindingMode.OneWay, Source = currentManager });
				BindingOperations.SetBinding(targetObject, MaskOpacityProperty, new Binding($"{nameof(IMessageModel.MaskOpacity)}") { Mode = BindingMode.OneWay, Source = currentManager });

			}
		}

		protected static IBindingOperationsProvider DefaultBindingsProvider => new BindingsProvider();

		static MessageContainer()
		{
			CommandHelper.RegisterCommandHandler(s_typeOfThis, ApplicationCommands.Stop, new ExecutedRoutedEventHandler(OnStopCommand));
			DataContextProperty.OverrideMetadata(s_typeOfThis, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnDataContextChanged)));
		}

		private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as MessageContainer)?.OnDataContextChanged(e);
		}

		protected virtual void OnDataContextChanged(DependencyPropertyChangedEventArgs e)
		{
			bool isLastConnected = e.OldValue is IMessageModel;
			bool isCurrentConnected = e.NewValue is IMessageModel;

			if (isLastConnected || isCurrentConnected)
			{
				BindingManager(e.NewValue as IMessageModel);
			}
		}

		protected virtual void BindingManager(IMessageModel manager)
		{
			if (m_bindingsProvider is null)
			{
				return;
			}
			if (manager is null)
			{
				m_bindingsProvider.ClearBindingOperations(this);
				messageContainerManager = null;
			}
			else
			{
				messageContainerManager = manager;
				m_bindingsProvider.SetBindingOperations(this, messageContainerManager);
			}
		}

		public MessageContainer()
		{
			InitCommandSources();
			Loaded += MessageContainer_Loaded;
			//Unloaded += MessageContainer_Unloaded;
		}

		//private async void MessageContainer_Unloaded(object sender, RoutedEventArgs e)
		//{
		//	//VisualStateManager.GoToState(this, "Unloaded", true);
		//}

		private void MessageContainer_Loaded(object sender, RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "Loaded", true);

			SetCurrentValue(MessageStatusProperty, MessageStatus.Opened);
			if (CloseOnTimeout)
			{
				if (HasTimer && TimeWalker.TimerStatus == TimerStatus.Running)
				{
					return;
				}
				TimeWalker.Interval = MessageTimeout;
				TimeWalker.Start();
			}
		}

		protected virtual void InitCommandSources()
		{
			m_commandSources.ParentInputElement = this;
			m_commandSources.RegisterCommandSource(ClosingCommandProperty,CommandComponentKind.Command, true);
		}

		private static async void OnStopCommand(object sender, ExecutedRoutedEventArgs e)
		{
			if (sender is MessageContainer container)
			{
				await container.TryCloseSelf();
				e.Handled = true;
			}
		}

		private async Task TryCloseSelf()
		{
			OnBeforeClosing();
			var result = await OnClosing();
			if (result)
			{
				OnClosed();
				await VisualStateHelper.GoToStateAsync(this, "Unloaded", true);
				ParentMessageHost?.TryRemoveItem(this);
			}
			else
			{
				OnCancelClosing();
			}
		}

		protected virtual void OnCancelClosing()
		{
			if (m_shouldRecoverTimeWalker)
			{
				TimeWalker.Start();
			}
			SetCurrentValue(MessageStatusProperty, MessageStatus.Opened);
		}

		protected virtual void OnClosed()
		{
			var walker = TimeWalker;
			if (m_shouldRecoverTimeWalker)
			{
				walker.Stop();
				m_shouldRecoverTimeWalker = false;
			}
			walker.TimeOut -= OnTimeOut;
			SetCurrentValue(MessageStatusProperty, MessageStatus.Closed);
		}

		protected virtual void OnBeforeClosing()
		{
			var walker = TimeWalker;
			if (walker.TimerStatus != TimerStatus.Stopped)
			{
				walker.Pause();
				m_shouldRecoverTimeWalker = true;
			}
		}

		protected virtual Task<bool> OnClosing()
		{
			//System.Diagnostics.Debug.WriteLine("OnClosing");

			if (ClosingCommand is null)
			{
				return Task.FromResult(true);
			}
			return m_commandSources.ExecuteCommandSourceAsync(ClosingCommandProperty, CommandComponentKind.Command);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			bool matched = VisualTreeExtension.TryFindVisualChildIf(this, (v) => { return v is ContentPresenter; }, out Visual matchedChild);

			if (matched)
			{
				m_contentPresenter = matchedChild as ContentPresenter;
			}
			else
			{
				m_contentPresenter = null;
			}
		}

		protected FiniteTimeWalker TimeWalker
		{
			get
			{
				if (m_timeWalker is null)
				{
					m_timeWalker = new FiniteTimeWalker();
					m_timeWalker.TimeOut += OnTimeOut;
				}
				return m_timeWalker;
			}
		}

		private async void OnTimeOut(object sender, EventArgs e)
		{
			await TryCloseSelf();
		}

		protected bool HasTimer => m_timeWalker != null;

		public MessageHost ParentMessageHost => ItemsControl.ItemsControlFromItemContainer(this) as MessageHost;

		public MessageStatus MessageStatus
		{
			get { return (MessageStatus)GetValue(MessageStatusProperty); }
			set { SetValue(MessageStatusProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MessageStatus.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessageStatusProperty =
			DependencyProperty.Register("MessageStatus", typeof(MessageStatus), s_typeOfThis, new FrameworkPropertyMetadata(MessageStatus.Unknown, new PropertyChangedCallback(OnMessageStatusChanged)));

		private static void OnMessageStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine("Begin OnMessageStatusChanged");
			if (d is MessageContainer container)
			{
				container.OnMessageStatusChanged((MessageStatus)e.OldValue, (MessageStatus)e.NewValue);
			}
			//System.Diagnostics.Debug.WriteLine("End OnMessageStatusChanged");

		}

		protected virtual async void OnMessageStatusChanged(MessageStatus oldValue, MessageStatus newValue)
		{
			if (newValue == MessageStatus.RequestClose)
			{
				await TryCloseSelf();
			}
		}

		public TimeSpan MessageTimeout
		{
			get { return (TimeSpan)GetValue(MessageTimeoutProperty); }
			set { SetValue(MessageTimeoutProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MessageTimeout.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessageTimeoutProperty =
			DependencyProperty.Register("MessageTimeout", typeof(TimeSpan), s_typeOfThis, new FrameworkPropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(OnMessageTimeoutChanged)));

		private static void OnMessageTimeoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageContainer container)
			{
				if (container.CloseOnTimeout && container.IsLoaded)
				{
					var timeout = (TimeSpan)e.NewValue;
					if (timeout == TimeSpan.Zero)
					{
						if (container.HasTimer)
						{
							container.TimeWalker.Stop();
						}
						return;
					}
					container.TimeWalker.Interval = timeout;
				}
			}
		}

		public bool CloseOnTimeout
		{
			get { return (bool)GetValue(CloseOnTimeoutProperty); }
			set { SetValue(CloseOnTimeoutProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CloseOnTimeout.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CloseOnTimeoutProperty =
			DependencyProperty.Register("CloseOnTimeout", typeof(bool), s_typeOfThis, new PropertyMetadata(false, new PropertyChangedCallback(OnCloseOnTimeoutChanged)));

		private static void OnCloseOnTimeoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageContainer container)
			{
				if (container.IsLoaded)
				{
					if ((bool)e.NewValue)
					{
						var timeout = container.MessageTimeout;
						if (timeout == TimeSpan.Zero)
						{
							if (container.HasTimer)
							{
								container.TimeWalker.Stop();
							}
							return;
						}
						var timer = container.TimeWalker;
						timer.Interval = timeout;
						timer.Start();
					}
					else
					{
						if (container.HasTimer)
						{
							container.TimeWalker.Stop();
						}
					}
				}
			}
		}

		public bool CloseOnClickAway
		{
			get { return (bool)GetValue(CloseOnClickAwayProperty); }
			set { SetValue(CloseOnClickAwayProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CloseOnClickAway.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CloseOnClickAwayProperty =
			DependencyProperty.Register("CloseOnClickAway", typeof(bool), s_typeOfThis, new PropertyMetadata(false, new PropertyChangedCallback(OnCloseOnClickAwayChanged)));

		private static void OnCloseOnClickAwayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageContainer container)
			{
				container.m_closeOnClickAway = (bool)e.NewValue;
			}
		}

		public object MessageResult
		{
			get { return (object)GetValue(MessageResultProperty); }
			set { SetValue(MessageResultProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MessageResult.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessageResultProperty =
			DependencyProperty.Register("MessageResult", typeof(object), s_typeOfThis, new PropertyMetadata(null));

		public ICommand ClosingCommand
		{
			get { return (ICommand)GetValue(ClosingCommandProperty); }
			set { SetValue(ClosingCommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ClosingCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ClosingCommandProperty =
			DependencyProperty.Register("ClosingCommand", typeof(ICommand), typeof(MessageContainer), new FrameworkPropertyMetadata(null, OnCommonCommandChanged));

		public IBindingOperationsProvider BindingOperationsProvider
		{
			get { return (IBindingOperationsProvider)GetValue(BindingOperationsProviderProperty); }
			set { SetValue(BindingOperationsProviderProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BindingOperationsProvider.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BindingOperationsProviderProperty =
			Bindable.BindingOperationsProviderProperty.AddOwner(s_typeOfThis, new FrameworkPropertyMetadata(DefaultBindingsProvider, new PropertyChangedCallback( OnBindingOperationsProviderChanged)));

		private static void OnBindingOperationsProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageContainer container)
			{
				var oldValue = e.OldValue as IBindingOperationsProvider;
				var newValue = e.NewValue as IBindingOperationsProvider;
				if (newValue is null)
				{
					if (oldValue != null)
					{
						oldValue.ClearBindingOperations(container);
					}
					container.m_bindingsProvider = null;
				}
				else
				{
					container.m_bindingsProvider = newValue;
				}
				container.BindingManager(container.messageContainerManager);
			}
		}

		public double MaskOpacity
		{
			get { return (double)GetValue(MaskOpacityProperty); }
			set { SetValue(MaskOpacityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MaskOpacity.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaskOpacityProperty =
			DependencyProperty.Register("MaskOpacity", typeof(double), s_typeOfThis, new FrameworkPropertyMetadata(0.5));

		protected static void OnCommonCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is MessageContainer container)
			{
				container.m_commandSources.TrySetCommand(e.Property, (ICommand)e.NewValue);
			}
		}

		protected override async void OnPreviewMouseUp(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseUp(e);
			if (m_closeOnClickAway)
			{
				var pt = e.GetPosition(this);
				if (m_contentPresenter != null)
				{
					var result = VisualTreeHelper.HitTest(m_contentPresenter, pt)?.VisualHit;
					if (result is null)
					{
						SetCurrentValue(MessageResultProperty, null);
						await TryCloseSelf();
					}
				}
			}
		}
	}
}
