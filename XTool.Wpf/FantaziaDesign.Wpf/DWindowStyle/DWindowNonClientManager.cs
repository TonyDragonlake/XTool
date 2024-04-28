using System;
using System.Windows;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FantaziaDesign.Wpf.Windows;
using FantaziaDesign.Core;
using FantaziaDesign.Interop;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public sealed class DWindowNonClientManager
	{
		internal sealed class __DefaultHitTestHandler : IDWindowNonClientHitTestHandler
		{
			public bool IsInCaptionRegion(Vec2i pt, Vec4i winRect, Vec4i client)
			{
				Rects.SetHeight(ref winRect, s_nonClientFrameThickness[1]);
				return Rects.ContainsPoint(winRect, pt);
			}

			public bool TryHandleSizeBoxHitTest(Vec2i pt, Vec4i winRect, Vec4i client, out User32.HT hitTestResult)
			{
				const int minThick = 4;
				int i = 0;
				while (i < 2)
				{
					if (client[i] - winRect[i] < minThick)
					{
						client[i] = winRect[i] + minThick;
					}
					i++;
				}
				while (i < 4)
				{
					if (client[i] - winRect[i] < minThick)
					{
						client[i] = winRect[i] - minThick;
					}
					i++;
				}

				var x = pt[0];
				var y = pt[1];

				var l = client[0];
				var t = client[1];
				var r = client[2];
				var b = client[3];

				var overLeft = x <= l;
				var overTop = y <= t;
				var overRight = r <= x;
				var overBottom = b <= y;
				if (overTop)
				{
					if (overLeft)
					{
						hitTestResult = User32.HT.TOPLEFT;
					}
					else if (overRight)
					{
						hitTestResult = User32.HT.TOPRIGHT;
					}
					else
					{
						hitTestResult = User32.HT.TOP;
					}
					return true;
				}
				if (overBottom)
				{
					if (overLeft)
					{
						hitTestResult = User32.HT.BOTTOMLEFT;
					}
					else if (overRight)
					{
						hitTestResult = User32.HT.BOTTOMRIGHT;
					}
					else
					{
						hitTestResult = User32.HT.BOTTOM;
					}
					return true;
				}
				if (overLeft)
				{
					hitTestResult = User32.HT.LEFT;
				}
				else if (overRight)
				{
					hitTestResult = User32.HT.RIGHT;
				}
				else
				{
					hitTestResult = User32.HT.CLIENT;
					return false;
				}
				return true;
			}
		}

		private static readonly Vec4i s_shadowMargin = new Vec4i(1, 1, 1, 1);

		private static readonly Vec4i s_flattenMargin = new Vec4i();

		private static readonly Vec4i s_borderFrameThickness = GetBorderFrameThickness();

		private static readonly Vec4i s_nonClientFrameThickness = GetNonClientFrameThickness();
		/*
		Available breakpoints
		Bootstrap includes six default breakpoints, sometimes referred to as grid tiers, for building responsively. 
		These breakpoints can be customized if you’re using our source Sass files.
		Breakpoint			Class infix		Dimensions
		X-Small				None			<576px
		Small				sm				≥576  px	432
		Medium				md				≥768  px	576
		Large				lg				≥992  px	744
		Extra large			xl				≥1200 px	900
		Extra extra large	xxl				≥1400 px	1050
		*/
		private static readonly int[] s_baseDim = new int[5] { 1050, 900, 744, 576, 432 };

		private static readonly IDWindowNonClientHitTestHandler s_defaultHitTestHandler = new __DefaultHitTestHandler();

		public static IDWindowNonClientHitTestHandler DefaultHitTestHandler => s_defaultHitTestHandler;

		private static Vec4i GetBorderFrameThickness()
		{
			var frameX = User32.GetSystemMetrics((int)User32.SM.CXFRAME);
			var frameY = User32.GetSystemMetrics((int)User32.SM.CYFRAME);
			if (ApplicationEnvironment.OSVersion.Major > 6)
			{
				var padded = User32.GetSystemMetrics((int)User32.SM.CXPADDEDBORDER);
				frameX += padded;
				frameY += padded;
			}
			return new Vec4i(frameX, frameY, frameX, frameY);
		}

		private static Vec4i GetNonClientFrameThickness()
		{
			var frameX = User32.GetSystemMetrics((int)User32.SM.CXFRAME);
			var frameY = User32.GetSystemMetrics((int)User32.SM.CYFRAME);
			var captionY = User32.GetSystemMetrics((int)User32.SM.CYCAPTION);
			if (ApplicationEnvironment.OSVersion.Major > 6)
			{
				var padded = User32.GetSystemMetrics((int)User32.SM.CXPADDEDBORDER);
				frameX += padded;
				frameY += padded;
				var border = User32.GetSystemMetrics((int)User32.SM.CYBORDER);
				captionY += border;
			}
			return new Vec4i(frameX, frameY + captionY, frameX, frameY);
		}

		public static Vec4i BorderFrameThickness => s_borderFrameThickness;
		public static Vec4i NonClientFrameThickness => s_nonClientFrameThickness;
		public static Vec4i ShadowMargin => s_shadowMargin;
		public static Vec4i FlattenMargin => s_flattenMargin;

		private static readonly Dictionary<IntPtr, DWindowNonClientManager> s_managers = new Dictionary<IntPtr, DWindowNonClientManager>();

		public static DWindowNonClientManager StartManaging(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
			{
				throw new InvalidOperationException("Cannot create NonClientManager for NULL HWND");
			}
			if (!s_managers.TryGetValue(hWnd, out var manager))
			{
				manager = new DWindowNonClientManager(hWnd);
				s_managers.Add(hWnd, manager);
			}
			return manager;
		}

		public static bool TryFind(Window window, out DWindowNonClientManager manager)
		{
			var hWnd = window.GetCriticalHandle();
			if (hWnd == IntPtr.Zero)
			{
				manager = null;
				return false;
			}
			return TryFind(hWnd, out manager);
		}

		public static bool TryFind(IntPtr hWnd, out DWindowNonClientManager manager)
		{
			if (hWnd == IntPtr.Zero)
			{
				manager = null;
				return false;
			}
			return s_managers.TryGetValue(hWnd, out manager);
		}

		public static bool StopManaging(IntPtr hWnd)
		{
			return s_managers.Remove(hWnd);
		}

		#region CaptionRole
		public static DWindowCaptionRole GetCaptionRole(DependencyObject obj)
		{
			return (DWindowCaptionRole)obj.GetValue(CaptionRoleProperty);
		}

		public static void SetCaptionRole(DependencyObject obj, DWindowCaptionRole value)
		{
			obj.SetValue(CaptionRoleProperty, value);
		}

		// Using a DependencyProperty as the backing store for CaptionRole.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionRoleProperty =
			DependencyProperty.RegisterAttached("CaptionRole", typeof(DWindowCaptionRole), typeof(DWindowNonClientManager), new FrameworkPropertyMetadata(DWindowCaptionRole.None, FrameworkPropertyMetadataOptions.Inherits));
		#endregion

		#region NonClientHitTestHandler
		public static IDWindowNonClientHitTestHandler GetNonClientHitTestHandler(DependencyObject obj)
		{
			return (IDWindowNonClientHitTestHandler)obj.GetValue(NonClientHitTestHandlerProperty);
		}

		public static void SetNonClientHitTestHandler(DependencyObject obj, IDWindowNonClientHitTestHandler value)
		{
			obj.SetValue(NonClientHitTestHandlerProperty, value);
		}

		// Using a DependencyProperty as the backing store for NonClientHitTestHandler.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty NonClientHitTestHandlerProperty =
			DependencyProperty.RegisterAttached("NonClientHitTestHandler", typeof(IDWindowNonClientHitTestHandler), typeof(DWindowNonClientManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnNonClientHitTestHandlerPropertyChanged)));

		private static void OnNonClientHitTestHandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = Window.GetWindow(d);
			if (window != null)
			{
				var hWnd = window.GetCriticalHandle();
				if (hWnd != IntPtr.Zero && s_managers.TryGetValue(hWnd, out var manager))
				{
					manager.NonClientHitTestHandler = e.NewValue as IDWindowNonClientHitTestHandler;
				}
			}
		}
		#endregion

		private IntPtr m_hWnd;
		private Vec4i m_windowRect;
		private Vec4i m_clientRect;
		private int m_borderStyle;
		private IDWindowNonClientHitTestHandler m_hitTestHandler;
		private bool m_isRectsValid;

		private DWindowNonClientManager(IntPtr handle)
		{
			m_hWnd = handle;
		}

		public IntPtr WindowHandle => m_hWnd;
		public Vec4i WindowRect { get { EnsureWindowRectsParam(); return m_windowRect; } }
		public Vec4i ClientRect { get { EnsureWindowRectsParam(); return m_clientRect; } }
		public bool UsingSystemFrame => m_borderStyle == 2;
		public IDWindowNonClientHitTestHandler NonClientHitTestHandler { get => m_hitTestHandler; set => m_hitTestHandler = value; }
		internal IDWindowNonClientHitTestHandler EnsuredHitTestHandler => m_hitTestHandler is null ? s_defaultHitTestHandler : m_hitTestHandler;

		public void SetBorderStyle(int borderStyle)
		{
			if (m_borderStyle != borderStyle)
			{
				m_borderStyle = borderStyle;
				Dwmapi.DwmExtendFrameIntoClientArea(m_hWnd, UsingSystemFrame ? FlattenMargin : ShadowMargin);
			}
		}

		public IntPtr InvokeCalculateSizeHandler(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
		{
			if (UsingSystemFrame)
			{
				Vec4i rect;
				var thickness = BorderFrameThickness;
				if (wParam.ToInt32() > 0)
				{
					var marshaller = User32.GetMarshaller<User32.NCCALCSIZE_PARAMS>();
					var ncp = marshaller.MarshalFromPointer(lParam);
					if (User32.IsZoomed(hWnd))
					{
						rect = ncp.rgrc0;
						Rects.DeflateRect(ref rect, thickness);
						ncp.rgrc0 = rect;
					}
					else
					{
						var newWinRect = ncp.rgrc0;
						var oldWinRect = ncp.rgrc1;
						var newClient = newWinRect;
						thickness[1] = 0;
						Rects.DeflateRect(ref newClient, thickness);
						ncp.rgrc0 = newClient;
						ncp.rgrc1 = newWinRect;
						ncp.rgrc2 = oldWinRect;
					}
					marshaller.TryMarshalToPointer(ncp, ref lParam);
					return new IntPtr((int)User32.WVR.REDRAW);
				}
				rect = Marshal.PtrToStructure<Vec4i>(lParam);
				thickness[1] = 0;
				Rects.DeflateRect(ref rect, thickness);
				Marshal.StructureToPtr(rect, lParam, true);
			}
			return IntPtr.Zero;
		}

		public void SetPreferedInitialWindowRect(IntPtr hOwner, int preferedOption, int minWidth = 0, int minHeight = 0)
		{
			User32.GetCursorPos(out var mouse);
			var monitor = User32.MonitorFromPoint(mouse, (int)User32.MONITOR_DEFAULTTO.NEAREST);
			if (monitor != IntPtr.Zero)
			{
				var mInfo = new User32.MONITORINFOEX();
				User32.MONITORINFOEX.Init(ref mInfo);
				if (User32.GetMonitorInfo(monitor, ref mInfo))
				{
					User32.GetWindowRect(m_hWnd, out var winRect);
					Rects.Decompose(out var vLeft, out var vTop, out var vWidth, out var vHeight, winRect, dataMode: RectMode.XYWH);
					var work = mInfo.rcWork;
					Rects.GetSize(out var wWork, out var hWork, work);
					var min = wWork;
					if (min > hWork)
					{
						min = hWork;
						foreach (var dim in s_baseDim)
						{
							if (min > dim)
							{
								vHeight = dim;
								break;
							}
						}
						vWidth = vHeight * 4 / 3;
						if (vWidth > wWork)
						{
							vWidth = vHeight;
						}
					}
					else
					{
						foreach (var dim in s_baseDim)
						{
							if (min > dim)
							{
								vWidth = dim;
								break;
							}
						}
						vHeight = vWidth * 4 / 3;
						if (vHeight > wWork)
						{
							vHeight = vWidth;
						}
					}
					var thickness = s_nonClientFrameThickness;
					if (UsingSystemFrame)
					{
						vWidth += thickness[0] + thickness[2];
						vHeight += thickness[1] + thickness[3];
					}
					else
					{
						vHeight += thickness[1];
					}
					if (vWidth < minWidth)
					{
						vWidth = minWidth;
					}
					if (vHeight < minHeight)
					{
						vHeight = minHeight;
					}
					if (preferedOption != 0)
					{
						var ownerRect = work;
						if (preferedOption == 2 && hOwner != IntPtr.Zero)
						{
							User32.GetWindowRect(hOwner, out ownerRect);
						}
						Rects.Decompose(out var oLeft, out var oTop, out var oWidth, out var oHeight, ownerRect, dataMode: RectMode.XYWH);
						vLeft = (2 * oLeft + oWidth - vWidth) / 2;
						vTop = (2 * oTop + oHeight - vHeight) / 2;
					}
					if (User32.IsZoomed(m_hWnd) || User32.IsIconic(m_hWnd))
					{
						var winPlace = new User32.WINDOWPLACEMENT();
						User32.WINDOWPLACEMENT.Init(ref winPlace);
						User32.GetWindowPlacement(m_hWnd, ref winPlace);
						var normalRect = winPlace.rcNormalPosition;
						Rects.Compose(ref normalRect, vLeft, vTop, vWidth, vHeight, dataMode: RectMode.XYWH);
						winPlace.rcNormalPosition = normalRect;
						User32.SetWindowPlacement(m_hWnd, ref winPlace);
					}
					else
					{
						User32.SetWindowPos(m_hWnd, IntPtr.Zero, vLeft, vTop, vWidth, vHeight, (int)(User32.SWP.NOZORDER | User32.SWP.NOACTIVATE));
					}
				}
			}
		}

		public bool IsInWindowRect(Vec2i pt)
		{
			return Rects.ContainsPoint(WindowRect, pt);
		}

		public bool TryHandleSizeBoxHitTest(Vec2i pt, out User32.HT hitTestResult)
		{
			return EnsuredHitTestHandler.TryHandleSizeBoxHitTest(pt, WindowRect, ClientRect, out hitTestResult);
		}

		public bool IsInCaptionRegion(Vec2i pt)
		{
			return EnsuredHitTestHandler.IsInCaptionRegion(pt, WindowRect, ClientRect);
		}

		private void EnsureWindowRectsParam()
		{
			if (m_isRectsValid)
			{
				return;
			}
			User32.GetWindowRect(m_hWnd, out m_windowRect);
			Win32Native.GetClientRectInScreenCoord(m_hWnd, out m_clientRect);
			m_isRectsValid = true;
		}

		public void InvalidateWindowRectsParam()
		{
			m_isRectsValid = false;
		}

	}

}
