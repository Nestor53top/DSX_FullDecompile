using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace ModernWpf.Markup;

[TypeConverter(typeof(ThemeResouceExtensionConverter))]
public class ThemeResourceExtension : DynamicResourceExtension
{
	private class SystemColorsSource : INotifyPropertyChanged
	{
		public static SystemColorsSource Current { get; } = new SystemColorsSource();

		public Color SystemColorButtonFaceColor => SystemColors.ControlColor;

		public Color SystemColorButtonTextColor => SystemColors.ControlTextColor;

		public Color SystemColorGrayTextColor => SystemColors.GrayTextColor;

		public Color SystemColorHighlightColor => SystemColors.HighlightColor;

		public Color SystemColorHighlightTextColor => SystemColors.HighlightTextColor;

		public Color SystemColorHotlightColor => SystemColors.HotTrackColor;

		public Color SystemColorWindowColor => SystemColors.WindowColor;

		public Color SystemColorWindowTextColor => SystemColors.WindowTextColor;

		public Color SystemColorActiveCaptionColor => SystemColors.ActiveCaptionColor;

		public Color SystemColorInactiveCaptionTextColor => SystemColors.InactiveCaptionTextColor;

		public event PropertyChangedEventHandler PropertyChanged;

		private SystemColorsSource()
		{
			SystemParameters.StaticPropertyChanged += OnSystemParametersPropertyChanged;
		}

		private void OnSystemParametersPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "HighContrast" && SystemParameters.HighContrast)
			{
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
			}
		}
	}

	public ThemeResourceExtension()
	{
	}

	public ThemeResourceExtension(object resourceKey)
		: base(resourceKey)
	{
	}

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (((DynamicResourceExtension)this).ResourceKey is string text && text.StartsWith("SystemColor", StringComparison.Ordinal))
		{
			return ((MarkupExtension)new Binding(text)
			{
				Source = SystemColorsSource.Current
			}).ProvideValue(serviceProvider);
		}
		return ((DynamicResourceExtension)this).ProvideValue(serviceProvider);
	}
}
