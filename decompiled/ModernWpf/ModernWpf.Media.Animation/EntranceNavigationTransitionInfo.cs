using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWpf.Media.Animation;

public sealed class EntranceNavigationTransitionInfo : NavigationTransitionInfo
{
	internal override NavigationAnimation GetEnterAnimation(FrameworkElement element, bool movingBackwards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00cb: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005c: Expected O, but got Unknown
		Storyboard val = new Storyboard();
		if (movingBackwards)
		{
			DoubleAnimationUsingKeyFrames val2 = new DoubleAnimationUsingKeyFrames();
			val2.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val2.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(1.0, KeyTime.op_Implicit(NavigationTransitionInfo.EnterDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val3 = val2;
			Storyboard.SetTargetProperty((DependencyObject)(object)val3, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val3);
		}
		else
		{
			DoubleAnimationUsingKeyFrames val4 = new DoubleAnimationUsingKeyFrames();
			val4.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(200.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val4.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(0.0, KeyTime.op_Implicit(NavigationTransitionInfo.EnterDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val5 = val4;
			Storyboard.SetTargetProperty((DependencyObject)(object)val5, NavigationTransitionInfo.TranslateYPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val5);
			DoubleAnimation val6 = new DoubleAnimation(1.0, Duration.op_Implicit(TimeSpan.Zero));
			Storyboard.SetTargetProperty((DependencyObject)(object)val6, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val6);
			((DependencyObject)element).SetCurrentValue(UIElement.RenderTransformProperty, (object)new TranslateTransform());
		}
		return new NavigationAnimation(element, val);
	}

	internal override NavigationAnimation GetExitAnimation(FrameworkElement element, bool movingBackwards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_0140: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_005f: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c4: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		Storyboard val = new Storyboard();
		if (movingBackwards)
		{
			DoubleAnimationUsingKeyFrames val2 = new DoubleAnimationUsingKeyFrames();
			val2.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val2.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(200.0, KeyTime.op_Implicit(NavigationTransitionInfo.ExitDuration), NavigationTransitionInfo.AccelerateKeySpline));
			DoubleAnimationUsingKeyFrames val3 = val2;
			Storyboard.SetTargetProperty((DependencyObject)(object)val3, NavigationTransitionInfo.TranslateYPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val3);
			DoubleAnimationUsingKeyFrames val4 = new DoubleAnimationUsingKeyFrames();
			val4.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val4.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(NavigationTransitionInfo.ExitDuration)));
			DoubleAnimationUsingKeyFrames val5 = val4;
			Storyboard.SetTargetProperty((DependencyObject)(object)val5, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val5);
			((DependencyObject)element).SetCurrentValue(UIElement.RenderTransformProperty, (object)new TranslateTransform());
		}
		else
		{
			DoubleAnimationUsingKeyFrames val6 = new DoubleAnimationUsingKeyFrames();
			val6.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val6.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(0.0, KeyTime.op_Implicit(NavigationTransitionInfo.ExitDuration), NavigationTransitionInfo.AccelerateKeySpline));
			DoubleAnimationUsingKeyFrames val7 = val6;
			Storyboard.SetTargetProperty((DependencyObject)(object)val7, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val7);
		}
		return new NavigationAnimation(element, val);
	}
}
