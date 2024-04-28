using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Interop;
using FantaziaDesign.Interop;
using FantaziaDesign.Wpf.Windows;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public sealed class DWindowProcedure
	{
		private static readonly object s_lockObj = new object();

		private static readonly Dictionary<IntPtr, DWindowProcedure> s_dWindowProcedures = new Dictionary<IntPtr, DWindowProcedure>();

		public static DWindowProcedure FromWindow(Window window)
		{
			if (window is null)
			{
				throw new ArgumentNullException(nameof(window));
			}
			var interopHelper = new WindowInteropHelper(window);
			var hWnd = interopHelper.EnsureHandle();
			if (hWnd == IntPtr.Zero)
			{
				throw new ArgumentException("Cannot create DWindowProcedure for zero window handle");
			}
			lock (s_lockObj)
			{
				if (!s_dWindowProcedures.TryGetValue(hWnd, out var procedure))
				{
					procedure = new DWindowProcedure() { m_hWnd = hWnd, m_wnd = window, m_interopHelper = interopHelper };
					s_dWindowProcedures.Add(hWnd, procedure);
				}
				return procedure;
			}
		}

		public static bool Contains(IntPtr hWnd)
		{
			if (hWnd == IntPtr.Zero)
			{
				return false;
			}
			return s_dWindowProcedures.ContainsKey(hWnd);
		}

		public static bool Contains(Window window)
		{
			if (window is null)
			{
				return false;
			}
			return Contains(new WindowInteropHelper(window).Handle);
		}

		public static bool Remove(IntPtr hWnd)
		{
			lock (s_lockObj)
			{
				return s_dWindowProcedures.Remove(hWnd);
			}
		}

		public static bool TryFind(IntPtr hWnd, out DWindowProcedure procedure)
		{
			if (hWnd == IntPtr.Zero)
			{
				procedure = null;
				return false;
			}
			return s_dWindowProcedures.TryGetValue(hWnd, out procedure);
		}

		public static bool TryFind(Window window, out DWindowProcedure procedure)
		{
			if (window is null)
			{
				procedure = null;
				return false;
			}
			return TryFind(new WindowInteropHelper(window).Handle, out procedure);
		}

		public static IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)User32.WM.DWMCOLORIZATIONCOLORCHANGED)
			{
				SystemThemeInfo.RefreshThemeColorization();
			}
			if (hWnd != IntPtr.Zero && s_dWindowProcedures.TryGetValue(hWnd, out var procedure))
			{
				var eventArgs = new DWindowProcedureEventArgs(hWnd, procedure.Window, msg, wParam, lParam);
				handled = procedure.InvokeMessageHandler(eventArgs, out var result);
				return result;
			}
			return User32.DefWindowProc(hWnd, msg, wParam, lParam);
		}

		private IntPtr m_hWnd;
		private Window m_wnd;
		private WindowInteropHelper m_interopHelper;
		private Dictionary<int, Func<DWindowProcedureEventArgs, IntPtr>> m_messageHandlers = new Dictionary<int, Func<DWindowProcedureEventArgs, IntPtr>>();

		private DWindowProcedure()
		{
		}

		public WindowInteropHelper InteropHelper => m_interopHelper;
		public IntPtr WindowHandle => m_hWnd;
		public Window Window => m_wnd;
		public IntPtr OwnerHandle => m_interopHelper.Owner;

		internal Dictionary<int, Func<DWindowProcedureEventArgs, IntPtr>> MessageHandlers { get => m_messageHandlers; }

		public void RegisterMessageHandler(int message, Func<DWindowProcedureEventArgs, IntPtr> messageHandler)
		{
			if (messageHandler is null)
			{
				m_messageHandlers.Remove(message);
			}
			else
			{
				if (m_messageHandlers.ContainsKey(message))
				{
					m_messageHandlers[message] = messageHandler;
				}
				else
				{
					m_messageHandlers.Add(message, messageHandler);
				}
			}
		}

		public void RegisterMessageHandler(User32.WM message, Func<DWindowProcedureEventArgs, IntPtr> messageHandler)
		{
			var msgCode = (int)message;
			if (messageHandler is null)
			{
				m_messageHandlers.Remove(msgCode);
			}
			else
			{
				if (m_messageHandlers.ContainsKey(msgCode))
				{
					m_messageHandlers[msgCode] = messageHandler;
				}
				else
				{
					m_messageHandlers.Add(msgCode, messageHandler);
				}
			}
		}

		public void UnregisterMessageHandler(int message)
		{
			m_messageHandlers.Remove(message);
		}

		public void UnregisterMessageHandler(User32.WM message)
		{
			m_messageHandlers.Remove((int)message);
		}

		public void UnregisterAll()
		{
			m_messageHandlers.Clear();
		}

		public bool InvokeMessageHandler(DWindowProcedureEventArgs eventArgs, out IntPtr result)
		{
			if (eventArgs is null)
			{
				throw new ArgumentNullException(nameof(eventArgs));
			}

			if (m_messageHandlers.TryGetValue(eventArgs.Message, out var messageHandler))
			{
				result = messageHandler.Invoke(eventArgs);
				return eventArgs.Handled;
			}
			result = IntPtr.Zero;
			return false;
		}

		public Func<DWindowProcedureEventArgs, IntPtr> GetMessageHandler(int message)
		{
			if (m_messageHandlers.TryGetValue(message, out var messageHandler))
			{
				return messageHandler;
			}
			return null;
		}

		public Func<DWindowProcedureEventArgs, IntPtr> GetMessageHandler(User32.WM message)
		{
			if (m_messageHandlers.TryGetValue((int)message, out var messageHandler))
			{
				return messageHandler;
			}
			return null;
		}
	}

}
