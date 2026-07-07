using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xaml;
using ModernWpf.Media.Animation;

namespace ModernWpf.Controls;

[TemplatePart(Name = "FirstContentPresenter", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "SecondContentPresenter", Type = typeof(ContentPresenter))]
public class Frame : Frame
{
	public static readonly DependencyProperty SourcePageTypeProperty;

	private static readonly DependencyPropertyKey CurrentSourcePageTypePropertyKey;

	public static readonly DependencyProperty CurrentSourcePageTypeProperty;

	private static readonly DependencyPropertyKey BackStackDepthPropertyKey;

	public static readonly DependencyProperty BackStackDepthProperty;

	public static readonly DependencyProperty ContentTransitionsProperty;

	private static readonly AttachableMemberIdentifier FrameProperty;

	private static readonly DependencyProperty NavigationTransitionInfoProperty;

	private const string FirstContentPresenterName = "FirstContentPresenter";

	private const string SecondContentPresenterName = "SecondContentPresenter";

	private ContentPresenter _oldContentPresenter;

	private ContentPresenter _newContentPresenter;

	private bool _movingBackwards;

	private bool _ignoreSourcePageTypeChanged;

	private Page _oldPage;

	private NavigationTransitionInfo _transitionInfoOverride;

	private NavigationAnimation _exitAnimation;

	private NavigationAnimation _enterAnimation;

	private DispatcherOperation _asyncBeginTransition;

	public Type SourcePageType
	{
		get
		{
			return (Type)((DependencyObject)this).GetValue(SourcePageTypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SourcePageTypeProperty, (object)value);
		}
	}

	public Type CurrentSourcePageType
	{
		get
		{
			return (Type)((DependencyObject)this).GetValue(CurrentSourcePageTypeProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(CurrentSourcePageTypePropertyKey, (object)value);
		}
	}

	public int BackStackDepth
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(BackStackDepthProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(BackStackDepthPropertyKey, (object)value);
		}
	}

	public TransitionCollection ContentTransitions
	{
		get
		{
			return (TransitionCollection)((DependencyObject)this).GetValue(ContentTransitionsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ContentTransitionsProperty, (object)value);
		}
	}

	private NavigationTransitionInfo DefaultNavigationTransitionInfo { get; set; }

	private JournalEntry BackEntry => ((Frame)this).BackStack?.OfType<JournalEntry>().FirstOrDefault();

	static Frame()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		SourcePageTypeProperty = DependencyProperty.Register("SourcePageType", typeof(Type), typeof(Frame), new PropertyMetadata(new PropertyChangedCallback(OnSourcePageTypePropertyChanged)));
		CurrentSourcePageTypePropertyKey = DependencyProperty.RegisterReadOnly("CurrentSourcePageType", typeof(Type), typeof(Frame), (PropertyMetadata)null);
		CurrentSourcePageTypeProperty = CurrentSourcePageTypePropertyKey.DependencyProperty;
		BackStackDepthPropertyKey = DependencyProperty.RegisterReadOnly("BackStackDepth", typeof(int), typeof(Frame), (PropertyMetadata)null);
		BackStackDepthProperty = BackStackDepthPropertyKey.DependencyProperty;
		ContentTransitionsProperty = DependencyProperty.Register("ContentTransitions", typeof(TransitionCollection), typeof(Frame), new PropertyMetadata(new PropertyChangedCallback(OnContentTransitionsPropertyChanged)));
		FrameProperty = new AttachableMemberIdentifier(typeof(Frame), "Frame");
		NavigationTransitionInfoProperty = DependencyProperty.RegisterAttached("NavigationTransitionInfo", typeof(NavigationTransitionInfo), typeof(Frame));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Frame), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(Frame)));
		Frame.NavigationUIVisibilityProperty.OverrideMetadata(typeof(Frame), (PropertyMetadata)new FrameworkPropertyMetadata((object)(NavigationUIVisibility)2));
		Control.IsTabStopProperty.OverrideMetadata(typeof(Frame), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
		UIElement.FocusableProperty.OverrideMetadata(typeof(Frame), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
		FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(Frame), (PropertyMetadata)new FrameworkPropertyMetadata((PropertyChangedCallback)null));
	}

	public Frame()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		((FrameworkElement)this).InheritanceBehavior = (InheritanceBehavior)0;
		((Frame)this).JournalOwnership = (JournalOwnership)1;
		((DependencyObject)this).SetCurrentValue(ContentTransitionsProperty, (object)new TransitionCollection());
		SetFrame(((Frame)this).NavigationService, this);
		((Frame)this).Navigating += new NavigatingCancelEventHandler(OnNavigating);
		((Frame)this).Navigated += new NavigatedEventHandler(OnNavigated);
		((Frame)this).NavigationStopped += new NavigationStoppedEventHandler(OnNavigationStopped);
		((Frame)this).NavigationFailed += new NavigationFailedEventHandler(OnNavigationFailed);
	}

	private static void OnSourcePageTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Frame)(object)sender).OnSourcePageTypePropertyChanged(args);
	}

	private void OnSourcePageTypePropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		if (!_ignoreSourcePageTypeChanged)
		{
			Navigate((Type)((DependencyPropertyChangedEventArgs)(ref args)).NewValue);
		}
	}

	private void UpdateBackStackDepth()
	{
		BackStackDepth = ((Frame)this).BackStack?.Cast<object>().Count() ?? 0;
	}

	private static void OnContentTransitionsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Frame)(object)sender).OnContentTransitionsPropertyChanged(args);
	}

	private void OnContentTransitionsPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		DefaultNavigationTransitionInfo = ((TransitionCollection)((DependencyPropertyChangedEventArgs)(ref args)).NewValue)?.OfType<NavigationThemeTransition>().LastOrDefault()?.DefaultNavigationTransitionInfo ?? new EntranceNavigationTransitionInfo();
	}

	internal static Frame GetFrame(NavigationService navigationService)
	{
		Frame result = default(Frame);
		AttachablePropertyServices.TryGetProperty<Frame>((object)navigationService, FrameProperty, ref result);
		return result;
	}

	private static void SetFrame(NavigationService navigationService, Frame value)
	{
		AttachablePropertyServices.SetProperty((object)navigationService, FrameProperty, (object)value);
	}

	private static NavigationTransitionInfo GetNavigationTransitionInfo(JournalEntry entry)
	{
		return (NavigationTransitionInfo)((DependencyObject)entry).GetValue(NavigationTransitionInfoProperty);
	}

	private static void SetNavigationTransitionInfo(JournalEntry entry, NavigationTransitionInfo value)
	{
		((DependencyObject)entry).SetValue(NavigationTransitionInfoProperty, (object)value);
	}

	public bool Navigate(Type sourcePageType)
	{
		return ((Frame)this).Navigate(Activator.CreateInstance(sourcePageType));
	}

	public bool Navigate(Type sourcePageType, object parameter)
	{
		return ((Frame)this).Navigate(Activator.CreateInstance(sourcePageType), parameter);
	}

	public bool Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
	{
		_transitionInfoOverride = infoOverride;
		return ((Frame)this).Navigate(Activator.CreateInstance(sourcePageType), parameter);
	}

	public bool Navigate(Uri source, object extraData, NavigationTransitionInfo infoOverride)
	{
		_transitionInfoOverride = infoOverride;
		return ((Frame)this).Navigate(source, extraData);
	}

	public void GoBack(NavigationTransitionInfo transitionInfoOverride)
	{
		_transitionInfoOverride = transitionInfoOverride;
		((Frame)this).GoBack();
	}

	public override void OnApplyTemplate()
	{
		((Frame)this).OnApplyTemplate();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("FirstContentPresenter");
		_oldContentPresenter = (ContentPresenter)(object)((templateChild is ContentPresenter) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("SecondContentPresenter");
		_newContentPresenter = (ContentPresenter)(object)((templateChild2 is ContentPresenter) ? templateChild2 : null);
		if (((ContentControl)this).Content != null)
		{
			((ContentControl)this).OnContentChanged((object)null, ((ContentControl)this).Content);
		}
	}

	protected override void OnContentChanged(object oldContent, object newContent)
	{
		((ContentControl)this).OnContentChanged(oldContent, newContent);
		StopTransition();
		if (oldContent is Page oldPage)
		{
			_oldPage = oldPage;
		}
		if (_oldContentPresenter == null || _newContentPresenter == null)
		{
			return;
		}
		bool flag = false;
		if (Helper.IsAnimationsEnabled)
		{
			FrameworkElement val = (FrameworkElement)((oldContent is FrameworkElement) ? oldContent : null);
			if (val != null)
			{
				FrameworkElement val2 = (FrameworkElement)((newContent is FrameworkElement) ? newContent : null);
				if (val2 != null)
				{
					NavigationTransitionInfo navigationTransitionInfo = _transitionInfoOverride ?? DefaultNavigationTransitionInfo;
					_exitAnimation = navigationTransitionInfo.GetExitAnimation(val, _movingBackwards);
					_enterAnimation = navigationTransitionInfo.GetEnterAnimation(val2, _movingBackwards);
					if (_exitAnimation != null || _enterAnimation != null)
					{
						ContentPresenter oldContentPresenter = _oldContentPresenter;
						ContentPresenter newContentPresenter = _newContentPresenter;
						_newContentPresenter = oldContentPresenter;
						_oldContentPresenter = newContentPresenter;
						((UIElement)_newContentPresenter).Opacity = 0.0;
						((UIElement)_newContentPresenter).Visibility = (Visibility)0;
						((UIElement)_newContentPresenter).IsHitTestVisible = false;
						_newContentPresenter.Content = val2;
						((UIElement)_oldContentPresenter).Opacity = 1.0;
						((UIElement)_oldContentPresenter).Visibility = (Visibility)0;
						((UIElement)_oldContentPresenter).IsHitTestVisible = false;
						_oldContentPresenter.Content = val;
						BeginTransition();
						flag = true;
					}
				}
			}
		}
		if (!flag)
		{
			((UIElement)_oldContentPresenter).Visibility = (Visibility)2;
			_oldContentPresenter.Content = null;
			((UIElement)_newContentPresenter).Visibility = (Visibility)0;
			_newContentPresenter.Content = ((ContentControl)this).Content;
		}
	}

	protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((FrameworkElement)this).OnPropertyChanged(e);
		if (((DependencyPropertyChangedEventArgs)(ref e)).Property == Frame.BackStackProperty)
		{
			OnBackStackPropertyChanged(e);
		}
	}

	private void OnBackStackPropertyChanged(DependencyPropertyChangedEventArgs e)
	{
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue is INotifyCollectionChanged notifyCollectionChanged2)
		{
			notifyCollectionChanged2.CollectionChanged += OnCollectionChanged;
		}
		UpdateBackStackDepth();
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e2)
		{
			UpdateBackStackDepth();
		}
	}

	private void OnNavigating(object sender, NavigatingCancelEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		if (((ContentControl)this).Content is Page page)
		{
			page.InternalOnNavigatingFrom(e);
			if (((CancelEventArgs)(object)e).Cancel)
			{
				return;
			}
		}
		_movingBackwards = (int)e.NavigationMode == 1;
		if (_transitionInfoOverride == null && _movingBackwards)
		{
			JournalEntry backEntry = BackEntry;
			if (backEntry != null)
			{
				_transitionInfoOverride = GetNavigationTransitionInfo(backEntry);
			}
		}
	}

	private void OnNavigated(object sender, NavigationEventArgs e)
	{
		if (_transitionInfoOverride != null)
		{
			if (!_movingBackwards)
			{
				JournalEntry backEntry = BackEntry;
				if (backEntry != null)
				{
					SetNavigationTransitionInfo(backEntry, _transitionInfoOverride);
				}
			}
			_transitionInfoOverride = null;
		}
		try
		{
			_ignoreSourcePageTypeChanged = true;
			Type currentSourcePageType = (SourcePageType = e.Content?.GetType());
			CurrentSourcePageType = currentSourcePageType;
		}
		finally
		{
			_ignoreSourcePageTypeChanged = false;
		}
		Page oldPage = _oldPage;
		if (oldPage != null)
		{
			_oldPage = null;
			oldPage.InternalOnNavigatedFrom(e);
		}
		if (e.Content is Page page)
		{
			page.InternalOnNavigatedTo(e);
		}
	}

	private void OnNavigationStopped(object sender, NavigationEventArgs e)
	{
		_transitionInfoOverride = null;
		_oldPage = null;
	}

	private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
	{
		_transitionInfoOverride = null;
		_oldPage = null;
	}

	private void BeginTransition()
	{
		if (_exitAnimation != null)
		{
			_exitAnimation.Completed += OnExitAnimationCompleted;
		}
		if (_enterAnimation != null)
		{
			_enterAnimation.Completed += OnEnterAnimationCompleted;
		}
		_asyncBeginTransition = ((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
		{
			_asyncBeginTransition = null;
			if (_exitAnimation != null)
			{
				_exitAnimation.Begin();
			}
			else if (_enterAnimation != null)
			{
				BeginEnterAnimation();
			}
		}, (DispatcherPriority)2);
	}

	private void BeginEnterAnimation()
	{
		if (_oldContentPresenter != null)
		{
			((UIElement)_oldContentPresenter).Visibility = (Visibility)2;
			_oldContentPresenter.Content = null;
		}
		if (_newContentPresenter != null)
		{
			((UIElement)_newContentPresenter).Opacity = 1.0;
		}
		_enterAnimation.Begin();
	}

	private void OnExitAnimationCompleted(object sender, EventArgs e)
	{
		if (_exitAnimation != null)
		{
			_exitAnimation.Stop();
			_exitAnimation = null;
		}
		if (_enterAnimation != null)
		{
			BeginEnterAnimation();
		}
		else
		{
			StopTransition();
		}
	}

	private void OnEnterAnimationCompleted(object sender, EventArgs e)
	{
		if (_enterAnimation != null)
		{
			_enterAnimation.Stop();
			_enterAnimation = null;
		}
		StopTransition();
	}

	private void StopTransition()
	{
		if (_asyncBeginTransition != null)
		{
			_asyncBeginTransition.Abort();
			_asyncBeginTransition = null;
		}
		if (_exitAnimation != null)
		{
			_exitAnimation.Stop();
			_exitAnimation = null;
		}
		if (_enterAnimation != null)
		{
			_enterAnimation.Stop();
			_enterAnimation = null;
		}
		if (_oldContentPresenter != null)
		{
			_oldContentPresenter.Content = null;
			((DependencyObject)_oldContentPresenter).ClearValue(UIElement.OpacityProperty);
			((DependencyObject)_oldContentPresenter).ClearValue(UIElement.IsHitTestVisibleProperty);
		}
		if (_newContentPresenter != null)
		{
			((DependencyObject)_newContentPresenter).ClearValue(UIElement.OpacityProperty);
			((DependencyObject)_newContentPresenter).ClearValue(UIElement.IsHitTestVisibleProperty);
		}
	}
}
