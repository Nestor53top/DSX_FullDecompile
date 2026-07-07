using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter;

public class CustomProperties
{
	private static readonly Regex KeyPattern = new Regex("^[a-zA-Z][a-zA-Z0-9]*$");

	private const int MaxCustomPropertiesCount = 60;

	internal IList<CustomProperty> Properties { get; } = new List<CustomProperty>();

	private CustomProperties SetProperty(CustomProperty property)
	{
		try
		{
			property.Validate();
		}
		catch (ValidationException ex)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, ex.Message);
			return this;
		}
		if (Properties.Count >= 60)
		{
			AppCenterLog.Error(AppCenterLog.LogTag, "Custom properties cannot contain more than " + 60 + " items.");
			return this;
		}
		CustomProperty customProperty = null;
		foreach (CustomProperty property2 in Properties)
		{
			if (property2.Name == property.Name)
			{
				customProperty = property2;
				break;
			}
		}
		if (customProperty != null)
		{
			AppCenterLog.Warn(AppCenterLog.LogTag, "Custom property \"" + property.Name + "\" is already set or cleared and will be overwritten.");
			Properties.Remove(customProperty);
		}
		Properties.Add(property);
		return this;
	}

	public CustomProperties PlatformSet(string key, string value)
	{
		return SetProperty(new StringProperty(key, value));
	}

	public CustomProperties PlatformSet(string key, DateTime value)
	{
		return SetProperty(new DateTimeProperty(key, value));
	}

	public CustomProperties PlatformSet(string key, int value)
	{
		return SetProperty(new NumberProperty(key, value));
	}

	public CustomProperties PlatformSet(string key, long value)
	{
		return SetProperty(new NumberProperty(key, value));
	}

	public CustomProperties PlatformSet(string key, float value)
	{
		return SetProperty(new NumberProperty(key, value));
	}

	public CustomProperties PlatformSet(string key, double value)
	{
		return SetProperty(new NumberProperty(key, value));
	}

	public CustomProperties PlatformSet(string key, decimal value)
	{
		return SetProperty(new NumberProperty(key, value));
	}

	public CustomProperties PlatformSet(string key, bool value)
	{
		return SetProperty(new BooleanProperty(key, value));
	}

	public CustomProperties PlatformClear(string key)
	{
		return SetProperty(new ClearProperty(key));
	}

	public CustomProperties Set(string key, string value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Set(string key, DateTime value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Set(string key, int value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Set(string key, long value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Set(string key, float value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Set(string key, double value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Set(string key, decimal value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Set(string key, bool value)
	{
		return PlatformSet(key, value);
	}

	public CustomProperties Clear(string key)
	{
		return PlatformClear(key);
	}
}
