using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWpf.Media.Animation;

public sealed class DrillInNavigationTransitionInfo : NavigationTransitionInfo
{
	internal override NavigationAnimation GetEnterAnimation(FrameworkElement element, bool movingBackwards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01a3: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Expected O, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		//IL_0210: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Expected O, but got Unknown
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_027d: Expected O, but got Unknown
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
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00c9: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0133: Expected O, but got Unknown
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		Storyboard val = new Storyboard();
		if (movingBackwards)
		{
			DoubleAnimationUsingKeyFrames val2 = new DoubleAnimationUsingKeyFrames();
			val2.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.15, KeyTime.op_Implicit(TimeSpan.Zero)));
			val2.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(1.0, KeyTime.op_Implicit(NavigationTransitionInfo.EnterDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val3 = val2;
			Storyboard.SetTargetProperty((DependencyObject)(object)val3, NavigationTransitionInfo.ScaleXPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val3);
			DoubleAnimationUsingKeyFrames val4 = new DoubleAnimationUsingKeyFrames();
			val4.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.15, KeyTime.op_Implicit(TimeSpan.Zero)));
			val4.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(1.0, KeyTime.op_Implicit(NavigationTransitionInfo.EnterDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val5 = val4;
			Storyboard.SetTargetProperty((DependencyObject)(object)val5, NavigationTransitionInfo.ScaleYPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val5);
			DoubleAnimationUsingKeyFrames val6 = new DoubleAnimationUsingKeyFrames();
			val6.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val6.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(1.0, KeyTime.op_Implicit(NavigationTransitionInfo.EnterDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val7 = val6;
			Storyboard.SetTargetProperty((DependencyObject)(object)val7, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val7);
		}
		else
		{
			DoubleAnimationUsingKeyFrames val8 = new DoubleAnimationUsingKeyFrames();
			val8.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.9, KeyTime.op_Implicit(TimeSpan.Zero)));
			val8.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(1.0, KeyTime.op_Implicit(NavigationTransitionInfo.MaxMoveDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val9 = val8;
			Storyboard.SetTargetProperty((DependencyObject)(object)val9, NavigationTransitionInfo.ScaleXPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val9);
			DoubleAnimationUsingKeyFrames val10 = new DoubleAnimationUsingKeyFrames();
			val10.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.9, KeyTime.op_Implicit(TimeSpan.Zero)));
			val10.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(1.0, KeyTime.op_Implicit(NavigationTransitionInfo.MaxMoveDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val11 = val10;
			Storyboard.SetTargetProperty((DependencyObject)(object)val11, NavigationTransitionInfo.ScaleYPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val11);
			DoubleAnimationUsingKeyFrames val12 = new DoubleAnimationUsingKeyFrames();
			val12.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val12.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(1.0, KeyTime.op_Implicit(NavigationTransitionInfo.MaxMoveDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val13 = val12;
			Storyboard.SetTargetProperty((DependencyObject)(object)val13, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val13);
		}
		((DependencyObject)element).SetCurrentValue(UIElement.RenderTransformProperty, (object)new ScaleTransform());
		((DependencyObject)element).SetCurrentValue(UIElement.RenderTransformOriginProperty, (object)new Point(0.5, 0.5));
		return new NavigationAnimation(element, val);
	}

	internal override NavigationAnimation GetExitAnimation(FrameworkElement element, bool movingBackwards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0059: Expected O, but got Unknown
		Storyboard val = new Storyboard();
		DoubleAnimationUsingKeyFrames val2 = new DoubleAnimationUsingKeyFrames();
		val2.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.0, KeyTime.op_Implicit(TimeSpan.Zero)));
		val2.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(0.0, KeyTime.op_Implicit(NavigationTransitionInfo.ExitDuration), NavigationTransitionInfo.AccelerateKeySpline));
		DoubleAnimationUsingKeyFrames val3 = val2;
		Storyboard.SetTargetProperty((DependencyObject)(object)val3, NavigationTransitionInfo.OpacityPath);
		((TimelineGroup)val).Children.Add((Timeline)(object)val3);
		return new NavigationAnimation(element, val);
	}
}
