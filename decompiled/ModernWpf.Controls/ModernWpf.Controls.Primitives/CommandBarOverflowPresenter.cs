using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public class CommandBarOverflowPresenter : ContentControl
{
	public static readonly DependencyProperty CornerRadiusProperty;

	public CornerRadius CornerRadius
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (CornerRadius)((DependencyObject)this).GetValue(CornerRadiusProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CornerRadiusProperty, (object)value);
		}
	}

	static CommandBarOverflowPresenter()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(CommandBarOverflowPresenter));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandBarOverflowPresenter), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(CommandBarOverflowPresenter)));
	}

	public CommandBarOverflowPresenter()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		((FrameworkElement)this).Loaded += new RoutedEventHandler(OnLoaded);
		((FrameworkElement)this).Unloaded += new RoutedEventHandler(OnUnloaded);
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		UpdateVisualState(useTransitions: false);
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (((UIElement)this).IsVisible)
		{
			UpdateVisualState(useTransitions: true);
		}
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		UpdateVisualState(useTransitions: false);
	}

	private void UpdateVisualState(bool useTransitions)
	{
		string text = "DisplayModeDefault";
		VisualStateManager.GoToState((FrameworkElement)(object)this, text, useTransitions);
	}

	private bool IsPopupOpenDown()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((FrameworkElement)this).TemplatedParent is CommandBarToolBar commandBarToolBar)
		{
			Point val = ((UIElement)this).TranslatePoint(new Point(0.0, 0.0), (UIElement)(object)commandBarToolBar);
			return ((Point)(ref val)).Y > 0.0;
		}
		return true;
	}
}
