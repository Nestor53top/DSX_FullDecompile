using System.Windows;
using System.Windows.Media;

namespace ModernWpf.Controls;

public class RatingItemImageInfo : RatingItemInfo
{
	public static readonly DependencyProperty DisabledImageProperty = DependencyProperty.Register("DisabledImage", typeof(ImageSource), typeof(RatingItemImageInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(RatingItemImageInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty PlaceholderImageProperty = DependencyProperty.Register("PlaceholderImage", typeof(ImageSource), typeof(RatingItemImageInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty PointerOverImageProperty = DependencyProperty.Register("PointerOverImage", typeof(ImageSource), typeof(RatingItemImageInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty PointerOverPlaceholderImageProperty = DependencyProperty.Register("PointerOverPlaceholderImage", typeof(ImageSource), typeof(RatingItemImageInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty UnsetImageProperty = DependencyProperty.Register("UnsetImage", typeof(ImageSource), typeof(RatingItemImageInfo), (PropertyMetadata)null);

	public ImageSource DisabledImage
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(DisabledImageProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DisabledImageProperty, (object)value);
		}
	}

	public ImageSource Image
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(ImageProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ImageProperty, (object)value);
		}
	}

	public ImageSource PlaceholderImage
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(PlaceholderImageProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PlaceholderImageProperty, (object)value);
		}
	}

	public ImageSource PointerOverImage
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(PointerOverImageProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PointerOverImageProperty, (object)value);
		}
	}

	public ImageSource PointerOverPlaceholderImage
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(PointerOverPlaceholderImageProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PointerOverPlaceholderImageProperty, (object)value);
		}
	}

	public ImageSource UnsetImage
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (ImageSource)((DependencyObject)this).GetValue(UnsetImageProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UnsetImageProperty, (object)value);
		}
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new RatingItemImageInfo();
	}
}
