namespace Squirrel.SimpleSplat;

internal static class LogManagerMixin
{
	public static IFullLogger GetLogger<T>(this ILogManager This)
	{
		return This.GetLogger(typeof(T));
	}
}
