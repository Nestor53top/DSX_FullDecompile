using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public static class FocusVisualHelper
{
	private sealed class FocusVisualAdorner : Adorner
	{
		private UIElement _adorderChild;

		public Control FocusedElement { get; }

		protected override int VisualChildrenCount => 1;

		public FocusVisualAdorner(Control focusedElement, UIElement adornedElement, Style focusVisualStyle)
			: base(adornedElement)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			FocusedElement = focusedElement;
			Control val = new Control();
			SetIsSystemFocusVisual(val, value: false);
			((FrameworkElement)val).Style = focusVisualStyle;
			((FrameworkElement)val).Margin = GetFocusVisualMargin((FrameworkElement)(object)focusedElement);
			TransferValue((DependencyObject)(object)focusedElement, (DependencyObject)(object)val, FocusVisualPrimaryBrushProperty);
			TransferValue((DependencyObject)(object)focusedElement, (DependencyObject)(object)val, FocusVisualPrimaryThicknessProperty);
			TransferValue((DependencyObject)(object)focusedElement, (DependencyObject)(object)val, FocusVisualSecondaryBrushProperty);
			TransferValue((DependencyObject)(object)focusedElement, (DependencyObject)(object)val, FocusVisualSecondaryThicknessProperty);
			_adorderChild = (UIElement)(object)val;
			((Adorner)this).IsClipEnabled = true;
			((UIElement)this).IsHitTestVisible = false;
			((UIElement)this).IsEnabled = false;
			((Visual)this).AddVisualChild((Visual)(object)_adorderChild);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Size renderSize = ((Adorner)this).AdornedElement.RenderSize;
			((UIElement)((Visual)this).GetVisualChild(0)).Measure(renderSize);
			return renderSize;
		}

		protected override Size ArrangeOverride(Size size)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			Size val = ((FrameworkElement)this).ArrangeOverride(size);
			((UIElement)((Visual)this).GetVisualChild(0)).Arrange(new Rect(default(Point), val));
			return val;
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index == 0)
			{
				return (Visual)(object)_adorderChild;
			}
			throw new ArgumentOutOfRangeException("index");
		}
	}

	public static readonly DependencyProperty FocusVisualPrimaryBrushProperty = DependencyProperty.RegisterAttached("FocusVisualPrimaryBrush", typeof(Brush), typeof(FocusVisualHelper));

	public static readonly DependencyProperty FocusVisualSecondaryBrushProperty = DependencyProperty.RegisterAttached("FocusVisualSecondaryBrush", typeof(Brush), typeof(FocusVisualHelper));

	public static readonly DependencyProperty FocusVisualPrimaryThicknessProperty = DependencyProperty.RegisterAttached("FocusVisualPrimaryThickness", typeof(Thickness), typeof(FocusVisualHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Thickness(2.0)));

	public static readonly DependencyProperty FocusVisualSecondaryThicknessProperty = DependencyProperty.RegisterAttached("FocusVisualSecondaryThickness", typeof(Thickness), typeof(FocusVisualHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Thickness(1.0)));

	public static readonly DependencyProperty FocusVisualMarginProperty = DependencyProperty.RegisterAttached("FocusVisualMargin", typeof(Thickness), typeof(FocusVisualHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)default(Thickness)));

	public static readonly DependencyProperty UseSystemFocusVisualsProperty = DependencyProperty.RegisterAttached("UseSystemFocusVisuals", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata((object)false));

	public static readonly DependencyProperty IsTemplateFocusTargetProperty = DependencyProperty.RegisterAttached("IsTemplateFocusTarget", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsTemplateFocusTargetChanged)));

	public static readonly DependencyProperty IsSystemFocusVisualProperty = DependencyProperty.RegisterAttached("IsSystemFocusVisual", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsSystemFocusVisualChanged)));

	private static readonly DependencyPropertyKey ShowFocusVisualPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowFocusVisual", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata(new PropertyChangedCallback(OnShowFocusVisualChanged)));

	public static readonly DependencyProperty ShowFocusVisualProperty = ShowFocusVisualPropertyKey.DependencyProperty;

	private static readonly DependencyProperty FocusedElementProperty = DependencyProperty.RegisterAttached("FocusedElement", typeof(FrameworkElement), typeof(FocusVisualHelper));

	private static readonly DependencyProperty TemplateFocusTargetProperty = DependencyProperty.RegisterAttached("TemplateFocusTarget", typeof(FrameworkElement), typeof(FocusVisualHelper));

	private static FocusVisualAdorner _focusVisualAdornerCache = null;

	public static Brush GetFocusVisualPrimaryBrush(FrameworkElement element)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)element).GetValue(FocusVisualPrimaryBrushProperty);
	}

	public static void SetFocusVisualPrimaryBrush(FrameworkElement element, Brush value)
	{
		((DependencyObject)element).SetValue(FocusVisualPrimaryBrushProperty, (object)value);
	}

	public static Brush GetFocusVisualSecondaryBrush(FrameworkElement element)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)element).GetValue(FocusVisualSecondaryBrushProperty);
	}

	public static void SetFocusVisualSecondaryBrush(FrameworkElement element, Brush value)
	{
		((DependencyObject)element).SetValue(FocusVisualSecondaryBrushProperty, (object)value);
	}

	public static Thickness GetFocusVisualPrimaryThickness(FrameworkElement element)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Thickness)((DependencyObject)element).GetValue(FocusVisualPrimaryThicknessProperty);
	}

	public static void SetFocusVisualPrimaryThickness(FrameworkElement element, Thickness value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)element).SetValue(FocusVisualPrimaryThicknessProperty, (object)value);
	}

	public static Thickness GetFocusVisualSecondaryThickness(FrameworkElement element)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Thickness)((DependencyObject)element).GetValue(FocusVisualSecondaryThicknessProperty);
	}

	public static void SetFocusVisualSecondaryThickness(FrameworkElement element, Thickness value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)element).SetValue(FocusVisualSecondaryThicknessProperty, (object)value);
	}

	public static Thickness GetFocusVisualMargin(FrameworkElement element)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Thickness)((DependencyObject)element).GetValue(FocusVisualMarginProperty);
	}

	public static void SetFocusVisualMargin(FrameworkElement element, Thickness value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)element).SetValue(FocusVisualMarginProperty, (object)value);
	}

	public static bool GetUseSystemFocusVisuals(Control control)
	{
		return (bool)((DependencyObject)control).GetValue(UseSystemFocusVisualsProperty);
	}

	public static void SetUseSystemFocusVisuals(Control control, bool value)
	{
		((DependencyObject)control).SetValue(UseSystemFocusVisualsProperty, (object)value);
	}

	public static bool GetIsTemplateFocusTarget(FrameworkElement element)
	{
		return (bool)((DependencyObject)element).GetValue(IsTemplateFocusTargetProperty);
	}

	public static void SetIsTemplateFocusTarget(FrameworkElement element, bool value)
	{
		((DependencyObject)element).SetValue(IsTemplateFocusTargetProperty, (object)value);
	}

	private static void OnIsTemplateFocusTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		DependencyObject templatedParent = val.TemplatedParent;
		Control val2 = (Control)(object)((templatedParent is Control) ? templatedParent : null);
		if (val2 != null)
		{
			if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
			{
				SetTemplateFocusTarget(val2, val);
			}
			else
			{
				((DependencyObject)val2).ClearValue(TemplateFocusTargetProperty);
			}
		}
	}

	public static bool GetIsSystemFocusVisual(Control control)
	{
		return (bool)((DependencyObject)control).GetValue(IsSystemFocusVisualProperty);
	}

	public static void SetIsSystemFocusVisual(Control control, bool value)
	{
		((DependencyObject)control).SetValue(IsSystemFocusVisualProperty, (object)value);
	}

	private static void OnIsSystemFocusVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		Control val = (Control)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((UIElement)val).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnFocusVisualIsVisibleChanged);
		}
		else
		{
			((UIElement)val).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnFocusVisualIsVisibleChanged);
		}
	}

	public static bool GetShowFocusVisual(FrameworkElement element)
	{
		return (bool)((DependencyObject)element).GetValue(ShowFocusVisualProperty);
	}

	private static void SetShowFocusVisual(FrameworkElement element, bool value)
	{
		((DependencyObject)element).SetValue(ShowFocusVisualPropertyKey, (object)value);
	}

	private static void OnShowFocusVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		Control val = (Control)(object)((d is Control) ? d : null);
		if (val == null)
		{
			return;
		}
		FrameworkElement templateFocusTarget = GetTemplateFocusTarget(val);
		if (templateFocusTarget == null)
		{
			return;
		}
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			bool flag = true;
			Control val2 = (Control)(object)((templateFocusTarget is Control) ? templateFocusTarget : null);
			if (val2 != null)
			{
				flag = GetUseSystemFocusVisuals(val2);
			}
			if (flag)
			{
				ShowFocusVisual(val, templateFocusTarget);
			}
		}
		else
		{
			HideFocusVisual();
		}
		static void HideFocusVisual()
		{
			if (_focusVisualAdornerCache != null)
			{
				DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)_focusVisualAdornerCache);
				AdornerLayer val3 = (AdornerLayer)(object)((parent is AdornerLayer) ? parent : null);
				if (val3 != null)
				{
					val3.Remove((Adorner)(object)_focusVisualAdornerCache);
				}
				_focusVisualAdornerCache = null;
			}
		}
		static void OnControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e2)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			((UIElement)(Control)sender).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnControlIsVisibleChanged);
			if (_focusVisualAdornerCache != null && _focusVisualAdornerCache.FocusedElement == sender)
			{
				HideFocusVisual();
			}
		}
		static void ShowFocusVisual(Control control, FrameworkElement target)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			HideFocusVisual();
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual)(object)target);
			if (adornerLayer != null)
			{
				Style val3 = target.FocusVisualStyle;
				if (val3 != null && val3.BasedOn == null && ((Collection<SetterBase>)(object)val3.Setters).Count == 0)
				{
					object obj = target.TryFindResource((object)SystemParameters.FocusVisualStyleKey);
					val3 = (Style)((obj is Style) ? obj : null);
				}
				if (val3 != null)
				{
					_focusVisualAdornerCache = new FocusVisualAdorner(control, (UIElement)(object)target, val3);
					adornerLayer.Add((Adorner)(object)_focusVisualAdornerCache);
					((UIElement)control).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnControlIsVisibleChanged);
				}
			}
		}
	}

	private static FrameworkElement GetFocusedElement(Control focusVisual)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (FrameworkElement)((DependencyObject)focusVisual).GetValue(FocusedElementProperty);
	}

	private static void SetFocusedElement(Control focusVisual, FrameworkElement value)
	{
		((DependencyObject)focusVisual).SetValue(FocusedElementProperty, (object)value);
	}

	private static FrameworkElement GetTemplateFocusTarget(Control control)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (FrameworkElement)((DependencyObject)control).GetValue(TemplateFocusTargetProperty);
	}

	private static void SetTemplateFocusTarget(Control control, FrameworkElement value)
	{
		((DependencyObject)control).SetValue(TemplateFocusTargetProperty, (object)value);
	}

	private static void OnFocusVisualIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Control val = (Control)sender;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)val);
			DependencyObject obj = ((parent is Adorner) ? parent : null);
			UIElement obj2 = ((obj != null) ? ((Adorner)obj).AdornedElement : null);
			FrameworkElement val2 = (FrameworkElement)(object)((obj2 is FrameworkElement) ? obj2 : null);
			if (val2 != null)
			{
				SetShowFocusVisual(val2, value: true);
				Control val3 = (Control)(object)((val2 is Control) ? val2 : null);
				if (val3 != null && (!GetUseSystemFocusVisuals(val3) || GetTemplateFocusTarget(val3) != null))
				{
					val.Template = null;
				}
				else
				{
					TransferValue((DependencyObject)(object)val2, (DependencyObject)(object)val, FocusVisualPrimaryBrushProperty);
					TransferValue((DependencyObject)(object)val2, (DependencyObject)(object)val, FocusVisualPrimaryThicknessProperty);
					TransferValue((DependencyObject)(object)val2, (DependencyObject)(object)val, FocusVisualSecondaryBrushProperty);
					TransferValue((DependencyObject)(object)val2, (DependencyObject)(object)val, FocusVisualSecondaryThicknessProperty);
					((FrameworkElement)val).Margin = GetFocusVisualMargin(val2);
				}
				SetFocusedElement(val, val2);
			}
		}
		else
		{
			FrameworkElement focusedElement = GetFocusedElement(val);
			if (focusedElement != null)
			{
				((DependencyObject)focusedElement).ClearValue(ShowFocusVisualPropertyKey);
				((DependencyObject)val).ClearValue(FocusVisualPrimaryBrushProperty);
				((DependencyObject)val).ClearValue(FocusVisualPrimaryThicknessProperty);
				((DependencyObject)val).ClearValue(FocusVisualSecondaryBrushProperty);
				((DependencyObject)val).ClearValue(FocusVisualSecondaryThicknessProperty);
				((DependencyObject)val).ClearValue(FrameworkElement.MarginProperty);
				((DependencyObject)val).ClearValue(Control.TemplateProperty);
				((DependencyObject)val).ClearValue(FocusedElementProperty);
			}
		}
	}

	private static void TransferValue(DependencyObject source, DependencyObject target, DependencyProperty dp)
	{
		if (!source.HasDefaultValue(dp))
		{
			target.SetValue(dp, source.GetValue(dp));
		}
	}
}
