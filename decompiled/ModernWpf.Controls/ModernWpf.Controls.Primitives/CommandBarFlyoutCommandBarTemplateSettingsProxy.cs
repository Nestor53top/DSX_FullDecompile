using System.Windows;

namespace ModernWpf.Controls.Primitives;

public class CommandBarFlyoutCommandBarTemplateSettingsProxy : Freezable
{
	public static readonly DependencyProperty FlyoutTemplateSettingsProperty = DependencyProperty.Register("FlyoutTemplateSettings", typeof(CommandBarFlyoutCommandBarTemplateSettings), typeof(CommandBarFlyoutCommandBarTemplateSettingsProxy), (PropertyMetadata)null);

	public CommandBarFlyoutCommandBarTemplateSettings FlyoutTemplateSettings
	{
		get
		{
			return (CommandBarFlyoutCommandBarTemplateSettings)((DependencyObject)this).GetValue(FlyoutTemplateSettingsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(FlyoutTemplateSettingsProperty, (object)value);
		}
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new CommandBarFlyoutCommandBarTemplateSettingsProxy();
	}
}
