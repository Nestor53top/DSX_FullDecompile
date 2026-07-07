using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ModernWpf;

internal static class AnimationHelper
{
	public static void DeferBegin(Storyboard storyboard)
	{
		((Timeline)storyboard).CurrentStateInvalidated += OnStoryboardCurrentStateInvalidated;
		static void OnStoryboardCurrentStateInvalidated(object sender, EventArgs e)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			Clock clock = (Clock)((sender is Clock) ? sender : null);
			if (clock != null && clock.HasControllableRoot && (int)clock.CurrentState == 0 && !clock.IsPaused)
			{
				clock.Controller.Pause();
				((DispatcherObject)clock).Dispatcher.BeginInvoke(delegate
				{
					if (clock.IsPaused)
					{
						clock.Controller.Resume();
					}
				}, (DispatcherPriority)6);
			}
		}
	}

	public static void DeferTransitions(VisualStateGroup group)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (VisualTransition transition in group.Transitions)
		{
			Storyboard storyboard = transition.Storyboard;
			if (storyboard != null)
			{
				DeferBegin(storyboard);
			}
		}
	}
}
