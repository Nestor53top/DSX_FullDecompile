using System.Windows;

namespace ModernWpf.Controls;

public class NavigationViewTemplateSettings : DependencyObject
{
	private static readonly DependencyPropertyKey TopPaddingPropertyKey = DependencyProperty.RegisterReadOnly("TopPadding", typeof(double), typeof(NavigationViewTemplateSettings), new PropertyMetadata((object)0.0));

	public static readonly DependencyProperty TopPaddingProperty = TopPaddingPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey OverflowButtonVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("OverflowButtonVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new PropertyMetadata((object)(Visibility)2));

	public static readonly DependencyProperty OverflowButtonVisibilityProperty = OverflowButtonVisibilityPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey PaneToggleButtonVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("PaneToggleButtonVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new PropertyMetadata((object)(Visibility)0));

	public static readonly DependencyProperty PaneToggleButtonVisibilityProperty = PaneToggleButtonVisibilityPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey BackButtonVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("BackButtonVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new PropertyMetadata((object)(Visibility)2));

	public static readonly DependencyProperty BackButtonVisibilityProperty = BackButtonVisibilityPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey TopPaneVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("TopPaneVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new PropertyMetadata((object)(Visibility)2));

	public static readonly DependencyProperty TopPaneVisibilityProperty = TopPaneVisibilityPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey LeftPaneVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("LeftPaneVisibility", typeof(Visibility), typeof(NavigationViewTemplateSettings), new PropertyMetadata((object)(Visibility)0));

	public static readonly DependencyProperty LeftPaneVisibilityProperty = LeftPaneVisibilityPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey SingleSelectionFollowsFocusPropertyKey = DependencyProperty.RegisterReadOnly("SingleSelectionFollowsFocus", typeof(bool), typeof(NavigationViewTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty SingleSelectionFollowsFocusProperty = SingleSelectionFollowsFocusPropertyKey.DependencyProperty;

	public double TopPadding
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(TopPaddingProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(TopPaddingPropertyKey, (object)value);
		}
	}

	public Visibility OverflowButtonVisibility
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Visibility)((DependencyObject)this).GetValue(OverflowButtonVisibilityProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(OverflowButtonVisibilityPropertyKey, (object)value);
		}
	}

	public Visibility PaneToggleButtonVisibility
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Visibility)((DependencyObject)this).GetValue(PaneToggleButtonVisibilityProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(PaneToggleButtonVisibilityPropertyKey, (object)value);
		}
	}

	public Visibility BackButtonVisibility
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Visibility)((DependencyObject)this).GetValue(BackButtonVisibilityProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(BackButtonVisibilityPropertyKey, (object)value);
		}
	}

	public Visibility TopPaneVisibility
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Visibility)((DependencyObject)this).GetValue(TopPaneVisibilityProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(TopPaneVisibilityPropertyKey, (object)value);
		}
	}

	public Visibility LeftPaneVisibility
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Visibility)((DependencyObject)this).GetValue(LeftPaneVisibilityProperty);
		}
		internal set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(LeftPaneVisibilityPropertyKey, (object)value);
		}
	}

	public bool SingleSelectionFollowsFocus
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(SingleSelectionFollowsFocusProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(SingleSelectionFollowsFocusPropertyKey, (object)value);
		}
	}
}
