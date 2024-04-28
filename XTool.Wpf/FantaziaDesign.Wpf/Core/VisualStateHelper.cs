using FantaziaDesign.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FantaziaDesign.Wpf.Core
{
	public sealed class VisualStateChangeListener
	{
		private FrameworkElement m_control;
		private FrameworkElement m_stateGroupsRoot;
		private TaskCompletionSource<bool> m_taskCompletionSource;
		private EventHandler m_visualStateStoryboard_Completed;
		private EventHandler<VisualStateChangedEventArgs> m_visualStateGroup_CurrentStateChanged;

		public VisualStateChangeListener(FrameworkElement frameworkElement)
		{
			if (frameworkElement != null)
			{
				m_control = frameworkElement;
				m_stateGroupsRoot = frameworkElement.GetStateGroupsRoot();
			}
		}

		public Task<bool> WaitForVisualStateChanged(string stateName, bool useTransitions)
		{
			if (m_stateGroupsRoot != null)
			{
				bool shouldWait = false;
				if (VisualStateHelper.TryGetVisualState(m_stateGroupsRoot, stateName, out VisualStateGroup visualStateGroup, out VisualState visualState))
				{
					var vsStyBd = visualState.Storyboard;
					// visualState.Storyboard is the final animation
					if (vsStyBd != null)
					{
						m_visualStateStoryboard_Completed = (s, e) =>
						{
							m_taskCompletionSource.SetResult(true);
							vsStyBd.Completed -= m_visualStateStoryboard_Completed;
						};
						vsStyBd.Completed += m_visualStateStoryboard_Completed;
						shouldWait = true;
					}
					// CurrentStateChanged is the final animation
					else if (useTransitions)
					{
						var visualTransition = VisualStateHelper.GetTransition(m_stateGroupsRoot, visualStateGroup, visualStateGroup.CurrentState, visualState);
						if (visualTransition != null)
						{
							m_visualStateGroup_CurrentStateChanged = (s, e) =>
							{
								m_taskCompletionSource.SetResult(true);
								visualStateGroup.CurrentStateChanged -= m_visualStateGroup_CurrentStateChanged;
							};
							visualStateGroup.CurrentStateChanged += m_visualStateGroup_CurrentStateChanged;
							shouldWait = true;
						}
					}
				}

				if (shouldWait)
				{
					m_taskCompletionSource = new TaskCompletionSource<bool>();
					VisualStateManager.GoToState(m_control, stateName, useTransitions);
					return m_taskCompletionSource.Task;
				}
			}
			return Task.FromResult(false);
		}
	}

	public static class VisualStateHelper
	{
		private static Func<FrameworkElement, FrameworkElement> GetStateGroupsRoot_FrameworkElement;
		private static Func<FrameworkElement, VisualStateGroup, VisualState, VisualState, VisualTransition> GetTransition_VisualStateManager;

		public static FrameworkElement GetStateGroupsRoot(this FrameworkElement frameworkElement)
		{
			if (GetStateGroupsRoot_FrameworkElement is null)
			{
				GetStateGroupsRoot_FrameworkElement = ReflectionUtil.BindInstancePropertyGetterToDelegate<FrameworkElement, FrameworkElement>("StateGroupsRoot", ReflectionUtil.NonPublicInstance, true);

			}
			return GetStateGroupsRoot_FrameworkElement(frameworkElement);
		}

		public static VisualTransition GetTransition(FrameworkElement stateGroupsRoot, VisualStateGroup group, VisualState from, VisualState to)
		{
			if (GetTransition_VisualStateManager is null)
			{
				GetTransition_VisualStateManager =
					ReflectionUtil.BindMethodToDelegate<Func<FrameworkElement, VisualStateGroup, VisualState, VisualState, VisualTransition>>(
					typeof(VisualStateManager),
					"GetTransition",
					ReflectionUtil.NonPublicStatic,
					typeof(FrameworkElement),
					typeof(VisualStateGroup),
					typeof(VisualState),
					typeof(VisualState)
					);
			}
			return GetTransition_VisualStateManager(stateGroupsRoot, group, from, to);
		}

		public static bool TryGetVisualState(FrameworkElement stateGroupsRoot, string stateName, out VisualStateGroup visualStateGroup, out VisualState visualState)
		{
			var visualStateGroups = VisualStateManager.GetVisualStateGroups(stateGroupsRoot);
			if (visualStateGroups != null && !string.IsNullOrWhiteSpace(stateName))
			{
				foreach (var vsgsItem in visualStateGroups)
				{
					var group = vsgsItem as VisualStateGroup;
					if (group != null)
					{
						foreach (var stateItem in group.States)
						{
							var state = stateItem as VisualState;
							if (state != null && stateName.Equals(state.Name))
							{
								visualStateGroup = group;
								visualState = state;
								return true;
							}
						}
					}
				}
			}

			visualStateGroup = null;
			visualState = null;
			return false;
		}
	
		public static Task<bool> GoToStateAsync(FrameworkElement control, string stateName, bool useTransitions)
		{
			var listener = new VisualStateChangeListener(control);
			return listener.WaitForVisualStateChanged(stateName, useTransitions);
		}
	}
}
