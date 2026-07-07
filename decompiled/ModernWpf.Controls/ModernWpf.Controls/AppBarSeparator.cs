using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class AppBarSeparator : Control, ICommandBarElement, IAppBarElement
{
	public static readonly DependencyProperty IsCompactProperty;

	public static readonly DependencyProperty IsInOverflowProperty;

	private static readonly DependencyProperty ApplicationViewStateProperty;

	public bool IsCompact
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsCompactProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsCompactProperty, (object)value);
		}
	}

	public bool IsInOverflow => (bool)((DependencyObject)this).GetValue(IsInOverflowProperty);

	private AppBarElementApplicationViewState ApplicationViewState => (AppBarElementApplicationViewState)((DependencyObject)this).GetValue(ApplicationViewStateProperty);

	static AppBarSeparator()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		IsCompactProperty = AppBarElementProperties.IsCompactProperty.AddOwner(typeof(AppBarSeparator));
		IsInOverflowProperty = AppBarElementProperties.IsInOverflowProperty.AddOwner(typeof(AppBarSeparator));
		ApplicationViewStateProperty = AppBarElementProperties.ApplicationViewStateProperty.AddOwner(typeof(AppBarSeparator));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AppBarSeparator), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(AppBarSeparator)));
		UIElement.FocusableProperty.OverrideMetadata(typeof(AppBarSeparator), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
		ToolBar.OverflowModeProperty.OverrideMetadata(typeof(AppBarSeparator), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverflowModePropertyChanged)));
	}

	public AppBarSeparator()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		((UIElement)this).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnIsVisibleChanged);
	}

	private void UpdateApplicationViewState()
	{
		AppBarElementApplicationViewState appBarElementApplicationViewState = ((IsInOverflow && ((UIElement)this).IsVisible) ? AppBarElementApplicationViewState.Overflow : (IsCompact ? AppBarElementApplicationViewState.Compact : AppBarElementApplicationViewState.FullSize));
		((DependencyObject)this).SetValue(AppBarElementProperties.ApplicationViewStatePropertyKey, (object)appBarElementApplicationViewState);
	}

	void IAppBarElement.UpdateApplicationViewState()
	{
		UpdateApplicationViewState();
	}

	void IAppBarElement.ApplyApplicationViewState()
	{
		UpdateVisualState();
	}

	public override void OnApplyTemplate()
	{
		((FrameworkElement)this).OnApplyTemplate();
		UpdateVisualState(useTransitions: false);
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnPropertyChanged(e);
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property == ToolBar.IsOverflowItemProperty)
		{
			AppBarElementProperties.UpdateIsInOverflow((DependencyObject)(object)this);
		}
	}

	private static void OnOverflowModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		AppBarElementProperties.UpdateIsInOverflow(d);
	}

	private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		UpdateApplicationViewState();
	}

	private void UpdateVisualState(bool useTransitions = true)
	{
		VisualStateManager.GoToState((FrameworkElement)(object)this, ApplicationViewState.ToString(), useTransitions);
	}
}
