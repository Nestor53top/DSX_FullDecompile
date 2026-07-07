using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using ModernWpf.Automation.Peers;

namespace ModernWpf.Controls;

public class ToggleSplitButton : SplitButton
{
	public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleSplitButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (FrameworkPropertyMetadataOptions)1280, new PropertyChangedCallback(OnIsCheckedPropertyChanged)));

	public bool IsChecked
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsCheckedProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsCheckedProperty, (object)value);
		}
	}

	internal override bool InternalIsChecked => IsChecked;

	public event TypedEventHandler<ToggleSplitButton, ToggleSplitButtonIsCheckedChangedEventArgs> IsCheckedChanged;

	private static void OnIsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((ToggleSplitButton)(object)d).OnIsCheckedChanged();
	}

	protected override AutomationPeer OnCreateAutomationPeer()
	{
		return (AutomationPeer)(object)new ToggleSplitButtonAutomationPeer(this);
	}

	private void OnIsCheckedChanged()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (m_hasLoaded)
		{
			this.IsCheckedChanged?.Invoke(this, new ToggleSplitButtonIsCheckedChangedEventArgs());
			AutomationPeer val = UIElementAutomationPeer.FromElement((UIElement)(object)this);
			if (val != null)
			{
				ToggleState val2 = (ToggleState)(IsChecked ? 1 : 0);
				ToggleState val3 = (ToggleState)((int)val2 != 1);
				val.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, (object)val3, (object)val2);
			}
		}
		UpdateVisualStates();
	}

	internal override void OnClickPrimary(object sender, RoutedEventArgs e)
	{
		Toggle();
		base.OnClickPrimary(sender, e);
	}

	internal void Toggle()
	{
		((DependencyObject)this).SetCurrentValue(IsCheckedProperty, (object)(!IsChecked));
	}
}
