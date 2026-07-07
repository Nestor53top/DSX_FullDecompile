using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class AppBarElementContainer : ContentControl, ICommandBarElement
{
	public static readonly DependencyProperty IsCompactProperty;

	public static readonly DependencyProperty IsInOverflowProperty;

	public bool IsCompact
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsCompactProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsCompactProperty, (object)value);
		}
	}

	public bool IsInOverflow => (bool)((DependencyObject)this).GetValue(IsInOverflowProperty);

	static AppBarElementContainer()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		IsCompactProperty = AppBarElementProperties.IsCompactProperty.AddOwner(typeof(AppBarElementContainer));
		IsInOverflowProperty = AppBarElementProperties.IsInOverflowProperty.AddOwner(typeof(AppBarElementContainer));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AppBarElementContainer), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(AppBarElementContainer)));
		ToolBar.OverflowModeProperty.OverrideMetadata(typeof(AppBarElementContainer), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverflowModePropertyChanged)));
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnPropertyChanged(e);
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property == ToolBar.IsOverflowItemProperty)
		{
			AppBarElementProperties.UpdateIsInOverflow((DependencyObject)(object)this);
		}
	}

	private static void OnOverflowModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		AppBarElementProperties.UpdateIsInOverflow(d);
	}
}
