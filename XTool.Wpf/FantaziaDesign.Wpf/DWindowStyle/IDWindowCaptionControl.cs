using System;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public interface IDWindowCaptionControl
	{
		DWindowCaptionRole CaptionRole { get; set; }
		IntPtr OwnerWindowHandle { get; }
		void InvokeCaptionRoleEvent();
	}
}