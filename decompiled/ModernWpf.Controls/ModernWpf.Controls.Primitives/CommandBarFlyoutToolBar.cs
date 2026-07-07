using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ModernWpf.Controls.Primitives;

public class CommandBarFlyoutToolBar : CommandBarToolBar
{
	private static readonly DependencyPropertyKey FlyoutTemplateSettingsPropertyKey;

	public static readonly DependencyProperty FlyoutTemplateSettingsProperty;

	private FrameworkElement m_layoutRoot;

	private FrameworkElement m_primaryItemsRoot;

	private FrameworkElement m_secondaryItemsRoot;

	private ButtonBase m_moreButton;

	private RoutedEventHandlerRevoker m_firstItemLoadedRevoker;

	private FrameworkElement m_currentPrimaryItemsEndElement;

	private FrameworkElement m_currentSecondaryItemsStartElement;

	private Storyboard m_openingStoryboard;

	private Storyboard m_closingStoryboard;

	private ClockState? m_openingStoryboardState;

	private ClockState? m_closingStoryboardState;

	private bool m_secondaryItemsRootSized;

	private bool m_openAnimationPending;

	private DispatcherOperation m_asyncOpenAnimation;

	public CommandBarFlyoutCommandBarTemplateSettings FlyoutTemplateSettings => (CommandBarFlyoutCommandBarTemplateSettings)((DependencyObject)this).GetValue(FlyoutTemplateSettingsProperty);

	private ObservableCollection<ICommandBarElement> PrimaryCommands => (((FrameworkElement)this).TemplatedParent as CommandBar)?.PrimaryCommands;

	private ObservableCollection<ICommandBarElement> SecondaryCommands => (((FrameworkElement)this).TemplatedParent as CommandBar)?.SecondaryCommands;

	private bool IsOpen
	{
		get
		{
			return ((ToolBar)this).IsOverflowOpen;
		}
		set
		{
			((ToolBar)this).IsOverflowOpen = value;
		}
	}

	private WeakReference<CommandBarFlyout> OwningFlyout => (((FrameworkElement)this).TemplatedParent as CommandBarFlyoutCommandBar)?.OwningFlyout;

	static CommandBarFlyoutToolBar()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		FlyoutTemplateSettingsPropertyKey = DependencyProperty.RegisterReadOnly("FlyoutTemplateSettings", typeof(CommandBarFlyoutCommandBarTemplateSettings), typeof(CommandBarFlyoutToolBar), (PropertyMetadata)null);
		FlyoutTemplateSettingsProperty = FlyoutTemplateSettingsPropertyKey.DependencyProperty;
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandBarFlyoutToolBar), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(CommandBarFlyoutToolBar)));
		ToolBar.IsOverflowOpenProperty.OverrideMetadata(typeof(CommandBarFlyoutToolBar), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnOverflowOpenChanged)));
	}

	public CommandBarFlyoutToolBar()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		((DependencyObject)this).SetValue(FlyoutTemplateSettingsPropertyKey, (object)new CommandBarFlyoutCommandBarTemplateSettings());
		((FrameworkElement)this).Loaded += (RoutedEventHandler)delegate
		{
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Expected O, but got Unknown
			UpdateUI();
			ObservableCollection<ICommandBarElement> commands = ((PrimaryCommands.Count > 0) ? PrimaryCommands : ((SecondaryCommands.Count > 0) ? SecondaryCommands : null));
			if (commands != null)
			{
				bool usingPrimaryCommands = commands == PrimaryCommands;
				bool ensureTabStopUniqueness = usingPrimaryCommands || true;
				ICommandBarElement commandBarElement = commands[0];
				FrameworkElement val = (FrameworkElement)((commandBarElement is FrameworkElement) ? commandBarElement : null);
				if (val != null)
				{
					if (SharedHelpers.IsFrameworkElementLoaded(val))
					{
						FocusCommand(commands, (Control)(object)(usingPrimaryCommands ? m_moreButton : null), firstCommand: true, ensureTabStopUniqueness);
					}
					else
					{
						m_firstItemLoadedRevoker = new RoutedEventHandlerRevoker((UIElement)(object)val, FrameworkElement.LoadedEvent, (Delegate)(RoutedEventHandler)delegate
						{
							FocusCommand(commands, (Control)(object)(usingPrimaryCommands ? m_moreButton : null), firstCommand: true, ensureTabStopUniqueness);
							m_firstItemLoadedRevoker?.Revoke();
						});
					}
				}
			}
		};
		((FrameworkElement)this).Unloaded += (RoutedEventHandler)delegate
		{
			StopOpenAnimation();
			SetOpacity(1.0);
		};
		((FrameworkElement)this).SizeChanged += (SizeChangedEventHandler)delegate
		{
			UpdateUI();
		};
		base.OverflowOpened += delegate
		{
			m_secondaryItemsRootSized = true;
			UpdateFlowsFromAndFlowsTo();
			UpdateUI();
		};
		base.OverflowClosed += delegate
		{
			m_secondaryItemsRootSized = false;
			if (PrimaryCommands.Count > 0)
			{
				EnsureFocusedPrimaryCommand();
			}
		};
		((UIElement)this).AddHandler(UIElement.MouseDownEvent, (Delegate)new MouseButtonEventHandler(OnMouseDown), true);
	}

	protected override void OnInitialized(EventArgs e)
	{
		((FrameworkElement)this).OnInitialized(e);
		PrimaryCommands.CollectionChanged += delegate
		{
			UpdateFlowsFromAndFlowsTo();
			UpdateUI();
		};
		SecondaryCommands.CollectionChanged += delegate
		{
			m_secondaryItemsRootSized = false;
			UpdateFlowsFromAndFlowsTo();
			UpdateUI();
		};
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		DetachEventHandlers();
		DependencyObject templateChild = ((FrameworkElement)this).GetTemplateChild("LayoutRoot");
		m_layoutRoot = (FrameworkElement)(object)((templateChild is FrameworkElement) ? templateChild : null);
		DependencyObject templateChild2 = ((FrameworkElement)this).GetTemplateChild("PrimaryItemsRoot");
		m_primaryItemsRoot = (FrameworkElement)(object)((templateChild2 is FrameworkElement) ? templateChild2 : null);
		DependencyObject templateChild3 = ((FrameworkElement)this).GetTemplateChild("OverflowContentRoot");
		m_secondaryItemsRoot = (FrameworkElement)(object)((templateChild3 is FrameworkElement) ? templateChild3 : null);
		DependencyObject templateChild4 = ((FrameworkElement)this).GetTemplateChild("MoreButton");
		m_moreButton = (ButtonBase)(object)((templateChild4 is ButtonBase) ? templateChild4 : null);
		if (m_layoutRoot != null)
		{
			object obj = m_layoutRoot.Resources[(object)"OpeningStoryboard"];
			m_openingStoryboard = (Storyboard)((obj is Storyboard) ? obj : null);
			object obj2 = m_layoutRoot.Resources[(object)"ClosingStoryboard"];
			m_closingStoryboard = (Storyboard)((obj2 is Storyboard) ? obj2 : null);
		}
		if (m_moreButton != null && ((Control)m_moreButton).IsTabStop)
		{
			((Control)m_moreButton).IsTabStop = false;
		}
		if (base.OverflowPopup is PopupEx popupEx)
		{
			popupEx.SuppressFadeAnimation = true;
		}
		AttachEventHandlers();
		UpdateFlowsFromAndFlowsTo();
		UpdateUI(useTransitions: false);
	}

	protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!(bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue && TryGetOwningFlyout(out var flyout) && flyout.IsOpen)
		{
			((UIElement)this).MoveFocus(new TraversalRequest((FocusNavigationDirection)0));
		}
		((UIElement)this).OnIsKeyboardFocusWithinChanged(e);
	}

	private static void OnOverflowOpenChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((CommandBarFlyoutToolBar)(object)element).OnOverflowOpenChanged(e);
	}

	private void OnOverflowOpenChanged(DependencyPropertyChangedEventArgs e)
	{
		UpdateFlowsFromAndFlowsTo();
		UpdateUI();
	}

	private void AttachEventHandlers()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		if (m_secondaryItemsRoot != null)
		{
			m_secondaryItemsRoot.SizeChanged += new SizeChangedEventHandler(SecondaryItemsRootSizeChanged);
			((UIElement)m_secondaryItemsRoot).PreviewKeyDown += new KeyEventHandler(SecondaryItemsRootPreviewKeyDown);
		}
		if (m_openingStoryboard != null)
		{
			((Timeline)m_openingStoryboard).Completed += OpeningStoryboardCompleted;
			((Timeline)m_openingStoryboard).CurrentStateInvalidated += OpeningStoryboardCurrentStateInvalidated;
		}
		if (m_closingStoryboard != null)
		{
			((Timeline)m_closingStoryboard).Completed += ClosingStoryboardCompleted;
			((Timeline)m_closingStoryboard).CurrentStateInvalidated += ClosingStoryboardCurrentStateInvalidated;
		}
	}

	private void DetachEventHandlers()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		if (m_secondaryItemsRoot != null)
		{
			((UIElement)m_secondaryItemsRoot).PreviewKeyDown -= new KeyEventHandler(SecondaryItemsRootPreviewKeyDown);
			m_secondaryItemsRoot.SizeChanged -= new SizeChangedEventHandler(SecondaryItemsRootSizeChanged);
		}
		m_firstItemLoadedRevoker?.Revoke();
		if (m_openingStoryboard != null)
		{
			((Timeline)m_openingStoryboard).Completed -= OpeningStoryboardCompleted;
			((Timeline)m_openingStoryboard).CurrentStateInvalidated -= OpeningStoryboardCurrentStateInvalidated;
			m_openingStoryboardState = null;
		}
		if (m_closingStoryboard != null)
		{
			((Timeline)m_closingStoryboard).Completed -= ClosingStoryboardCompleted;
			((Timeline)m_closingStoryboard).CurrentStateInvalidated -= ClosingStoryboardCurrentStateInvalidated;
			m_closingStoryboardState = null;
		}
	}

	internal bool HasOpenAnimation()
	{
		if (m_openingStoryboard != null)
		{
			return SharedHelpers.IsAnimationsEnabled;
		}
		return false;
	}

	internal void PlayOpenAnimation()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		StopOpenAnimation();
		if (m_openingStoryboard != null && m_openingStoryboardState != (ClockState?)0)
		{
			if (((FrameworkElement)this).TemplatedParent is CommandBar { IsOpen: not false })
			{
				m_openAnimationPending = true;
				SetOpacity(0.0);
				return;
			}
			m_openAnimationPending = false;
			SetOpacity(0.0);
			DispatcherHelper.DoEvents((DispatcherPriority)8);
			SetOpacity(1.0);
			m_openingStoryboard.Begin(m_layoutRoot, true);
		}
	}

	internal bool HasCloseAnimation()
	{
		if (m_closingStoryboard != null)
		{
			return SharedHelpers.IsAnimationsEnabled;
		}
		return false;
	}

	internal void PlayCloseAnimation(Action onCompleteFunc)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		StopOpenAnimation();
		if (m_closingStoryboard != null)
		{
			if (m_closingStoryboardState != (ClockState?)0)
			{
				((Timeline)m_closingStoryboard).Completed += closingStoryboardCompletedCallback;
				UpdateTemplateSettings();
				m_closingStoryboard.Begin(m_layoutRoot, true);
			}
		}
		else
		{
			onCompleteFunc();
		}
		void closingStoryboardCompletedCallback(object sender, EventArgs e)
		{
			((Timeline)m_closingStoryboard).Completed -= closingStoryboardCompletedCallback;
			onCompleteFunc();
		}
	}

	private void UpdateFlowsFromAndFlowsTo()
	{
		ButtonBase moreButton = m_moreButton;
		EnsureTabStopUniqueness(PrimaryCommands, (Control)(object)moreButton);
		EnsureTabStopUniqueness(SecondaryCommands, null);
		if (m_currentPrimaryItemsEndElement != null)
		{
			m_currentPrimaryItemsEndElement = null;
		}
		if (m_currentSecondaryItemsStartElement != null)
		{
			m_currentSecondaryItemsStartElement = null;
		}
		if (!IsOpen)
		{
			return;
		}
		ObservableCollection<ICommandBarElement> primaryCommands = PrimaryCommands;
		for (int num = primaryCommands.Count - 1; num >= 0; num--)
		{
			ICommandBarElement commandBarElement = primaryCommands[num];
			if (isElementFocusable(commandBarElement, checkTabStop: false))
			{
				m_currentPrimaryItemsEndElement = (FrameworkElement)((commandBarElement is FrameworkElement) ? commandBarElement : null);
				break;
			}
		}
		if (moreButton != null && m_currentPrimaryItemsEndElement != null)
		{
			m_currentPrimaryItemsEndElement = (FrameworkElement)(object)moreButton;
		}
		foreach (ICommandBarElement secondaryCommand in SecondaryCommands)
		{
			if (isElementFocusable(secondaryCommand, checkTabStop: false))
			{
				m_currentSecondaryItemsStartElement = (FrameworkElement)((secondaryCommand is FrameworkElement) ? secondaryCommand : null);
				break;
			}
		}
		bool isElementFocusable(ICommandBarElement element, bool checkTabStop)
		{
			Control control = (Control)((element is Control) ? element : null);
			return IsControlFocusable(control, checkTabStop);
		}
	}

	private void UpdateUI(bool useTransitions = true)
	{
		UpdateTemplateSettings();
		UpdateVisualState(useTransitions);
		UpdateShadow();
	}

	private void UpdateVisualState(bool useTransitions)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		bool shouldExpandUp;
		if (IsOpen)
		{
			if (!m_secondaryItemsRootSized)
			{
				return;
			}
			shouldExpandUp = false;
			if (m_secondaryItemsRoot != null && ((UIElement)this).IsVisible && ((UIElement)m_secondaryItemsRoot).IsVisible)
			{
				((UIElement)this).UpdateLayout();
				Point val = ((UIElement)m_secondaryItemsRoot).TranslatePoint(default(Point), (UIElement)(object)this);
				shouldExpandUp = ((Point)(ref val)).Y < 0.0;
			}
			if (m_openAnimationPending)
			{
				m_openAnimationPending = false;
				CancelAsyncOpenAnimation();
				m_asyncOpenAnimation = ((DispatcherObject)this).Dispatcher.BeginInvoke(delegate
				{
					m_asyncOpenAnimation = null;
					SetOpacity(1.0);
					m_openingStoryboard.Begin(m_layoutRoot, true);
					updateExpansionStates();
				}, (DispatcherPriority)7);
			}
			else if (m_asyncOpenAnimation == null)
			{
				updateExpansionStates();
			}
			if (PrimaryCommands.Count != 0)
			{
				if (shouldExpandUp)
				{
					VisualStateManager.GoToState((FrameworkElement)(object)this, "ExpandedUpWithPrimaryCommands", useTransitions);
				}
				else
				{
					VisualStateManager.GoToState((FrameworkElement)(object)this, "ExpandedDownWithPrimaryCommands", useTransitions);
				}
			}
			else if (shouldExpandUp)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "ExpandedUpWithoutPrimaryCommands", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "ExpandedDownWithoutPrimaryCommands", useTransitions);
			}
		}
		else
		{
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Default", useTransitions);
			VisualStateManager.GoToState((FrameworkElement)(object)this, "Collapsed", useTransitions);
		}
		void updateExpansionStates()
		{
			if (shouldExpandUp)
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "ExpandedUp", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState((FrameworkElement)(object)this, "ExpandedDown", useTransitions);
			}
		}
	}

	private void UpdateTemplateSettings()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		if (m_primaryItemsRoot == null || m_secondaryItemsRoot == null)
		{
			return;
		}
		CommandBarFlyoutCommandBarTemplateSettings flyoutTemplateSettings = FlyoutTemplateSettings;
		if (flyoutTemplateSettings == null)
		{
			return;
		}
		double maxWidth = ((FrameworkElement)this).MaxWidth;
		Size val = default(Size);
		((Size)(ref val))._002Ector(double.PositiveInfinity, double.PositiveInfinity);
		((UIElement)m_primaryItemsRoot).Measure(val);
		Size desiredSize = ((UIElement)m_primaryItemsRoot).DesiredSize;
		double num = Math.Min(maxWidth, ((Size)(ref desiredSize)).Width);
		if (m_secondaryItemsRoot != null)
		{
			((UIElement)m_secondaryItemsRoot).Measure(val);
			Size desiredSize2 = ((UIElement)m_secondaryItemsRoot).DesiredSize;
			flyoutTemplateSettings.ExpandedWidth = Math.Min(maxWidth, Math.Max(num, ((Size)(ref desiredSize2)).Width));
			flyoutTemplateSettings.ExpandUpOverflowVerticalPosition = 0.0 - ((Size)(ref desiredSize2)).Height;
			flyoutTemplateSettings.ExpandUpAnimationStartPosition = ((Size)(ref desiredSize2)).Height / 2.0;
			flyoutTemplateSettings.ExpandUpAnimationEndPosition = 0.0;
			flyoutTemplateSettings.ExpandUpAnimationHoldPosition = ((Size)(ref desiredSize2)).Height;
			flyoutTemplateSettings.ExpandDownAnimationStartPosition = (0.0 - ((Size)(ref desiredSize2)).Height) / 2.0;
			flyoutTemplateSettings.ExpandDownAnimationEndPosition = 0.0;
			flyoutTemplateSettings.ExpandDownAnimationHoldPosition = 0.0 - ((Size)(ref desiredSize2)).Height;
			flyoutTemplateSettings.OverflowContentClipRect = new Rect(0.0, 0.0, flyoutTemplateSettings.ExpandedWidth, ((Size)(ref desiredSize2)).Height + 2.0);
		}
		else
		{
			flyoutTemplateSettings.ExpandedWidth = num;
			flyoutTemplateSettings.ExpandUpOverflowVerticalPosition = 0.0;
			flyoutTemplateSettings.ExpandUpAnimationStartPosition = 0.0;
			flyoutTemplateSettings.ExpandUpAnimationEndPosition = 0.0;
			flyoutTemplateSettings.ExpandUpAnimationHoldPosition = 0.0;
			flyoutTemplateSettings.ExpandDownAnimationStartPosition = 0.0;
			flyoutTemplateSettings.ExpandDownAnimationEndPosition = 0.0;
			flyoutTemplateSettings.ExpandDownAnimationHoldPosition = 0.0;
			flyoutTemplateSettings.OverflowContentClipRect = new Rect(0.0, 0.0, 0.0, 0.0);
		}
		double expandedWidth = flyoutTemplateSettings.ExpandedWidth;
		if (num == 0.0)
		{
			num = expandedWidth;
		}
		flyoutTemplateSettings.WidthExpansionDelta = num - expandedWidth;
		flyoutTemplateSettings.WidthExpansionAnimationStartPosition = (0.0 - flyoutTemplateSettings.WidthExpansionDelta) / 2.0;
		flyoutTemplateSettings.WidthExpansionAnimationEndPosition = 0.0 - flyoutTemplateSettings.WidthExpansionDelta;
		flyoutTemplateSettings.ContentClipRect = new Rect(0.0, 0.0, expandedWidth, ((Size)(ref desiredSize)).Height);
		if (IsOpen)
		{
			flyoutTemplateSettings.CurrentWidth = expandedWidth;
		}
		else
		{
			flyoutTemplateSettings.CurrentWidth = num;
		}
		bool flag = false;
		if (m_closingStoryboard != null)
		{
			flag = m_closingStoryboardState == (ClockState?)0;
		}
		if (!flag)
		{
			if (IsOpen)
			{
				flyoutTemplateSettings.OpenAnimationStartPosition = (0.0 - expandedWidth) / 2.0;
				flyoutTemplateSettings.OpenAnimationEndPosition = 0.0;
			}
			else
			{
				flyoutTemplateSettings.OpenAnimationStartPosition = flyoutTemplateSettings.WidthExpansionDelta - num / 2.0;
				flyoutTemplateSettings.OpenAnimationEndPosition = flyoutTemplateSettings.WidthExpansionDelta;
			}
			flyoutTemplateSettings.CloseAnimationEndPosition = 0.0 - expandedWidth;
		}
		flyoutTemplateSettings.WidthExpansionMoreButtonAnimationStartPosition = flyoutTemplateSettings.WidthExpansionDelta / 2.0;
		flyoutTemplateSettings.WidthExpansionMoreButtonAnimationEndPosition = flyoutTemplateSettings.WidthExpansionDelta;
		if (PrimaryCommands.Count > 0)
		{
			flyoutTemplateSettings.ExpandDownOverflowVerticalPosition = ((FrameworkElement)this).Height;
		}
		else
		{
			flyoutTemplateSettings.ExpandDownOverflowVerticalPosition = 0.0;
		}
	}

	private void EnsureFocusedPrimaryCommand()
	{
		ButtonBase moreButton = m_moreButton;
		Control val = GetFirstTabStopControl(PrimaryCommands);
		if (val == null && moreButton != null && ((Control)moreButton).IsTabStop)
		{
			val = (Control)(object)moreButton;
		}
		if (val != null)
		{
			if (!((UIElement)val).IsFocused)
			{
				FocusControl(val, null, updateTabStop: false);
			}
		}
		else
		{
			FocusCommand(PrimaryCommands, (Control)(object)moreButton, firstCommand: true, ensureTabStopUniqueness: true);
		}
	}

	protected override void OnKeyDown(KeyEventArgs args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Invalid comparison between Unknown and I4
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Invalid comparison between Unknown and I4
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Invalid comparison between Unknown and I4
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Invalid comparison between Unknown and I4
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Invalid comparison between Unknown and I4
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Invalid comparison between Unknown and I4
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Invalid comparison between Unknown and I4
		if (((RoutedEventArgs)args).Handled)
		{
			return;
		}
		Key key = args.Key;
		if ((int)key != 3)
		{
			CommandBarFlyout flyout;
			if ((int)key != 13)
			{
				if (key - 23 <= 3)
				{
					bool flag = m_primaryItemsRoot != null && (int)m_primaryItemsRoot.FlowDirection == 1;
					bool flag2 = ((int)args.Key == 23 && !flag) || ((int)args.Key == 25 && flag);
					bool flag3 = ((int)args.Key == 25 && !flag) || ((int)args.Key == 23 && flag);
					bool flag4 = (int)args.Key == 26;
					bool flag5 = (int)args.Key == 24;
					ButtonBase moreButton = m_moreButton;
					if (flag4 && moreButton != null && ((UIElement)moreButton).IsFocused && SecondaryCommands.Count > 0)
					{
						if (!IsOpen)
						{
							IsOpen = true;
						}
						if (FocusCommand(SecondaryCommands, null, firstCommand: true, ensureTabStopUniqueness: true))
						{
							((RoutedEventArgs)args).Handled = true;
						}
					}
					if (!((RoutedEventArgs)args).Handled && PrimaryCommands.Count > 0)
					{
						Control val = null;
						int num = 0;
						int num2 = PrimaryCommands.Count;
						int num3 = 1;
						if (flag2 || flag5)
						{
							num3 = -1;
							num = num2 - 1;
							num2 = -1;
							if (moreButton != null && ((UIElement)moreButton).IsFocused)
							{
								val = (Control)(object)moreButton;
							}
						}
						for (int i = num; i != num2; i += num3)
						{
							ICommandBarElement commandBarElement = PrimaryCommands[i];
							Control val2 = (Control)((commandBarElement is Control) ? commandBarElement : null);
							if (val2 != null)
							{
								if (((UIElement)val2).IsFocused)
								{
									val = val2;
								}
								else if (val != null && IsControlFocusable(val2, checkTabStop: false) && FocusControl(val2, val, updateTabStop: true))
								{
									((RoutedEventArgs)args).Handled = true;
									break;
								}
							}
						}
						if (!((RoutedEventArgs)args).Handled)
						{
							if ((flag3 || flag4) && val != null && moreButton != null && IsControlFocusable((Control)(object)moreButton, checkTabStop: false))
							{
								if (FocusControl((Control)(object)moreButton, val, updateTabStop: true))
								{
									((RoutedEventArgs)args).Handled = true;
								}
							}
							else if (flag5 && SecondaryCommands.Count > 0)
							{
								if (!IsOpen)
								{
									IsOpen = true;
								}
								if (FocusCommand(SecondaryCommands, null, firstCommand: false, ensureTabStopUniqueness: true))
								{
									((RoutedEventArgs)args).Handled = true;
								}
							}
						}
					}
					if (!((RoutedEventArgs)args).Handled)
					{
						((RoutedEventArgs)args).Handled = true;
					}
				}
			}
			else if (TryGetOwningFlyout(out flyout))
			{
				flyout.Hide();
				((RoutedEventArgs)args).Handled = true;
			}
		}
		else if (SecondaryCommands.Count > 0 && !IsOpen)
		{
			IsOpen = true;
			FocusCommand(SecondaryCommands, null, firstCommand: true, ensureTabStopUniqueness: true);
		}
		base.OnKeyDown(args);
	}

	private bool IsControlFocusable(Control control, bool checkTabStop)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (control != null && (int)((UIElement)control).Visibility == 0 && ((UIElement)control).IsEnabled)
		{
			if (checkTabStop)
			{
				return control.IsTabStop;
			}
			return true;
		}
		return false;
	}

	private Control GetFirstTabStopControl(IList<ICommandBarElement> commands)
	{
		foreach (ICommandBarElement command in commands)
		{
			Control val = (Control)((command is Control) ? command : null);
			if (val != null && val.IsTabStop)
			{
				return val;
			}
		}
		return null;
	}

	private bool FocusControl(Control newFocus, Control oldFocus, bool updateTabStop)
	{
		if (updateTabStop)
		{
			newFocus.IsTabStop = true;
		}
		if (((UIElement)newFocus).Focus())
		{
			if (oldFocus != null && updateTabStop)
			{
				oldFocus.IsTabStop = false;
			}
			return true;
		}
		return false;
	}

	private bool FocusCommand(IList<ICommandBarElement> commands, Control moreButton, bool firstCommand, bool ensureTabStopUniqueness)
	{
		Control val = null;
		int num = 0;
		int num2 = commands.Count;
		int num3 = 1;
		if (!firstCommand)
		{
			num3 = -1;
			num = num2 - 1;
			num2 = -1;
		}
		for (int i = num; i != num2; i += num3)
		{
			ICommandBarElement commandBarElement = commands[i];
			Control val2 = (Control)((commandBarElement is Control) ? commandBarElement : null);
			if (val2 == null || !IsControlFocusable(val2, !ensureTabStopUniqueness))
			{
				continue;
			}
			if (val == null)
			{
				if (FocusControl(val2, null, ensureTabStopUniqueness))
				{
					if (ensureTabStopUniqueness && moreButton != null && moreButton.IsTabStop)
					{
						moreButton.IsTabStop = false;
					}
					val = val2;
					if (!ensureTabStopUniqueness)
					{
						break;
					}
				}
			}
			else if (val != null && val2.IsTabStop)
			{
				val2.IsTabStop = false;
			}
		}
		return val != null;
	}

	private void EnsureTabStopUniqueness(IList<ICommandBarElement> commands, Control moreButton)
	{
		bool flag = moreButton != null && moreButton.IsTabStop;
		if (flag || GetFirstTabStopControl(commands) != null)
		{
			foreach (ICommandBarElement command in commands)
			{
				Control val = (Control)((command is Control) ? command : null);
				if (val != null && IsControlFocusable(val, checkTabStop: false) && val.IsTabStop)
				{
					if (!flag)
					{
						flag = true;
					}
					else
					{
						val.IsTabStop = false;
					}
				}
			}
			return;
		}
		foreach (ICommandBarElement command2 in commands)
		{
			Control val2 = (Control)((command2 is Control) ? command2 : null);
			if (val2 != null && IsControlFocusable(val2, checkTabStop: false))
			{
				val2.IsTabStop = true;
				break;
			}
		}
	}

	private void UpdateShadow()
	{
		if (PrimaryCommands.Count > 0)
		{
			AddShadow();
		}
		else if (PrimaryCommands.Count == 0)
		{
			ClearShadow();
		}
	}

	private void AddShadow()
	{
	}

	internal void ClearShadow()
	{
	}

	private void SecondaryItemsRootSizeChanged(object sender, SizeChangedEventArgs e)
	{
		UpdateUI();
	}

	private void SecondaryItemsRootPreviewKeyDown(object sender, KeyEventArgs args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Invalid comparison between Unknown and I4
		if (((RoutedEventArgs)args).Handled)
		{
			return;
		}
		Key key = args.Key;
		CommandBarFlyout flyout;
		if ((int)key != 13)
		{
			if ((int)key != 24 && (int)key != 26)
			{
				return;
			}
			if (SecondaryCommands.Count > 1)
			{
				Control val = null;
				int num = 0;
				int num2 = SecondaryCommands.Count;
				int num3 = 1;
				int num4 = 0;
				if ((int)args.Key == 24)
				{
					num3 = -1;
					num = num2 - 1;
					num2 = -1;
				}
				do
				{
					for (int i = num; i != num2; i += num3)
					{
						ICommandBarElement commandBarElement = SecondaryCommands[i];
						Control val2 = (Control)((commandBarElement is Control) ? commandBarElement : null);
						if (val2 != null)
						{
							if (((UIElement)val2).IsFocused)
							{
								val = val2;
							}
							else if (val != null && IsControlFocusable(val2, checkTabStop: false) && val != val2 && FocusControl(val2, val, updateTabStop: true))
							{
								((RoutedEventArgs)args).Handled = true;
								return;
							}
						}
					}
					if (num4 == 0 && PrimaryCommands.Count > 0)
					{
						ButtonBase moreButton = m_moreButton;
						if (num3 == 1 && FocusCommand(PrimaryCommands, (Control)(object)moreButton, firstCommand: true, ensureTabStopUniqueness: true))
						{
							((RoutedEventArgs)args).Handled = true;
							return;
						}
						if (num3 == -1 && val != null && moreButton != null && IsControlFocusable((Control)(object)moreButton, checkTabStop: false) && FocusControl((Control)(object)moreButton, val, updateTabStop: true))
						{
							((RoutedEventArgs)args).Handled = true;
							return;
						}
					}
					num4++;
				}
				while (num4 < 2 && val != null);
			}
			((RoutedEventArgs)args).Handled = true;
		}
		else if (TryGetOwningFlyout(out flyout))
		{
			flyout.Hide();
		}
	}

	private void OpeningStoryboardCompleted(object sender, EventArgs e)
	{
		m_openingStoryboard.Stop(m_layoutRoot);
	}

	private void ClosingStoryboardCompleted(object sender, EventArgs e)
	{
		m_closingStoryboard.Stop(m_layoutRoot);
		SetOpacity(0.0);
	}

	private void OpeningStoryboardCurrentStateInvalidated(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Clock val = (Clock)sender;
		m_openingStoryboardState = val.CurrentState;
	}

	private void ClosingStoryboardCurrentStateInvalidated(object sender, EventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Clock val = (Clock)sender;
		m_closingStoryboardState = val.CurrentState;
	}

	private void CancelAsyncOpenAnimation()
	{
		if (m_asyncOpenAnimation != null)
		{
			m_asyncOpenAnimation.Abort();
			m_asyncOpenAnimation = null;
		}
	}

	private void StopOpenAnimation()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		CancelAsyncOpenAnimation();
		if (m_openAnimationPending)
		{
			m_openAnimationPending = false;
			SetOpacity(1.0);
		}
		if (m_openingStoryboard != null && m_openingStoryboardState == (ClockState?)0)
		{
			m_openingStoryboard.Stop(m_layoutRoot);
		}
	}

	private void OnMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (!((ToolBar)this).IsOverflowOpen && ((RoutedEventArgs)e).Handled && ((RoutedEventArgs)e).OriginalSource == this && TryGetOwningFlyout(out var flyout))
		{
			flyout.Hide();
		}
	}

	private bool TryGetOwningFlyout(out CommandBarFlyout flyout)
	{
		WeakReference<CommandBarFlyout> owningFlyout = OwningFlyout;
		if (owningFlyout != null)
		{
			return owningFlyout.TryGetTarget(out flyout);
		}
		flyout = null;
		return false;
	}

	private void SetOpacity(double value)
	{
		if (m_layoutRoot != null)
		{
			((UIElement)m_layoutRoot).Opacity = value;
		}
		if (m_secondaryItemsRoot != null)
		{
			((UIElement)m_secondaryItemsRoot).Opacity = value;
		}
	}
}
