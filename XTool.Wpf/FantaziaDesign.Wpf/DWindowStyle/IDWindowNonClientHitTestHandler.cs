using FantaziaDesign.Core;
using FantaziaDesign.Interop;

namespace FantaziaDesign.Wpf.DWindowStyle
{
	public interface IDWindowNonClientHitTestHandler
	{
		bool TryHandleSizeBoxHitTest(Vec2i pt, Vec4i winRect, Vec4i client, out User32.HT hitTestResult);
		bool IsInCaptionRegion(Vec2i pt, Vec4i winRect, Vec4i client);
	}

}
