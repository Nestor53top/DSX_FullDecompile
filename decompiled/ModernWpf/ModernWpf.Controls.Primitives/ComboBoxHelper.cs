using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ModernWpf.Controls.Primitives;

public sealed class ComboBoxHelper
{
	private const string c_popupBorderName = "PopupBorder";

	private const string c_editableTextName = "PART_EditableTextBox";

	private const string c_backgroundName = "Background";

	private const string c_highlightBackgroundName = "HighlightBackground";

	private const string c_overlayCornerRadiusKey = "OverlayCornerRadius";

	public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.RegisterAttached("TextBoxStyle", typeof(Style), typeof(ComboBoxHelper), (PropertyMetadata)null);

	public static readonly DependencyProperty KeepInteriorCornersSquareProperty = DependencyProperty.RegisterAttached("KeepInteriorCornersSquare", typeof(bool), typeof(ComboBoxHelper), new PropertyMetadata((object)false, new PropertyChangedCallback(OnKeepInteriorCornersSquareChanged)));

	internal ComboBoxHelper()
	{
	}

	public static Style GetTextBoxStyle(ComboBox comboBox)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Style)((DependencyObject)comboBox).GetValue(TextBoxStyleProperty);
	}

	public static void SetTextBoxStyle(ComboBox comboBox, Style value)
	{
		((DependencyObject)comboBox).SetValue(TextBoxStyleProperty, (object)value);
	}

	public static bool GetKeepInteriorCornersSquare(ComboBox comboBox)
	{
		return (bool)((DependencyObject)comboBox).GetValue(KeepInteriorCornersSquareProperty);
	}

	public static void SetKeepInteriorCornersSquare(ComboBox comboBox, bool value)
	{
		((DependencyObject)comboBox).SetValue(KeepInteriorCornersSquareProperty, (object)value);
	}

	private static void OnKeepInteriorCornersSquareChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		ComboBox val = (ComboBox)(object)((sender is ComboBox) ? sender : null);
		if (val != null)
		{
			if ((bool)((DependencyPropertyChangedEventArgs)(ref args)).NewValue)
			{
				val.DropDownOpened += OnDropDownOpened;
				val.DropDownClosed += OnDropDownClosed;
			}
			else
			{
				val.DropDownOpened -= OnDropDownOpened;
				val.DropDownClosed -= OnDropDownClosed;
			}
		}
	}

	private static void OnDropDownOpened(object sender, object args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		ComboBox comboBox = (ComboBox)sender;
		((DispatcherObject)comboBox).Dispatcher.BeginInvoke(delegate
		{
			UpdateCornerRadius(comboBox, isDropDownOpen: true);
		});
	}

	private static void OnDropDownClosed(object sender, object args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		UpdateCornerRadius((ComboBox)sender, isDropDownOpen: false);
	}

	private static void UpdateCornerRadius(ComboBox comboBox, bool isDropDownOpen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		CornerRadius val = ControlHelper.GetCornerRadius((Control)(object)comboBox);
		CornerRadius val2 = (CornerRadius)ResourceLookup((Control)(object)comboBox, "OverlayCornerRadius");
		if (isDropDownOpen)
		{
			bool num = IsPopupOpenDown(comboBox);
			CornerRadiusFilterConverter cornerRadiusFilterConverter = new CornerRadiusFilterConverter();
			CornerRadiusFilterKind filterKind = ((!num) ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom);
			val2 = cornerRadiusFilterConverter.Convert(val2, filterKind);
			CornerRadiusFilterKind filterKind2 = (num ? CornerRadiusFilterKind.Top : CornerRadiusFilterKind.Bottom);
			val = cornerRadiusFilterConverter.Convert(val, filterKind2);
		}
		Border templateChild = GetTemplateChild<Border>("PopupBorder", (Control)(object)comboBox);
		if (templateChild != null)
		{
			templateChild.CornerRadius = val2;
		}
		if (comboBox.IsEditable)
		{
			TextBox templateChild2 = GetTemplateChild<TextBox>("PART_EditableTextBox", (Control)(object)comboBox);
			if (templateChild2 != null)
			{
				ControlHelper.SetCornerRadius((Control)(object)templateChild2, val);
			}
			return;
		}
		Border templateChild3 = GetTemplateChild<Border>("Background", (Control)(object)comboBox);
		if (templateChild3 != null)
		{
			templateChild3.CornerRadius = val;
		}
		Border templateChild4 = GetTemplateChild<Border>("HighlightBackground", (Control)(object)comboBox);
		if (templateChild4 != null)
		{
			templateChild4.CornerRadius = val;
		}
	}

	private static bool IsPopupOpenDown(ComboBox comboBox)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		Border templateChild = GetTemplateChild<Border>("PopupBorder", (Control)(object)comboBox);
		if (templateChild != null)
		{
			TextBox templateChild2 = GetTemplateChild<TextBox>("PART_EditableTextBox", (Control)(object)comboBox);
			if (templateChild2 != null)
			{
				Point val = ((UIElement)templateChild).TranslatePoint(new Point(0.0, 0.0), (UIElement)(object)templateChild2);
				num = ((Point)(ref val)).Y;
			}
		}
		return num > 0.0;
	}

	private static object ResourceLookup(Control control, object key)
	{
		return ((FrameworkElement)control).TryFindResource(key);
	}

	private static T GetTemplateChild<T>(string childName, Control control) where T : DependencyObject
	{
		ControlTemplate template = control.Template;
		object obj = ((template != null) ? ((FrameworkTemplate)template).FindName(childName, (FrameworkElement)(object)control) : null);
		return (T)((obj is T) ? obj : null);
	}
}
