namespace NuGet;

internal interface IPropertyProvider
{
	dynamic GetPropertyValue(string propertyName);
}
