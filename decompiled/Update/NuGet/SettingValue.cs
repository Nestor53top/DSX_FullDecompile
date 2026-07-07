using System;

namespace NuGet;

internal class SettingValue
{
	public string Key { get; private set; }

	public string Value { get; private set; }

	public bool IsMachineWide { get; private set; }

	public int Priority { get; private set; }

	public SettingValue(string key, string value, bool isMachineWide, int priority = 0)
	{
		Key = key;
		Value = value;
		IsMachineWide = isMachineWide;
		Priority = priority;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SettingValue settingValue))
		{
			return false;
		}
		if (settingValue.Key == Key && settingValue.Value == Value)
		{
			return settingValue.IsMachineWide == settingValue.IsMachineWide;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Tuple.Create(Key, Value, IsMachineWide).GetHashCode();
	}
}
