using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using FantaziaDesign.Core;

namespace FantaziaDesign.Wpf.Controls
{
	public class ScrollLogicManager
	{
		private bool m_allowScroll = true;
		private byte m_orientationFlags;
		private Vector m_offset;
		private Vector m_computedOffset = new Vector(0.0, 0.0);
		private Size m_viewport;
		private Size m_extent;

		public static double CoerceOffset(double offset, double extent, double viewport)
		{
			if (offset > extent - viewport)
			{
				offset = extent - viewport;
			}
			if (offset < 0)
			{
				offset = 0;
			}
			return offset;
		}

		public virtual bool TryMakeVisible(ref Rect visualBound)
		{
			var viewport = ViewportRect;
			visualBound.X += viewport.X;
			visualBound.Y += viewport.Y;

			// Compute the offsets required to minimally scroll the child maximally into view.
			double minX = ComputeScrollOffsetWithMinimalScroll(viewport.Left, viewport.Right, visualBound.Left, visualBound.Right);
			double minY = ComputeScrollOffsetWithMinimalScroll(viewport.Top, viewport.Bottom, visualBound.Top, visualBound.Bottom);

			// We have computed the scrolling offsets; scroll to them.
			bool result = SetOffset(minX, minY);

			// Compute the visible rectangle of the child relative to the viewport.
			viewport.X = minX;
			viewport.Y = minY;
			visualBound.Intersect(viewport);

			if (!visualBound.IsEmpty)
			{
				visualBound.X -= viewport.X;
				visualBound.Y -= viewport.Y;
			}
			// Return if should scroll
			return result;
		}

		private static double ComputeScrollOffsetWithMinimalScroll(double minView, double maxView, double minChild, double maxChild)
		{
			bool alignTop = false;
			bool alignBottom = false;
			return ComputeScrollOffsetWithMinimalScroll(minView, maxView, minChild, maxChild, ref alignTop, ref alignBottom);
		}

		private static double ComputeScrollOffsetWithMinimalScroll(double topView, double bottomView, double topChild, double bottomChild, ref bool alignTop, ref bool alignBottom)
		{
			// # CHILD POSITION       CHILD SIZE      SCROLL      REMEDY
			// 1 Above viewport       <= viewport     Down        Align top edge of child & viewport
			// 2 Above viewport       > viewport      Down        Align bottom edge of child & viewport
			// 3 Below viewport       <= viewport     Up          Align bottom edge of child & viewport
			// 4 Below viewport       > viewport      Up          Align top edge of child & viewport
			// 5 Entirely within viewport             NA          No scroll.
			// 6 Spanning viewport                    NA          No scroll.
			//
			// Note: "Above viewport" = childTop above viewportTop, childBottom above viewportBottom
			//       "Below viewport" = childTop below viewportTop, childBottom below viewportBottom
			// These child thus may overlap with the viewport, but will scroll the same direction/

			bool fAbove = topChild < topView && bottomChild < bottomView;
			bool fBelow = bottomChild > bottomView && topChild > topView;
			bool fLarger = (bottomChild - topChild) > (bottomView - topView);

			// Handle Cases:  1 & 4 above
			if ((fAbove && !fLarger)
			   || (fBelow && fLarger)
			   || alignTop)
			{
				alignTop = true;
				return topChild;
			}

			// Handle Cases: 2 & 3 above
			else if (fAbove || fBelow || alignBottom)
			{
				alignBottom = true;
				return (bottomChild - (bottomView - topView));
			}

			// Handle cases: 5 & 6 above.
			return topView;
		}

		public void ClearLayout()
		{
			m_offset = default(Vector);
			m_viewport = default(Size);
			m_extent = default(Size);
			//_physicalViewport = 0.0;
		}

		public bool AllowScroll
		{
			get => m_allowScroll;
			set
			{
				if (m_allowScroll != value)
				{
					m_allowScroll = value;
					if (!m_allowScroll)
					{
						ClearLayout();
					}
				}
			}
		}

		public Vector Offset { get => m_offset; }
		public Vector ComputedOffset { get => m_computedOffset; }
		public Size Viewport { get => m_viewport; }
		public Size Extent { get => m_extent; }

		public bool CanHorizontallyScroll
		{
			get => BitUtil.GetBitStatus(ref m_orientationFlags, 1);
			set => BitUtil.SetBitStatus(ref m_orientationFlags, 1, value);
		}

		public bool CanVerticallyScroll
		{
			get => BitUtil.GetBitStatus(ref m_orientationFlags, 2);
			set => BitUtil.SetBitStatus(ref m_orientationFlags, 2, value);
		}

		public Rect ViewportRect { get => new Rect(m_offset.X, m_offset.Y, m_viewport.Width, m_viewport.Height); }

		public Rect ExtentRect { get => new Rect(m_extent); }

		public bool SetOffset(double x, double y)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			bool or = false;
			if (m_offset.X != x)
			{
				m_offset.X = x;
				or = true;
			}
			if (m_offset.Y != y)
			{
				m_offset.Y = y;
				or = true;
			}
			return or;
		}

		public bool SetOffsetX(double x)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			if (m_offset.X != x)
			{
				m_offset.X = x;
				return true;
			}
			return false;
		}

		public bool SetOffsetY(double y)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			if (m_offset.Y != y)
			{
				m_offset.Y = y;
				return true;
			}
			return false;
		}

		public bool SetComputedOffset(double x, double y)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			bool or = false;
			if (m_computedOffset.X != x)
			{
				m_computedOffset.X = x;
				or = true;
			}
			if (m_computedOffset.Y != y)
			{
				m_computedOffset.Y = y;
				or = true;
			}
			return or;
		}

		public bool SetViewportWidth(double width)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			if (m_viewport.Width != width)
			{
				m_viewport.Width = width;
				return true;
			}
			return false;
		}

		public bool SetViewportHeight(double height)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			if (m_viewport.Height != height)
			{
				m_viewport.Height = height;
				return true;
			}
			return false;
		}

		public bool SetViewport(double width, double height)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			bool or = false;
			if (m_viewport.Width != width)
			{
				m_viewport.Width = width;
				or = true;
			}
			if (m_viewport.Height != height)
			{
				m_viewport.Height = height;
				or = true;
			}
			return or;
		}

		public bool SetExtentWidth(double width)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			if (m_extent.Width != width)
			{
				m_extent.Width = width;
				return true;
			}
			return false;
		}

		public bool SetExtentHeight(double height)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			if (m_extent.Height != height)
			{
				m_extent.Height = height;
				return true;
			}
			return false;
		}

		public bool SetExtent(double width, double height)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			bool or = false;
			if (m_extent.Width != width)
			{
				m_extent.Width = width;
				or = true;
			}
			if (m_extent.Height != height)
			{
				m_extent.Height = height;
				or = true;
			}
			return or;
		}

		public void SetOffsetDirectly(Vector offset)
		{
			m_offset = offset;
		}

		public void SetComputedOffsetDirectly(Vector computedOffset)
		{
			m_computedOffset = computedOffset;
		}

		public void SetViewportDirectly(Size viewport)
		{
			m_viewport = viewport;
		}

		public void SetExtentDirectly(Size extent)
		{
			m_extent = extent;
		}


		public bool ComputeOffset(OrientationFlags orientation)
		{
			if (!m_allowScroll)
			{
				return false;
			}
			byte flag = (byte)orientation;
			if (flag == 0)
			{
				flag = 3;
			}

			bool or = false;

			if (BitUtil.GetBitStatus(ref flag, 1))
			{
				double cOffsetX = CoerceOffset(m_offset.X, m_extent.Width, m_viewport.Width);
				if (m_viewport.Width != cOffsetX)
				{
					m_viewport.Width = cOffsetX;
					or = true;
				}
			}
			if (BitUtil.GetBitStatus(ref flag, 2))
			{
				double cOffsetY = CoerceOffset(m_offset.Y, m_extent.Height, m_viewport.Height);
				if (m_viewport.Height != cOffsetY)
				{
					m_viewport.Height = cOffsetY;
					or = true;
				}
			}
			return or;
		}

		//// call in measureoverride / arrangeoverride
		//// Verifies scrolling data using the passed viewport and extent as newly computed values.
		//// Checks the X/Y offset and coerces them into the range [0, Extent - ViewportSize]
		//// If extent, viewport, or the newly coerced offsets are different than the existing offset,
		////   cachces are updated and InvalidateScrollInfo() is called.
		public bool VerifyScrollDataChanged(Size viewport, Size extent)
		{
			if (!m_allowScroll)
			{
				return false;
			}

			bool fValid = true;

			var vWidth = viewport.Width;
			var vHeight = viewport.Height;
			var eWidth = extent.Width;
			var eHeight = extent.Height;
			// These two lines of code are questionable, but they are needed right now as VSB may return
			//  Infinity size from measure, which is a regression from the old scrolling model.
			// They also have the incidental affect of probably avoiding reinvalidation at Arrange
			//   when inside a parent that measures you to Infinity.
			if (double.IsInfinity(vWidth))
			{
				vWidth = eWidth;
			}

			if (double.IsInfinity(vHeight))
			{
				vHeight = eHeight;
			}


			//fValid &= viewport == scrollData.Viewport;
			//fValid &= extent == scrollData.Extent;
			//scrollData.Viewport = viewport;
			//scrollData.Extent = extent;

			//Vector computedOffset = new Vector(
			//	CoerceOffset(scrollData.Offset.X, scrollData.Extent.Width, scrollData.Viewport.Width),
			//	CoerceOffset(scrollData.Offset.Y, scrollData.Extent.Height, scrollData.Viewport.Height));

			//fValid &= scrollData.ComputedOffset == computedOffset;
			//scrollData.ComputedOffset = computedOffset;
			//if (!fValid)
			//{
			//	OnScrollChange();
			//}
			//=====================================================

			fValid &= SetViewport(vWidth, vHeight);
			fValid &= SetExtent(eWidth, eHeight);
			fValid &= ComputeOffset(OrientationFlags.Both);
			return !fValid;
		}

	}

	// world rect => (0,0,ExtentWidth,ExtentHeight)
	// Viewport rect => (HorizontalOffset,VerticalOffset,ViewportWidth,ViewportHeight)
	public abstract class ScrollablePanel : Panel, IScrollInfo
	{
		private ScrollLogicManager m_scrollLogicManager;

		protected virtual ScrollLogicManager ScrollLogicManager
		{
			get
			{
				if (m_scrollLogicManager is null)
				{
					m_scrollLogicManager = new ScrollLogicManager();
				}
				return m_scrollLogicManager;
			}
		}

		public bool CanVerticallyScroll
		{
			get => ScrollLogicManager.CanVerticallyScroll;
			set => ScrollLogicManager.CanVerticallyScroll = value;
		}

		public bool CanHorizontallyScroll
		{
			get => ScrollLogicManager.CanHorizontallyScroll;
			set => ScrollLogicManager.CanHorizontallyScroll = value;
		}

		public double ExtentWidth { get => ScrollLogicManager.Extent.Width; }
		public double ExtentHeight { get => ScrollLogicManager.Extent.Height; }
		public double ViewportWidth { get => ScrollLogicManager.Viewport.Width; }
		public double ViewportHeight { get => ScrollLogicManager.Viewport.Height; }
		public double HorizontalOffset { get => ScrollLogicManager.ComputedOffset.X; }
		public double VerticalOffset { get => ScrollLogicManager.ComputedOffset.Y; }

		public ScrollViewer ScrollOwner { get; set; }
		public double ScrollableWidth => Math.Max(0.0, ExtentWidth - ViewportWidth);
		public double ScrollableHeight => Math.Max(0.0, ExtentHeight - ViewportHeight);

		public bool CanScroll { get => ScrollOwner != null && ScrollLogicManager.AllowScroll; }

		public abstract void LineDown();
		public abstract void LineLeft();
		public abstract void LineRight();
		public abstract void LineUp();

		public virtual Rect MakeVisible(Visual visual, Rect rectangle)
		{
			if (rectangle.IsEmpty || visual == null || visual == this || !IsAncestorOf(visual))
			{
				return Rect.Empty;
			}
			rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
			if (!CanScroll)
			{
				return rectangle;
			}
			
			if (ScrollLogicManager.TryMakeVisible(ref rectangle))
			{
				InvalidateMeasure();
				OnScrollChange();
			}
			// Return the rectangle
			return rectangle;
		}

		protected void OnScrollChange()
		{
			if (ScrollOwner != null)
			{
				ScrollOwner.InvalidateScrollInfo();
			}
		}


		public abstract void MouseWheelDown();
		public abstract void MouseWheelLeft();
		public abstract void MouseWheelRight();
		public abstract void MouseWheelUp();
		public abstract void PageDown();
		public abstract void PageLeft();
		public abstract void PageRight();
		public abstract void PageUp();

		public virtual void SetHorizontalOffset(double offset)
		{
			if (ScrollLogicManager.SetOffsetX(offset))
			{
				InvalidateMeasure();
			}
		}

		public virtual void SetVerticalOffset(double offset)
		{
			if (ScrollLogicManager.SetOffsetY(offset))
			{
				InvalidateMeasure();
			}
		}

		protected override Size MeasureOverride(Size constraint)
		{
			Size desiredSize = new Size();
			int count = InternalChildren.Count;
			if (count > 0)
			{
				Size childrenConstraint = constraint;
				if (CanScroll)
				{
					if (ScrollLogicManager.CanHorizontallyScroll)
					{
						childrenConstraint.Width = double.PositiveInfinity;
					}
					if (ScrollLogicManager.CanVerticallyScroll)
					{
						childrenConstraint.Height = double.PositiveInfinity;
					}
				}
				desiredSize = MeasureChildren(constraint, childrenConstraint);
			}

			// If we're handling scrolling (as the physical scrolling client, validate properties.
			if (ScrollLogicManager.VerifyScrollDataChanged(constraint, desiredSize))
			{
				OnScrollChange();
			}
			desiredSize.Width = Math.Min(constraint.Width, desiredSize.Width);
			desiredSize.Height = Math.Min(constraint.Height, desiredSize.Height);
			return desiredSize;
		}
		/// <summary>
		/// measure children override method
		/// </summary>
		/// <param name="constraint">constraint from panel's parent</param>
		/// <param name="childrenConstraint">Infinity size if allow children scroll</param>
		/// <returns>return maximum size children required, will assign to Scrolldata.Extent</returns>
		protected abstract Size MeasureChildren(Size constraint, Size childrenConstraint);

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			int count = InternalChildren.Count;
			// Verifies IScrollInfo properties & invalidates ScrollViewer if necessary.
			if (ScrollLogicManager.VerifyScrollDataChanged(arrangeSize, m_scrollLogicManager.Extent))
			{
				OnScrollChange();
			}
			if (count > 0)
			{
				Vector computedOffset = CanScroll
					? -ScrollLogicManager.ComputedOffset
					: default(Vector);
				arrangeSize = ArrangeChildren(arrangeSize, computedOffset);

				//foreach (UIElement child in InternalChildren)
				//{
				//	if (child != null)
				//	{
				//		Rect childRect = new Rect(child.DesiredSize);
				//		childRect.X = -HorizontalOffset;
				//		childRect.Y = -VerticalOffset;
				//		//this is needed to stretch the child to arrange space,
				//		childRect.Width = Math.Max(childRect.Width, arrangeSize.Width);
				//		childRect.Height = Math.Max(childRect.Height, arrangeSize.Height);
				//		child.Arrange(childRect);
				//	}
				//}
			}
			return arrangeSize;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="arrangeSize">arrangeSize from panel's parent</param>
		/// <param name="computedOffset">current scroll offset; if panel can scroll content, childRect should offset by this value before Arrange child</param>
		/// <returns>final size for panel</returns>
		protected abstract Size ArrangeChildren(Size arrangeSize, Vector computedOffset);
	}
}
