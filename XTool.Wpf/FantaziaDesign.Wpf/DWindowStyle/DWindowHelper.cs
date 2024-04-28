using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using FantaziaDesign.Wpf.Windows;
using FantaziaDesign.Interop;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public enum DWindowCaptionRole : byte
	{
		None = 0,
		Minimize = 1,
		Restore = 2,
		Maximize = 4,
		Close = 8,
		Caption = 16,
		Client = 32,
	}

	[Flags]
	public enum DWindowCaptionFlags : byte
	{
		None = 0,
		Minimize = 1,
		Restore = 2,
		Maximize = 4,
		Close = 8,
		AllButtons = Minimize | Restore | Maximize | Close,
		Caption = 16,
		Client = 32,
	}

	public enum DWindowBorderStyle
	{
		Default,
		None,
		Line,
		Frame
	}

	public static class DpiHelper
	{
		public static void ScalePoint(this DpiScale dpiScale, ref double x, ref double y, int scaleMode = 1)
		{
			var rX = dpiScale.DpiScaleX;
			var rY = dpiScale.DpiScaleY;
			if (scaleMode < 0)
			{
				rX = 1 / rX;
				rY = 1 / rY;
			}
			x *= rX;
			y *= rY;
		}

		public static void LogicalPixelsToDevice(DpiScale dpiScale, ref double pX, ref double pY)
		{
			dpiScale.ScalePoint(ref pX, ref pY);
		}

		public static void LogicalPixelsToDevice(DpiScale dpiScale, ref Point point)
		{
			var pX = point.X;
			var pY = point.Y;
			dpiScale.ScalePoint(ref pX, ref pY);
			point.X = pX;
			point.Y = pY;
		}

		public static void DevicePixelsToLogical(DpiScale dpiScale, ref double pX, ref double pY)
		{
			dpiScale.ScalePoint(ref pX, ref pY, -1);
		}

		public static void DevicePixelsToLogical(DpiScale dpiScale, ref Point point)
		{
			var pX = point.X;
			var pY = point.Y;
			dpiScale.ScalePoint(ref pX, ref pY, -1);
			point.X = pX;
			point.Y = pY;
		}

		public static Rect LogicalRectToDevice(DpiScale dpiScale, Rect rectangle)
		{
			var p1 = rectangle.TopLeft;
			var p2 = rectangle.BottomRight;
			DpiHelper.LogicalPixelsToDevice(dpiScale, ref p1);
			DpiHelper.LogicalPixelsToDevice(dpiScale, ref p2);
			return new Rect(p1, p2);
		}

		public static Rect DeviceRectToLogical(DpiScale dpiScale, Rect rectangle)
		{
			var p1 = rectangle.TopLeft;
			var p2 = rectangle.BottomRight;
			DpiHelper.DevicePixelsToLogical(dpiScale, ref p1);
			DpiHelper.DevicePixelsToLogical(dpiScale, ref p2);
			return new Rect(p1, p2);
		}

		public static void LogicalSizeToDevice(DpiScale dpiScale, ref Size size)
		{
			var w = size.Width;
			var h = size.Height;
			dpiScale.ScalePoint(ref w, ref h);
			size.Width = w;
			size.Height = h;
		}

		public static void DeviceSizeToLogical(DpiScale dpiScale, ref Size size)
		{
			var w = size.Width;
			var h = size.Height;
			dpiScale.ScalePoint(ref w, ref h, -1);
			size.Width = w;
			size.Height = h;
		}
	}

	public static class DWindowStyleHelper
	{
		#region WindowStyleHookHelper

		internal static SolidColorBrush InactivedColorBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));

		internal static SolidColorBrush WindowBorderColorBrushForLightTheme = Brushes.Gray;

		internal static SolidColorBrush WindowBorderColorBrushForSysLightTheme = Brushes.Transparent;

		internal static SolidColorBrush WindowBorderColorBrushForDarkTheme = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));

		private static string minimize = "M 0,4 L 10,4 10,5 0,5 0,4 z";
		private static string restore = "M 1,3 L 1,9 7,9 7,3 1,3z M 3,1 L3,2 8,2 8,7 9,7 9,1 3,1z M 2,0 L 10,0 10,8 8,8 8,10 0,10 0,2 2,2 2,0z";
		private static string maximize = "M 1,1 L 1,9 9,9 9,1 1,1z M 0,0 L 10,0 10,10 0,10 0,0z";
		private static string close = "M10,0 L9.3,0 5,4.3 0.8,0 0,0 0,0.8 4.3,5 0,9.3 0,10 0.8,10 5,5.8 9.3,10 10,10 10,9.3 5.8,5 10,0.8z";

		public static Geometry DefaultMinimizeGeometry { get; set; } = Geometry.Parse(minimize);

		public static Geometry DefaultRestoreGeometry { get; set; } = Geometry.Parse(restore);

		public static Geometry DefaultMaximizeGeometry { get; set; } = Geometry.Parse(maximize);

		public static Geometry DefaultCloseGeometry { get; set; } = Geometry.Parse(close);

		#region UseDesignedWindowSize
		public static bool GetUseDesignedWindowSize(DependencyObject obj)
		{
			return (bool)obj.GetValue(UseDesignedWindowSizeProperty);
		}

		public static void SetUseDesignedWindowSize(DependencyObject obj, bool value)
		{
			obj.SetValue(UseDesignedWindowSizeProperty, value);
		}

		// Using a DependencyProperty as the backing store for UseDesignedWindowSize.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UseDesignedWindowSizeProperty =
			DependencyProperty.RegisterAttached("UseDesignedWindowSize", typeof(bool), typeof(DWindowStyleHelper), new PropertyMetadata(false));
		#endregion

		public static bool GetUseDWindowStyleHooker(DependencyObject obj)
		{
			return (bool)obj.GetValue(UseDWindowStyleHookerProperty);
		}

		public static void SetUseDWindowStyleHooker(DependencyObject obj, bool value)
		{
			obj.SetValue(UseDWindowStyleHookerProperty, value);
		}

		// Using a DependencyProperty as the backing store for UseDWindowStyleHooker.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UseDWindowStyleHookerProperty =
			DependencyProperty.RegisterAttached("UseDWindowStyleHooker", typeof(bool), typeof(DWindowStyleHelper), new PropertyMetadata(false, new PropertyChangedCallback(OnUseDWindowStyleHookerChanged)));

		private static void OnUseDWindowStyleHookerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			bool hook = Convert.ToBoolean(e.NewValue);
			if (hook && d is Window window && window.GetCriticalHandle() == IntPtr.Zero)
			{
				if (!DWindowProcedure.Contains(window))
				{
					window.SourceInitialized += delegate
					{
						RegisterHandle(window);
						InitializeSettings(window, true);
					};
				}
			}
		}

		public static DWindowBorderStyle GetDWindowBorderStyle(DependencyObject obj)
		{
			return (DWindowBorderStyle)obj.GetValue(DWindowBorderStyleProperty);
		}

		public static void SetDWindowBorderStyle(DependencyObject obj, DWindowBorderStyle value)
		{
			obj.SetValue(DWindowBorderStyleProperty, value);
		}

		// Using a DependencyProperty as the backing store for DWindowBorderStyle.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DWindowBorderStyleProperty =
			DependencyProperty.RegisterAttached("DWindowBorderStyle", typeof(DWindowBorderStyle), typeof(DWindowStyleHelper), new PropertyMetadata(DWindowBorderStyle.Default));

		public static Brush GetWindowBorderBrush(DependencyObject obj)
		{
			return (Brush)obj.GetValue(WindowBorderBrushProperty);
		}

		internal static void SetWindowBorderBrush(DependencyObject obj, Brush value)
		{
			obj.SetValue(WindowBorderBrushPropertyKey, value);
		}

		// Using a DependencyProperty as the backing store for WindowBorderBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyPropertyKey WindowBorderBrushPropertyKey =
			DependencyProperty.RegisterAttachedReadOnly("WindowBorderBrush", typeof(Brush), typeof(DWindowStyleHelper), new PropertyMetadata(Brushes.Transparent));

		public static readonly DependencyProperty WindowBorderBrushProperty = WindowBorderBrushPropertyKey.DependencyProperty;

		public static Brush GetWindowColorizationBrush(DependencyObject obj)
		{
			return (Brush)obj.GetValue(WindowColorizationBrushProperty);
		}

		internal static void SetWindowColorizationBrush(DependencyObject obj, Brush value)
		{
			obj.SetValue(WindowColorizationBrushPropertyKey, value);
		}

		// Using a DependencyProperty as the backing store for WindowColorizationBrush.  This enables animation, styling, binding, etc...
		public static readonly DependencyPropertyKey WindowColorizationBrushPropertyKey =
			DependencyProperty.RegisterAttachedReadOnly("WindowColorizationBrush", typeof(Brush), typeof(DWindowStyleHelper), new PropertyMetadata(Brushes.Transparent));

		public static readonly DependencyProperty WindowColorizationBrushProperty = WindowColorizationBrushPropertyKey.DependencyProperty;

		public static DWindowCaptionFlags GetWindowButtonHitTestVisible(DependencyObject obj)
		{
			return (DWindowCaptionFlags)obj.GetValue(WindowButtonHitTestVisibleProperty);
		}

		public static void SetWindowButtonHitTestVisible(DependencyObject obj, DWindowCaptionFlags value)
		{
			obj.SetValue(WindowButtonHitTestVisibleProperty, value);

			if (obj is DWindow dWindow)
			{
				dWindow.SetWindowButtonHitTestVisible(value);
			}
		}

		// Using a DependencyProperty as the backing store for WindowButtonHitTestVisible.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WindowButtonHitTestVisibleProperty =
			DependencyProperty.RegisterAttached("WindowButtonHitTestVisible", typeof(DWindowCaptionFlags), typeof(DWindowStyleHelper), new PropertyMetadata(DWindowCaptionFlags.AllButtons));

		public static bool GetEnableUserDefineWindowTitle(DependencyObject obj)
		{
			return (bool)obj.GetValue(EnableUserDefineWindowTitleProperty);
		}

		public static void SetEnableUserDefineWindowTitle(DependencyObject obj, bool value)
		{
			obj.SetValue(EnableUserDefineWindowTitleProperty, value);
		}

		// Using a DependencyProperty as the backing store for EnableUserDefineWindowTitle.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EnableUserDefineWindowTitleProperty =
			DependencyProperty.RegisterAttached("EnableUserDefineWindowTitle", typeof(bool), typeof(DWindowStyleHelper), new PropertyMetadata(false));

		internal static void InitializeSettings(Window window, bool onSourceInitialized = false)
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
				SetWindowColorizationBrush(window, windowColorizationBrush);
			}
			// window 10 use normal border style
			else
			{
				SystemThemeInfo.RefreshThemeColorization();
				if (SystemThemeInfo.CanUseThemeColorizationBrush)
				{
					windowColorizationBrush = GetThemeColorizationBrush();
				}
				else
				{
					windowColorizationBrush = SystemParameters.WindowGlassBrush;
				}
				SetWindowColorizationBrush(window, windowColorizationBrush);
				borderStyle = DWindowBorderStyle.Line;
			}
			SetDWindowBorderStyle(window, borderStyle);
			SetWindowBorderBrush(window, windowColorizationBrush);
			if (DWindowNonClientManager.TryFind(window, out var nonClient))
			{
				nonClient.SetBorderStyle((int)borderStyle);
				if (onSourceInitialized && GetUseDesignedWindowSize(window))
				{
					nonClient.SetPreferedInitialWindowRect(IntPtr.Zero, (int)window.WindowStartupLocation);
				}
			}
		}

		public static void PostSystemCommand(Window window, User32.SC sysCmd)
		{
			if (window is null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			IntPtr hWnd = window.GetCriticalHandle();
			if (hWnd == IntPtr.Zero)
			{
				hWnd = new WindowInteropHelper(window).EnsureHandle();
			}
			User32.PostMessage(hWnd, (int)User32.WM.SYSCOMMAND, new IntPtr((int)sysCmd), IntPtr.Zero);
		}

		public static void PostSystemCommand(Window window, User32.SC sysCmd, ref IntPtr lParam)
		{
			if (window is null)
			{
				throw new ArgumentNullException(nameof(window));
			}
			IntPtr hWnd = window.GetCriticalHandle();
			if (hWnd == IntPtr.Zero)
			{
				hWnd = new WindowInteropHelper(window).EnsureHandle();
			}
			User32.PostMessage(hWnd, (int)User32.WM.SYSCOMMAND, new IntPtr((int)sysCmd), lParam);
		}

		internal static Brush GetThemeColorizationBrush()
		{
			if (SystemThemeInfo.IsSysUsingLightTheme)
			{
				return WindowBorderColorBrushForSysLightTheme;
			}
			if (SystemThemeInfo.IsAppUsingLightTheme)
			{
				return WindowBorderColorBrushForLightTheme;
			}
			return WindowBorderColorBrushForDarkTheme;
		}

		public static void UpdateWindowColorizationBrush(Window window)
		{
			if (window is DWindow dWindow)
			{
				dWindow.InvokeWin32MsgDWMColorizationColorChanged();
				return;
			}

			Brush windowColorizationBrush = Brushes.Transparent;
			if (GetDWindowBorderStyle(window) == DWindowBorderStyle.Line)
			{
				if (SystemThemeInfo.CanUseThemeColorizationBrush)
				{
					windowColorizationBrush = GetThemeColorizationBrush();
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
			SetWindowColorizationBrush(window, windowColorizationBrush);
			if (window.IsActive)
			{
				SetWindowBorderBrush(window, windowColorizationBrush);
			}
			else
			{
				SetWindowBorderBrush(window, InactivedColorBrush);
			}
		}

		public static void RegisterHandle(Window window)
		{
			var procedure = DWindowProcedure.FromWindow(window);
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
					window.Closed += delegate
					{
						UnregisterHandle(handle);
					};
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}

		public static void UnregisterHandle(IntPtr handle)
		{
			if (DWindowProcedure.Remove(handle))
			{
				DWindowNonClientManager.StopManaging(handle);
				var source = HwndSource.FromHwnd(handle);
				source.RemoveHook(DWindowProcedure.WndProc);
			}
		}




		#endregion

	}

}
