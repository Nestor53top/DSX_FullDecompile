using System.Windows;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public sealed class ProgressBarTemplateSettings : DependencyObject
{
	private static readonly DependencyPropertyKey ContainerAnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("ContainerAnimationStartPosition", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ContainerAnimationStartPositionProperty = ContainerAnimationStartPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ContainerAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("ContainerAnimationEndPosition", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ContainerAnimationEndPositionProperty = ContainerAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey Container2AnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("Container2AnimationStartPosition", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty Container2AnimationStartPositionProperty = Container2AnimationStartPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey Container2AnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("Container2AnimationEndPosition", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty Container2AnimationEndPositionProperty = Container2AnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ContainerAnimationMidPositionPropertyKey = DependencyProperty.RegisterReadOnly("ContainerAnimationMidPosition", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ContainerAnimationMidPositionProperty = ContainerAnimationMidPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey IndicatorLengthDeltaPropertyKey = DependencyProperty.RegisterReadOnly("IndicatorLengthDelta", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty IndicatorLengthDeltaProperty = IndicatorLengthDeltaPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ClipRectPropertyKey = DependencyProperty.RegisterReadOnly("ClipRect", typeof(RectangleGeometry), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ClipRectProperty = ClipRectPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey EllipseAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("EllipseAnimationEndPosition", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty EllipseAnimationEndPositionProperty = EllipseAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey EllipseAnimationWellPositionPropertyKey = DependencyProperty.RegisterReadOnly("EllipseAnimationWellPosition", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty EllipseAnimationWellPositionProperty = EllipseAnimationWellPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey EllipseDiameterPropertyKey = DependencyProperty.RegisterReadOnly("EllipseDiameter", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty EllipseDiameterProperty = EllipseDiameterPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey EllipseOffsetPropertyKey = DependencyProperty.RegisterReadOnly("EllipseOffset", typeof(double), typeof(ProgressBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty EllipseOffsetProperty = EllipseOffsetPropertyKey.DependencyProperty;

	public double ContainerAnimationStartPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ContainerAnimationStartPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ContainerAnimationStartPositionPropertyKey, (object)value);
		}
	}

	public double ContainerAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ContainerAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ContainerAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double Container2AnimationStartPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(Container2AnimationStartPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(Container2AnimationStartPositionPropertyKey, (object)value);
		}
	}

	public double Container2AnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(Container2AnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(Container2AnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double ContainerAnimationMidPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ContainerAnimationMidPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ContainerAnimationMidPositionPropertyKey, (object)value);
		}
	}

	public double IndicatorLengthDelta
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(IndicatorLengthDeltaProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(IndicatorLengthDeltaPropertyKey, (object)value);
		}
	}

	public RectangleGeometry ClipRect
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (RectangleGeometry)((DependencyObject)this).GetValue(ClipRectProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ClipRectPropertyKey, (object)value);
		}
	}

	public double EllipseAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(EllipseAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(EllipseAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double EllipseAnimationWellPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(EllipseAnimationWellPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(EllipseAnimationWellPositionPropertyKey, (object)value);
		}
	}

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

	public double EllipseOffset
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(EllipseOffsetProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(EllipseOffsetPropertyKey, (object)value);
		}
	}

	internal ProgressBarTemplateSettings()
	{
	}
}
