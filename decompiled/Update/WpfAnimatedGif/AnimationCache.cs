using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WpfAnimatedGif;

internal static class AnimationCache
{
	private class CacheKey
	{
		private readonly ImageSource _source;

		private readonly RepeatBehavior _repeatBehavior;

		public CacheKey(ImageSource source, RepeatBehavior repeatBehavior)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			_source = source;
			_repeatBehavior = repeatBehavior;
		}

		private bool Equals(CacheKey other)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (ImageEquals(_source, other._source))
			{
				return object.Equals(_repeatBehavior, other._repeatBehavior);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if ((object)obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((CacheKey)obj);
		}

		public override int GetHashCode()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			return (ImageGetHashCode(_source) * 397) ^ ((object)_repeatBehavior/*cast due to constrained. prefix*/).GetHashCode();
		}

		private static int ImageGetHashCode(ImageSource image)
		{
			if (image != null)
			{
				Uri uri = GetUri(image);
				if (uri != null)
				{
					return uri.GetHashCode();
				}
			}
			return 0;
		}

		private static bool ImageEquals(ImageSource x, ImageSource y)
		{
			if (object.Equals(x, y))
			{
				return true;
			}
			if (x == null != (y == null))
			{
				return false;
			}
			if ((object)((object)x).GetType() != ((object)y).GetType())
			{
				return false;
			}
			Uri uri = GetUri(x);
			Uri uri2 = GetUri(y);
			if (uri != null)
			{
				return uri == uri2;
			}
			return false;
		}

		private static Uri GetUri(ImageSource image)
		{
			BitmapImage val = (BitmapImage)(object)((image is BitmapImage) ? image : null);
			if (val != null && val.UriSource != null)
			{
				if (val.UriSource.IsAbsoluteUri)
				{
					return val.UriSource;
				}
				if (val.BaseUri != null)
				{
					return new Uri(val.BaseUri, val.UriSource);
				}
			}
			BitmapFrame val2 = (BitmapFrame)(object)((image is BitmapFrame) ? image : null);
			if (val2 != null)
			{
				string text = ((object)val2).ToString();
				if (text != ((object)val2).GetType().FullName && Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out Uri result))
				{
					if (result.IsAbsoluteUri)
					{
						return result;
					}
					if (val2.BaseUri != null)
					{
						return new Uri(val2.BaseUri, result);
					}
				}
			}
			return null;
		}
	}

	private static readonly Dictionary<CacheKey, ObjectAnimationUsingKeyFrames> _animationCache = new Dictionary<CacheKey, ObjectAnimationUsingKeyFrames>();

	private static readonly Dictionary<CacheKey, int> _referenceCount = new Dictionary<CacheKey, int>();

	public static void IncrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CacheKey key = new CacheKey(source, repeatBehavior);
		_referenceCount.TryGetValue(key, out var value);
		value++;
		_referenceCount[key] = value;
	}

	public static void DecrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CacheKey key = new CacheKey(source, repeatBehavior);
		_referenceCount.TryGetValue(key, out var value);
		if (value > 0)
		{
			value--;
			_referenceCount[key] = value;
		}
		if (value == 0)
		{
			_animationCache.Remove(key);
			_referenceCount.Remove(key);
		}
	}

	public static void AddAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames animation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CacheKey key = new CacheKey(source, repeatBehavior);
		_animationCache[key] = animation;
	}

	public static void RemoveAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames animation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CacheKey key = new CacheKey(source, repeatBehavior);
		_animationCache.Remove(key);
	}

	public static ObjectAnimationUsingKeyFrames GetAnimation(ImageSource source, RepeatBehavior repeatBehavior)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CacheKey key = new CacheKey(source, repeatBehavior);
		_animationCache.TryGetValue(key, out var value);
		return value;
	}
}
