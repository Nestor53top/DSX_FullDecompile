using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ModernWpf.Media.Animation;

public sealed class FadeOutThemeAnimation : DoubleAnimation
{
	private static readonly Duration DefaultDuration;

	public static readonly DependencyProperty TargetNameProperty;

	public string TargetName
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(TargetNameProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TargetNameProperty, (object)value);
		}
	}

	static FadeOutThemeAnimation()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		DefaultDuration = Duration.op_Implicit(TimeSpan.FromMilliseconds(167.0));
		TargetNameProperty = Storyboard.TargetNameProperty.AddOwner(typeof(FadeOutThemeAnimation));
		DoubleAnimation.ToProperty.OverrideMetadata(typeof(FadeOutThemeAnimation), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.0));
		Timeline.DurationProperty.OverrideMetadata(typeof(FadeOutThemeAnimation), (PropertyMetadata)new FrameworkPropertyMetadata((object)DefaultDuration));
		Storyboard.TargetPropertyProperty.OverrideMetadata(typeof(FadeOutThemeAnimation), (PropertyMetadata)new FrameworkPropertyMetadata((object)new PropertyPath((object)UIElement.OpacityProperty)));
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new FadeOutThemeAnimation();
	}
}
