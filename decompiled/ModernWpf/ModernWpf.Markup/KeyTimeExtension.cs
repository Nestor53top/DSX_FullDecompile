using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace ModernWpf.Markup;

[MarkupExtensionReturnType(typeof(KeyTime))]
public class KeyTimeExtension : MarkupExtension
{
	public TimeSpan TimeSpan { get; set; } = TimeSpan.Zero;

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return KeyTime.FromTimeSpan(TimeSpan);
	}
}
