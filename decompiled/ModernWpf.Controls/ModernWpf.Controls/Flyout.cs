using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

[ContentProperty("Content")]
[StyleTypedProperty(Property = "FlyoutPresenterStyle", StyleTargetType = typeof(FlyoutPresenter))]
public class Flyout : FlyoutBase
{
	private enum AnimateFrom
	{
		None,
		Top,
		Bottom,
		Left,
		Right
	}

	private const double c_translation = 40.0;

	private static readonly TimeSpan s_translateDuration = TimeSpan.FromMilliseconds(367.0);

	private static readonly PropertyPath s_opacityPath = new PropertyPath((object)UIElement.OpacityProperty);

	private static readonly PropertyPath s_translateXPath = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)", Array.Empty<object>());

	private static readonly PropertyPath s_translateYPath = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)", Array.Empty<object>());

	private static readonly KeySpline s_decelerateKeySpline = new KeySpline(0.1, 0.9, 0.2, 1.0);

	private static readonly BitmapCache s_bitmapCacheMode = new BitmapCache();

	public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(Flyout));

	public static readonly DependencyProperty FlyoutPresenterStyleProperty = DependencyProperty.Register("FlyoutPresenterStyle", typeof(Style), typeof(Flyout));

	private Storyboard m_openingStoryboard;

	private DoubleKeyFrame m_fromHorizontalOffsetKeyFrame;

	private DoubleKeyFrame m_fromVerticalOffsetKeyFrame;

	public UIElement Content
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (UIElement)((DependencyObject)this).GetValue(ContentProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentProperty, (object)value);
		}
	}

	public Style FlyoutPresenterStyle
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Style)((DependencyObject)this).GetValue(FlyoutPresenterStyleProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FlyoutPresenterStyleProperty, (object)value);
		}
	}

	internal override PopupAnimation DesiredPopupAnimation => (PopupAnimation)0;

	private bool IsPopupOpenDown
	{
		get
		{
			if (TryGetPopupOffset(out var offset))
			{
				return ((Point)(ref offset)).Y > 0.0;
			}
			return false;
		}
	}

	private bool IsPopupOpenRight
	{
		get
		{
			if (TryGetPopupOffset(out var offset))
			{
				return ((Point)(ref offset)).X > 0.0;
			}
			return false;
		}
	}

	protected override Control CreatePresenter()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		FlyoutPresenter flyoutPresenter = new FlyoutPresenter();
		((FrameworkElement)flyoutPresenter).SetBinding(ContentControl.ContentProperty, (BindingBase)new Binding
		{
			Path = new PropertyPath((object)ContentProperty),
			Source = this
		});
		((FrameworkElement)flyoutPresenter).SetBinding(FrameworkElement.StyleProperty, (BindingBase)new Binding
		{
			Path = new PropertyPath((object)FlyoutPresenterStyleProperty),
			Source = this
		});
		return (Control)(object)flyoutPresenter;
	}

	internal override void OnOpened()
	{
		if (base.AreOpenCloseAnimationsEnabled && SharedHelpers.IsAnimationsEnabled)
		{
			PlayOpenAnimation();
		}
		base.OnOpened();
	}

	internal override void OnClosed()
	{
		if (m_openingStoryboard != null)
		{
			UIElement child = ((Popup)base.InternalPopup).Child;
			Control val = (Control)(object)((child is Control) ? child : null);
			if (val != null)
			{
				m_openingStoryboard.Stop((FrameworkElement)(object)val);
			}
		}
		base.OnClosed();
	}

	private void PlayOpenAnimation()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		Control val = (Control)((Popup)base.InternalPopup).Child;
		EnsureOpeningStoryboard(val);
		AnimateFrom animateFrom = GetAnimateFrom();
		UpdateFromOffsetKeyFrames(animateFrom);
		if (!(((UIElement)val).RenderTransform is TranslateTransform))
		{
			((UIElement)val).RenderTransform = (Transform)new TranslateTransform();
		}
		if (animateFrom != AnimateFrom.None)
		{
			DpiScale dpi = VisualTreeHelper.GetDpi((Visual)(object)val);
			BitmapCache cacheMode = new BitmapCache(((DpiScale)(ref dpi)).PixelsPerDip);
			((UIElement)val).CacheMode = (CacheMode)(object)cacheMode;
		}
		m_openingStoryboard.Begin((FrameworkElement)(object)val, true);
	}

	private void EnsureOpeningStoryboard(Control presenter)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009c: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00df: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0111: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_0154: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_0186: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		if (m_openingStoryboard == null)
		{
			DoubleAnimationUsingKeyFrames val = new DoubleAnimationUsingKeyFrames();
			val.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero)));
			val.KeyFrames.Add((DoubleKeyFrame)new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.FromMilliseconds(83.0))));
			val.KeyFrames.Add((DoubleKeyFrame)new LinearDoubleKeyFrame(1.0, KeyTime.op_Implicit(TimeSpan.FromMilliseconds(166.0))));
			DoubleAnimationUsingKeyFrames val2 = val;
			Storyboard.SetTarget((DependencyObject)(object)val2, (DependencyObject)(object)presenter);
			Storyboard.SetTargetProperty((DependencyObject)(object)val2, s_opacityPath);
			DoubleAnimationUsingKeyFrames val3 = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrameCollection keyFrames = val3.KeyFrames;
			DiscreteDoubleKeyFrame val4 = new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero));
			DoubleKeyFrame val5 = (DoubleKeyFrame)val4;
			m_fromHorizontalOffsetKeyFrame = (DoubleKeyFrame)val4;
			keyFrames.Add(val5);
			val3.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(0.0, KeyTime.op_Implicit(s_translateDuration), s_decelerateKeySpline));
			DoubleAnimationUsingKeyFrames val6 = val3;
			Storyboard.SetTarget((DependencyObject)(object)val6, (DependencyObject)(object)presenter);
			Storyboard.SetTargetProperty((DependencyObject)(object)val6, s_translateXPath);
			DoubleAnimationUsingKeyFrames val7 = new DoubleAnimationUsingKeyFrames();
			DoubleKeyFrameCollection keyFrames2 = val7.KeyFrames;
			DiscreteDoubleKeyFrame val8 = new DiscreteDoubleKeyFrame(0.0, KeyTime.op_Implicit(TimeSpan.Zero));
			val5 = (DoubleKeyFrame)val8;
			m_fromVerticalOffsetKeyFrame = (DoubleKeyFrame)val8;
			keyFrames2.Add(val5);
			val7.KeyFrames.Add((DoubleKeyFrame)new SplineDoubleKeyFrame(0.0, KeyTime.op_Implicit(s_translateDuration), s_decelerateKeySpline));
			DoubleAnimationUsingKeyFrames val9 = val7;
			Storyboard.SetTarget((DependencyObject)(object)val9, (DependencyObject)(object)presenter);
			Storyboard.SetTargetProperty((DependencyObject)(object)val9, s_translateYPath);
			Storyboard val10 = new Storyboard();
			((TimelineGroup)val10).Children.Add((Timeline)(object)val2);
			((TimelineGroup)val10).Children.Add((Timeline)(object)val6);
			((TimelineGroup)val10).Children.Add((Timeline)(object)val9);
			((Timeline)val10).FillBehavior = (FillBehavior)1;
			m_openingStoryboard = val10;
			((Timeline)m_openingStoryboard).Completed += delegate
			{
				((DependencyObject)presenter).ClearValue(UIElement.CacheModeProperty);
			};
		}
	}

	private AnimateFrom GetAnimateFrom()
	{
		if (((Popup)base.InternalPopup).PlacementTarget != null)
		{
			switch (base.Placement)
			{
			case FlyoutPlacementMode.Top:
			case FlyoutPlacementMode.Bottom:
			case FlyoutPlacementMode.TopEdgeAlignedLeft:
			case FlyoutPlacementMode.TopEdgeAlignedRight:
			case FlyoutPlacementMode.BottomEdgeAlignedLeft:
			case FlyoutPlacementMode.BottomEdgeAlignedRight:
				if (!IsPopupOpenDown)
				{
					return AnimateFrom.Bottom;
				}
				return AnimateFrom.Top;
			case FlyoutPlacementMode.Left:
			case FlyoutPlacementMode.Right:
			case FlyoutPlacementMode.LeftEdgeAlignedTop:
			case FlyoutPlacementMode.LeftEdgeAlignedBottom:
			case FlyoutPlacementMode.RightEdgeAlignedTop:
			case FlyoutPlacementMode.RightEdgeAlignedBottom:
				if (!IsPopupOpenRight)
				{
					return AnimateFrom.Right;
				}
				return AnimateFrom.Left;
			}
		}
		return AnimateFrom.None;
	}

	private void UpdateFromOffsetKeyFrames(AnimateFrom animateFrom)
	{
		switch (animateFrom)
		{
		case AnimateFrom.None:
			m_fromHorizontalOffsetKeyFrame.Value = 0.0;
			m_fromVerticalOffsetKeyFrame.Value = 0.0;
			break;
		case AnimateFrom.Top:
			m_fromHorizontalOffsetKeyFrame.Value = 0.0;
			m_fromVerticalOffsetKeyFrame.Value = -40.0;
			break;
		case AnimateFrom.Bottom:
			m_fromHorizontalOffsetKeyFrame.Value = 0.0;
			m_fromVerticalOffsetKeyFrame.Value = 40.0;
			break;
		case AnimateFrom.Left:
			m_fromHorizontalOffsetKeyFrame.Value = -40.0;
			m_fromVerticalOffsetKeyFrame.Value = 0.0;
			break;
		case AnimateFrom.Right:
			m_fromHorizontalOffsetKeyFrame.Value = 40.0;
			m_fromVerticalOffsetKeyFrame.Value = 0.0;
			break;
		}
	}

	private bool TryGetPopupOffset(out Point offset)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		UIElement child = ((Popup)base.InternalPopup).Child;
		UIElement placementTarget = ((Popup)base.InternalPopup).PlacementTarget;
		if (child != null && placementTarget != null && child.IsVisible && placementTarget.IsVisible)
		{
			offset = child.TranslatePoint(new Point(0.0, 0.0), placementTarget);
			return true;
		}
		offset = default(Point);
		return false;
	}
}
