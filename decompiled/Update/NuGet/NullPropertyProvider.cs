namespace NuGet;

internal class NullPropertyProvider : IPropertyProvider
{
	private static readonly NullPropertyProvider _instance = new NullPropertyProvider();

	public static NullPropertyProvider Instance => _instance;

	private NullPropertyProvider()
	{
	}

	public dynamic GetPropertyValue(string propertyName)
	{
		return null;
	}
}
