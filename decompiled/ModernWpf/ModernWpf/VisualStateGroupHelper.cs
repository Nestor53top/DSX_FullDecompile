using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Animation;

namespace ModernWpf;

internal static class VisualStateGroupHelper
{
	private static readonly DependencyProperty CurrentStoryboardsProperty = DependencyProperty.RegisterAttached("CurrentStoryboards", typeof(Collection<Storyboard>), typeof(VisualStateGroupHelper));

	private static readonly Lazy<Action<VisualStateGroup, VisualState>> _setCurrentState = new Lazy<Action<VisualStateGroup, VisualState>>(CreateSetCurrentStateDelegate);

	internal static bool IsSupported => _setCurrentState.Value != null;

	internal static void SetCurrentState(this VisualStateGroup group, VisualState value)
	{
		if (!IsSupported)
		{
			throw new InvalidOperationException();
		}
		_setCurrentState.Value(group, value);
	}

	internal static VisualState GetState(this VisualStateGroup group, string stateName)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		for (int i = 0; i < group.States.Count; i++)
		{
			VisualState val = (VisualState)group.States[i];
			if (val.Name == stateName)
			{
				return val;
			}
		}
		return null;
	}

	internal static Collection<Storyboard> GetCurrentStoryboards(VisualStateGroup group)
	{
		Collection<Storyboard> collection = (Collection<Storyboard>)((DependencyObject)group).GetValue(CurrentStoryboardsProperty);
		if (collection == null)
		{
			collection = new Collection<Storyboard>();
			((DependencyObject)group).SetValue(CurrentStoryboardsProperty, (object)collection);
		}
		return collection;
	}

	internal static void StartNewThenStopOld(this VisualStateGroup group, FrameworkElement element, params Storyboard[] newStoryboards)
	{
		Collection<Storyboard> currentStoryboards = GetCurrentStoryboards(group);
		for (int i = 0; i < currentStoryboards.Count; i++)
		{
			if (currentStoryboards[i] != null)
			{
				currentStoryboards[i].Remove(element);
			}
		}
		currentStoryboards.Clear();
		for (int j = 0; j < newStoryboards.Length; j++)
		{
			if (newStoryboards[j] != null)
			{
				newStoryboards[j].Begin(element, (HandoffBehavior)0, true);
				currentStoryboards.Add(newStoryboards[j]);
			}
		}
	}

	private static Action<VisualStateGroup, VisualState> CreateSetCurrentStateDelegate()
	{
		try
		{
			return DelegateHelper.CreatePropertySetter<VisualStateGroup, VisualState>("CurrentState", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, nonPublic: true);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
