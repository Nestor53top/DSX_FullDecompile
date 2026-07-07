using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using MS.Win32;

namespace ModernWpf.Controls;

internal static class SharedHelpers
{
	public static bool IsAnimationsEnabled
	{
		get
		{
			if (SystemParameters.ClientAreaAnimation)
			{
				return RenderCapability.Tier > 0;
			}
			return false;
		}
	}

	public static bool IsRS1OrHigher()
	{
		return true;
	}

	public static bool IsRS2OrHigher()
	{
		return true;
	}

	public static bool IsRS3OrHigher()
	{
		return true;
	}

	public static bool IsRS4OrHigher()
	{
		return true;
	}

	public static bool IsRS5OrHigher()
	{
		return true;
	}

	public static bool IsControlCornerRadiusAvailable()
	{
		return true;
	}

	public static bool IsThemeShadowAvailable()
	{
		return false;
	}

	public static bool IsOnXbox()
	{
		return false;
	}

	public static void QueueCallbackForCompositionRendering(Action callback)
	{
		CompositionTarget.Rendering += onRendering;
		void onRendering(object sender, EventArgs e)
		{
			CompositionTarget.Rendering -= onRendering;
			callback();
		}
	}

	public static bool DoRectsIntersect(Rect rect1, Rect rect2)
	{
		if (!(((Rect)(ref rect1)).Width <= 0.0) && !(((Rect)(ref rect1)).Height <= 0.0) && !(((Rect)(ref rect2)).Width <= 0.0) && !(((Rect)(ref rect2)).Height <= 0.0) && ((Rect)(ref rect2)).X <= ((Rect)(ref rect1)).X + ((Rect)(ref rect1)).Width && ((Rect)(ref rect2)).X + ((Rect)(ref rect2)).Width >= ((Rect)(ref rect1)).X && ((Rect)(ref rect2)).Y <= ((Rect)(ref rect1)).Y + ((Rect)(ref rect1)).Height)
		{
			return ((Rect)(ref rect2)).Y + ((Rect)(ref rect2)).Height >= ((Rect)(ref rect1)).Y;
		}
		return false;
	}

	public static object FindResource(string resource, ResourceDictionary resources, object defaultValue)
	{
		if (!resources.Contains((object)resource))
		{
			return defaultValue;
		}
		return resources[(object)resource];
	}

	public static object FindResource(string resource, FrameworkElement element, object defaultValue)
	{
		return element.TryFindResource((object)resource) ?? defaultValue;
	}

	public static object FindInApplicationResources(string resource, object defaultValue)
	{
		return FindResource(resource, Application.Current.Resources, defaultValue);
	}

	public static void SetBinding(string pathString, DependencyObject target, DependencyProperty targetProperty)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		Binding val = new Binding(pathString)
		{
			RelativeSource = RelativeSource.TemplatedParent
		};
		BindingOperations.SetBinding(target, targetProperty, (BindingBase)(object)val);
	}

	public static bool IsFrameworkElementLoaded(FrameworkElement frameworkElement)
	{
		return frameworkElement.IsLoaded;
	}

	public static AncestorType GetAncestorOfType<AncestorType>(DependencyObject firstGuess) where AncestorType : DependencyObject
	{
		DependencyObject val = firstGuess;
		AncestorType val2 = default(AncestorType);
		while (val != null && val2 == null)
		{
			val2 = (AncestorType)(object)((val is AncestorType) ? val : null);
			val = VisualTreeHelper.GetParent(val);
		}
		if (val2 != null)
		{
			return val2;
		}
		return default(AncestorType);
	}

	internal static void ForwardCollectionChange<T>(ObservableCollection<T> source, ObservableCollection<T> destination, NotifyCollectionChangedEventArgs args)
	{
		switch (args.Action)
		{
		case NotifyCollectionChangedAction.Add:
			destination.Insert(args.NewStartingIndex, (T)args.NewItems[0]);
			break;
		case NotifyCollectionChangedAction.Remove:
			destination.RemoveAt(args.OldStartingIndex);
			break;
		case NotifyCollectionChangedAction.Replace:
			destination[args.NewStartingIndex] = (T)args.NewItems[0];
			break;
		case NotifyCollectionChangedAction.Move:
			destination.Move(args.OldStartingIndex, args.NewStartingIndex);
			break;
		case NotifyCollectionChangedAction.Reset:
			CopyList(source, destination);
			break;
		}
	}

	public static void RaiseAutomationPropertyChangedEvent(UIElement element, object oldValue, object newValue)
	{
		AutomationPeer val = UIElementAutomationPeer.FromElement(element);
		if (val != null)
		{
			val.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue, newValue);
		}
	}

	public static IconElement MakeIconElementFrom(IconSource iconSource)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (iconSource is FontIconSource fontIconSource)
		{
			FontIcon fontIcon = new FontIcon();
			fontIcon.Glyph = fontIconSource.Glyph;
			fontIcon.FontSize = fontIconSource.FontSize;
			Brush foreground = fontIconSource.Foreground;
			if (foreground != null)
			{
				fontIcon.Foreground = foreground;
			}
			if (fontIconSource.FontFamily != null)
			{
				fontIcon.FontFamily = fontIconSource.FontFamily;
			}
			fontIcon.FontWeight = fontIconSource.FontWeight;
			fontIcon.FontStyle = fontIconSource.FontStyle;
			return fontIcon;
		}
		if (iconSource is SymbolIconSource symbolIconSource)
		{
			SymbolIcon symbolIcon = new SymbolIcon();
			symbolIcon.Symbol = symbolIconSource.Symbol;
			Brush foreground2 = symbolIconSource.Foreground;
			if (foreground2 != null)
			{
				symbolIcon.Foreground = foreground2;
			}
			return symbolIcon;
		}
		if (iconSource is BitmapIconSource bitmapIconSource)
		{
			BitmapIcon bitmapIcon = new BitmapIcon();
			if (bitmapIconSource.UriSource != null)
			{
				bitmapIcon.UriSource = bitmapIconSource.UriSource;
			}
			bitmapIcon.ShowAsMonochrome = bitmapIconSource.ShowAsMonochrome;
			Brush foreground3 = bitmapIconSource.Foreground;
			if (foreground3 != null)
			{
				bitmapIcon.Foreground = foreground3;
			}
			return bitmapIcon;
		}
		if (iconSource is PathIconSource pathIconSource)
		{
			PathIcon pathIcon = new PathIcon();
			if (pathIconSource.Data != null)
			{
				pathIcon.Data = pathIconSource.Data;
			}
			Brush foreground4 = pathIconSource.Foreground;
			if (foreground4 != null)
			{
				pathIcon.Foreground = foreground4;
			}
			return pathIcon;
		}
		return null;
	}

	public static BindingExpressionBase SetBinding(this FrameworkElement element, DependencyProperty dp, DependencyProperty sourceDP, DependencyObject source)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		return element.SetBinding(dp, (BindingBase)new Binding
		{
			Path = new PropertyPath((object)sourceDP),
			Source = source
		});
	}

	public static void CopyList<T>(IList<T> source, IList<T> destination)
	{
		destination.Clear();
		foreach (T item in source)
		{
			destination.Add(item);
		}
	}

	public static Window GetActiveWindow()
	{
		IntPtr activeWindow = UnsafeNativeMethods.GetActiveWindow();
		if (activeWindow != IntPtr.Zero)
		{
			HwndSource obj = HwndSource.FromHwnd(activeWindow);
			Visual obj2 = ((obj != null) ? ((PresentationSource)obj).RootVisual : null);
			return (Window)(object)((obj2 is Window) ? obj2 : null);
		}
		return null;
	}

	public static string SafeSubstring(this string s, int startIndex)
	{
		return s.SafeSubstring(startIndex, s.Length - startIndex);
	}

	public static string SafeSubstring(this string s, int startIndex, int length)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (startIndex > s.Length)
		{
			return string.Empty;
		}
		if (length > s.Length - startIndex)
		{
			length = s.Length - startIndex;
		}
		return s.Substring(startIndex, length);
	}

	public static bool IndexOf(this UIElementCollection collection, UIElement element, out int index)
	{
		int num = collection.IndexOf(element);
		if (num >= 0)
		{
			index = num;
			return true;
		}
		index = 0;
		return false;
	}

	public static string TryGetStringRepresentationFromObject(object obj)
	{
		return obj?.ToString() ?? string.Empty;
	}
}
