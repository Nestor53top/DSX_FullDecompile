using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace ModernWpf;

internal static class MenuDropAlignmentHelper
{
	private static readonly FieldInfo _menuDropAlignmentField;

	static MenuDropAlignmentHelper()
	{
		try
		{
			_menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.Static | BindingFlags.NonPublic);
		}
		catch (Exception)
		{
		}
		if (_menuDropAlignmentField != null)
		{
			EnsureStandardPopupAlignment();
			SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
		}
	}

	public static void EnsureStandardPopupAlignment()
	{
		if (SystemParameters.MenuDropAlignment)
		{
			try
			{
				_menuDropAlignmentField.SetValue(null, false);
			}
			catch (Exception)
			{
			}
		}
	}

	private static void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == "MenuDropAlignment")
		{
			EnsureStandardPopupAlignment();
		}
	}
}
