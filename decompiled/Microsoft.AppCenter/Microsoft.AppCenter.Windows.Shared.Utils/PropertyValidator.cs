using System.Collections.Generic;

namespace Microsoft.AppCenter.Windows.Shared.Utils;

public static class PropertyValidator
{
	internal const int MaxProperties = 20;

	internal const int MaxPropertyKeyLength = 125;

	internal const int MaxPropertyValueLength = 125;

	public static IDictionary<string, string> ValidateProperties(IDictionary<string, string> properties, string logName)
	{
		if (properties == null)
		{
			return null;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> property in properties)
		{
			if (dictionary.Count >= 20)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, $"{logName} : properties cannot contain more than {20} items. Skipping other properties.");
				break;
			}
			string text = property.Key;
			string text2 = property.Value;
			if (string.IsNullOrEmpty(text))
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, logName + " : a property key cannot be null or empty. Property will be skipped.");
				continue;
			}
			if (text2 == null)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, logName + " : property '" + text + "' : property value cannot be null. Property will be skipped.");
				continue;
			}
			if (text.Length > 125)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, $"{logName} : property '{text}' : property key length cannot be longer than {125} characters. Property key will be truncated.");
				text = text.Substring(0, 125);
			}
			if (text2.Length > 125)
			{
				AppCenterLog.Warn(AppCenterLog.LogTag, $"{logName} : property '{text}' : property value length cannot be longer than {125} characters. Property value will be truncated.");
				text2 = text2.Substring(0, 125);
			}
			dictionary.Add(text, text2);
		}
		return dictionary;
	}
}
