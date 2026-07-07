using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WpfAnimatedGif;

internal class ImageAnimationController : IDisposable
{
	private static readonly DependencyPropertyDescriptor _sourceDescriptor;

	private readonly Image _image;

	private readonly ObjectAnimationUsingKeyFrames _animation;

	private readonly AnimationClock _clock;

	private readonly ClockController _clockController;

	public int FrameCount => _animation.KeyFrames.Count;

	public TimeSpan Duration
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			Duration duration = ((Timeline)_animation).Duration;
			if (!((Duration)(ref duration)).HasTimeSpan)
			{
				return TimeSpan.Zero;
			}
			duration = ((Timeline)_animation).Duration;
			return ((Duration)(ref duration)).TimeSpan;
		}
	}

	public bool IsPaused => ((Clock)_clock).IsPaused;

	public bool IsComplete => (int)((Clock)_clock).CurrentState == 1;

	public int CurrentFrame
	{
		get
		{
			TimeSpan? time = ((Clock)_clock).CurrentTime;
			return ((IEnumerable)_animation.KeyFrames).Cast<ObjectKeyFrame>().Select(delegate(ObjectKeyFrame f, int i)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				KeyTime keyTime = f.KeyTime;
				return new
				{
					Time = ((KeyTime)(ref keyTime)).TimeSpan,
					Index = i
				};
			}).FirstOrDefault(fi =>
			{
				TimeSpan time2 = fi.Time;
				TimeSpan? timeSpan = time;
				return time2 >= timeSpan;
			})?.Index ?? (-1);
		}
	}

	public event EventHandler CurrentFrameChanged;

	static ImageAnimationController()
	{
		_sourceDescriptor = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
	}

	internal ImageAnimationController(Image image, ObjectAnimationUsingKeyFrames animation, bool autoStart)
	{
		_image = image;
		_animation = animation;
		((Timeline)_animation).Completed += AnimationCompleted;
		_clock = ((AnimationTimeline)_animation).CreateClock();
		_clockController = ((Clock)_clock).Controller;
		((PropertyDescriptor)(object)_sourceDescriptor).AddValueChanged((object)image, (EventHandler)ImageSourceChanged);
		_clockController.Pause();
		((UIElement)_image).ApplyAnimationClock(Image.SourceProperty, _clock);
		if (autoStart)
		{
			_clockController.Resume();
		}
	}

	private void AnimationCompleted(object sender, EventArgs e)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		((UIElement)_image).RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationCompletedEvent, (object)_image));
	}

	private void ImageSourceChanged(object sender, EventArgs e)
	{
		OnCurrentFrameChanged();
	}

	public void GotoFrame(int index)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ObjectKeyFrame val = _animation.KeyFrames[index];
		ClockController clockController = _clockController;
		KeyTime keyTime = val.KeyTime;
		clockController.Seek(((KeyTime)(ref keyTime)).TimeSpan, (TimeSeekOrigin)0);
	}

	public void Pause()
	{
		_clockController.Pause();
	}

	public void Play()
	{
		_clockController.Resume();
	}

	private void OnCurrentFrameChanged()
	{
		this.CurrentFrameChanged?.Invoke(this, EventArgs.Empty);
	}

	~ImageAnimationController()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			((UIElement)_image).BeginAnimation(Image.SourceProperty, (AnimationTimeline)null);
			((Timeline)_animation).Completed -= AnimationCompleted;
			((PropertyDescriptor)(object)_sourceDescriptor).RemoveValueChanged((object)_image, (EventHandler)ImageSourceChanged);
			_image.Source = null;
		}
	}
}
