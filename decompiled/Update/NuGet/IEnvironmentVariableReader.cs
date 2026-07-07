namespace NuGet;

internal interface IEnvironmentVariableReader
{
	string GetEnvironmentVariable(string variable);
}
