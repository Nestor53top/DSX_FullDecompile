using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ModernWpf.Controls;

public class BitmapIconSource : IconSource
{
	public static readonly DependencyProperty UriSourceProperty = BitmapImage.UriSourceProperty.AddOwner(typeof(BitmapIconSource));

	public static readonly DependencyProperty ShowAsMonochromeProperty = DependencyProperty.Register("ShowAsMonochrome", typeof(bool), typeof(BitmapIconSource), new PropertyMetadata((object)true));

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
}
