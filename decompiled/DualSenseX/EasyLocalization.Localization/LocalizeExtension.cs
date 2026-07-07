using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace EasyLocalization.Localization;

public class LocalizeExtension : MarkupExtension
{
	public string Key { get; set; }

	public string Placeholder { get; set; }

	public Binding KeySource { get; set; }

	public Binding CountSource { get; set; }

	public LocalizeExtension()
	{
	}

	public LocalizeExtension(string key)
	{
		Key = key;
	}

	public LocalizeExtension(string key, Binding countSource)
	{
		Key = key;
		CountSource = countSource;
	}

	public LocalizeExtension(Binding keySource, Binding countSource)
	{
		KeySource = keySource;
		CountSource = countSource;
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a2: Expected O, but got Unknown
		IServiceProvider obj = ((serviceProvider is IProvideValueTarget) ? serviceProvider : null);
		object obj2 = ((obj != null) ? ((IProvideValueTarget)obj).TargetObject : null);
		FrameworkElement val = (FrameworkElement)((obj2 is FrameworkElement) ? obj2 : null);
		object obj3 = ((obj != null) ? ((IProvideValueTarget)obj).TargetProperty : null);
		DependencyProperty val2 = (DependencyProperty)((obj3 is DependencyProperty) ? obj3 : null);
		_ = ((val != null) ? val.Name : null) + "_" + ((val2 != null) ? val2.Name : null);
		MultiBinding val3 = new MultiBinding
		{
			Converter = (IMultiValueConverter)(object)new LocalizationConverter(Key, Placeholder),
			NotifyOnSourceUpdated = true
		};
		val3.Bindings.Add((BindingBase)new Binding
		{
			Source = LocalizationManager.Instance,
			Path = new PropertyPath("CurrentCulture", Array.Empty<object>())
		});
		if (KeySource != null)
		{
			val3.Bindings.Add((BindingBase)(object)KeySource);
		}
		if (CountSource != null)
		{
			val3.Bindings.Add((BindingBase)(object)CountSource);
		}
		return ((MarkupExtension)val3).ProvideValue(serviceProvider);
	}
}
