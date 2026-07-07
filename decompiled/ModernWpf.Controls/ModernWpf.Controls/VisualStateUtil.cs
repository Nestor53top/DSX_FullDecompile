using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

internal static class VisualStateUtil
{
	public static void GoToStateIfGroupExists(Control control, string groupName, string stateName, bool useTransitions)
	{
		VisualStateManager.GoToState((FrameworkElement)(object)control, stateName, useTransitions);
	}
}
