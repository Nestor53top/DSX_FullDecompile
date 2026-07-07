using System;
using System.ComponentModel;
using System.Windows;

namespace ModernWpf.DesignTime;

public abstract class IntellisenseResourcesBase : ResourceDictionary, ISupportInitialize
{
	public Uri Source
	{
		get
		{
			return ((ResourceDictionary)this).Source;
		}
		set
		{
			if (DesignMode.DesignModeEnabled)
			{
				((ResourceDictionary)this).Source = value;
			}
		}
	}

	public void EndInit()
	{
		((ResourceDictionary)this).Clear();
		((ResourceDictionary)this).MergedDictionaries.Clear();
		((ResourceDictionary)this).EndInit();
	}

	void ISupportInitialize.EndInit()
	{
		EndInit();
	}
}
