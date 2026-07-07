using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class NumberBoxAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
	public bool IsReadOnly => false;

	public double Minimum => GetImpl().Minimum;

	public double Maximum => GetImpl().Maximum;

	public double Value => GetImpl().Value;

	public double SmallChange => GetImpl().SmallChange;

	public double LargeChange => GetImpl().LargeChange;

	public NumberBoxAutomationPeer(NumberBox owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 3)
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override string GetClassNameCore()
	{
		return "NumberBox";
	}

	protected override string GetNameCore()
	{
		string text = ((FrameworkElementAutomationPeer)this).GetNameCore();
		if (string.IsNullOrEmpty(text) && ((UIElementAutomationPeer)this).Owner is NumberBox { Header: var header })
		{
			text = header?.ToString();
		}
		return text ?? string.Empty;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)16;
	}

	private NumberBox GetImpl()
	{
		NumberBox result = null;
		if (((UIElementAutomationPeer)this).Owner is NumberBox numberBox)
		{
			result = numberBox;
		}
		return result;
	}

	public void SetValue(double value)
	{
		GetImpl().Value = value;
	}

	public void RaiseValueChangedEvent(double oldValue, double newValue)
	{
		((AutomationPeer)this).RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, (object)oldValue, (object)newValue);
	}
}
