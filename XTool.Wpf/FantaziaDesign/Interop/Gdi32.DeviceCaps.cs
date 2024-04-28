namespace FantaziaDesign.Interop
{
	public static partial class Gdi32
	{
		/// <summary>
		/// Device Parameters for GetDeviceCaps()
		/// </summary>
		public enum DeviceCaps
		{
			DRIVERVERSION = 0, /* Device driver version*/
			TECHNOLOGY = 2, /* Device classification*/
			HORZSIZE = 4, /* Horizontal size in millimeters */
			VERTSIZE = 6, /* Vertical size in millimeters */
			HORZRES = 8, /* Horizontal width in pixels */
			VERTRES = 10, /* Vertical height in pixels*/
			BITSPIXEL = 12, /* Number of bits per pixel */
			PLANES = 14, /* Number of planes */
			NUMBRUSHES = 16, /* Number of brushes the device has */
			NUMPENS = 18, /* Number of pens the device has*/
			NUMMARKERS = 20, /* Number of markers the device has */
			NUMFONTS = 22, /* Number of fonts the device has */
			NUMCOLORS = 24, /* Number of colors the device supports */
			PDEVICESIZE = 26, /* Size required for device descriptor*/
			CURVECAPS = 28, /* Curve capabilities */
			LINECAPS = 30, /* Line capabilities*/
			POLYGONALCAPS = 32, /* Polygonal capabilities */
			TEXTCAPS = 34, /* Text capabilities*/
			CLIPCAPS = 36, /* Clipping capabilities*/
			RASTERCAPS = 38, /* Bitblt capabilities*/
			ASPECTX = 40, /* Length of the X leg*/
			ASPECTY = 42, /* Length of the Y leg*/
			ASPECTXY = 44, /* Length of the hypotenuse */
			LOGPIXELSX = 88, /* Logical pixels/inch in X */
			LOGPIXELSY = 90, /* Logical pixels/inch in Y */
			SIZEPALETTE = 104, /* Number of entries in physical palette*/
			NUMRESERVED = 106, /* Number of reserved entries in palette*/
			COLORRES = 108, /* Actual color resolution*/
			PHYSICALWIDTH = 110,/* Physical Width in device units */
			PHYSICALHEIGHT = 111,/* Physical Height in device units*/
			PHYSICALOFFSETX = 112,/* Physical Printable Area x margin */
			PHYSICALOFFSETY = 113,/* Physical Printable Area y margin */
			SCALINGFACTORX = 114,/* Scaling factor x */
			SCALINGFACTORY = 115,/* Scaling factor y */
			VREFRESH = 116, /* Current vertical refresh rate of the display device (for displays only) in Hz */
			DESKTOPVERTRES = 117, /* Horizontal width of entire desktop in pixels */
			DESKTOPHORZRES = 118, /* Vertical height of entire desktop in pixels */
			BLTALIGNMENT = 119, /* Preferred blt alignment*/
			SHADEBLENDCAPS = 120, /* Shading and blending caps */
			COLORMGMTCAPS = 121, /* Color Management caps */
		}

		/// <summary>
		/// Device Technologies
		/// </summary>
		public enum DT
		{
			PLOTTER = 0,/* Vector plotter */
			RASDISPLAY = 1,/* Raster display */
			RASPRINTER = 2,/* Raster printer */
			RASCAMERA = 3,/* Raster camera*/
			CHARSTREAM = 4,/* Character-stream, PLP*/
			METAFILE = 5,/* Metafile, VDM*/
			DISPFILE = 6,/* Display-file */
		}

		/// <summary>
		/// Curve Capabilities
		/// </summary>
		public enum CC
		{
			NONE = 0,/* Curves not supported */
			CIRCLES = 1,/* Can do circles */
			PIE = 2,/* Can do pie wedges*/
			CHORD = 4,/* Can do chord arcs*/
			ELLIPSES = 8,/* Can do ellipese*/
			WIDE = 16,/* Can do wide lines*/
			STYLED = 32,/* Can do styled lines*/
			WIDESTYLED = 64,/* Can do wide styled lines */
			INTERIORS = 128,/* Can do interiors */
			ROUNDRECT = 256,/**/
		}

		/// <summary>
		/// Line Capabilities
		/// </summary>
		public enum LC
		{
			NONE = 0,/* Lines not supported*/
			POLYLINE = 2,/* Can do polylines */
			MARKER = 4,/* Can do markers */
			POLYMARKER = 8,/* Can do polymarkers */
			WIDE = 16,/* Can do wide lines*/
			STYLED = 32,/* Can do styled lines*/
			WIDESTYLED = 64,/* Can do wide styled lines */
			INTERIORS = 128,/* Can do interiors */
		}

		/// <summary>
		/// Polygonal Capabilities
		/// </summary>
		public enum PC
		{
			NONE = 0,/* Polygonals not supported */
			POLYGON = 1,/* Can do polygons*/
			RECTANGLE = 2,/* Can do rectangles*/
			WINDPOLYGON = 4,/* Can do winding polygons*/
			TRAPEZOID = 4,/* Can do trapezoids*/
			SCANLINE = 8,/* Can do scanlines */
			WIDE = 16,/* Can do wide borders*/
			STYLED = 32,/* Can do styled borders*/
			WIDESTYLED = 64,/* Can do wide styled borders */
			INTERIORS = 128,/* Can do interiors */
			POLYPOLYGON = 256,/* Can do polypolygons*/
			PATHS = 512,/* Can do paths */
		}

		/// <summary>
		/// Clipping Capabilities
		/// </summary>
		public enum CP
		{
			NONE = 0, /* No clipping of output*/
			RECTANGLE = 1, /* Output clipped to rects*/
			REGION = 2, /* obsolete */
		}

		/// <summary>
		/// Text Capabilities
		/// </summary>
		public enum TC
		{
			OP_CHARACTER = 0x00000001, /* Can do OutputPrecision CHARACTER*/
			OP_STROKE = 0x00000002, /* Can do OutputPrecision STROKE */
			CP_STROKE = 0x00000004, /* Can do ClipPrecision STROKE */
			CR_90 = 0x00000008, /* Can do CharRotAbility90 */
			CR_ANY = 0x00000010, /* Can do CharRotAbilityANY*/
			SF_X_YINDEP = 0x00000020, /* Can do ScaleFreedomX_YINDEPENDENT */
			SA_DOUBLE = 0x00000040, /* Can do ScaleAbilityDOUBLE */
			SA_INTEGER = 0x00000080, /* Can do ScaleAbilityINTEGER*/
			SA_CONTIN = 0x00000100, /* Can do ScaleAbilityCONTINUOUS */
			EA_DOUBLE = 0x00000200, /* Can do EmboldenAbility DOUBLE */
			IA_ABLE = 0x00000400, /* Can do ItalisizeAbilityABLE */
			UA_ABLE = 0x00000800, /* Can do UnderlineAbilityABLE */
			SO_ABLE = 0x00001000, /* Can do StrikeOutAbilityABLE */
			RA_ABLE = 0x00002000, /* Can do RasterFontAbleABLE */
			VA_ABLE = 0x00004000, /* Can do VectorFontAbleABLE */
			RESERVED = 0x00008000,
			SCROLLBLT = 0x00010000, /* Don't do text scroll with blt */
		}

		/// <summary>
		/// Raster Capabilities
		/// </summary>
		public enum RC
		{
			NONE = 0,
			BITBLT = 1, /* Can do standard BLT. */
			BANDING = 2, /* Device requires banding support*/
			SCALING = 4, /* Device requires scaling support*/
			BITMAP64 = 8, /* Device can support >64K bitmap */
			GDI20_OUTPUT = 0x0010, /* has 2.0 output calls */
			GDI20_STATE = 0x0020,
			SAVEBITMAP = 0x0040,
			DI_BITMAP = 0x0080, /* supports DIB to memory */
			PALETTE = 0x0100, /* supports a palette */
			DIBTODEV = 0x0200, /* supports DIBitsToDevice*/
			BIGFONT = 0x0400, /* supports >64K fonts*/
			STRETCHBLT = 0x0800, /* supports StretchBlt*/
			FLOODFILL = 0x1000, /* supports FloodFill */
			STRETCHDIB = 0x2000, /* supports StretchDIBits */
			OP_DX_OUTPUT = 0x4000,
			DEVBITS = 0x8000,
		}

		/// <summary>
		/// Shading and blending caps
		/// </summary>
		public enum SB
		{
			NONE = 0x00000000,
			CONST_ALPHA = 0x00000001,
			PIXEL_ALPHA = 0x00000002,
			PREMULT_ALPHA = 0x00000004,
			GRAD_RECT = 0x00000010,
			GRAD_TRI = 0x00000020,
		}

		/// <summary>
		/// Color Management caps
		/// </summary>
		public enum CM
		{
			NONE = 0x00000000,
			DEVICE_ICM = 0x00000001,
			GAMMA_RAMP = 0x00000002,
			CMYK_COLOR = 0x00000004,
		}




	}
}
