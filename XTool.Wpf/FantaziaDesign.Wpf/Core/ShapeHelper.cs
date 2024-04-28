using System;
using System.Windows.Media;
using FantaziaDesign.Core;
using System.Windows.Shapes;

namespace FantaziaDesign.Wpf.Core
{

	public static class ShapeHelper
	{
		private static Action<Shape> ResetRenderedGeometry_Shape;
		private static Func<Shape, Pen> GetPen_Shape;

		public static void ResetRenderedGeometryInShape(this Shape shape)
		{
			if (ResetRenderedGeometry_Shape is null)
			{
				ResetRenderedGeometry_Shape =
					ReflectionUtil.BindMethodToDelegate<Action<Shape>>(typeof(Shape), "ResetRenderedGeometry", ReflectionUtil.NonPublicInstance);
			}
			ResetRenderedGeometry_Shape(shape);
		}

		public static Pen GetPenInShape(this Shape shape)
		{
			if (GetPen_Shape is null)
			{
				GetPen_Shape =
					ReflectionUtil.BindMethodToDelegate<Func<Shape, Pen>>(typeof(Shape), "GetPen", ReflectionUtil.NonPublicInstance);
			}
			return GetPen_Shape(shape);
		}

	}
}
