using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class DropDownButton : Button
{
	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty FocusVisualMarginProperty;

	public static readonly DependencyProperty FlyoutProperty;

	private bool m_isFlyoutOpen;

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

	public bool UseSystemFocusVisuals
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(UseSystemFocusVisualsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UseSystemFocusVisualsProperty, (object)value);
		}
	}

	public Thickness FocusVisualMargin
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Thickness)((DependencyObject)this).GetValue(FocusVisualMarginProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(FocusVisualMarginProperty, (object)value);
		}
	}

	public FlyoutBase Flyout
	{
		get
		{
			return (FlyoutBase)((DependencyObject)this).GetValue(FlyoutProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FlyoutProperty, (object)value);
		}
	}

	internal bool IsFlyoutOpen => m_isFlyoutOpen;

	static DropDownButton()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(DropDownButton));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(DropDownButton));
		FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(DropDownButton));
		FlyoutProperty = FlyoutService.FlyoutProperty.AddOwner(typeof(DropDownButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlyoutPropertyChanged)));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(DropDownButton)));
	}

	private static void OnFlyoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DropDownButton)(object)d).OnFlyoutPropertyChanged(e);
	}

	private void OnFlyoutPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is FlyoutBase flyoutBase)
		{
			flyoutBase.Opened -= OnFlyoutOpened;
			flyoutBase.Closed -= OnFlyoutClosed;
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue is FlyoutBase flyoutBase2)
		{
			flyoutBase2.Opened += OnFlyoutOpened;
			flyoutBase2.Closed += OnFlyoutClosed;
		}
	}

	internal void OpenFlyout()
	{
		Flyout?.ShowAt((FrameworkElement)(object)this);
	}

	internal void CloseFlyout()
	{
		Flyout?.Hide();
	}

	private void OnFlyoutOpened(object sender, object e)
	{
		m_isFlyoutOpen = true;
		SharedHelpers.RaiseAutomationPropertyChangedEvent((UIElement)(object)this, (object)(ExpandCollapseState)0, (object)(ExpandCollapseState)1);
	}

	private void OnFlyoutClosed(object sender, object e)
	{
		m_isFlyoutOpen = false;
		SharedHelpers.RaiseAutomationPropertyChangedEvent((UIElement)(object)this, (object)(ExpandCollapseState)1, (object)(ExpandCollapseState)0);
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new DropDownButtonAutomationPeer(this);
	}
}
