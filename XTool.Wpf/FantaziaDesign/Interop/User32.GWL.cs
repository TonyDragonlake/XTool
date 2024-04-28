namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		/// <summary>
		/// Window field offsets for GetWindowLong()
		/// </summary>
		public enum GWL
		{
			WNDPROC = -4,
			HINSTANCE = -6,
			HWNDPARENT = -8,
			STYLE = -16,
			EXSTYLE = -20,
			USERDATA = -21,
			ID = -12,
			// used for WM_STYLECHANGED EVENT WPARAM
			ALLSTYLE = STYLE | EXSTYLE
		}
		

	}
}
