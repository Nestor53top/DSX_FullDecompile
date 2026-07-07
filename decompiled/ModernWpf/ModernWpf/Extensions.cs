using System.Windows;
using System.Windows.Media;

namespace ModernWpf;

internal static class Extensions
{
	public static GeneralTransform SafeTransformToVisual(this Visual self, Visual visual)
	{
		if (self.FindCommonVisualAncestor((DependencyObject)(object)visual) != null)
		{
			return self.TransformToVisual(visual);
		}
		return (GeneralTransform)(object)Transform.Identity;
	}
}
