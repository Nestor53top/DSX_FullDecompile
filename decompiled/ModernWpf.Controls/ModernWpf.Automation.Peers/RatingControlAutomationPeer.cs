using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ModernWpf.Controls;

namespace ModernWpf.Automation.Peers;

public class RatingControlAutomationPeer : FrameworkElementAutomationPeer, IValueProvider, IRangeValueProvider
{
	private static readonly ResourceAccessor ResourceAccessor = new ResourceAccessor(typeof(RatingControl));

	public bool IsReadOnly => GetRatingControl().IsReadOnly;

	string IValueProvider.Value
	{
		get
		{
			double value = GetRatingControl().Value;
			if (value == -1.0)
			{
				double placeholderValue = GetRatingControl().PlaceholderValue;
				if (placeholderValue == -1.0)
				{
					return ResourceAccessor.GetLocalizedStringResource("RatingUnset");
				}
				return GenerateValue_ValueString(ResourceAccessor.GetLocalizedStringResource("CommunityRatingString"), placeholderValue);
			}
			return GenerateValue_ValueString(ResourceAccessor.GetLocalizedStringResource("BasicRatingString"), value);
		}
	}

	public double SmallChange => 1.0;

	public double LargeChange => 1.0;

	public double Maximum => GetRatingControl().MaxRating;

	public double Minimum => 0.0;

	public double Value
	{
		get
		{
			double value = GetRatingControl().Value;
			if (value == -1.0)
			{
				return 0.0;
			}
			return value;
		}
	}

	public RatingControlAutomationPeer(RatingControl owner)
		: base((FrameworkElement)(object)owner)
	{
	}

	protected override string GetLocalizedControlTypeCore()
	{
		return ResourceAccessor.GetLocalizedStringResource("RatingLocalizedControlType");
	}

	public void SetValue(string value)
	{
		if (double.TryParse(value, out var result))
		{
			GetRatingControl().Value = result;
		}
	}

	public void SetValue(double value)
	{
		GetRatingControl().Value = value;
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if ((int)patternInterface == 2 || (int)patternInterface == 3)
		{
			return this;
		}
		return ((UIElementAutomationPeer)this).GetPattern(patternInterface);
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return (AutomationControlType)15;
	}

	internal void RaisePropertyChangedEvent(double newValue)
	{
		double value = GetRatingControl().Value;
		if (newValue == -1.0)
		{
			newValue = 0.0;
		}
		((AutomationPeer)this).RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, (object)value.ToString(), (object)newValue.ToString());
		((AutomationPeer)this).RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, (object)value, (object)newValue);
	}

	private RatingControl GetRatingControl()
	{
		return (RatingControl)(object)((UIElementAutomationPeer)this).Owner;
	}

	private int DetermineFractionDigits(double value)
	{
		value *= 100.0;
		int num = (int)value;
		if (num % 100 == 0)
		{
			return 0;
		}
		if (num % 10 == 0)
		{
			return 1;
		}
		return 2;
	}

	private string GenerateValue_ValueString(string resourceString, double ratingValue)
	{
		string arg = GetRatingControl().MaxRating.ToString();
		string arg2 = ratingValue.ToString("F" + DetermineFractionDigits(ratingValue));
		return string.Format(resourceString, arg2, arg);
	}
}
