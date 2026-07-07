using System.Windows;
using ModernWpf.Controls;

internal static class CppWinRTHelpers
{
	public static WinRTReturn GetTemplateChildT<WinRTReturn>(string childName, IControlProtected controlProtected) where WinRTReturn : DependencyObject
	{
		DependencyObject templateChild = controlProtected.GetTemplateChild(childName);
		if (templateChild != null)
		{
			return (WinRTReturn)(object)((templateChild is WinRTReturn) ? templateChild : null);
		}
		return default(WinRTReturn);
	}
}
