using System.Windows;

namespace ModernWpf.Controls.Primitives;

public sealed class CommandBarFlyoutCommandBarTemplateSettings : DependencyObject
{
	private static readonly DependencyPropertyKey CloseAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("CloseAnimationEndPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty CloseAnimationEndPositionProperty = CloseAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ContentClipRectPropertyKey = DependencyProperty.RegisterReadOnly("ContentClipRect", typeof(Rect), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ContentClipRectProperty = ContentClipRectPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey CurrentWidthPropertyKey = DependencyProperty.RegisterReadOnly("CurrentWidth", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty CurrentWidthProperty = CurrentWidthPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandDownAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandDownAnimationEndPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandDownAnimationEndPositionProperty = ExpandDownAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandDownAnimationHoldPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandDownAnimationHoldPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandDownAnimationHoldPositionProperty = ExpandDownAnimationHoldPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandDownAnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandDownAnimationStartPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandDownAnimationStartPositionProperty = ExpandDownAnimationStartPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandDownOverflowVerticalPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandDownOverflowVerticalPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandDownOverflowVerticalPositionProperty = ExpandDownOverflowVerticalPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandedWidthPropertyKey = DependencyProperty.RegisterReadOnly("ExpandedWidth", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandedWidthProperty = ExpandedWidthPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandUpAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandUpAnimationEndPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandUpAnimationEndPositionProperty = ExpandUpAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandUpAnimationHoldPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandUpAnimationHoldPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandUpAnimationHoldPositionProperty = ExpandUpAnimationHoldPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandUpAnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandUpAnimationStartPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandUpAnimationStartPositionProperty = ExpandUpAnimationStartPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ExpandUpOverflowVerticalPositionPropertyKey = DependencyProperty.RegisterReadOnly("ExpandUpOverflowVerticalPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ExpandUpOverflowVerticalPositionProperty = ExpandUpOverflowVerticalPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey OpenAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("OpenAnimationEndPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty OpenAnimationEndPositionProperty = OpenAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey OpenAnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("OpenAnimationStartPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty OpenAnimationStartPositionProperty = OpenAnimationStartPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey OverflowContentClipRectPropertyKey = DependencyProperty.RegisterReadOnly("OverflowContentClipRect", typeof(Rect), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty OverflowContentClipRectProperty = OverflowContentClipRectPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey WidthExpansionAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("WidthExpansionAnimationEndPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty WidthExpansionAnimationEndPositionProperty = WidthExpansionAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey WidthExpansionAnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("WidthExpansionAnimationStartPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty WidthExpansionAnimationStartPositionProperty = WidthExpansionAnimationStartPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey WidthExpansionDeltaPropertyKey = DependencyProperty.RegisterReadOnly("WidthExpansionDelta", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty WidthExpansionDeltaProperty = WidthExpansionDeltaPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey WidthExpansionMoreButtonAnimationEndPositionPropertyKey = DependencyProperty.RegisterReadOnly("WidthExpansionMoreButtonAnimationEndPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty WidthExpansionMoreButtonAnimationEndPositionProperty = WidthExpansionMoreButtonAnimationEndPositionPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey WidthExpansionMoreButtonAnimationStartPositionPropertyKey = DependencyProperty.RegisterReadOnly("WidthExpansionMoreButtonAnimationStartPosition", typeof(double), typeof(CommandBarFlyoutCommandBarTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty WidthExpansionMoreButtonAnimationStartPositionProperty = WidthExpansionMoreButtonAnimationStartPositionPropertyKey.DependencyProperty;

	public double CloseAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(CloseAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(CloseAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public Rect ContentClipRect
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Rect)((DependencyObject)this).GetValue(ContentClipRectProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(ContentClipRectPropertyKey, (object)value);
		}
	}

	public double CurrentWidth
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(CurrentWidthProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(CurrentWidthPropertyKey, (object)value);
		}
	}

	public double ExpandDownAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandDownAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandDownAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double ExpandDownAnimationHoldPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandDownAnimationHoldPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandDownAnimationHoldPositionPropertyKey, (object)value);
		}
	}

	public double ExpandDownAnimationStartPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandDownAnimationStartPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandDownAnimationStartPositionPropertyKey, (object)value);
		}
	}

	public double ExpandDownOverflowVerticalPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandDownOverflowVerticalPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandDownOverflowVerticalPositionPropertyKey, (object)value);
		}
	}

	public double ExpandedWidth
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandedWidthProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandedWidthPropertyKey, (object)value);
		}
	}

	public double ExpandUpAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandUpAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandUpAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double ExpandUpAnimationHoldPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandUpAnimationHoldPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandUpAnimationHoldPositionPropertyKey, (object)value);
		}
	}

	public double ExpandUpAnimationStartPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandUpAnimationStartPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandUpAnimationStartPositionPropertyKey, (object)value);
		}
	}

	public double ExpandUpOverflowVerticalPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ExpandUpOverflowVerticalPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ExpandUpOverflowVerticalPositionPropertyKey, (object)value);
		}
	}

	public double OpenAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(OpenAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(OpenAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double OpenAnimationStartPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(OpenAnimationStartPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(OpenAnimationStartPositionPropertyKey, (object)value);
		}
	}

	public Rect OverflowContentClipRect
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Rect)((DependencyObject)this).GetValue(OverflowContentClipRectProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(OverflowContentClipRectPropertyKey, (object)value);
		}
	}

	public double WidthExpansionAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(WidthExpansionAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(WidthExpansionAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double WidthExpansionAnimationStartPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(WidthExpansionAnimationStartPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(WidthExpansionAnimationStartPositionPropertyKey, (object)value);
		}
	}

	public double WidthExpansionDelta
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(WidthExpansionDeltaProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(WidthExpansionDeltaPropertyKey, (object)value);
		}
	}

	public double WidthExpansionMoreButtonAnimationEndPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(WidthExpansionMoreButtonAnimationEndPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(WidthExpansionMoreButtonAnimationEndPositionPropertyKey, (object)value);
		}
	}

	public double WidthExpansionMoreButtonAnimationStartPosition
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(WidthExpansionMoreButtonAnimationStartPositionProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(WidthExpansionMoreButtonAnimationStartPositionPropertyKey, (object)value);
		}
	}

	internal CommandBarFlyoutCommandBarTemplateSettings()
	{
	}
}
