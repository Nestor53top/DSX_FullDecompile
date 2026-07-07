using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernWpf.Media.Animation;

public sealed class SlideNavigationTransitionInfo : NavigationTransitionInfo, ISlideNavigationTransitionInfo2
{
	public static readonly DependencyProperty EffectProperty = DependencyProperty.Register("Effect", typeof(SlideNavigationTransitionEffect), typeof(SlideNavigationTransitionInfo), (PropertyMetadata)null);

	public SlideNavigationTransitionEffect Effect
	{
		get
		{
			return (SlideNavigationTransitionEffect)((DependencyObject)this).GetValue(EffectProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EffectProperty, (object)value);
		}
	}

	internal override NavigationAnimation GetEnterAnimation(FrameworkElement element, bool movingBackwards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d8: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0069: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Expected O, but got Unknown
		//IL_01a2: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		Storyboard val = new Storyboard();
		SlideNavigationTransitionEffect effect = Effect;
		if (effect == SlideNavigationTransitionEffect.FromBottom)
		{
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
		}
		else
		{
			bool flag = ((effect != SlideNavigationTransitionEffect.FromLeft) ? movingBackwards : (!movingBackwards));
			DoubleAnimationUsingKeyFrames val7 = new DoubleAnimationUsingKeyFrames();
			val7.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame((double)(flag ? (-200) : 200), KeyTime.op_Implicit(TimeSpan.Zero)));
			val7.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(0.0, KeyTime.op_Implicit(NavigationTransitionInfo.EnterDuration), NavigationTransitionInfo.DecelerateKeySpline));
			DoubleAnimationUsingKeyFrames val8 = val7;
			Storyboard.SetTargetProperty((DependencyObject)(object)val8, NavigationTransitionInfo.TranslateXPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val8);
			DoubleAnimation val9 = new DoubleAnimation(1.0, Duration.op_Implicit(TimeSpan.Zero));
			Storyboard.SetTargetProperty((DependencyObject)(object)val9, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val9);
			((DependencyObject)element).SetCurrentValue(UIElement.RenderTransformProperty, (object)new TranslateTransform());
		}
		return new NavigationAnimation(element, val);
	}

	internal override NavigationAnimation GetExitAnimation(FrameworkElement element, bool movingBackwards)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0151: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00d1: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01da: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0242: Expected O, but got Unknown
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		Storyboard val = new Storyboard();
		SlideNavigationTransitionEffect effect = Effect;
		if (effect == SlideNavigationTransitionEffect.FromBottom)
		{
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
		}
		else
		{
			bool flag = ((effect != SlideNavigationTransitionEffect.FromLeft) ? (!movingBackwards) : movingBackwards);
			DoubleAnimationUsingKeyFrames val8 = new DoubleAnimationUsingKeyFrames();
			val8.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val8.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame((double)(flag ? (-200) : 200), KeyTime.op_Implicit(NavigationTransitionInfo.ExitDuration), NavigationTransitionInfo.AccelerateKeySpline));
			DoubleAnimationUsingKeyFrames val9 = val8;
			Storyboard.SetTargetProperty((DependencyObject)(object)val9, NavigationTransitionInfo.TranslateXPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val9);
			DoubleAnimationUsingKeyFrames val10 = new DoubleAnimationUsingKeyFrames();
			val10.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(1.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val10.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(NavigationTransitionInfo.ExitDuration)));
			DoubleAnimationUsingKeyFrames val11 = val10;
			Storyboard.SetTargetProperty((DependencyObject)(object)val11, NavigationTransitionInfo.OpacityPath);
			((TimelineGroup)val).Children.Add((Timeline)(object)val11);
			((DependencyObject)element).SetCurrentValue(UIElement.RenderTransformProperty, (object)new TranslateTransform());
		}
		return new NavigationAnimation(element, val);
	}
}
