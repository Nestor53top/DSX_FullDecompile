using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ModernWpf;

public static class VisualTree
{
	public static FrameworkElement FindDescendantByName(this DependencyObject element, string name)
	{
		if (element == null || string.IsNullOrWhiteSpace(name))
		{
			return null;
		}
		DependencyObject obj = ((element is FrameworkElement) ? element : null);
		if (name.Equals((obj != null) ? ((FrameworkElement)obj).Name : null, StringComparison.OrdinalIgnoreCase))
		{
			return (FrameworkElement)(object)((element is FrameworkElement) ? element : null);
		}
		int childrenCount = VisualTreeHelper.GetChildrenCount(element);
		for (int i = 0; i < childrenCount; i++)
		{
			FrameworkElement val = VisualTreeHelper.GetChild(element, i).FindDescendantByName(name);
			if (val != null)
			{
				return val;
			}
		}
		return null;
	}

	public static T FindDescendant<T>(this DependencyObject element) where T : DependencyObject
	{
		T val = default(T);
		int childrenCount = VisualTreeHelper.GetChildrenCount(element);
		for (int i = 0; i < childrenCount; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(element, i);
			T val2 = (T)(object)((child is T) ? child : null);
			if (val2 != null)
			{
				val = val2;
				break;
			}
			val = child.FindDescendant<T>();
			if (val != null)
			{
				break;
			}
		}
		return val;
	}

	public static object FindDescendant(this DependencyObject element, Type type)
	{
		object obj = null;
		int childrenCount = VisualTreeHelper.GetChildrenCount(element);
		for (int i = 0; i < childrenCount; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(element, i);
			if (((object)child).GetType() == type)
			{
				obj = child;
				break;
			}
			obj = child.FindDescendant(type);
			if (obj != null)
			{
				break;
			}
		}
		return obj;
	}

	public static IEnumerable<T> FindDescendants<T>(this DependencyObject element) where T : DependencyObject
	{
		int childrenCount = VisualTreeHelper.GetChildrenCount(element);
		for (int i = 0; i < childrenCount; i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(element, i);
			T val = (T)(object)((child is T) ? child : null);
			if (val != null)
			{
				yield return val;
			}
			foreach (T item in child.FindDescendants<T>())
			{
				yield return item;
			}
		}
	}

	public static FrameworkElement FindAscendantByName(this DependencyObject element, string name)
	{
		if (element == null || string.IsNullOrWhiteSpace(name))
		{
			return null;
		}
		DependencyObject parent = VisualTreeHelper.GetParent(element);
		if (parent == null)
		{
			return null;
		}
		DependencyObject obj = ((parent is FrameworkElement) ? parent : null);
		if (name.Equals((obj != null) ? ((FrameworkElement)obj).Name : null, StringComparison.OrdinalIgnoreCase))
		{
			return (FrameworkElement)(object)((parent is FrameworkElement) ? parent : null);
		}
		return parent.FindAscendantByName(name);
	}

	public static T FindAscendant<T>(this DependencyObject element) where T : DependencyObject
	{
		DependencyObject parent = VisualTreeHelper.GetParent(element);
		if (parent == null)
		{
			return default(T);
		}
		if (parent is T)
		{
			return (T)(object)((parent is T) ? parent : null);
		}
		return parent.FindAscendant<T>();
	}

	public static object FindAscendant(this DependencyObject element, Type type)
	{
		DependencyObject parent = VisualTreeHelper.GetParent(element);
		if (parent == null)
		{
			return null;
		}
		if (((object)parent).GetType() == type)
		{
			return parent;
		}
		return parent.FindAscendant(type);
	}

	public static IEnumerable<DependencyObject> FindAscendants(this DependencyObject element)
	{
		for (DependencyObject parent = VisualTreeHelper.GetParent(element); parent != null; parent = VisualTreeHelper.GetParent(parent))
		{
			yield return parent;
		}
	}
}
