using System.Windows;

namespace ModernWpf.Controls.Primitives;

public sealed class ProgressRingTemplateSettings : DependencyObject
{
	private static readonly DependencyPropertyKey EllipseDiameterPropertyKey = DependencyProperty.RegisterReadOnly("EllipseDiameter", typeof(double), typeof(ProgressRingTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty EllipseDiameterProperty = EllipseDiameterPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey EllipseOffsetPropertyKey = DependencyProperty.RegisterReadOnly("EllipseOffset", typeof(Thickness), typeof(ProgressRingTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty EllipseOffsetProperty = EllipseOffsetPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey MaxSideLengthPropertyKey = DependencyProperty.RegisterReadOnly("MaxSideLength", typeof(double), typeof(ProgressRingTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty MaxSideLengthProperty = MaxSideLengthPropertyKey.DependencyProperty;

	public double EllipseDiameter
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(EllipseDiameterProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(EllipseDiameterPropertyKey, (object)value);
		}
	}

	public Thickness EllipseOffset
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Thickness)((DependencyObject)this).GetValue(EllipseOffsetProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(EllipseOffsetPropertyKey, (object)value);
		}
	}

	public double MaxSideLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(MaxSideLengthProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(MaxSideLengthPropertyKey, (object)value);
		}
	}

	internal ProgressRingTemplateSettings()
	{
	}
}
