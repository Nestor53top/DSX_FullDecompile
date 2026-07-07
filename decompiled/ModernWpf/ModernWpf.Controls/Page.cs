using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace ModernWpf.Controls;

public class Page : PageFunctionBase
{
	private static readonly DependencyPropertyKey FramePropertyKey;

	public static readonly DependencyProperty FrameProperty;

	private static readonly Type NavigationServiceType;

	public Frame Frame
	{
		get
		{
			return (Frame)((DependencyObject)this).GetValue(FrameProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(FramePropertyKey, (object)value);
		}
	}

	static Page()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		FramePropertyKey = DependencyProperty.RegisterReadOnly("Frame", typeof(Frame), typeof(Page), (PropertyMetadata)null);
		FrameProperty = FramePropertyKey.DependencyProperty;
		NavigationServiceType = typeof(NavigationService);
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Page), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(Page)));
		Page.BackgroundProperty.OverrideMetadata(typeof(Page), (PropertyMetadata)new FrameworkPropertyMetadata((object)Brushes.Transparent));
		Page.FontSizeProperty.OverrideMetadata(typeof(Page), (PropertyMetadata)new FrameworkPropertyMetadata((object)14.0));
	}

	private void UpdateFrame(NavigationService navigationService)
	{
		if (navigationService != null)
		{
			Frame frame = Frame.GetFrame(navigationService);
			if (frame != null)
			{
				Frame = frame;
				return;
			}
		}
		((DependencyObject)this).ClearValue(FramePropertyKey);
	}

	protected virtual void OnNavigatedTo(NavigationEventArgs e)
	{
	}

	protected virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
	{
	}

	protected virtual void OnNavigatedFrom(NavigationEventArgs e)
	{
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		((FrameworkElement)this).OnPropertyChanged(e);
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property.PropertyType == NavigationServiceType && ((DependencyPropertyChangedEventArgs)(ref e)).Property.OwnerType == NavigationServiceType)
		{
			UpdateFrame((NavigationService)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
		}
	}

	internal void InternalOnNavigatedTo(NavigationEventArgs e)
	{
		OnNavigatedTo(e);
	}

	internal void InternalOnNavigatingFrom(NavigatingCancelEventArgs e)
	{
		OnNavigatingFrom(e);
	}

	internal void InternalOnNavigatedFrom(NavigationEventArgs e)
	{
		OnNavigatedFrom(e);
	}
}
