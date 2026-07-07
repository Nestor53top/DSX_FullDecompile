using System;
using System.Windows.Navigation;

namespace ModernWpf.Navigation;

public static class Extensions
{
	public static Type SourcePageType(this NavigatingCancelEventArgs e)
	{
		return e.Content?.GetType();
	}

	public static Type SourcePageType(this NavigationEventArgs e)
	{
		return e.Content?.GetType();
	}

	public static object Parameter(this NavigatingCancelEventArgs e)
	{
		return e.ExtraData;
	}

	public static object Parameter(this NavigationEventArgs e)
	{
		return e.ExtraData;
	}
}
