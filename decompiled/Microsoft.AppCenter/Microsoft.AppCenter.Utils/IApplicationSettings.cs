namespace Microsoft.AppCenter.Utils;

public interface IApplicationSettings
{
	T GetValue<T>(string key, T defaultValue = default(T));

	void SetValue(string key, object value);

	bool ContainsKey(string key);

	void Remove(string key);
}
