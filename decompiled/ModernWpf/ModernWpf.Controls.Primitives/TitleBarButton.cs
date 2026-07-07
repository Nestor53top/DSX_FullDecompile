using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public class TitleBarButton : Button
{
	public static readonly DependencyProperty IsActiveProperty;

	public static readonly DependencyProperty InactiveBackgroundProperty;

	public static readonly DependencyProperty InactiveForegroundProperty;

	public static readonly DependencyProperty HoverBackgroundProperty;

	public static readonly DependencyProperty HoverForegroundProperty;

	public static readonly DependencyProperty PressedBackgroundProperty;

	public static readonly DependencyProperty PressedForegroundProperty;

	public bool IsActive
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsActiveProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsActiveProperty, (object)value);
		}
	}

	public Brush InactiveBackground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(InactiveBackgroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InactiveBackgroundProperty, (object)value);
		}
	}

	public Brush InactiveForeground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(InactiveForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(InactiveForegroundProperty, (object)value);
		}
	}

	public Brush HoverBackground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(HoverBackgroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HoverBackgroundProperty, (object)value);
		}
	}

	public Brush HoverForeground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(HoverForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(HoverForegroundProperty, (object)value);
		}
	}

	public Brush PressedBackground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(PressedBackgroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PressedBackgroundProperty, (object)value);
		}
	}

	public Brush PressedForeground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(PressedForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PressedForegroundProperty, (object)value);
		}
	}

	static TitleBarButton()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(TitleBarButton), new PropertyMetadata((object)false));
		InactiveBackgroundProperty = DependencyProperty.Register("InactiveBackground", typeof(Brush), typeof(TitleBarButton), (PropertyMetadata)null);
		InactiveForegroundProperty = DependencyProperty.Register("InactiveForeground", typeof(Brush), typeof(TitleBarButton), (PropertyMetadata)null);
		HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(TitleBarButton), (PropertyMetadata)null);
		HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(TitleBarButton), (PropertyMetadata)null);
		PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(TitleBarButton), (PropertyMetadata)null);
		PressedForegroundProperty = DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(TitleBarButton), (PropertyMetadata)null);
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBarButton), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(TitleBarButton)));
	}
}
