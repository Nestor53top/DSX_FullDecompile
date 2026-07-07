using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace EasyLocalization.Localization;

public class LocalizationConverter : IMultiValueConverter
{
	private string _key;

	private string _alternativeKey;

	public LocalizationConverter(string key, string alternativeKey)
	{
		_key = key;
		_alternativeKey = alternativeKey;
	}

	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		object key;
		switch (values.Length)
		{
		default:
			return LocalizationManager.Instance.GetValue(_key) ?? LocalizationManager.Instance.GetValue(_alternativeKey, nullWhenUnfound: false);
		case 2:
			key = values.FirstOrDefault((object v) => v is string);
			if (key == null)
			{
				int count = System.Convert.ToInt32(values.First((object v) => !(v is CultureInfo)));
				return LocalizationManager.Instance.GetValue(_key, count) ?? LocalizationManager.Instance.GetValue(_alternativeKey, count, nullWhenUnfound: false);
			}
			return LocalizationManager.Instance.GetValue(key.ToString()) ?? LocalizationManager.Instance.GetValue(_alternativeKey, nullWhenUnfound: false);
		case 3:
		{
			key = values.First((object v) => v is string);
			int count = System.Convert.ToInt32(values.First((object v) => v != key && !(v is CultureInfo)));
			return LocalizationManager.Instance.GetValue(key.ToString(), count) ?? LocalizationManager.Instance.GetValue(_alternativeKey, count, nullWhenUnfound: false);
		}
		}
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
