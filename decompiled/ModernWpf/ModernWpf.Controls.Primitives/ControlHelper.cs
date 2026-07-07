using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public static class ControlHelper
{
	public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlHelper), (PropertyMetadata)null);

	public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(object), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeaderChanged)));

	public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnHeaderTemplateChanged)));

	private static readonly DependencyPropertyKey HeaderVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HeaderVisibility", typeof(Visibility), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)(Visibility)2));

	public static readonly DependencyProperty HeaderVisibilityProperty = HeaderVisibilityPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)string.Empty, new PropertyChangedCallback(OnPlaceholderTextChanged)));

	private static readonly DependencyPropertyKey PlaceholderTextVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("PlaceholderTextVisibility", typeof(Visibility), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)(Visibility)2));

	public static readonly DependencyProperty PlaceholderTextVisibilityProperty = PlaceholderTextVisibilityPropertyKey.DependencyProperty;

	public static readonly DependencyProperty PlaceholderForegroundProperty = DependencyProperty.RegisterAttached("PlaceholderForeground", typeof(Brush), typeof(ControlHelper), (PropertyMetadata)null);

	public static readonly DependencyProperty DescriptionProperty = DependencyProperty.RegisterAttached("Description", typeof(object), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDescriptionChanged)));

	private static readonly DependencyPropertyKey DescriptionVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DescriptionVisibility", typeof(Visibility), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)(Visibility)2));

	public static readonly DependencyProperty DescriptionVisibilityProperty = DescriptionVisibilityPropertyKey.DependencyProperty;

	public static CornerRadius GetCornerRadius(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (CornerRadius)((DependencyObject)control).GetValue(CornerRadiusProperty);
	}

	public static void SetCornerRadius(Control control, CornerRadius value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)control).SetValue(CornerRadiusProperty, (object)value);
	}

	public static object GetHeader(Control control)
	{
		return ((DependencyObject)control).GetValue(HeaderProperty);
	}

	public static void SetHeader(Control control, object value)
	{
		((DependencyObject)control).SetValue(HeaderProperty, value);
	}

	private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateHeaderVisibility((Control)d);
	}

	public static DataTemplate GetHeaderTemplate(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (DataTemplate)((DependencyObject)control).GetValue(HeaderTemplateProperty);
	}

	public static void SetHeaderTemplate(Control control, DataTemplate value)
	{
		((DependencyObject)control).SetValue(HeaderTemplateProperty, (object)value);
	}

	private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateHeaderVisibility((Control)d);
	}

	public static Visibility GetHeaderVisibility(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Visibility)((DependencyObject)control).GetValue(HeaderVisibilityProperty);
	}

	private static void SetHeaderVisibility(Control control, Visibility value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)control).SetValue(HeaderVisibilityPropertyKey, (object)value);
	}

	private static void UpdateHeaderVisibility(Control control)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Visibility value = ((GetHeaderTemplate(control) == null) ? ((Visibility)(IsNullOrEmptyString(GetHeader(control)) ? 2 : 0)) : ((Visibility)0));
		SetHeaderVisibility(control, value);
	}

	public static string GetPlaceholderText(Control control)
	{
		return (string)((DependencyObject)control).GetValue(PlaceholderTextProperty);
	}

	public static void SetPlaceholderText(Control control, string value)
	{
		((DependencyObject)control).SetValue(PlaceholderTextProperty, (object)value);
	}

	private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdatePlaceholderTextVisibility((Control)d);
	}

	public static Visibility GetPlaceholderTextVisibility(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Visibility)((DependencyObject)control).GetValue(PlaceholderTextVisibilityProperty);
	}

	private static void SetPlaceholderTextVisibility(Control control, Visibility value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)control).SetValue(PlaceholderTextVisibilityPropertyKey, (object)value);
	}

	private static void UpdatePlaceholderTextVisibility(Control control)
	{
		SetPlaceholderTextVisibility(control, (Visibility)(string.IsNullOrEmpty(GetPlaceholderText(control)) ? 2 : 0));
	}

	public static Brush GetPlaceholderForeground(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)control).GetValue(PlaceholderForegroundProperty);
	}

	public static void SetPlaceholderForeground(Control control, Brush value)
	{
		((DependencyObject)control).SetValue(PlaceholderForegroundProperty, (object)value);
	}

	public static object GetDescription(Control control)
	{
		return ((DependencyObject)control).GetValue(DescriptionProperty);
	}

	public static void SetDescription(Control control, object value)
	{
		((DependencyObject)control).SetValue(DescriptionProperty, value);
	}

	private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		UpdateDescriptionVisibility((Control)d);
	}

	public static Visibility GetDescriptionVisibility(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Visibility)((DependencyObject)control).GetValue(DescriptionVisibilityProperty);
	}

	private static void SetDescriptionVisibility(Control control, Visibility value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)control).SetValue(DescriptionVisibilityPropertyKey, (object)value);
	}

	private static void UpdateDescriptionVisibility(Control control)
	{
		SetDescriptionVisibility(control, (Visibility)(IsNullOrEmptyString(GetDescription(control)) ? 2 : 0));
	}

	internal static bool IsNullOrEmptyString(object obj)
	{
		if (obj != null)
		{
			if (obj is string value)
			{
				return string.IsNullOrEmpty(value);
			}
			return false;
		}
		return true;
	}
}
