using System;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using WpfAnimatedGif.Decoding;

namespace WpfAnimatedGif;

internal static class ImageBehavior
{
	private struct Int32Size
	{
		public int Width { get; private set; }

		public int Height { get; private set; }

		public Int32Size(int width, int height)
		{
			this = default(Int32Size);
			Width = width;
			Height = height;
		}
	}

	private class FrameMetadata
	{
		public int Left { get; set; }

		public int Top { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public TimeSpan Delay { get; set; }

		public FrameDisposalMethod DisposalMethod { get; set; }
	}

	private enum FrameDisposalMethod
	{
		None,
		DoNotDispose,
		RestoreBackground,
		RestorePrevious
	}

	public static readonly DependencyProperty AnimatedSourceProperty = DependencyProperty.RegisterAttached("AnimatedSource", typeof(ImageSource), typeof(ImageBehavior), (PropertyMetadata)new UIPropertyMetadata((object)null, new PropertyChangedCallback(AnimatedSourceChanged)));

	public static readonly DependencyProperty RepeatBehaviorProperty = DependencyProperty.RegisterAttached("RepeatBehavior", typeof(RepeatBehavior), typeof(ImageBehavior), (PropertyMetadata)new UIPropertyMetadata((object)default(RepeatBehavior), new PropertyChangedCallback(RepeatBehaviorChanged)));

	public static readonly DependencyProperty AnimateInDesignModeProperty = DependencyProperty.RegisterAttached("AnimateInDesignMode", typeof(bool), typeof(ImageBehavior), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, (FrameworkPropertyMetadataOptions)32, new PropertyChangedCallback(AnimateInDesignModeChanged)));

	public static readonly DependencyProperty AutoStartProperty = DependencyProperty.RegisterAttached("AutoStart", typeof(bool), typeof(ImageBehavior), new PropertyMetadata((object)true));

	private static readonly DependencyPropertyKey AnimationControllerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("AnimationController", typeof(ImageAnimationController), typeof(ImageBehavior), new PropertyMetadata((PropertyChangedCallback)null));

	private static readonly DependencyPropertyKey IsAnimationLoadedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsAnimationLoaded", typeof(bool), typeof(ImageBehavior), new PropertyMetadata((object)false));

	public static readonly DependencyProperty IsAnimationLoadedProperty = IsAnimationLoadedPropertyKey.DependencyProperty;

	public static readonly RoutedEvent AnimationLoadedEvent = EventManager.RegisterRoutedEvent("AnimationLoaded", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(ImageBehavior));

	public static readonly RoutedEvent AnimationCompletedEvent = EventManager.RegisterRoutedEvent("AnimationCompleted", (RoutingStrategy)1, typeof(RoutedEventHandler), typeof(ImageBehavior));

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static ImageSource GetAnimatedSource(Image obj)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (ImageSource)((DependencyObject)obj).GetValue(AnimatedSourceProperty);
	}

	public static void SetAnimatedSource(Image obj, ImageSource value)
	{
		((DependencyObject)obj).SetValue(AnimatedSourceProperty, (object)value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static RepeatBehavior GetRepeatBehavior(Image obj)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (RepeatBehavior)((DependencyObject)obj).GetValue(RepeatBehaviorProperty);
	}

	public static void SetRepeatBehavior(Image obj, RepeatBehavior value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)obj).SetValue(RepeatBehaviorProperty, (object)value);
	}

	public static bool GetAnimateInDesignMode(DependencyObject obj)
	{
		return (bool)obj.GetValue(AnimateInDesignModeProperty);
	}

	public static void SetAnimateInDesignMode(DependencyObject obj, bool value)
	{
		obj.SetValue(AnimateInDesignModeProperty, (object)value);
	}

	[AttachedPropertyBrowsableForType(typeof(Image))]
	public static bool GetAutoStart(Image obj)
	{
		return (bool)((DependencyObject)obj).GetValue(AutoStartProperty);
	}

	public static void SetAutoStart(Image obj, bool value)
	{
		((DependencyObject)obj).SetValue(AutoStartProperty, (object)value);
	}

	public static ImageAnimationController GetAnimationController(Image imageControl)
	{
		return (ImageAnimationController)((DependencyObject)imageControl).GetValue(AnimationControllerPropertyKey.DependencyProperty);
	}

	private static void SetAnimationController(DependencyObject obj, ImageAnimationController value)
	{
		obj.SetValue(AnimationControllerPropertyKey, (object)value);
	}

	public static bool GetIsAnimationLoaded(Image image)
	{
		return (bool)((DependencyObject)image).GetValue(IsAnimationLoadedProperty);
	}

	private static void SetIsAnimationLoaded(Image image, bool value)
	{
		((DependencyObject)image).SetValue(IsAnimationLoadedPropertyKey, (object)value);
	}

	public static void AddAnimationLoadedHandler(Image image, RoutedEventHandler handler)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		((UIElement)image).AddHandler(AnimationLoadedEvent, (Delegate)(object)handler);
	}

	public static void RemoveAnimationLoadedHandler(Image image, RoutedEventHandler handler)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		((UIElement)image).RemoveHandler(AnimationLoadedEvent, (Delegate)(object)handler);
	}

	public static void AddAnimationCompletedHandler(Image d, RoutedEventHandler handler)
	{
		if (d != null)
		{
			((UIElement)d).AddHandler(AnimationCompletedEvent, (Delegate)(object)handler);
		}
	}

	public static void RemoveAnimationCompletedHandler(Image d, RoutedEventHandler handler)
	{
		if (d != null)
		{
			((UIElement)d).RemoveHandler(AnimationCompletedEvent, (Delegate)(object)handler);
		}
	}

	private static void AnimatedSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		Image val = (Image)(object)((o is Image) ? o : null);
		if (val == null)
		{
			return;
		}
		object oldValue = ((DependencyPropertyChangedEventArgs)(ref e)).OldValue;
		ImageSource val2 = (ImageSource)((oldValue is ImageSource) ? oldValue : null);
		object newValue = ((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		ImageSource val3 = (ImageSource)((newValue is ImageSource) ? newValue : null);
		if (val2 == val3)
		{
			return;
		}
		if (val2 != null)
		{
			((FrameworkElement)val).Loaded -= new RoutedEventHandler(ImageControlLoaded);
			((FrameworkElement)val).Unloaded -= new RoutedEventHandler(ImageControlUnloaded);
			AnimationCache.DecrementReferenceCount(val2, GetRepeatBehavior(val));
			GetAnimationController(val)?.Dispose();
			val.Source = null;
		}
		if (val3 != null)
		{
			((FrameworkElement)val).Loaded += new RoutedEventHandler(ImageControlLoaded);
			((FrameworkElement)val).Unloaded += new RoutedEventHandler(ImageControlUnloaded);
			if (((FrameworkElement)val).IsLoaded)
			{
				InitAnimationOrImage(val);
			}
		}
	}

	private static void ImageControlLoaded(object sender, RoutedEventArgs e)
	{
		Image val = (Image)((sender is Image) ? sender : null);
		if (val != null)
		{
			InitAnimationOrImage(val);
		}
	}

	private static void ImageControlUnloaded(object sender, RoutedEventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Image val = (Image)((sender is Image) ? sender : null);
		if (val != null)
		{
			ImageSource animatedSource = GetAnimatedSource(val);
			if (animatedSource != null)
			{
				AnimationCache.DecrementReferenceCount(animatedSource, GetRepeatBehavior(val));
			}
			GetAnimationController(val)?.Dispose();
		}
	}

	private static void RepeatBehaviorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Image val = (Image)(object)((o is Image) ? o : null);
		if (val == null)
		{
			return;
		}
		ImageSource animatedSource = GetAnimatedSource(val);
		if (animatedSource != null)
		{
			if (!object.Equals(((DependencyPropertyChangedEventArgs)(ref e)).OldValue, ((DependencyPropertyChangedEventArgs)(ref e)).NewValue))
			{
				AnimationCache.DecrementReferenceCount(animatedSource, (RepeatBehavior)((DependencyPropertyChangedEventArgs)(ref e)).OldValue);
			}
			if (((FrameworkElement)val).IsLoaded)
			{
				InitAnimationOrImage(val);
			}
		}
	}

	private static void AnimateInDesignModeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
	{
		Image val = (Image)(object)((o is Image) ? o : null);
		if (val == null)
		{
			return;
		}
		bool flag = (bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue;
		if (GetAnimatedSource(val) != null && ((FrameworkElement)val).IsLoaded)
		{
			if (flag)
			{
				InitAnimationOrImage(val);
			}
			else
			{
				((UIElement)val).BeginAnimation(Image.SourceProperty, (AnimationTimeline)null);
			}
		}
	}

	private static void InitAnimationOrImage(Image imageControl)
	{
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Expected O, but got Unknown
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		GetAnimationController(imageControl)?.Dispose();
		SetAnimationController((DependencyObject)(object)imageControl, null);
		SetIsAnimationLoaded(imageControl, value: false);
		ImageSource animatedSource = GetAnimatedSource(imageControl);
		BitmapSource source = (BitmapSource)(object)((animatedSource is BitmapSource) ? animatedSource : null);
		bool isInDesignMode = DesignerProperties.GetIsInDesignMode((DependencyObject)(object)imageControl);
		bool animateInDesignMode = GetAnimateInDesignMode((DependencyObject)(object)imageControl);
		bool flag = !isInDesignMode || animateInDesignMode;
		bool flag2 = IsLoadingDeferred(source);
		if (source != null && flag && !flag2)
		{
			if (source.IsDownloading)
			{
				EventHandler handler = null;
				handler = delegate
				{
					source.DownloadCompleted -= handler;
					InitAnimationOrImage(imageControl);
				};
				source.DownloadCompleted += handler;
				imageControl.Source = (ImageSource)(object)source;
				return;
			}
			ObjectAnimationUsingKeyFrames animation = GetAnimation(imageControl, source);
			if (animation != null)
			{
				if (animation.KeyFrames.Count > 0)
				{
					TryTwice(delegate
					{
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Expected O, but got Unknown
						imageControl.Source = (ImageSource)animation.KeyFrames[0].Value;
					});
				}
				else
				{
					imageControl.Source = (ImageSource)(object)source;
				}
				ImageAnimationController value = new ImageAnimationController(imageControl, animation, GetAutoStart(imageControl));
				SetAnimationController((DependencyObject)(object)imageControl, value);
				SetIsAnimationLoaded(imageControl, value: true);
				((UIElement)imageControl).RaiseEvent(new RoutedEventArgs(AnimationLoadedEvent, (object)imageControl));
				return;
			}
		}
		imageControl.Source = (ImageSource)(object)source;
		if (source != null)
		{
			SetIsAnimationLoaded(imageControl, value: true);
			((UIElement)imageControl).RaiseEvent(new RoutedEventArgs(AnimationLoadedEvent, (object)imageControl));
		}
	}

	private static ObjectAnimationUsingKeyFrames GetAnimation(Image imageControl, BitmapSource source)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		ObjectAnimationUsingKeyFrames animation = AnimationCache.GetAnimation((ImageSource)(object)source, GetRepeatBehavior(imageControl));
		if (animation != null)
		{
			return animation;
		}
		GifFile gifFile;
		BitmapDecoder decoder = GetDecoder(source, out gifFile);
		GifBitmapDecoder val = (GifBitmapDecoder)(object)((decoder is GifBitmapDecoder) ? decoder : null);
		if (val != null && ((BitmapDecoder)val).Frames.Count > 1)
		{
			Int32Size fullSize = GetFullSize((BitmapDecoder)(object)val, gifFile);
			int num = 0;
			animation = new ObjectAnimationUsingKeyFrames();
			TimeSpan zero = TimeSpan.Zero;
			BitmapSource baseFrame = null;
			foreach (BitmapFrame frame in ((BitmapDecoder)val).Frames)
			{
				FrameMetadata frameMetadata = GetFrameMetadata((BitmapDecoder)(object)val, gifFile, num);
				BitmapSource val2 = MakeFrame(fullSize, (BitmapSource)(object)frame, frameMetadata, baseFrame);
				DiscreteObjectKeyFrame val3 = new DiscreteObjectKeyFrame((object)val2, KeyTime.op_Implicit(zero));
				animation.KeyFrames.Add((ObjectKeyFrame)(object)val3);
				zero += frameMetadata.Delay;
				switch (frameMetadata.DisposalMethod)
				{
				case FrameDisposalMethod.None:
				case FrameDisposalMethod.DoNotDispose:
					baseFrame = val2;
					break;
				case FrameDisposalMethod.RestoreBackground:
					baseFrame = ((!IsFullFrame(frameMetadata, fullSize)) ? ClearArea(val2, frameMetadata) : null);
					break;
				}
				num++;
			}
			((Timeline)animation).Duration = Duration.op_Implicit(zero);
			((Timeline)animation).RepeatBehavior = GetActualRepeatBehavior(imageControl, (BitmapDecoder)(object)val, gifFile);
			AnimationCache.AddAnimation((ImageSource)(object)source, GetRepeatBehavior(imageControl), animation);
			AnimationCache.IncrementReferenceCount((ImageSource)(object)source, GetRepeatBehavior(imageControl));
			return animation;
		}
		return null;
	}

	private static BitmapSource ClearArea(BitmapSource frame, FrameMetadata metadata)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_006b: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		DrawingVisual val = new DrawingVisual();
		DrawingContext val2 = val.RenderOpen();
		try
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(0.0, 0.0, (double)frame.PixelWidth, (double)frame.PixelHeight);
			Rect val4 = default(Rect);
			((Rect)(ref val4))._002Ector((double)metadata.Left, (double)metadata.Top, (double)metadata.Width, (double)metadata.Height);
			PathGeometry val5 = Geometry.Combine((Geometry)new RectangleGeometry(val3), (Geometry)new RectangleGeometry(val4), (GeometryCombineMode)3, (Transform)null);
			val2.PushClip((Geometry)(object)val5);
			val2.DrawImage((ImageSource)(object)frame, val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		RenderTargetBitmap val6 = new RenderTargetBitmap(frame.PixelWidth, frame.PixelHeight, frame.DpiX, frame.DpiY, PixelFormats.Pbgra32);
		val6.Render((Visual)(object)val);
		WriteableBitmap val7 = new WriteableBitmap((BitmapSource)val6);
		if (((Freezable)val7).CanFreeze && !((Freezable)val7).IsFrozen)
		{
			((Freezable)val7).Freeze();
		}
		return (BitmapSource)(object)val7;
	}

	private static void TryTwice(Action action)
	{
		try
		{
			action();
		}
		catch (Exception)
		{
			action();
		}
	}

	private static bool IsLoadingDeferred(BitmapSource source)
	{
		BitmapImage val = (BitmapImage)(object)((source is BitmapImage) ? source : null);
		if (val == null)
		{
			return false;
		}
		if (val.UriSource != null && !val.UriSource.IsAbsoluteUri)
		{
			return val.BaseUri == null;
		}
		return false;
	}

	private static BitmapDecoder GetDecoder(BitmapSource image, out GifFile gifFile)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		gifFile = null;
		BitmapDecoder val = null;
		Stream stream = null;
		Uri result = null;
		BitmapCreateOptions val2 = (BitmapCreateOptions)0;
		BitmapImage val3 = (BitmapImage)(object)((image is BitmapImage) ? image : null);
		if (val3 != null)
		{
			val2 = val3.CreateOptions;
			if (val3.StreamSource != null)
			{
				stream = val3.StreamSource;
			}
			else if (val3.UriSource != null)
			{
				result = val3.UriSource;
				if (val3.BaseUri != null && !result.IsAbsoluteUri)
				{
					result = new Uri(val3.BaseUri, result);
				}
			}
		}
		else
		{
			BitmapFrame val4 = (BitmapFrame)(object)((image is BitmapFrame) ? image : null);
			if (val4 != null)
			{
				val = val4.Decoder;
				Uri.TryCreate(val4.BaseUri, ((object)val4).ToString(), out result);
			}
		}
		if (val == null)
		{
			if (stream != null)
			{
				stream.Position = 0L;
				val = BitmapDecoder.Create(stream, val2, (BitmapCacheOption)1);
			}
			else if (result != null && result.IsAbsoluteUri)
			{
				val = BitmapDecoder.Create(result, val2, (BitmapCacheOption)1);
			}
		}
		if (val is GifBitmapDecoder && !CanReadNativeMetadata(val))
		{
			if (stream != null)
			{
				stream.Position = 0L;
				gifFile = GifFile.ReadGifFile(stream, metadataOnly: true);
			}
			else
			{
				if (!(result != null))
				{
					throw new InvalidOperationException("Can't get URI or Stream from the source. AnimatedSource should be either a BitmapImage, or a BitmapFrame constructed from a URI.");
				}
				gifFile = DecodeGifFile(result);
			}
		}
		if (val == null)
		{
			throw new InvalidOperationException("Can't get a decoder from the source. AnimatedSource should be either a BitmapImage or a BitmapFrame.");
		}
		return val;
	}

	private static bool CanReadNativeMetadata(BitmapDecoder decoder)
	{
		try
		{
			return decoder.Metadata != null;
		}
		catch
		{
			return false;
		}
	}

	private static GifFile DecodeGifFile(Uri uri)
	{
		Stream stream = null;
		if (uri.Scheme == PackUriHelper.UriSchemePack)
		{
			StreamResourceInfo val = ((!(uri.Authority == "siteoforigin:,,,")) ? Application.GetResourceStream(uri) : Application.GetRemoteStream(uri));
			if (val != null)
			{
				stream = val.Stream;
			}
		}
		else
		{
			stream = new WebClient().OpenRead(uri);
		}
		if (stream != null)
		{
			using (stream)
			{
				return GifFile.ReadGifFile(stream, metadataOnly: true);
			}
		}
		return null;
	}

	private static bool IsFullFrame(FrameMetadata metadata, Int32Size fullSize)
	{
		if (metadata.Left == 0 && metadata.Top == 0 && metadata.Width == fullSize.Width)
		{
			return metadata.Height == fullSize.Height;
		}
		return false;
	}

	private static BitmapSource MakeFrame(Int32Size fullSize, BitmapSource rawFrame, FrameMetadata metadata, BitmapSource baseFrame)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		if (baseFrame == null && IsFullFrame(metadata, fullSize))
		{
			return rawFrame;
		}
		DrawingVisual val = new DrawingVisual();
		DrawingContext val2 = val.RenderOpen();
		try
		{
			if (baseFrame != null)
			{
				Rect val3 = default(Rect);
				((Rect)(ref val3))._002Ector(0.0, 0.0, (double)fullSize.Width, (double)fullSize.Height);
				val2.DrawImage((ImageSource)(object)baseFrame, val3);
			}
			Rect val4 = default(Rect);
			((Rect)(ref val4))._002Ector((double)metadata.Left, (double)metadata.Top, (double)metadata.Width, (double)metadata.Height);
			val2.DrawImage((ImageSource)(object)rawFrame, val4);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		RenderTargetBitmap val5 = new RenderTargetBitmap(fullSize.Width, fullSize.Height, 96.0, 96.0, PixelFormats.Pbgra32);
		val5.Render((Visual)(object)val);
		WriteableBitmap val6 = new WriteableBitmap((BitmapSource)val5);
		if (((Freezable)val6).CanFreeze && !((Freezable)val6).IsFrozen)
		{
			((Freezable)val6).Freeze();
		}
		return (BitmapSource)(object)val6;
	}

	private static RepeatBehavior GetActualRepeatBehavior(Image imageControl, BitmapDecoder decoder, GifFile gifMetadata)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		RepeatBehavior repeatBehavior = GetRepeatBehavior(imageControl);
		if (repeatBehavior != default(RepeatBehavior))
		{
			return repeatBehavior;
		}
		int num = ((int?)gifMetadata?.RepeatCount) ?? GetRepeatCount(decoder);
		if (num == 0)
		{
			return RepeatBehavior.Forever;
		}
		return new RepeatBehavior((double)num);
	}

	private static int GetRepeatCount(BitmapDecoder decoder)
	{
		BitmapMetadata applicationExtension = GetApplicationExtension(decoder, "NETSCAPE2.0");
		if (applicationExtension != null)
		{
			byte[] queryOrNull = applicationExtension.GetQueryOrNull<byte[]>("/Data");
			if (queryOrNull != null && queryOrNull.Length >= 4)
			{
				return BitConverter.ToUInt16(queryOrNull, 2);
			}
		}
		return 1;
	}

	private static BitmapMetadata GetApplicationExtension(BitmapDecoder decoder, string application)
	{
		int num = 0;
		string query = "/appext";
		for (BitmapMetadata queryOrNull = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(query); queryOrNull != null; queryOrNull = decoder.Metadata.GetQueryOrNull<BitmapMetadata>(query))
		{
			byte[] queryOrNull2 = queryOrNull.GetQueryOrNull<byte[]>("/Application");
			if (queryOrNull2 != null && Encoding.ASCII.GetString(queryOrNull2) == application)
			{
				return queryOrNull;
			}
			query = $"/[{++num}]appext";
		}
		return null;
	}

	private static FrameMetadata GetFrameMetadata(BitmapDecoder decoder, GifFile gifMetadata, int frameIndex)
	{
		if (gifMetadata != null && gifMetadata.Frames.Count > frameIndex)
		{
			return GetFrameMetadata(gifMetadata.Frames[frameIndex]);
		}
		return GetFrameMetadata(decoder.Frames[frameIndex]);
	}

	private static FrameMetadata GetFrameMetadata(BitmapFrame frame)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		BitmapMetadata metadata = (BitmapMetadata)((ImageSource)frame).Metadata;
		TimeSpan delay = TimeSpan.FromMilliseconds(100.0);
		int queryOrDefault = metadata.GetQueryOrDefault("/grctlext/Delay", 10);
		if (queryOrDefault != 0)
		{
			delay = TimeSpan.FromMilliseconds((double)(queryOrDefault * 10));
		}
		FrameDisposalMethod queryOrDefault2 = (FrameDisposalMethod)metadata.GetQueryOrDefault("/grctlext/Disposal", 0);
		return new FrameMetadata
		{
			Left = metadata.GetQueryOrDefault("/imgdesc/Left", 0),
			Top = metadata.GetQueryOrDefault("/imgdesc/Top", 0),
			Width = metadata.GetQueryOrDefault("/imgdesc/Width", ((BitmapSource)frame).PixelWidth),
			Height = metadata.GetQueryOrDefault("/imgdesc/Height", ((BitmapSource)frame).PixelHeight),
			Delay = delay,
			DisposalMethod = queryOrDefault2
		};
	}

	private static FrameMetadata GetFrameMetadata(GifFrame gifMetadata)
	{
		GifImageDescriptor descriptor = gifMetadata.Descriptor;
		FrameMetadata frameMetadata = new FrameMetadata
		{
			Left = descriptor.Left,
			Top = descriptor.Top,
			Width = descriptor.Width,
			Height = descriptor.Height,
			Delay = TimeSpan.FromMilliseconds(100.0),
			DisposalMethod = FrameDisposalMethod.None
		};
		GifGraphicControlExtension gifGraphicControlExtension = gifMetadata.Extensions.OfType<GifGraphicControlExtension>().FirstOrDefault();
		if (gifGraphicControlExtension != null)
		{
			if (gifGraphicControlExtension.Delay != 0)
			{
				frameMetadata.Delay = TimeSpan.FromMilliseconds((double)gifGraphicControlExtension.Delay);
			}
			frameMetadata.DisposalMethod = (FrameDisposalMethod)gifGraphicControlExtension.DisposalMethod;
		}
		return frameMetadata;
	}

	private static Int32Size GetFullSize(BitmapDecoder decoder, GifFile gifMetadata)
	{
		if (gifMetadata != null)
		{
			GifLogicalScreenDescriptor logicalScreenDescriptor = gifMetadata.Header.LogicalScreenDescriptor;
			return new Int32Size(logicalScreenDescriptor.Width, logicalScreenDescriptor.Height);
		}
		int queryOrDefault = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Width", 0);
		int queryOrDefault2 = decoder.Metadata.GetQueryOrDefault("/logscrdesc/Height", 0);
		return new Int32Size(queryOrDefault, queryOrDefault2);
	}

	private static T GetQueryOrDefault<T>(this BitmapMetadata metadata, string query, T defaultValue)
	{
		if (metadata.ContainsQuery(query))
		{
			return (T)Convert.ChangeType(metadata.GetQuery(query), typeof(T));
		}
		return defaultValue;
	}

	private static T GetQueryOrNull<T>(this BitmapMetadata metadata, string query) where T : class
	{
		if (metadata.ContainsQuery(query))
		{
			return metadata.GetQuery(query) as T;
		}
		return null;
	}
}
