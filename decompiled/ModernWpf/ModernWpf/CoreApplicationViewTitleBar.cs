using System.Windows;
using System.Windows.Data;
using ModernWpf.Controls;

namespace ModernWpf;

internal sealed class CoreApplicationViewTitleBar
{
	private class Listener : DependencyObject
	{
		public static readonly DependencyProperty ExtendViewIntoTitleBarProperty = DependencyProperty.Register("ExtendViewIntoTitleBar", typeof(bool), typeof(Listener), new PropertyMetadata(new PropertyChangedCallback(OnExtendViewIntoTitleBarPropertyChanged)));

		public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(Listener), new PropertyMetadata(new PropertyChangedCallback(OnHeightPropertyChanged)));

		public static readonly DependencyProperty SystemOverlayLeftInsetProperty = DependencyProperty.Register("SystemOverlayLeftInset", typeof(double), typeof(Listener), new PropertyMetadata(new PropertyChangedCallback(OnSystemOverlayLeftInsetPropertyChanged)));

		public static readonly DependencyProperty SystemOverlayRightInsetProperty = DependencyProperty.Register("SystemOverlayRightInset", typeof(double), typeof(Listener), new PropertyMetadata(new PropertyChangedCallback(OnSystemOverlayRightInsetPropertyChanged)));

		private readonly CoreApplicationViewTitleBar _owner;

		public bool ExtendViewIntoTitleBar
		{
			get
			{
				return (bool)((DependencyObject)this).GetValue(ExtendViewIntoTitleBarProperty);
			}
			set
			{
				((DependencyObject)this).SetValue(ExtendViewIntoTitleBarProperty, (object)value);
			}
		}

		public double Height
		{
			get
			{
				return (double)((DependencyObject)this).GetValue(HeightProperty);
			}
			set
			{
				((DependencyObject)this).SetValue(HeightProperty, (object)value);
			}
		}

		public double SystemOverlayLeftInset
		{
			get
			{
				return (double)((DependencyObject)this).GetValue(SystemOverlayLeftInsetProperty);
			}
			set
			{
				((DependencyObject)this).SetValue(SystemOverlayLeftInsetProperty, (object)value);
			}
		}

		public double SystemOverlayRightInset
		{
			get
			{
				return (double)((DependencyObject)this).GetValue(SystemOverlayRightInsetProperty);
			}
			set
			{
				((DependencyObject)this).SetValue(SystemOverlayRightInsetProperty, (object)value);
			}
		}

		public Listener(CoreApplicationViewTitleBar owner)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Expected O, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			_owner = owner;
			Window owner2 = _owner._owner;
			BindingOperations.SetBinding((DependencyObject)(object)this, ExtendViewIntoTitleBarProperty, (BindingBase)new Binding
			{
				Path = new PropertyPath((object)TitleBar.ExtendViewIntoTitleBarProperty),
				Source = owner2
			});
			BindingOperations.SetBinding((DependencyObject)(object)this, HeightProperty, (BindingBase)new Binding
			{
				Path = new PropertyPath((object)TitleBar.HeightProperty),
				Source = owner2
			});
			BindingOperations.SetBinding((DependencyObject)(object)this, SystemOverlayLeftInsetProperty, (BindingBase)new Binding
			{
				Path = new PropertyPath((object)TitleBar.SystemOverlayLeftInsetProperty),
				Source = owner2
			});
			BindingOperations.SetBinding((DependencyObject)(object)this, SystemOverlayRightInsetProperty, (BindingBase)new Binding
			{
				Path = new PropertyPath((object)TitleBar.SystemOverlayRightInsetProperty),
				Source = owner2
			});
		}

		private static void OnExtendViewIntoTitleBarPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Listener)(object)sender).OnExtendViewIntoTitleBarPropertyChanged(args);
		}

		private void OnExtendViewIntoTitleBarPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			_owner.RaiseLayoutMetricsChanged();
			_owner.RaiseIsVisibleChanged();
		}

		private static void OnHeightPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Listener)(object)sender).OnHeightPropertyChanged(args);
		}

		private void OnHeightPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			_owner.RaiseLayoutMetricsChanged();
		}

		private static void OnSystemOverlayLeftInsetPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Listener)(object)sender).OnSystemOverlayLeftInsetPropertyChanged(args);
		}

		private void OnSystemOverlayLeftInsetPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			_owner.RaiseLayoutMetricsChanged();
		}

		private static void OnSystemOverlayRightInsetPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Listener)(object)sender).OnSystemOverlayRightInsetPropertyChanged(args);
		}

		private void OnSystemOverlayRightInsetPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			_owner.RaiseLayoutMetricsChanged();
		}
	}

	private static readonly DependencyProperty TitleBarProperty = DependencyProperty.RegisterAttached("TitleBar", typeof(CoreApplicationViewTitleBar), typeof(CoreApplicationViewTitleBar));

	private readonly Window _owner;

	private readonly Listener _listener;

	public bool ExtendViewIntoTitleBar
	{
		get
		{
			return TitleBar.GetExtendViewIntoTitleBar(_owner);
		}
		set
		{
			TitleBar.SetExtendViewIntoTitleBar(_owner, value);
		}
	}

	public double Height => TitleBar.GetHeight(_owner);

	public bool IsVisible => true;

	public double SystemOverlayLeftInset => TitleBar.GetSystemOverlayLeftInset(_owner);

	public double SystemOverlayRightInset => TitleBar.GetSystemOverlayRightInset(_owner);

	public event TypedEventHandler<CoreApplicationViewTitleBar, object> IsVisibleChanged;

	public event TypedEventHandler<CoreApplicationViewTitleBar, object> LayoutMetricsChanged;

	private CoreApplicationViewTitleBar(Window owner)
	{
		_owner = owner;
		_listener = new Listener(this);
	}

	private void RaiseIsVisibleChanged()
	{
		this.IsVisibleChanged?.Invoke(this, null);
	}

	private void RaiseLayoutMetricsChanged()
	{
		this.LayoutMetricsChanged?.Invoke(this, null);
	}

	internal static CoreApplicationViewTitleBar GetTitleBar(Window window)
	{
		CoreApplicationViewTitleBar coreApplicationViewTitleBar = (CoreApplicationViewTitleBar)((DependencyObject)window).GetValue(TitleBarProperty);
		if (coreApplicationViewTitleBar == null)
		{
			coreApplicationViewTitleBar = new CoreApplicationViewTitleBar(window);
			SetTitleBar(window, coreApplicationViewTitleBar);
		}
		return coreApplicationViewTitleBar;
	}

	internal static CoreApplicationViewTitleBar GetTitleBar(DependencyObject dependencyObject)
	{
		Window window = Window.GetWindow(dependencyObject);
		if (window != null)
		{
			return GetTitleBar(window);
		}
		return null;
	}

	private static void SetTitleBar(Window window, CoreApplicationViewTitleBar value)
	{
		((DependencyObject)window).SetValue(TitleBarProperty, (object)value);
	}
}
