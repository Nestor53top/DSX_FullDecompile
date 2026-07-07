using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification.Interop;

namespace Hardcodet.Wpf.TaskbarNotification;

internal static class Util
{
	public static readonly object SyncRoot;

	private static readonly bool isDesignMode;

	public static bool IsDesignMode => isDesignMode;

	static Util()
	{
		SyncRoot = new object();
		isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;
	}

	public static Window CreateHelperWindow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		return new Window
		{
			Width = 0.0,
			Height = 0.0,
			ShowInTaskbar = false,
			WindowStyle = (WindowStyle)0,
			AllowsTransparency = true,
			Opacity = 0.0
		};
	}

	public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command)
	{
		return WriteIconData(ref data, command, data.ValidMembers);
	}

	public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command, IconDataMembers flags)
	{
		if (IsDesignMode)
		{
			return true;
		}
		data.ValidMembers = flags;
		lock (SyncRoot)
		{
			return WinApi.Shell_NotifyIcon(command, ref data);
		}
	}

	public static BalloonFlags GetBalloonFlag(this BalloonIcon icon)
	{
		return icon switch
		{
			BalloonIcon.None => BalloonFlags.None, 
			BalloonIcon.Info => BalloonFlags.Info, 
			BalloonIcon.Warning => BalloonFlags.Warning, 
			BalloonIcon.Error => BalloonFlags.Error, 
			_ => throw new ArgumentOutOfRangeException("icon"), 
		};
	}

	public static Icon ToIcon(this ImageSource imageSource)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		if (imageSource == null)
		{
			return null;
		}
		Uri uri = new Uri(((object)imageSource).ToString());
		StreamResourceInfo resourceStream = Application.GetResourceStream(uri);
		if (resourceStream == null)
		{
			string format = "The supplied image source '{0}' could not be resolved.";
			format = string.Format(format, imageSource);
			throw new ArgumentException(format);
		}
		return new Icon(resourceStream.Stream);
	}

	public static bool Is<T>(this T value, params T[] candidates)
	{
		if (candidates == null)
		{
			return false;
		}
		foreach (T val in candidates)
		{
			if (value.Equals(val))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsMatch(this MouseEvent me, PopupActivationMode activationMode)
	{
		return activationMode switch
		{
			PopupActivationMode.LeftClick => me == MouseEvent.IconLeftMouseUp, 
			PopupActivationMode.RightClick => me == MouseEvent.IconRightMouseUp, 
			PopupActivationMode.LeftOrRightClick => me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconRightMouseUp), 
			PopupActivationMode.LeftOrDoubleClick => me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconDoubleClick), 
			PopupActivationMode.DoubleClick => me.Is(MouseEvent.IconDoubleClick), 
			PopupActivationMode.MiddleClick => me == MouseEvent.IconMiddleMouseUp, 
			PopupActivationMode.All => me != MouseEvent.MouseMove, 
			_ => throw new ArgumentOutOfRangeException("activationMode"), 
		};
	}

	public static void ExecuteIfEnabled(this ICommand command, object commandParameter, IInputElement target)
	{
		if (command == null)
		{
			return;
		}
		RoutedCommand val = (RoutedCommand)((command is RoutedCommand) ? command : null);
		if (val != null)
		{
			if (val.CanExecute(commandParameter, target))
			{
				val.Execute(commandParameter, target);
			}
		}
		else if (command.CanExecute(commandParameter))
		{
			command.Execute(commandParameter);
		}
	}

	internal static Dispatcher GetDispatcher(this DispatcherObject source)
	{
		if (Application.Current != null)
		{
			return ((DispatcherObject)Application.Current).Dispatcher;
		}
		if (source.Dispatcher != null)
		{
			return source.Dispatcher;
		}
		return Dispatcher.CurrentDispatcher;
	}

	public static bool IsDataContextDataBound(this FrameworkElement element)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		return element.GetBindingExpression(FrameworkElement.DataContextProperty) != null;
	}
}
