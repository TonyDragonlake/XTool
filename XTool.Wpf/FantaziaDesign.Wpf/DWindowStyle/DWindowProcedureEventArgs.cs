using System;
using System.Windows;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public class DWindowProcedureEventArgs
	{
		private IntPtr m_hWnd;
		private Window m_wnd;
		private int m_msg;
		private IntPtr m_wParam;
		private IntPtr m_lParam;
		private bool m_handled;

		public DWindowProcedureEventArgs()
		{
		}

		public DWindowProcedureEventArgs(IntPtr hwnd, Window wnd, int msg, IntPtr wParam, IntPtr lParam)
		{
			m_hWnd = hwnd;
			m_wnd = wnd;
			m_msg = msg;
			m_wParam = wParam;
			m_lParam = lParam;
		}

		public IntPtr WindowHandle { get => m_hWnd; internal set => m_hWnd = value; }
		public Window Window { get => m_wnd; internal set => m_wnd = value; }
		public int Message { get => m_msg; set => m_msg = value; }
		public IntPtr WParam { get => m_wParam; set => m_wParam = value; }
		public IntPtr LParam { get => m_lParam; set => m_lParam = value; }
		public bool Handled { get => m_handled; set => m_handled = value; }

	}

}
