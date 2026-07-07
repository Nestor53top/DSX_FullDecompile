using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWpf;

public class SimpleVisualStateManager : VisualStateManager
{
	private static readonly Duration DurationZero = new Duration(TimeSpan.Zero);

	protected override bool GoToStateCore(FrameworkElement control, FrameworkElement stateGroupsRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
	{
		if (state != null)
		{
			useTransitions &= Helper.IsAnimationsEnabled;
			if (group.Transitions.Count > 0 && VisualStateGroupHelper.IsSupported)
			{
				return GoToStateInternal(control, stateGroupsRoot, group, state, useTransitions);
			}
			return ((VisualStateManager)this).GoToStateCore(control, stateGroupsRoot, stateName, group, state, useTransitions);
		}
		return false;
	}

	internal static Collection<VisualStateGroup> GetVisualStateGroupsInternal(FrameworkElement obj)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		ValueSource valueSource = DependencyPropertyHelper.GetValueSource((DependencyObject)(object)obj, VisualStateManager.VisualStateGroupsProperty);
		if ((int)((ValueSource)(ref valueSource)).BaseValueSource != 1)
		{
			return ((DependencyObject)obj).GetValue(VisualStateManager.VisualStateGroupsProperty) as Collection<VisualStateGroup>;
		}
		return null;
	}

	internal static bool TryGetState(IList<VisualStateGroup> groups, string stateName, out VisualStateGroup group, out VisualState state)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			VisualStateGroup val = groups[i];
			VisualState state2 = val.GetState(stateName);
			if (state2 != null)
			{
				group = val;
				state = state2;
				return true;
			}
		}
		group = null;
		state = null;
		return false;
	}

	private bool GoToStateInternal(FrameworkElement control, FrameworkElement stateGroupsRoot, VisualStateGroup group, VisualState state, bool useTransitions)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (stateGroupsRoot == null)
		{
			throw new ArgumentNullException("stateGroupsRoot");
		}
		if (state == null)
		{
			throw new ArgumentNullException("state");
		}
		if (group == null)
		{
			throw new InvalidOperationException();
		}
		VisualState lastState = group.CurrentState;
		if (lastState == state)
		{
			return true;
		}
		VisualTransition transition = (useTransitions ? GetTransition(stateGroupsRoot, group, lastState, state) : null);
		if (transition == null || (transition.GeneratedDuration == DurationZero && (transition.Storyboard == null || ((Timeline)transition.Storyboard).Duration == DurationZero)))
		{
			if (transition != null && transition.Storyboard != null)
			{
				group.StartNewThenStopOld(stateGroupsRoot, transition.Storyboard, state.Storyboard);
			}
			else
			{
				group.StartNewThenStopOld(stateGroupsRoot, state.Storyboard);
			}
			((VisualStateManager)this).RaiseCurrentStateChanging(group, lastState, state, control, stateGroupsRoot);
			((VisualStateManager)this).RaiseCurrentStateChanged(group, lastState, state, control, stateGroupsRoot);
		}
		else
		{
			if (transition.Storyboard != null)
			{
				EventHandler transitionCompleted = null;
				transitionCompleted = delegate
				{
					if (ShouldRunStateStoryboard(control, stateGroupsRoot, state, group))
					{
						group.StartNewThenStopOld(stateGroupsRoot, state.Storyboard);
					}
					((VisualStateManager)this).RaiseCurrentStateChanged(group, lastState, state, control, stateGroupsRoot);
					((Timeline)transition.Storyboard).Completed -= transitionCompleted;
				};
				((Timeline)transition.Storyboard).Completed += transitionCompleted;
			}
			group.StartNewThenStopOld(stateGroupsRoot, transition.Storyboard);
			((VisualStateManager)this).RaiseCurrentStateChanging(group, lastState, state, control, stateGroupsRoot);
		}
		group.SetCurrentState(state);
		return true;
	}

	private static bool ShouldRunStateStoryboard(FrameworkElement control, FrameworkElement stateGroupsRoot, VisualState state, VisualStateGroup group)
	{
		bool flag = true;
		bool flag2 = true;
		if (control != null && !((UIElement)control).IsVisible)
		{
			flag = PresentationSource.FromVisual((Visual)(object)control) != null;
		}
		if (stateGroupsRoot != null && !((UIElement)stateGroupsRoot).IsVisible)
		{
			flag2 = PresentationSource.FromVisual((Visual)(object)stateGroupsRoot) != null;
		}
		if (flag && flag2)
		{
			return state == group.CurrentState;
		}
		return false;
	}

	internal static VisualTransition GetTransition(FrameworkElement element, VisualStateGroup group, VisualState from, VisualState to)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (group == null)
		{
			throw new ArgumentNullException("group");
		}
		if (to == null)
		{
			throw new ArgumentNullException("to");
		}
		VisualTransition val = null;
		VisualTransition val2 = null;
		int num = -1;
		IList<VisualTransition> list = (IList<VisualTransition>)group.Transitions;
		if (list != null)
		{
			foreach (VisualTransition item in list)
			{
				if (val2 == null && IsDefault(item))
				{
					val2 = item;
					continue;
				}
				int num2 = -1;
				VisualState state = group.GetState(item.From);
				VisualState state2 = group.GetState(item.To);
				if (from == state)
				{
					num2++;
				}
				else if (state != null)
				{
					continue;
				}
				if (to == state2)
				{
					num2 += 2;
				}
				else if (state2 != null)
				{
					continue;
				}
				if (num2 > num)
				{
					num = num2;
					val = item;
				}
			}
		}
		return val ?? val2;
	}

	internal static bool IsDefault(VisualTransition transition)
	{
		if (transition.From == null)
		{
			return transition.To == null;
		}
		return false;
	}
}
