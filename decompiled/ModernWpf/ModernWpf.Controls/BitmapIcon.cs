using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ModernWpf.Controls;

public class BitmapIcon : IconElement
{
	public static readonly DependencyProperty UriSourceProperty;

	public static readonly DependencyProperty ShowAsMonochromeProperty;

	private Image _image;

	private Rectangle _foreground;

	private ImageBrush _opacityMask;

	public Uri UriSource
	{
		get
		{
			return (Uri)((DependencyObject)this).GetValue(UriSourceProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UriSourceProperty, (object)value);
		}
	}

	public bool ShowAsMonochrome
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(ShowAsMonochromeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowAsMonochromeProperty, (object)value);
		}
	}

	static BitmapIcon()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		UriSourceProperty = BitmapImage.UriSourceProperty.AddOwner(typeof(BitmapIcon), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnUriSourceChanged)));
		ShowAsMonochromeProperty = DependencyProperty.Register("ShowAsMonochrome", typeof(bool), typeof(BitmapIcon), new PropertyMetadata((object)true, new PropertyChangedCallback(OnShowAsMonochromeChanged)));
		IconElement.ForegroundProperty.OverrideMetadata(typeof(BitmapIcon), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnForegroundChanged)));
	}

	private static void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((BitmapIcon)(object)d).ApplyUriSource();
	}

	private static void OnShowAsMonochromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((BitmapIcon)(object)d).ApplyShowAsMonochrome();
	}

	private protected override void InitializeChildren()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		_image = new Image
		{
			Visibility = (Visibility)1
		};
		_opacityMask = new ImageBrush();
		_foreground = new Rectangle
		{
			OpacityMask = (Brush)(object)_opacityMask
		};
		ApplyForeground();
		ApplyUriSource();
		base.Children.Add((UIElement)(object)_image);
		ApplyShowAsMonochrome();
	}

	private protected override void OnShouldInheritForegroundFromVisualParentChanged()
	{
		ApplyForeground();
	}

	private protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		if (base.ShouldInheritForegroundFromVisualParent)
		{
			ApplyForeground();
		}
	}

	private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((BitmapIcon)(object)d).ApplyForeground();
	}

	private void ApplyForeground()
	{
		if (_foreground != null)
		{
			((Shape)_foreground).Fill = (base.ShouldInheritForegroundFromVisualParent ? base.VisualParentForeground : base.Foreground);
		}
	}

	private void ApplyUriSource()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (_image != null && _opacityMask != null)
		{
			Uri uriSource = UriSource;
			if (uriSource != null)
			{
				BitmapImage val = new BitmapImage(uriSource);
				_image.Source = (ImageSource)(object)val;
				_opacityMask.ImageSource = (ImageSource)(object)val;
			}
			else
			{
				((DependencyObject)_image).ClearValue(Image.SourceProperty);
				((DependencyObject)_opacityMask).ClearValue(ImageBrush.ImageSourceProperty);
			}
		}
	}

	private void ApplyShowAsMonochrome()
	{
		bool showAsMonochrome = ShowAsMonochrome;
		if (_image != null)
		{
			((UIElement)_image).Visibility = (Visibility)(showAsMonochrome ? 1 : 0);
		}
		if (_foreground == null)
		{
			return;
		}
		if (showAsMonochrome)
		{
			if (!base.Children.Contains((UIElement)(object)_foreground))
			{
				base.Children.Add((UIElement)(object)_foreground);
			}
		}
		else
		{
			base.Children.Remove((UIElement)(object)_foreground);
		}
	}
}
