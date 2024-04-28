using System;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using FantaziaDesign.Core;
using FantaziaDesign.Wpf.Input;
using FantaziaDesign.Interop;
using System.Text.RegularExpressions;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public static class DWindowMessageHandlers
	{
		public static IntPtr HandleWin32MsgNCCalcSize(DWindowProcedureEventArgs eventArgs)
		{
			eventArgs.Handled = true;
			var window = eventArgs.Window;
			var hwnd = eventArgs.WindowHandle;
			var wParam = eventArgs.WParam;
			var lParam = eventArgs.LParam;
			if (DWindowNonClientManager.TryFind(window, out var nonClient))
			{
				return nonClient.InvokeCalculateSizeHandler(hwnd, wParam, lParam);
			}
			return User32.DefWindowProc(hwnd,eventArgs.Message, wParam, lParam);
		}

		public static IntPtr HandleWin32MsgNCActivate(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			var wParam = eventArgs.WParam;
			bool isNCActived = Win32Native.LOWORD(wParam) != 0;
			if (window is DWindow dWindow)
			{
				dWindow.InvokeWin32MsgNCActivate(isNCActived);
			}
			else
			{
				if (isNCActived)
				{
					DWindowStyleHelper.SetWindowBorderBrush(window, DWindowStyleHelper.GetWindowColorizationBrush(window));
				}
				else
				{
					DWindowStyleHelper.SetWindowBorderBrush(window, DWindowStyleHelper.InactivedColorBrush);
				}
			}

			eventArgs.Handled = true;
			return User32.DefWindowProc(eventArgs.WindowHandle, eventArgs.Message, wParam, new IntPtr(-1));
		}

		public static IntPtr HandleWin32MsgDWMColorizationColorChanged(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			DWindowStyleHelper.UpdateWindowColorizationBrush(window);
			return IntPtr.Zero;
		}

		public static IntPtr HandleWin32MsgNCUAHDrawFrameAndDrawCaption(DWindowProcedureEventArgs eventArgs)
		{
			eventArgs.Handled = true;
			return IntPtr.Zero;
		}

		public static IntPtr HandleWin32MsgNCLButtonUp(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			var hWnd = eventArgs.WindowHandle;
			var wParam = eventArgs.WParam;
			var lParam = eventArgs.LParam;
			var msg = eventArgs.Message;

			IntPtr res = IntPtr.Zero;
			if (!Dwmapi.DwmDefWindowProc(hWnd, msg, wParam, lParam, ref res))
			{
				var hitTestRes = (User32.HT)wParam.ToInt32();
				switch (hitTestRes)
				{
					case User32.HT.SIZE:
					case User32.HT.MINBUTTON:
					case User32.HT.MAXBUTTON:
					case User32.HT.CLOSE:
						{
							Win32Native.TranslateLParamPointToClient(hWnd, ref lParam);
							User32.PostMessage(hWnd, (int)User32.WM.LBUTTONUP, new IntPtr(0x0001), lParam);
							eventArgs.Handled = true;
						}
						break;
					default:
						break;
				}
				return IntPtr.Zero;
			}
			else
			{
				eventArgs.Handled = true;
				return res;
			}
		}

		public static IntPtr HandleWin32MsgNCLButtonDown(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			var hWnd = eventArgs.WindowHandle;
			var wParam = eventArgs.WParam;
			var lParam = eventArgs.LParam;
			var msg = eventArgs.Message;
			IntPtr res = IntPtr.Zero;
			if (!Dwmapi.DwmDefWindowProc(hWnd, msg, wParam, lParam, ref res))
			{
				var hitTestRes = (User32.HT)wParam.ToInt32();
				switch (hitTestRes)
				{
					case User32.HT.SIZE:
					case User32.HT.MINBUTTON:
					case User32.HT.MAXBUTTON:
					case User32.HT.CLOSE:
						{
							Win32Native.TranslateLParamPointToClient(hWnd, ref lParam);
							User32.PostMessage(hWnd, (int)User32.WM.LBUTTONDOWN, new IntPtr(0x0001), lParam);
							eventArgs.Handled = true;
						}
						break;
					default:
						break;
				}
				return IntPtr.Zero;
			}
			else
			{
				eventArgs.Handled = true;
				return res;
			}
		}

		//public static IntPtr HandleWin32MsgMouseLeave(DWindowProcedureEventArgs eventArgs)
		//{
		//	System.Diagnostics.Debug.WriteLine("HandleWin32MsgMouseLeave");
		//	var hWnd = eventArgs.WindowHandle;
		//	var wParam = eventArgs.WParam;
		//	var lParam = eventArgs.LParam;
		//	var msg = eventArgs.Message;
		//	return User32.DefWindowProc(hWnd, msg, wParam, lParam);
		//}

		//public static IntPtr HandleWin32MsgNCMouseLeave(DWindowProcedureEventArgs eventArgs)
		//{
		//	System.Diagnostics.Debug.WriteLine("HandleWin32MsgNCMouseLeave");
		//	var hWnd = eventArgs.WindowHandle;
		//	var wParam = eventArgs.WParam;
		//	var lParam = eventArgs.LParam;
		//	var msg = eventArgs.Message;
		//	return User32.DefWindowProc(hWnd, msg, wParam, lParam);
		//}

		public static IntPtr HandleWin32MsgNCMouseMove(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			var hWnd = eventArgs.WindowHandle;
			var wParam = eventArgs.WParam;
			var lParam = eventArgs.LParam;
			var msg = eventArgs.Message;
			IntPtr res = IntPtr.Zero;
			//var captured = User32.GetCapture();
			//bool isCaptured = captured == hWnd;
			//System.Diagnostics.Debug.WriteLine($"Capture : {isCaptured} | captured = {captured}, hwnd = {hWnd}");
			//System.Diagnostics.Debug.Indent();
			if (!Dwmapi.DwmDefWindowProc(hWnd, msg, wParam, lParam, ref res))
			{

				//var hitTestRes = (User32.HT)wParam.ToInt32();
				//System.Diagnostics.Debug.WriteLine($"Custom DefWindowProc : HT = {hitTestRes}");

				//switch (hitTestRes)
				//{
				//	case User32.HT.SIZE:
				//	case User32.HT.MINBUTTON:
				//	case User32.HT.MAXBUTTON:
				//	case User32.HT.CLOSE:
				//		{
				//			Win32Native.TranslateLParamPointToClient(hWnd, ref lParam);
				//			User32.PostMessage(hWnd, (int)User32.WM.MOUSEMOVE, new IntPtr(0x0001), lParam);
				//			eventArgs.Handled = true;
				//		}
				//		break;
				//	default:
				//		break;
				//}
				//return IntPtr.Zero;


				var hitTestRes = (User32.HT)wParam.ToInt32();
				Win32Native.TranslateLParamPointToClient(hWnd, lParam, out int x, out int y);
				switch (hitTestRes)
				{
					case User32.HT.SIZE:
					case User32.HT.MINBUTTON:
					case User32.HT.MAXBUTTON:
					case User32.HT.CLOSE:
						{
							InputHelper.SimulateRawMouseInput(hWnd, x, y, WpfRawMouseActions.AbsoluteMove | WpfRawMouseActions.Activate, 0);
						}
						break;
					default:
						InputHelper.SimulateRawMouseInput(hWnd, x, y, WpfRawMouseActions.AbsoluteMove | WpfRawMouseActions.Deactivate, 0);
						break;
				}
				return IntPtr.Zero;
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("DwmDefWindowProc");
				//System.Diagnostics.Debug.Unindent();

				eventArgs.Handled = true;
				return res;
			}
		}

		public static IntPtr HandleWin32MsgDWMCompositionChanged(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			if (window is DWindow dWindow)
			{
				dWindow.InvokeWin32MsgDWMCompositionChanged();
			}
			else
			{
				DWindowStyleHelper.InitializeSettings(window);
			}
			eventArgs.Handled = false;
			return IntPtr.Zero;
		}

		public static IntPtr HandleWin32MsgGetMinMaxInfo(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			var lParam = eventArgs.LParam;
			var hWnd = eventArgs.WindowHandle;
			if (window != null && window.WindowStyle == WindowStyle.None)
			{
				var monitor = User32.MonitorFromWindow(hWnd, (int)User32.MONITOR_DEFAULTTO.NEAREST);
				if (monitor != IntPtr.Zero)
				{
					var mInfo = new User32.MONITORINFOEX();
					User32.MONITORINFOEX.Init(ref mInfo);
					if (User32.GetMonitorInfo(monitor, ref mInfo))
					{
						var marshaller = User32.GetMarshaller<User32.MINMAXINFO>();
						var mmInfo = marshaller.MarshalFromPointer(lParam);
						var ml = mInfo.rcMonitor[0];
						var mt = mInfo.rcMonitor[1];
						var wl = mInfo.rcWork[0];
						var wt = mInfo.rcWork[1];
						var wr = mInfo.rcWork[2];
						var wb = mInfo.rcWork[3];
						mmInfo.ptMaxPosition[0] = wl - ml;
						mmInfo.ptMaxPosition[1] = wt - mt;
						mmInfo.ptMaxSize[0] = wr - wl;
						mmInfo.ptMaxSize[1] = wb - wt;
						marshaller.TryMarshalToPointer(mmInfo, ref lParam);
					}
				}
				eventArgs.Handled = false;
			}
			return IntPtr.Zero;
		}

		public static IntPtr HandleWin32MsgNCHitTest(DWindowProcedureEventArgs eventArgs)
		{
			var window = eventArgs.Window;
			var lParam = eventArgs.LParam;
			var hWnd = eventArgs.WindowHandle;
			bool handled = false;
			if (DWindowNonClientManager.TryFind(window, out var nonClient))
			{
				Win32Native.UnzipToWords(lParam, out var pt);
				if (!nonClient.IsInWindowRect(pt))
				{
					return IntPtr.Zero;
				}
				var hitRes = User32.HT.CLIENT;
				if (!User32.IsZoomed(hWnd))
				{
					if (nonClient.TryHandleSizeBoxHitTest(pt, out hitRes))
					{
						eventArgs.Handled = true;
						return new IntPtr((int)hitRes);
					}
				}
				if (nonClient.IsInCaptionRegion(pt))
				{
					hitRes = CaptionUIHitTest(hWnd, window, pt, ref handled);
					eventArgs.Handled = handled;
					return new IntPtr((int)hitRes);
				}
			}
			return IntPtr.Zero;
		}

		public static IntPtr HandleWin32MsgStyleChanged(DWindowProcedureEventArgs eventArgs)
		{
			var wParam = eventArgs.WParam;
			var lParam = eventArgs.LParam;
			var flag = (User32.GWL)wParam.ToInt32();
			var styles = Marshal.PtrToStructure<Vec2ui>(lParam);
			if (styles[0] != styles[1])
			{
				if (flag == User32.GWL.STYLE)
				{
					var oldStyle = (User32.WS)styles[0];
					var newStyle = (User32.WS)styles[1];
					// System.Diagnostics.Debug.WriteLine($"\t{nameof(oldStyle)} = {oldStyle}\n\t{nameof(newStyle)} = {newStyle}");
					var addedStyle = newStyle &(~oldStyle);
					var removedStyle = oldStyle & (~newStyle);
					if ((addedStyle & User32.WS.CAPTION) == User32.WS.CAPTION || (removedStyle & User32.WS.CAPTION) == User32.WS.CAPTION)
					{
						var window = eventArgs.Window;
						if (window is DWindow dWindow)
						{
							dWindow.InvokeWin32MsgDWMCompositionChanged();
						}
						else
						{
							DWindowStyleHelper.InitializeSettings(window);
						}
					}
					// System.Diagnostics.Debug.WriteLine($"\t{nameof(addedStyle)} = {addedStyle}\n\t{nameof(removedStyle)} = {removedStyle}");
				}
				//else if (flag == User32.GWL.EXSTYLE)
				//{
				//	var oldStyle = (User32.WS_EX)styles[0];
				//	var newStyle = (User32.WS_EX)styles[1];
				//	System.Diagnostics.Debug.WriteLine($"\t{nameof(oldStyle)} = {oldStyle}\n\t{nameof(newStyle)} = {newStyle}");
				//	var addedStyle = newStyle & (~oldStyle);
				//	var removedStyle = oldStyle & (~newStyle);
				//	System.Diagnostics.Debug.WriteLine($"\t{nameof(addedStyle)} = {addedStyle}\n\t{nameof(removedStyle)} = {removedStyle}");
				//}
			}
			return User32.DefWindowProc(eventArgs.WindowHandle, eventArgs.Message, wParam, lParam);
		}

		private static IntPtr HandleWin32Size(DWindowProcedureEventArgs eventArgs)
		{
			var hWnd = eventArgs.WindowHandle;
			if (DWindowNonClientManager.TryFind(hWnd, out var nonClient))
			{
				nonClient.InvalidateWindowRectsParam();
			}
			return User32.DefWindowProc(eventArgs.WindowHandle, eventArgs.Message, eventArgs.WParam, eventArgs.LParam);
		}

		private static IntPtr HandleWin32Move(DWindowProcedureEventArgs eventArgs)
		{
			var hWnd = eventArgs.WindowHandle;
			if (DWindowNonClientManager.TryFind(hWnd, out var nonClient))
			{
				nonClient.InvalidateWindowRectsParam();
			}
			return User32.DefWindowProc(eventArgs.WindowHandle, eventArgs.Message, eventArgs.WParam, eventArgs.LParam);
		}

		private static User32.HT CaptionUIHitTest(IntPtr hWnd, Window wnd, Vec2i pt, ref bool handled)
		{
			User32.ScreenToClient(hWnd, ref pt);
			var dpi = VisualTreeHelper.GetDpi(wnd);
			var hitPoint = new Point(pt[0], pt[1]);
			DpiHelper.DevicePixelsToLogical(dpi, ref hitPoint);
			var role = DWindowCaptionRole.None;
			VisualTreeHelper.HitTest(wnd, null,
				new HitTestResultCallback(
					(result)
					=>
					{
						var d = result.VisualHit;
						if (d != null)
						{
							role = DWindowNonClientManager.GetCaptionRole(d);
							if (role == DWindowCaptionRole.None)
							{
								var templateParent = (d as FrameworkElement)?.TemplatedParent;
								if (templateParent != null)
								{
									role = DWindowNonClientManager.GetCaptionRole(templateParent);
								}
							}
							return HitTestResultBehavior.Stop;
						}
						return HitTestResultBehavior.Continue;
					}
					),
				new PointHitTestParameters(hitPoint));
			handled = true;
			var hitTest = User32.HT.CAPTION;
			switch (role)
			{
				case DWindowCaptionRole.Minimize: return User32.HT.MINBUTTON;
				case DWindowCaptionRole.Restore: return User32.HT.SIZE;
				case DWindowCaptionRole.Maximize: return User32.HT.ZOOM;
				case DWindowCaptionRole.Close: return User32.HT.CLOSE;
				case DWindowCaptionRole.Client: return User32.HT.CLIENT;
				default:
					break;
			}
			return hitTest;
		}

		public static void InitializeDefaultMessageHandlers(DWindowProcedure procedure)
		{
			if (procedure is null)
			{
				return;
			}
			procedure.RegisterMessageHandler(User32.WM.GETMINMAXINFO, HandleWin32MsgGetMinMaxInfo);
			procedure.RegisterMessageHandler(User32.WM.NCCALCSIZE, HandleWin32MsgNCCalcSize);
			procedure.RegisterMessageHandler(User32.WM.NCACTIVATE, HandleWin32MsgNCActivate);
			procedure.RegisterMessageHandler(User32.WM.NCHITTEST, HandleWin32MsgNCHitTest);
			procedure.RegisterMessageHandler(User32.WM.DWMCOLORIZATIONCOLORCHANGED, HandleWin32MsgDWMColorizationColorChanged);
			procedure.RegisterMessageHandler(User32.WM.DWMCOMPOSITIONCHANGED, HandleWin32MsgDWMCompositionChanged);
			procedure.RegisterMessageHandler(User32.WM.NCMOUSEMOVE, HandleWin32MsgNCMouseMove);
			procedure.RegisterMessageHandler(User32.WM.NCLBUTTONDOWN, HandleWin32MsgNCLButtonDown);
			procedure.RegisterMessageHandler(User32.WM.NCLBUTTONUP, HandleWin32MsgNCLButtonUp);
			procedure.RegisterMessageHandler(User32.WM.STYLECHANGED, HandleWin32MsgStyleChanged);
			procedure.RegisterMessageHandler(User32.WM.MOVE, HandleWin32Move);
			procedure.RegisterMessageHandler(User32.WM.SIZE, HandleWin32Size);
			//procedure.RegisterMessageHandler(User32.WM.NCMOUSELEAVE, HandleWin32MsgNCMouseLeave);

			//procedure.RegisterMessageHandler(User32.WM.MOUSELEAVE, HandleWin32MsgMouseLeave);

			procedure.RegisterMessageHandler(0x00AE, HandleWin32MsgNCUAHDrawFrameAndDrawCaption);
			procedure.RegisterMessageHandler(0x00AF, HandleWin32MsgNCUAHDrawFrameAndDrawCaption);
		}

	}

}
