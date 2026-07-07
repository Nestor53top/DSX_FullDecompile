using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives;

public static class ValidationHelper
{
	public static readonly DependencyProperty IsTemplateValidationAdornerSiteProperty = DependencyProperty.RegisterAttached("IsTemplateValidationAdornerSite", typeof(bool), typeof(ValidationHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsTemplateValidationAdornerSiteChanged)));

	public static bool GetIsTemplateValidationAdornerSite(FrameworkElement element)
	{
		return (bool)((DependencyObject)element).GetValue(IsTemplateValidationAdornerSiteProperty);
	}

	public static void SetIsTemplateValidationAdornerSite(FrameworkElement element, bool value)
	{
		((DependencyObject)element).SetValue(IsTemplateValidationAdornerSiteProperty, (object)value);
	}

	private static void OnIsTemplateValidationAdornerSiteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		FrameworkElement val = (FrameworkElement)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			Validation.SetErrorTemplate((DependencyObject)(object)val, (ControlTemplate)null);
			Validation.SetValidationAdornerSiteFor((DependencyObject)(object)val, val.TemplatedParent);
		}
		else
		{
			((DependencyObject)val).ClearValue(Validation.ErrorTemplateProperty);
			((DependencyObject)val).ClearValue(Validation.ValidationAdornerSiteForProperty);
		}
	}
}
