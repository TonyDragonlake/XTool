using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using FantaziaDesign.Core;
using System.Collections;

namespace FantaziaDesign.Wpf.Core
{
	public sealed class ReadOnlyVisualChildren : IReadOnlyCollection<Visual>, IReadOnlyList<Visual>
	{
		private Visual _parent;

		public ReadOnlyVisualChildren(Visual parent)
		{
			_parent = parent;
		}

		public ReadOnlyVisualChildren(DependencyObject parent)
		{
			_parent = parent as Visual;
		}

		public int Count => _parent is null ? 0 : VisualTreeHelper.GetChildrenCount(_parent);

		public Visual this[int index]
		{
			get
			{
				if (index >= 0 && index < Count)
				{
					return VisualTreeHelper.GetChild(_parent, index) as Visual;
				}
				throw new IndexOutOfRangeException("Index Out Of Range when getting visual child");
			}
		}

		private IEnumerable<Visual> VisualEnumerable()
		{
			var len = Count;
			for (int i = 0; i < len; i++)
			{
				var d = VisualTreeHelper.GetChild(_parent, i);
				if (d is Visual visual)
				{
					yield return visual;
				}
				yield return null;
			}
		}

		public IEnumerator<Visual> GetEnumerator()
		{
			return VisualEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public static class VisualTreeExtension
	{
		private static IReadOnlyCollection<Visual> GetVisualChildrenCallback(Visual parent)
		{
			if (VisualTreeHelper.GetChildrenCount(parent) > 0)
			{
				return new ReadOnlyVisualChildren(parent);
			}
			return null;
		}

		private static bool SkipIfNull(Visual visual)
		{
			return visual is null;
		}

		public static GenericTraverserBase<Visual> GetVisualTreeTraverser(Visual rootParent, TraversalMethod traversalMethod = TraversalMethod.BreathFirst)
		{
			if (traversalMethod == TraversalMethod.BreathFirst)
			{
				return new GenericBreathFirstTraverser<Visual>(rootParent, GetVisualChildrenCallback)
				{
					SkippingCondition = SkipIfNull
				};
			}
			else
			{
				return new GenericDeepFirstTraverser<Visual>(rootParent, GetVisualChildrenCallback)
				{
					SkippingCondition = SkipIfNull
				};
			}
		}

		public static bool TryFindVisualAncestorIf(Visual childVisual, Predicate<Visual> matchCondition, out Visual matchedParent)
		{
			if (childVisual is null)
			{
				throw new ArgumentNullException(nameof(childVisual));
			}

			if (matchCondition is null)
			{
				matchedParent = VisualTreeHelper.GetParent(childVisual) as Visual;
				return matchedParent != null;
			}
			DependencyObject currentObj = childVisual;
			Visual parent; 
			do
			{
				currentObj = VisualTreeHelper.GetParent(currentObj);
				parent = currentObj as Visual;
				if (parent != null)
				{
					if (matchCondition.Invoke(parent))
					{
						matchedParent = parent;
						return true;
					}
				}
			} while (currentObj != null);
			matchedParent = null;
			return false;
		}

		public static bool TryFindVisualChildIf(Visual rootParent, Predicate<Visual> matchCondition, out Visual matchedChild)
		{
			if (rootParent is null)
			{
				throw new ArgumentNullException(nameof(rootParent));
			}

			if (matchCondition is null)
			{
				if (VisualTreeHelper.GetChildrenCount(rootParent) > 0)
				{
					matchedChild = VisualTreeHelper.GetChild(rootParent, 0) as Visual;
					return true;
				}
				matchedChild = null;
				return false;
			}

			var visualEnum = GetVisualTreeTraverser(rootParent);
			while (visualEnum.MoveNext())
			{
				var current = visualEnum.Current;
				if (matchCondition.Invoke(current))
				{
					matchedChild = current;
					return true;
				}
			}
			matchedChild = null;
			return false;
		}

	}


}
