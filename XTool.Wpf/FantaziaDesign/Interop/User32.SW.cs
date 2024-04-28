namespace FantaziaDesign.Interop
{

	public static partial class User32
	{
		/// <summary>
		/// ShowWindow() Commands & Identifiers for the WM_SHOWWINDOW message
		/// </summary>
		public enum SW : uint
		{
			/*
			 * ShowWindow() Commands
			*/
			HIDE = 0,
			SHOWNORMAL = 1,
			NORMAL = 1,
			SHOWMINIMIZED = 2,
			SHOWMAXIMIZED = 3,
			MAXIMIZE = 3,
			SHOWNOACTIVATE = 4,
			SHOW = 5,
			MINIMIZE = 6,
			SHOWMINNOACTIVE = 7,
			SHOWNA = 8,
			RESTORE = 9,
			SHOWDEFAULT = 10,
			FORCEMINIMIZE = 11,
			MAX = 11,
			/*
			 * Identifiers for the WM_SHOWWINDOW message
			 */
			PARENTCLOSING = 1,
			OTHERZOOM = 2,
			PARENTOPENING = 3,
			OTHERUNZOOM = 4,
		}

	}
}
