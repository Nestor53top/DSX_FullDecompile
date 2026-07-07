using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ModernWpf.Controls;

[TypeConverter(typeof(IconElementConverter))]
public abstract class IconElement : FrameworkElement
{
	public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(IconElement), (PropertyMetadata)new FrameworkPropertyMetadata((object)SystemColors.ControlTextBrush, (FrameworkPropertyMetadataOptions)32, new PropertyChangedCallback(OnForegroundPropertyChanged)));

	private static readonly DependencyProperty VisualParentForegroundProperty = DependencyProperty.Register("VisualParentForeground", typeof(Brush), typeof(IconElement), new PropertyMetadata((object)null, new PropertyChangedCallback(OnVisualParentForegroundPropertyChanged)));

	private Grid _layoutRoot;

	private bool _isForegroundDefaultOrInherited = true;

	private bool _shouldInheritForegroundFromVisualParent;

	[Bindable(true)]
	[Category("Appearance")]
	public Brush Foreground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(ForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ForegroundProperty, (object)value);
		}
	}

	private protected Brush VisualParentForeground
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Brush)((DependencyObject)this).GetValue(VisualParentForegroundProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(VisualParentForegroundProperty, (object)value);
		}
	}

	private protected bool ShouldInheritForegroundFromVisualParent
	{
		get
		{
			return _shouldInheritForegroundFromVisualParent;
		}
		private set
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			if (_shouldInheritForegroundFromVisualParent != value)
			{
				_shouldInheritForegroundFromVisualParent = value;
				if (_shouldInheritForegroundFromVisualParent)
				{
					((FrameworkElement)this).SetBinding(VisualParentForegroundProperty, (BindingBase)new Binding
					{
						Path = new PropertyPath((object)TextElement.ForegroundProperty),
						Source = ((Visual)this).VisualParent
					});
				}
				else
				{
					((DependencyObject)this).ClearValue(VisualParentForegroundProperty);
				}
				OnShouldInheritForegroundFromVisualParentChanged();
			}
		}
	}

	private protected UIElementCollection Children
	{
		get
		{
			EnsureLayoutRoot();
			return ((Panel)_layoutRoot).Children;
		}
	}

	protected override int VisualChildrenCount => 1;

	private protected IconElement()
	{
	}

	private static void OnForegroundPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((IconElement)(object)sender).OnForegroundPropertyChanged(args);
	}

	private void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		ValueSource valueSource = DependencyPropertyHelper.GetValueSource((DependencyObject)(object)this, ((DependencyPropertyChangedEventArgs)(ref args)).Property);
		BaseValueSource baseValueSource = ((ValueSource)(ref valueSource)).BaseValueSource;
		_isForegroundDefaultOrInherited = (int)baseValueSource <= 2;
		UpdateShouldInheritForegroundFromVisualParent();
	}

	private static void OnVisualParentForegroundPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((IconElement)(object)sender).OnVisualParentForegroundPropertyChanged(args);
	}

	private protected virtual void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
	}

	private protected virtual void OnShouldInheritForegroundFromVisualParentChanged()
	{
	}

	private void UpdateShouldInheritForegroundFromVisualParent()
	{
		ShouldInheritForegroundFromVisualParent = _isForegroundDefaultOrInherited && ((FrameworkElement)this).Parent != null && ((Visual)this).VisualParent != null && ((FrameworkElement)this).Parent != ((Visual)this).VisualParent;
	}

	private protected abstract void InitializeChildren();

	protected override Visual GetVisualChild(int index)
	{
		if (index == 0)
		{
			EnsureLayoutRoot();
			return (Visual)(object)_layoutRoot;
		}
		throw new ArgumentOutOfRangeException("index");
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		EnsureLayoutRoot();
		((UIElement)_layoutRoot).Measure(availableSize);
		return ((UIElement)_layoutRoot).DesiredSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EnsureLayoutRoot();
		((UIElement)_layoutRoot).Arrange(new Rect(default(Point), finalSize));
		return finalSize;
	}

	protected override void OnVisualParentChanged(DependencyObject oldParent)
	{
		((FrameworkElement)this).OnVisualParentChanged(oldParent);
		UpdateShouldInheritForegroundFromVisualParent();
	}

	private void EnsureLayoutRoot()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		if (_layoutRoot == null)
		{
			_layoutRoot = new Grid
			{
				Background = (Brush)(object)Brushes.Transparent,
				SnapsToDevicePixels = true
			};
			InitializeChildren();
			((Visual)this).AddVisualChild((Visual)(object)_layoutRoot);
		}
	}
}
