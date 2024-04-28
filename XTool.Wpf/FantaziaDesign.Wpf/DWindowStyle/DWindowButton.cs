using System;
using System.Windows;
using System.Windows.Controls;
using FantaziaDesign.Wpf.Windows;
using FantaziaDesign.Interop;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public sealed class DWindowButton : Button, IDWindowCaptionControl
	{
		private IntPtr m_hOwnerWnd;
		private DWindowCaptionRole m_role;
		public IntPtr OwnerWindowHandle
		{
			get
			{
				if (m_hOwnerWnd == IntPtr.Zero)
				{
					var window = Window.GetWindow(this);
					if (window is null)
					{
						return IntPtr.Zero;
					}
					m_hOwnerWnd = window.GetCriticalHandle();
				}
				return m_hOwnerWnd;
			}
		}

		public DWindowCaptionRole CaptionRole
		{
			get { return (DWindowCaptionRole)GetValue(CaptionRoleProperty); }
			set { SetValue(CaptionRoleProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CaptionRole.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionRoleProperty =
			DWindowNonClientManager.CaptionRoleProperty.AddOwner(typeof(DWindowButton), new FrameworkPropertyMetadata(DWindowCaptionRole.None, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnCaptionRolePropertyChanged)));

		private static void OnCaptionRolePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is DWindowButton wndBtn)
			{
				wndBtn.m_role = (DWindowCaptionRole)e.NewValue;
			}
		}

		protected override void OnClick()
		{
			InvokeCaptionRoleEvent();
			base.OnClick();
		}

		public void InvokeCaptionRoleEvent()
		{
			var hWnd = OwnerWindowHandle;
			if (hWnd == IntPtr.Zero)
			{
				return;
			}
			User32.SC sysCmd;
			switch (m_role)
			{
				case DWindowCaptionRole.Minimize:
					sysCmd = User32.SC.MINIMIZE;
					break;
				case DWindowCaptionRole.Restore:
					sysCmd = User32.SC.RESTORE;
					break;
				case DWindowCaptionRole.Maximize:
					sysCmd = User32.SC.MAXIMIZE;
					break;
				case DWindowCaptionRole.Close:
					sysCmd = User32.SC.CLOSE;
					break;
				default:
					return;
			}
			User32.PostMessage(hWnd, (int)User32.WM.SYSCOMMAND, new IntPtr((int)sysCmd), IntPtr.Zero);
		}
	}

}
