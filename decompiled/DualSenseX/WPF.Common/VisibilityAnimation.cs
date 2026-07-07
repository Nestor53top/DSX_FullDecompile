using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace WPF.Common;

public class VisibilityAnimation
{
	public enum AnimationType
	{
		None,
		Fade
	}

	public const int AnimationDuration = 100;

	private static readonly Dictionary<FrameworkElement, bool> _hookedElements;

	public static readonly DependencyProperty AnimationTypeProperty;

	public static AnimationType GetAnimationType(DependencyObject obj)
	{
		return (AnimationType)obj.GetValue(AnimationTypeProperty);
	}

	public static void SetAnimationType(DependencyObject obj, AnimationType value)
	{
		obj.SetValue(AnimationTypeProperty, (object)value);
	}

	private static void OnAnimationTypePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		FrameworkElement val = (FrameworkElement)(object)((dependencyObject is FrameworkElement) ? dependencyObject : null);
		if (val != null)
		{
			if (GetAnimationType((DependencyObject)(object)val) != AnimationType.None)
			{
				HookVisibilityChanges(val);
			}
			else
			{
				UnHookVisibilityChanges(val);
			}
		}
	}

	private static void HookVisibilityChanges(FrameworkElement frameworkElement)
	{
		_hookedElements.Add(frameworkElement, value: false);
	}

	private static void UnHookVisibilityChanges(FrameworkElement frameworkElement)
	{
		if (_hookedElements.ContainsKey(frameworkElement))
		{
			_hookedElements.Remove(frameworkElement);
		}
	}

	static VisibilityAnimation()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0076: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		_hookedElements = new Dictionary<FrameworkElement, bool>();
		AnimationTypeProperty = DependencyProperty.RegisterAttached("AnimationType", typeof(AnimationType), typeof(VisibilityAnimation), (PropertyMetadata)new FrameworkPropertyMetadata((object)AnimationType.None, new PropertyChangedCallback(OnAnimationTypePropertyChanged)));
		UIElement.VisibilityProperty.AddOwner(typeof(FrameworkElement), (PropertyMetadata)new FrameworkPropertyMetadata((object)(Visibility)0, new PropertyChangedCallback(VisibilityChanged), new CoerceValueCallback(CoerceVisibility)));
	}

	private static void VisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
	}

	private static object CoerceVisibility(DependencyObject dependencyObject, object baseValue)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		FrameworkElement frameworkElement = (FrameworkElement)(object)((dependencyObject is FrameworkElement) ? dependencyObject : null);
		if (frameworkElement == null)
		{
			return baseValue;
		}
		Visibility visibility = (Visibility)baseValue;
		if (visibility == ((UIElement)frameworkElement).Visibility)
		{
			return baseValue;
		}
		if (!IsHookedElement(frameworkElement))
		{
			return baseValue;
		}
		if (UpdateAnimationStartedFlag(frameworkElement))
		{
			return baseValue;
		}
		DoubleAnimation val = new DoubleAnimation
		{
			Duration = new Duration(TimeSpan.FromMilliseconds(100.0))
		};
		((Timeline)val).Completed += delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			if ((int)visibility == 0)
			{
				UpdateAnimationStartedFlag(frameworkElement);
			}
			else if (BindingOperations.IsDataBound((DependencyObject)(object)frameworkElement, UIElement.VisibilityProperty))
			{
				Binding binding = BindingOperations.GetBinding((DependencyObject)(object)frameworkElement, UIElement.VisibilityProperty);
				BindingOperations.SetBinding((DependencyObject)(object)frameworkElement, UIElement.VisibilityProperty, (BindingBase)(object)binding);
			}
			else
			{
				((UIElement)frameworkElement).Visibility = visibility;
			}
		};
		if ((int)visibility == 2 || (int)visibility == 1)
		{
			val.From = 1.0;
			val.To = 0.0;
		}
		else
		{
			val.From = 0.0;
			val.To = 1.0;
		}
		((UIElement)frameworkElement).BeginAnimation(UIElement.OpacityProperty, (AnimationTimeline)(object)val);
		return (object)(Visibility)0;
	}

	private static bool IsHookedElement(FrameworkElement frameworkElement)
	{
		return _hookedElements.ContainsKey(frameworkElement);
	}

	private static bool UpdateAnimationStartedFlag(FrameworkElement frameworkElement)
	{
		bool flag = _hookedElements[frameworkElement];
		_hookedElements[frameworkElement] = !flag;
		return flag;
	}
}
