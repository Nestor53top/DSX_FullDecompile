using System.Windows;

namespace ModernWpf.Controls;

public class RatingItemInfo : Freezable
{
	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new RatingItemInfo();
	}
}
