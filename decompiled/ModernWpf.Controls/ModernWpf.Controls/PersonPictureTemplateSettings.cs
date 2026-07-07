using System.Windows;
using System.Windows.Media;

namespace ModernWpf.Controls;

public sealed class PersonPictureTemplateSettings : DependencyObject
{
	private static readonly DependencyPropertyKey ActualImageBrushPropertyKey = DependencyProperty.RegisterReadOnly("ActualImageBrush", typeof(ImageBrush), typeof(PersonPictureTemplateSettings), (PropertyMetadata)null);

	public static readonly DependencyProperty ActualImageBrushProperty = ActualImageBrushPropertyKey.DependencyProperty;

	private static readonly DependencyPropertyKey ActualInitialsPropertyKey = DependencyProperty.RegisterReadOnly("ActualInitials", typeof(string), typeof(PersonPictureTemplateSettings), new PropertyMetadata((object)string.Empty));

	public static readonly DependencyProperty ActualInitialsProperty = ActualInitialsPropertyKey.DependencyProperty;

	public ImageBrush ActualImageBrush
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageBrush)((DependencyObject)this).GetValue(ActualImageBrushProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ActualImageBrushPropertyKey, (object)value);
		}
	}

	public string ActualInitials
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(ActualInitialsProperty);
		}
		internal set
		{
			((DependencyObject)this).SetValue(ActualInitialsPropertyKey, (object)value);
		}
	}
}
