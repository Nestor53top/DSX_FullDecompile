using System.Windows;

namespace ModernWpf.Controls.Primitives;

public class VisualStateGroupListener : FrameworkElement
{
	public static readonly DependencyProperty GroupProperty;

	private static readonly DependencyPropertyKey CurrentStateNamePropertyKey;

	public static readonly DependencyProperty CurrentStateNameProperty;

	public static readonly DependencyProperty ListenerProperty;

	public VisualStateGroup Group
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (VisualStateGroup)((DependencyObject)this).GetValue(GroupProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(GroupProperty, (object)value);
		}
	}

	public string CurrentStateName
	{
		get
		{
			return (string)((DependencyObject)this).GetValue(CurrentStateNameProperty);
		}
		private set
		{
			((DependencyObject)this).SetValue(CurrentStateNamePropertyKey, (object)value);
		}
	}

	static VisualStateGroupListener()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		GroupProperty = DependencyProperty.Register("Group", typeof(VisualStateGroup), typeof(VisualStateGroupListener), new PropertyMetadata(new PropertyChangedCallback(OnGroupChanged)));
		CurrentStateNamePropertyKey = DependencyProperty.RegisterReadOnly("CurrentStateName", typeof(string), typeof(VisualStateGroupListener), (PropertyMetadata)null);
		CurrentStateNameProperty = CurrentStateNamePropertyKey.DependencyProperty;
		ListenerProperty = DependencyProperty.RegisterAttached("Listener", typeof(VisualStateGroupListener), typeof(VisualStateGroupListener), new PropertyMetadata(new PropertyChangedCallback(OnListenerChanged)));
		UIElement.VisibilityProperty.OverrideMetadata(typeof(VisualStateGroupListener), (PropertyMetadata)new FrameworkPropertyMetadata((object)(Visibility)2));
	}

	private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0023: Expected O, but got Unknown
		((VisualStateGroupListener)(object)d).OnGroupChanged((VisualStateGroup)((DependencyPropertyChangedEventArgs)(ref e)).OldValue, (VisualStateGroup)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	private void OnGroupChanged(VisualStateGroup oldGroup, VisualStateGroup newGroup)
	{
		if (oldGroup != null)
		{
			oldGroup.CurrentStateChanged -= OnCurrentStateChanged;
		}
		if (newGroup != null)
		{
			newGroup.CurrentStateChanged += OnCurrentStateChanged;
		}
		UpdateCurrentStateName((newGroup != null) ? newGroup.CurrentState : null);
	}

	private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
	{
		UpdateCurrentStateName(e.NewState);
	}

	private void UpdateCurrentStateName(VisualState currentState)
	{
		if (currentState != null)
		{
			CurrentStateName = currentState.Name;
		}
		else
		{
			((DependencyObject)this).ClearValue(CurrentStateNamePropertyKey);
		}
	}

	public static VisualStateGroupListener GetListener(VisualStateGroup group)
	{
		return (VisualStateGroupListener)((DependencyObject)group).GetValue(ListenerProperty);
	}

	public static void SetListener(VisualStateGroup group, VisualStateGroupListener value)
	{
		((DependencyObject)group).SetValue(ListenerProperty, (object)value);
	}

	private static void OnListenerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		if (((DependencyPropertyChangedEventArgs)(ref e)).OldValue is VisualStateGroupListener visualStateGroupListener)
		{
			((DependencyObject)visualStateGroupListener).ClearValue(GroupProperty);
		}
		if (((DependencyPropertyChangedEventArgs)(ref e)).NewValue is VisualStateGroupListener visualStateGroupListener2)
		{
			visualStateGroupListener2.Group = (VisualStateGroup)d;
		}
	}
}
