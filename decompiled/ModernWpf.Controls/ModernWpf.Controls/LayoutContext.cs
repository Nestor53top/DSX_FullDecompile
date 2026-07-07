using System;
using System.Windows;

namespace ModernWpf.Controls;

public class LayoutContext : DependencyObject, ILayoutContextOverrides
{
	public object LayoutState
	{
		get
		{
			return LayoutStateCore;
		}
		set
		{
			LayoutStateCore = value;
		}
	}

	protected virtual object LayoutStateCore
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	object ILayoutContextOverrides.LayoutStateCore
	{
		get
		{
			return LayoutStateCore;
		}
		set
		{
			LayoutStateCore = value;
		}
	}

	internal LayoutContext()
	{
	}
}
