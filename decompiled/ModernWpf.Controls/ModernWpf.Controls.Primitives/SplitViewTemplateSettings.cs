using System.Windows;

namespace ModernWpf.Controls.Primitives;

public sealed class SplitViewTemplateSettings : DependencyObject
{
	private static readonly DependencyPropertyKey CompactPaneGridLengthPropertyKey = DependencyProperty.RegisterReadOnly("CompactPaneGridLength", typeof(GridLength), typeof(SplitViewTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty CompactPaneGridLengthProperty = CompactPaneGridLengthPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey NegativeOpenPaneLengthPropertyKey = DependencyProperty.RegisterReadOnly("NegativeOpenPaneLength", typeof(double), typeof(SplitViewTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty NegativeOpenPaneLengthProperty = NegativeOpenPaneLengthPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey NegativeOpenPaneLengthMinusCompactLengthPropertyKey = DependencyProperty.RegisterReadOnly("NegativeOpenPaneLengthMinusCompactLength", typeof(double), typeof(SplitViewTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty NegativeOpenPaneLengthMinusCompactLengthProperty = NegativeOpenPaneLengthMinusCompactLengthPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey OpenPaneGridLengthPropertyKey = DependencyProperty.RegisterReadOnly("OpenPaneGridLength", typeof(GridLength), typeof(SplitViewTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty OpenPaneGridLengthProperty = OpenPaneGridLengthPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey OpenPaneLengthPropertyKey = DependencyProperty.RegisterReadOnly("OpenPaneLength", typeof(double), typeof(SplitViewTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty OpenPaneLengthProperty = OpenPaneLengthPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey OpenPaneLengthMinusCompactLengthPropertyKey = DependencyProperty.RegisterReadOnly("OpenPaneLengthMinusCompactLength", typeof(double), typeof(SplitViewTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty OpenPaneLengthMinusCompactLengthProperty = OpenPaneLengthMinusCompactLengthPropertyKey.DependencyProperty;

	public GridLength CompactPaneGridLength
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (GridLength)((DependencyObject)this).GetValue(CompactPaneGridLengthProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(CompactPaneGridLengthPropertyKey, (object)value);
		}
	}

	public double NegativeOpenPaneLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(NegativeOpenPaneLengthProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(NegativeOpenPaneLengthPropertyKey, (object)value);
		}
	}

	public double NegativeOpenPaneLengthMinusCompactLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(NegativeOpenPaneLengthMinusCompactLengthProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(NegativeOpenPaneLengthMinusCompactLengthPropertyKey, (object)value);
		}
	}

	public GridLength OpenPaneGridLength
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (GridLength)((DependencyObject)this).GetValue(OpenPaneGridLengthProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(OpenPaneGridLengthPropertyKey, (object)value);
		}
	}

	public double OpenPaneLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(OpenPaneLengthProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(OpenPaneLengthPropertyKey, (object)value);
		}
	}

	public double OpenPaneLengthMinusCompactLength
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(OpenPaneLengthMinusCompactLengthProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(OpenPaneLengthMinusCompactLengthPropertyKey, (object)value);
		}
	}

	internal SplitViewTemplateSettings()
	{
	}
}
