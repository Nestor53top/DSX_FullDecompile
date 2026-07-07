using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls;

public class SplitButton : ContentControl, ICommandSource
{
	private class OpenFlyoutCommand : ICommand
	{
		private readonly SplitButton m_owner;

		public event EventHandler CanExecuteChanged;

		public OpenFlyoutCommand(SplitButton owner)
		{
			m_owner = owner;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			m_owner.OpenFlyout();
		}
	}

	private static readonly ResourceAccessor ResourceAccessor;

	public static readonly DependencyProperty CornerRadiusProperty;

	public static readonly DependencyProperty UseSystemFocusVisualsProperty;

	public static readonly DependencyProperty FocusVisualMarginProperty;

	public static readonly DependencyProperty CommandProperty;

	public static readonly DependencyProperty CommandParameterProperty;

	public static readonly DependencyProperty CommandTargetProperty;

	public static readonly DependencyProperty FlyoutProperty;

	private static readonly DependencyProperty PrimaryButtonIsPressedProperty;

	private static readonly DependencyProperty PrimaryButtonIsMouseOverProperty;

	private static readonly DependencyProperty SecondaryButtonIsPressedProperty;

	private static readonly DependencyProperty SecondaryButtonIsMouseOverProperty;

	private static readonly DependencyProperty FlyoutPlacementProperty;

	internal bool m_hasLoaded;

	private Button m_primaryButton;

	private Button m_secondaryButton;

	private bool m_isFlyoutOpen;

	private bool m_isKeyDown;

	private readonly CornerRadiusFilterConverter m_cornerRadiusFilterConverter = new CornerRadiusFilterConverter();

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

	public ICommand Command
	{
		get
		{
			return (ICommand)((DependencyObject)this).GetValue(CommandProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CommandProperty, (object)value);
		}
	}

	public object CommandParameter
	{
		get
		{
			return ((DependencyObject)this).GetValue(CommandParameterProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CommandParameterProperty, value);
		}
	}

	public IInputElement CommandTarget
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (IInputElement)((DependencyObject)this).GetValue(CommandTargetProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(CommandTargetProperty, (object)value);
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

	internal virtual bool InternalIsChecked => false;

	public event TypedEventHandler<SplitButton, SplitButtonClickEventArgs> Click;

	static SplitButton()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Expected O, but got Unknown
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		ResourceAccessor = new ResourceAccessor(typeof(SplitButton));
		CornerRadiusProperty = ControlHelper.CornerRadiusProperty.AddOwner(typeof(SplitButton));
		UseSystemFocusVisualsProperty = FocusVisualHelper.UseSystemFocusVisualsProperty.AddOwner(typeof(SplitButton));
		FocusVisualMarginProperty = FocusVisualHelper.FocusVisualMarginProperty.AddOwner(typeof(SplitButton));
		CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(SplitButton));
		CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(SplitButton));
		CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(SplitButton));
		FlyoutProperty = DependencyProperty.Register("Flyout", typeof(FlyoutBase), typeof(SplitButton), new PropertyMetadata(new PropertyChangedCallback(OnFlyoutChanged)));
		PrimaryButtonIsPressedProperty = DependencyProperty.Register("PrimaryButtonIsPressed", typeof(bool), typeof(SplitButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnVisualPropertyChanged)));
		PrimaryButtonIsMouseOverProperty = DependencyProperty.Register("PrimaryButtonIsMouseOver", typeof(bool), typeof(SplitButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnVisualPropertyChanged)));
		SecondaryButtonIsPressedProperty = DependencyProperty.Register("SecondaryButtonIsPressed", typeof(bool), typeof(SplitButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnVisualPropertyChanged)));
		SecondaryButtonIsMouseOverProperty = DependencyProperty.Register("SecondaryButtonIsMouseOver", typeof(bool), typeof(SplitButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnVisualPropertyChanged)));
		FlyoutPlacementProperty = FlyoutBase.PlacementProperty.AddOwner(typeof(SplitButton), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlyoutPlacementChanged)));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(SplitButton)));
	}

	public SplitButton()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		((UIElement)this).KeyDown += new KeyEventHandler(OnSplitButtonKeyDown);
		((UIElement)this).KeyUp += new KeyEventHandler(OnSplitButtonKeyUp);
		((UIElement)this).InputBindings.Add((InputBinding)new KeyBinding((ICommand)new OpenFlyoutCommand(this), (Key)26, (ModifierKeys)1));
	}

	private static void OnFlyoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((SplitButton)(object)d).OnFlyoutChanged((FlyoutBase)((DependencyPropertyChangedEventArgs)(ref e)).OldValue, (FlyoutBase)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	public override void OnApplyTemplate()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		((FrameworkElement)this).OnApplyTemplate();
		UnregisterEvents();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("PrimaryButton");
		m_primaryButton = (Button)(object)((templateChild is Button) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("SecondaryButton");
		m_secondaryButton = (Button)(object)((templateChild2 is Button) ? templateChild2 : null);
		if (m_primaryButton != null)
		{
			((ButtonBase)m_primaryButton).Click += new RoutedEventHandler(OnClickPrimary);
			((FrameworkElement)(object)this).SetBinding(PrimaryButtonIsPressedProperty, ButtonBase.IsPressedProperty, (DependencyObject)(object)m_primaryButton);
			((FrameworkElement)(object)this).SetBinding(PrimaryButtonIsMouseOverProperty, UIElement.IsMouseOverProperty, (DependencyObject)(object)m_primaryButton);
		}
		if (m_secondaryButton != null)
		{
			string localizedStringResource = ResourceAccessor.GetLocalizedStringResource("SplitButtonSecondaryButtonName");
			AutomationProperties.SetName((DependencyObject)(object)m_secondaryButton, localizedStringResource);
			((ButtonBase)m_secondaryButton).Click += new RoutedEventHandler(OnClickSecondary);
			((FrameworkElement)(object)this).SetBinding(SecondaryButtonIsPressedProperty, ButtonBase.IsPressedProperty, (DependencyObject)(object)m_secondaryButton);
			((FrameworkElement)(object)this).SetBinding(SecondaryButtonIsMouseOverProperty, UIElement.IsMouseOverProperty, (DependencyObject)(object)m_secondaryButton);
		}
		UpdateVisualStates();
		m_hasLoaded = true;
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new SplitButtonAutomationPeer(this);
	}

	private void OnFlyoutChanged(FlyoutBase oldFlyout, FlyoutBase newFlyout)
	{
		RegisterFlyoutEvents(oldFlyout, newFlyout);
		UpdateVisualStates();
	}

	private void RegisterFlyoutEvents(FlyoutBase oldFlyout, FlyoutBase newFlyout)
	{
		if (oldFlyout != null)
		{
			oldFlyout.Opened -= OnFlyoutOpened;
			oldFlyout.Closed -= OnFlyoutClosed;
			((DependencyObject)this).ClearValue(FlyoutPlacementProperty);
		}
		if (newFlyout != null)
		{
			newFlyout.Opened += OnFlyoutOpened;
			newFlyout.Closed += OnFlyoutClosed;
			((FrameworkElement)(object)this).SetBinding(FlyoutPlacementProperty, FlyoutBase.PlacementProperty, (DependencyObject)(object)newFlyout);
		}
	}

	private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((SplitButton)(object)d).UpdateVisualStates();
	}

	internal void UpdateVisualStates(bool useTransitions = true)
	{
		if (m_isKeyDown)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "SecondaryButtonSpan", useTransitions);
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "SecondaryButtonRight", useTransitions);
		}
		Button primaryButton = m_primaryButton;
		Button secondaryButton = m_secondaryButton;
		if (primaryButton == null || m_secondaryButton == null)
		{
			return;
		}
		if (m_isFlyoutOpen)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "FlyoutOpen", useTransitions);
		}
		else if (InternalIsChecked)
		{
			if (m_isKeyDown)
			{
				if (((ButtonBase)primaryButton).IsPressed || ((ButtonBase)secondaryButton).IsPressed || m_isKeyDown)
				{
					VisualStateManager.GoToState((FrameworkElement)(object)this, "CheckedTouchPressed", useTransitions);
				}
				else
				{
					VisualStateManager.GoToState((FrameworkElement)(object)this, "Checked", useTransitions);
				}
			}
			else if (((ButtonBase)primaryButton).IsPressed)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "CheckedPrimaryPressed", useTransitions);
			}
			else if (((UIElement)primaryButton).IsMouseOver)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "CheckedPrimaryPointerOver", useTransitions);
			}
			else if (((ButtonBase)secondaryButton).IsPressed)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "CheckedSecondaryPressed", useTransitions);
			}
			else if (((UIElement)secondaryButton).IsMouseOver)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "CheckedSecondaryPointerOver", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "Checked", useTransitions);
			}
		}
		else if (m_isKeyDown)
		{
			if (((ButtonBase)primaryButton).IsPressed || ((ButtonBase)secondaryButton).IsPressed || m_isKeyDown)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "TouchPressed", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "Normal", useTransitions);
			}
		}
		else if (((ButtonBase)primaryButton).IsPressed)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "PrimaryPressed", useTransitions);
		}
		else if (((UIElement)primaryButton).IsMouseOver)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "PrimaryPointerOver", useTransitions);
		}
		else if (((ButtonBase)secondaryButton).IsPressed)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "SecondaryPressed", useTransitions);
		}
		else if (((UIElement)secondaryButton).IsMouseOver)
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "SecondaryPointerOver", useTransitions);
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Normal", useTransitions);
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

	internal virtual void OnClickPrimary(object sender, RoutedEventArgs e)
	{
		this.Click?.Invoke(this, new SplitButtonClickEventArgs());
		AutomationPeer val = UIElementAutomationPeer.FromElement((UIElement)(object)this);
		if (val != null)
		{
			val.RaiseAutomationEvent((AutomationEvents)5);
		}
	}

	private void OnFlyoutOpened(object sender, object e)
	{
		m_isFlyoutOpen = true;
		UpdateVisualStates();
		SharedHelpers.RaiseAutomationPropertyChangedEvent((UIElement)(object)this, (object)(ExpandCollapseState)0, (object)(ExpandCollapseState)1);
	}

	private void OnFlyoutClosed(object sender, object e)
	{
		m_isFlyoutOpen = false;
		UpdateVisualStates();
		SharedHelpers.RaiseAutomationPropertyChangedEvent((UIElement)(object)this, (object)(ExpandCollapseState)1, (object)(ExpandCollapseState)0);
	}

	private static void OnFlyoutPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((SplitButton)(object)d).UpdateVisualStates();
	}

	private void OnClickSecondary(object sender, RoutedEventArgs e)
	{
		OpenFlyout();
	}

	private void OnSplitButtonKeyDown(object sender, KeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		Key key = args.Key;
		if ((int)key == 18 || (int)key == 6)
		{
			m_isKeyDown = true;
			UpdateVisualStates();
		}
	}

	private void OnSplitButtonKeyUp(object sender, KeyEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		Key key = args.Key;
		if ((int)key == 18 || (int)key == 6)
		{
			m_isKeyDown = false;
			UpdateVisualStates();
			if (((UIElement)this).IsEnabled)
			{
				OnClickPrimary(null, null);
				((RoutedEventArgs)args).Handled = true;
			}
		}
		else if ((int)key == 93 && ((UIElement)this).IsEnabled)
		{
			OpenFlyout();
			((RoutedEventArgs)args).Handled = true;
		}
	}

	private void UnregisterEvents()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		if (m_primaryButton != null)
		{
			((ButtonBase)m_primaryButton).Click -= new RoutedEventHandler(OnClickPrimary);
			((DependencyObject)this).ClearValue(PrimaryButtonIsPressedProperty);
			((DependencyObject)this).ClearValue(PrimaryButtonIsMouseOverProperty);
		}
		if (m_secondaryButton != null)
		{
			((ButtonBase)m_secondaryButton).Click -= new RoutedEventHandler(OnClickSecondary);
			((DependencyObject)this).ClearValue(SecondaryButtonIsPressedProperty);
			((DependencyObject)this).ClearValue(SecondaryButtonIsMouseOverProperty);
		}
	}
}
