using System.Windows;

namespace ModernWpf.Controls;

internal interface IControlProtected
{
	DependencyObject GetTemplateChild(string childName);
}
