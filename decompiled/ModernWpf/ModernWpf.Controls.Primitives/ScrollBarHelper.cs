using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public static class ScrollBarHelper
{
	private const string StateExpanded = "Expanded";

	private const string StateCollapsed = "Collapsed";

	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ScrollBarHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnIsEnabledChanged)));

	public static readonly DependencyProperty IndicatorModeProperty = DependencyProperty.RegisterAttached("IndicatorMode", typeof(ScrollingIndicatorMode), typeof(ScrollBarHelper), new PropertyMetadata((object)ScrollingIndicatorMode.MouseIndicator, new PropertyChangedCallback(OnIndicatorModeChanged)));

	public static readonly DependencyProperty CollapsedThumbBackgroundColorProperty = DependencyProperty.RegisterAttached("CollapsedThumbBackgroundColor", typeof(Color?), typeof(ScrollBarHelper), new PropertyMetadata((object)null));

	public static readonly DependencyProperty ExpandedThumbBackgroundColorProperty = DependencyProperty.RegisterAttached("ExpandedThumbBackgroundColor", typeof(Color?), typeof(ScrollBarHelper), new PropertyMetadata((object)null));

	public static bool GetIsEnabled(ScrollBar scrollBar)
	{
		return (bool)((DependencyObject)scrollBar).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(ScrollBar scrollBar, bool value)
	{
		((DependencyObject)scrollBar).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		ScrollBar val = (ScrollBar)d;
		_ = (bool)((DependencyPropertyChangedEventArgs)(ref e)).OldValue;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((UIElement)val).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
			((UIElement)val).MouseEnter += new MouseEventHandler(OnIsMouseOverChanged);
			((UIElement)val).MouseLeave += new MouseEventHandler(OnIsMouseOverChanged);
			((UIElement)val).IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
			if (((FrameworkElement)val).IsLoaded)
			{
				UpdateVisualState(val);
			}
		}
		else
		{
			((UIElement)val).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
			((UIElement)val).MouseEnter -= new MouseEventHandler(OnIsMouseOverChanged);
			((UIElement)val).MouseLeave -= new MouseEventHandler(OnIsMouseOverChanged);
			((UIElement)val).IsEnabledChanged -= new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
		}
	}

	public static ScrollingIndicatorMode GetIndicatorMode(ScrollBar scrollBar)
	{
		return (ScrollingIndicatorMode)((DependencyObject)scrollBar).GetValue(IndicatorModeProperty);
	}

	public static void SetIndicatorMode(ScrollBar scrollBar, ScrollingIndicatorMode value)
	{
		((DependencyObject)scrollBar).SetValue(IndicatorModeProperty, (object)value);
	}

	private static void OnIndicatorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		UpdateVisualState((ScrollBar)d);
	}

	public static Color? GetCollapsedThumbBackgroundColor(ScrollBar scrollBar)
	{
		return (Color?)((DependencyObject)scrollBar).GetValue(CollapsedThumbBackgroundColorProperty);
	}

	public static void SetCollapsedThumbBackgroundColor(ScrollBar scrollBar, Color? value)
	{
		((DependencyObject)scrollBar).SetValue(CollapsedThumbBackgroundColorProperty, (object)value);
	}

	public static Color? GetExpandedThumbBackgroundColor(ScrollBar scrollBar)
	{
		return (Color?)((DependencyObject)scrollBar).GetValue(ExpandedThumbBackgroundColorProperty);
	}

	public static void SetExpandedThumbBackgroundColor(ScrollBar scrollBar, Color? value)
	{
		((DependencyObject)scrollBar).SetValue(ExpandedThumbBackgroundColorProperty, (object)value);
	}

	private static void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			ScrollBar val = (ScrollBar)sender;
			((FrameworkElement)val).ApplyTemplate();
			UpdateVisualState(val, useTransitions: false);
		}
	}

	private static void OnIsMouseOverChanged(object sender, MouseEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		ScrollBar val = (ScrollBar)sender;
		if (((UIElement)val).IsEnabled)
		{
			UpdateVisualState(val);
		}
	}

	private static void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		UpdateVisualState((ScrollBar)sender);
	}

	private static void UpdateVisualState(ScrollBar scrollBar, bool useTransitions = true)
	{
		string text;
		if (((UIElement)scrollBar).IsEnabled)
		{
			text = ((GetIndicatorMode(scrollBar) == ScrollingIndicatorMode.MouseIndicator) ? "Expanded" : (((UIElement)scrollBar).IsMouseOver ? "Expanded" : "Collapsed"));
		}
		else
		{
			text = "Collapsed";
			useTransitions = false;
		}
		VisualStateManager.GoToState((FrameworkElement)(object)scrollBar, text, useTransitions);
	}
}
