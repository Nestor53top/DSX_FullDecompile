using System;
using System.Windows;

namespace ModernWpf.Controls;

public sealed class SelectTemplateEventArgs : EventArgs
{
	public string TemplateKey { get; set; }

	public object DataContext { get; internal set; }

	public UIElement Owner { get; internal set; }

	internal SelectTemplateEventArgs()
	{
	}
}
