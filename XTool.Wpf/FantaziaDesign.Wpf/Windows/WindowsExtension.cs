using System;
using System.Windows;
using System.Windows.Interop;
using FantaziaDesign.Core;

namespace FantaziaDesign.Wpf.Windows
{
	public static class WindowsExtension
	{
		private static Func<WindowInteropHelper, Window> s_GetWindow_WindowInteropHelper;

		private static Func<Window, IntPtr> s_GetCriticalHandle_Window;

		public static Window GetWindow(this WindowInteropHelper interopHelper)
		{
			if (interopHelper is null)
			{
				throw new ArgumentNullException(nameof(interopHelper));
			}

			if (s_GetWindow_WindowInteropHelper is null)
			{
				s_GetWindow_WindowInteropHelper = ReflectionUtil.BindInstanceFieldGetterToDelegate<WindowInteropHelper, Window>("_window", ReflectionUtil.NonPublicInstance);
			}
			return s_GetWindow_WindowInteropHelper(interopHelper);
		}

		public static IntPtr GetCriticalHandle(this Window window)
		{
			if (window is null)
			{
				throw new ArgumentNullException(nameof(window));
			}
			if (s_GetCriticalHandle_Window is null)
			{
				s_GetCriticalHandle_Window = ReflectionUtil.BindInstancePropertyGetterToDelegate<Window, IntPtr>("CriticalHandle", ReflectionUtil.NonPublicInstance, true);
			}
			return s_GetCriticalHandle_Window(window);
		}


		public static Point ToPoint(this PointInt pointInt)
		{
			if (pointInt is null)
			{
				return default(Point);
			}

			return new Point(pointInt.X, pointInt.Y);
		}

		public static Size ToSize(this SizeInt sizeInt)
		{
			if (sizeInt is null)
			{
				return Size.Empty;
			}

			return new Size(sizeInt.Width, sizeInt.Height);
		}

		public static Rect ToRect(this RectInt rectInt)
		{
			if (rectInt is null)
			{
				return Rect.Empty;
			}

			return new Rect(rectInt.X, rectInt.Y, rectInt.Width, rectInt.Height);
		}

		public static void CopyToPoint(this PointInt source, ref Point target)
		{
			if (source is null)
			{
				return;
			}
			target.X = source.X;
			target.Y = source.Y;
		}


		public static void CopyToSize(this SizeInt source, ref Size target)
		{
			if (source is null)
			{
				return;
			}
			target.Width = source.Width;
			target.Height = source.Height;
		}

		public static void CopyToRect(this RectInt source, ref Rect target)
		{
			if (source is null)
			{
				return;
			}
			target.Width = source.Width;
			target.Height = source.Height;
		}

		public static bool EqualsRect(RectInt rectInt, Rect rect)
		{
			return rectInt.Left == rect.Left
				&& rectInt.Top == rect.Top
				&& rectInt.Right == rect.Right
				&& rectInt.Bottom == rect.Bottom;
		}

		//public static void SetExtendWindowMargin(this Window window, int left = 0, int right = 0, int top = 0, int bottom = 0)
		//{
		//	IntPtr handle = new WindowInteropHelper(window).Handle;
		//	if (handle != IntPtr.Zero)
		//	{
		//		NativeWindowUtil.SetExtendWindowMarginWithoutCheck(handle, left, right, top, bottom);
		//	}
		//}

		//public static Rect GetWindowRect(IntPtr hwnd)
		//{
		//	if (NativeMethods.GetWindowRect(hwnd, out var windowRect))
		//	{
		//		if (Rects.TryGetLocationAndSizeRaw(windowRect, out int left, out int top, out int width, out int height))
		//		{
		//			return new Rect(left, top, width, height);
		//		}
		//		return Rect.Empty;
		//	}
		//	else
		//	{
		//		throw new Win32Exception(Marshal.GetLastWin32Error());
		//	}
		//}

		//public static Rect GetClientRect(IntPtr hwnd)
		//{
		//	if (NativeMethods.GetClientRect(hwnd, out var clientRect))
		//	{
		//		if (Rects.TryGetLocationAndSizeRaw(clientRect, out int left, out int top, out int width, out int height))
		//		{
		//			return new Rect(left, top, width, height);
		//		}
		//		return Rect.Empty;
		//	}
		//	else
		//	{
		//		throw new Win32Exception(Marshal.GetLastWin32Error());
		//	}
		//}

		public static bool AreClose(Point point1, Point point2)
		{
			return DoubleUtil.AreClose(point1.X, point2.X) && DoubleUtil.AreClose(point1.Y, point2.Y);
		}

		public static bool AreClose(Size size1, Size size2)
		{
			return DoubleUtil.AreClose(size1.Width, size2.Width) && DoubleUtil.AreClose(size1.Height, size2.Height);
		}

		public static bool AreClose(Vector vector1, Vector vector2)
		{
			return DoubleUtil.AreClose(vector1.X, vector2.X) && DoubleUtil.AreClose(vector1.Y, vector2.Y);
		}

		public static bool AreClose(Rect rect1, Rect rect2)
		{
			if (rect1.IsEmpty)
			{
				return rect2.IsEmpty;
			}
			return !rect2.IsEmpty && DoubleUtil.AreClose(rect1.X, rect2.X) && DoubleUtil.AreClose(rect1.Y, rect2.Y) && DoubleUtil.AreClose(rect1.Height, rect2.Height) && DoubleUtil.AreClose(rect1.Width, rect2.Width);
		}


		public static bool RectHasNaN(Rect r)
		{
			return DoubleUtil.IsNaN(r.X) || DoubleUtil.IsNaN(r.Y) || DoubleUtil.IsNaN(r.Height) || DoubleUtil.IsNaN(r.Width);
		}

	}

}
