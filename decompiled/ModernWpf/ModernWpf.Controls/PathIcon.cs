using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ModernWpf.Controls;

public class PathIcon : IconElement
{
	public static readonly DependencyProperty DataProperty;

	private Path _path;

	public Geometry Data
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Geometry)((DependencyObject)this).GetValue(DataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DataProperty, (object)value);
		}
	}

	static PathIcon()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		DataProperty = Path.DataProperty.AddOwner(typeof(PathIcon), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDataChanged)));
		IconElement.ForegroundProperty.OverrideMetadata(typeof(PathIcon), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnForegroundChanged)));
	}

	private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((PathIcon)(object)d).ApplyData();
	}

	private protected override void InitializeChildren()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		_path = new Path
		{
			HorizontalAlignment = (HorizontalAlignment)3,
			VerticalAlignment = (VerticalAlignment)3,
			Stretch = (Stretch)2
		};
		ApplyForeground();
		ApplyData();
		base.Children.Add((UIElement)(object)_path);
	}

	private protected override void OnShouldInheritForegroundFromVisualParentChanged()
	{
		ApplyForeground();
	}

	private protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		if (base.ShouldInheritForegroundFromVisualParent)
		{
			ApplyForeground();
		}
	}

	private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((PathIcon)(object)d).ApplyForeground();
	}

	private void ApplyForeground()
	{
		if (_path != null)
		{
			((Shape)_path).Fill = (base.ShouldInheritForegroundFromVisualParent ? base.VisualParentForeground : base.Foreground);
		}
	}

	private void ApplyData()
	{
		if (_path != null)
		{
			_path.Data = Data;
		}
	}
}
