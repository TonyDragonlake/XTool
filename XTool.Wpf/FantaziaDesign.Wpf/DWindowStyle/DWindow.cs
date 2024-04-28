using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using FantaziaDesign.Core;
using FantaziaDesign.Wpf.Controls;
using FantaziaDesign.Wpf.Windows;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	[TemplatePart(Name = PART_MinimizeButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_RestoreButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_MaximizeButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
	public class DWindow : Window
	{
		internal const string PART_MinimizeButton = nameof(PART_MinimizeButton);
		internal const string PART_RestoreButton = nameof(PART_RestoreButton);
		internal const string PART_MaximizeButton = nameof(PART_MaximizeButton);
		internal const string PART_CloseButton = nameof(PART_CloseButton);

		static DWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DWindow), new FrameworkPropertyMetadata(typeof(DWindow)));
		}

		private byte m_windowButtonStatus;
		private DWindowProcedure m_procedure;
		private TemplatePartDictionary<DWindow> m_templateParts;
		private Brush m_colorizationBrush = Brushes.Transparent;

		public DWindow() : base()
		{
			m_templateParts = new TemplatePartDictionary<DWindow>(this);
			m_windowButtonStatus = (byte)DWindowCaptionFlags.AllButtons;
		}

		[Browsable(false)]
		public DWindowBorderStyle DWindowBorderStyle
		{
			get { return (DWindowBorderStyle)GetValue(DWindowBorderStyleProperty); }
			set { SetValue(DWindowBorderStyleProperty, value); }
		}
		[Browsable(false)]
		// Using a DependencyProperty as the backing store for WindowBorderStyle.  This enables animation, styling, binding, etc...
		public static DependencyProperty DWindowBorderStyleProperty =
			DWindowStyleHelper.DWindowBorderStyleProperty.AddOwner(typeof(DWindow));

		public Brush DWindowColorizationBrush
		{
			get
			{
				return m_colorizationBrush;
			}
			internal set
			{
				if (m_colorizationBrush != value)
				{
					m_colorizationBrush = value;
				}
			}
		}

		public WindowInteropHelper InteropHelper
		{
			get
			{
				var result = m_procedure?.InteropHelper;
				return result is null ? new WindowInteropHelper(this) : result;
			}
		}

		public IntPtr NativeHandle => (m_procedure?.WindowHandle).GetValueOrDefault();

		public Brush DWindowBorderBrush
		{
			get { return (Brush)GetValue(DWindowBorderBrushPropertyKey.DependencyProperty); }
			internal set { SetValue(DWindowBorderBrushPropertyKey, value); }
		}

		// Using a DependencyProperty as the backing store for DWindowBorderBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyPropertyKey DWindowBorderBrushPropertyKey =
			DependencyProperty.RegisterReadOnly("DWindowBorderBrush", typeof(Brush), typeof(DWindow), new PropertyMetadata(Brushes.Transparent));

		public bool IsMinimizeActionEnabled
		{
			get { return (bool)GetValue(IsMinimizeActionEnabledProperty); }
			set { SetValue(IsMinimizeActionEnabledProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsMinimizeActionEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsMinimizeActionEnabledProperty =
			DependencyProperty.Register("IsMinimizeActionEnabled", typeof(bool), typeof(DWindow), new PropertyMetadata(true));

		public bool IsZoomingActionEnabled
		{
			get { return (bool)GetValue(IsZoomingActionEnabledProperty); }
			set { SetValue(IsZoomingActionEnabledProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsZoomingActionEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsZoomingActionEnabledProperty =
			DependencyProperty.Register("IsZoomingActionEnabled", typeof(bool), typeof(DWindow), new PropertyMetadata(true));

		public bool UseDesignedWindowSize
		{
			get { return (bool)GetValue(UseDesignedWindowSizeProperty); }
			set { SetValue(UseDesignedWindowSizeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for UseDesignedWindowSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UseDesignedWindowSizeProperty =
			DWindowStyleHelper.UseDesignedWindowSizeProperty.AddOwner(typeof(DWindow));

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			m_templateParts.InitializeTemplateParts();
			if (m_templateParts.TryGetValue<Button>(PART_MinimizeButton, out var minimizeButton))
			{
				minimizeButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Minimize);
			}
			if (m_templateParts.TryGetValue<Button>(PART_RestoreButton, out var restoreButton))
			{
				restoreButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Restore);
			}
			if (m_templateParts.TryGetValue<Button>(PART_MaximizeButton, out var maximizeButton))
			{
				maximizeButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Maximize);
			}
			if (m_templateParts.TryGetValue<Button>(PART_CloseButton, out var closeButton))
			{
				closeButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Close);
			}
		}

		internal static void InitializeSettings(DWindow window, bool onSourceInitialized = false)
		{
			SystemThemeInfo.RefreshDwmCompositionSettings();
			Brush windowColorizationBrush = Brushes.Transparent;
			var borderStyle = DWindowBorderStyle.None;
			if (ApplicationEnvironment.OSVersion.Major <= 6 || window.WindowStyle == WindowStyle.None)
			{
				if (window.WindowStyle != WindowStyle.None)
				{
					window.WindowStyle = WindowStyle.None;
				}
				if (!SystemThemeInfo.IsDwmCompositionEnabled)
				{
					windowColorizationBrush = Brushes.LightGray;
					borderStyle = DWindowBorderStyle.Frame;
				}
				window.DWindowColorizationBrush = windowColorizationBrush;
			}
			// window 10 use normal border style
			else
			{
				SystemThemeInfo.RefreshThemeColorization();
				if (SystemThemeInfo.CanUseThemeColorizationBrush)
				{
					windowColorizationBrush = DWindowStyleHelper.GetThemeColorizationBrush();
				}
				else
				{
					windowColorizationBrush = SystemParameters.WindowGlassBrush;
				}
				window.DWindowColorizationBrush = windowColorizationBrush;
				borderStyle = DWindowBorderStyle.Line;
			}
			window.DWindowBorderStyle = borderStyle;
			window.DWindowBorderBrush = window.m_colorizationBrush;
			if (DWindowNonClientManager.TryFind(window, out var nonClient))
			{
				nonClient.SetBorderStyle((int)borderStyle);
				if (onSourceInitialized && window.UseDesignedWindowSize)
				{
					nonClient.SetPreferedInitialWindowRect(IntPtr.Zero, (int)window.WindowStartupLocation);
				}
			}
		}

		private static void RegisterHandle(DWindow window)
		{
			var procedure = DWindowProcedure.FromWindow(window);
			window.m_procedure = procedure;
			var handle = procedure.WindowHandle;
			var source = HwndSource.FromHwnd(handle);
			try
			{
				if (source != null)
				{
					source.AddHook(DWindowProcedure.WndProc);
					source.CompositionTarget.BackgroundColor = Colors.Transparent;
					DWindowNonClientManager.StartManaging(handle);
					DWindowMessageHandlers.InitializeDefaultMessageHandlers(procedure);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private static void UnregisterHandle(IntPtr handle)
		{
			if (DWindowProcedure.Remove(handle))
			{
				DWindowNonClientManager.StopManaging(handle);
				var source = HwndSource.FromHwnd(handle);
				source.RemoveHook(DWindowProcedure.WndProc);
			}
		}

		private static void UpdateWindowColorizationBrush(DWindow window)
		{
			Brush windowColorizationBrush = Brushes.Transparent;
			if (window.DWindowBorderStyle == DWindowBorderStyle.Line)
			{
				if (SystemThemeInfo.CanUseThemeColorizationBrush)
				{
					windowColorizationBrush = DWindowStyleHelper.GetThemeColorizationBrush();
				}
				else
				{
					windowColorizationBrush = SystemParameters.WindowGlassBrush;
				}
			}
			else if (!SystemThemeInfo.IsDwmCompositionEnabled)
			{
				windowColorizationBrush = SystemParameters.WindowGlassBrush;
			}
			else
			{
				return;
			}
			window.DWindowColorizationBrush = windowColorizationBrush;
			window.DWindowBorderBrush = window.IsActive ? windowColorizationBrush : DWindowStyleHelper.InactivedColorBrush;
		}

		internal void InvokeWin32MsgNCActivate(bool isNCActived)
		{
			DWindowBorderBrush = isNCActived ? m_colorizationBrush : DWindowStyleHelper.InactivedColorBrush;
		}

		internal void InvokeWin32MsgDWMColorizationColorChanged()
		{
			UpdateWindowColorizationBrush(this);
		}

		internal void InvokeWin32MsgDWMCompositionChanged()
		{
			InitializeSettings(this);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			RegisterHandle(this);
			InitializeSettings(this, true);
			base.OnSourceInitialized(e);
		}

		protected override void OnClosed(EventArgs e)
		{
			UnregisterHandle(NativeHandle);
			base.OnClosed(e);
		}

		internal void SetWindowButtonHitTestVisible(DWindowCaptionFlags value)
		{
			m_windowButtonStatus = (byte)value;

			TryApplyDWindowButtonFlags();
		}

		private void TryApplyDWindowButtonFlags()
		{
			if (m_templateParts.IsTemplatePartsInitialized)
			{
				if (m_templateParts.TryGetValue<Button>(PART_MinimizeButton, out var minimizeButton))
				{
					minimizeButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Minimize);
				}
				if (m_templateParts.TryGetValue<Button>(PART_RestoreButton, out var restoreButton))
				{
					restoreButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Restore);
				}
				if (m_templateParts.TryGetValue<Button>(PART_MaximizeButton, out var maximizeButton))
				{
					maximizeButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Maximize);
				}
				if (m_templateParts.TryGetValue<Button>(PART_CloseButton, out var closeButton))
				{
					closeButton.IsEnabled = BitUtil.GetBitStatus(ref m_windowButtonStatus, (byte)DWindowCaptionFlags.Close);
				}
			}
		}
	}
}
