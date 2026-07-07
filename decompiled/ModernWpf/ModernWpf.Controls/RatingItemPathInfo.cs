using System.Windows;
using System.Windows.Media;

namespace ModernWpf.Controls;

public class RatingItemPathInfo : RatingItemInfo
{
	public static readonly DependencyProperty DisabledDataProperty = DependencyProperty.Register("DisabledData", typeof(Geometry), typeof(RatingItemPathInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(RatingItemPathInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty PlaceholderDataProperty = DependencyProperty.Register("PlaceholderData", typeof(Geometry), typeof(RatingItemPathInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty PointerOverDataProperty = DependencyProperty.Register("PointerOverData", typeof(Geometry), typeof(RatingItemPathInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty PointerOverPlaceholderDataProperty = DependencyProperty.Register("PointerOverPlaceholderData", typeof(Geometry), typeof(RatingItemPathInfo), (PropertyMetadata)null);

	public static readonly DependencyProperty UnsetDataProperty = DependencyProperty.Register("UnsetData", typeof(Geometry), typeof(RatingItemPathInfo), (PropertyMetadata)null);

	public Geometry DisabledData
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Geometry)((DependencyObject)this).GetValue(DisabledDataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(DisabledDataProperty, (object)value);
		}
	}

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

	public Geometry PlaceholderData
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Geometry)((DependencyObject)this).GetValue(PlaceholderDataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PlaceholderDataProperty, (object)value);
		}
	}

	public Geometry PointerOverData
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Geometry)((DependencyObject)this).GetValue(PointerOverDataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PointerOverDataProperty, (object)value);
		}
	}

	public Geometry PointerOverPlaceholderData
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Geometry)((DependencyObject)this).GetValue(PointerOverPlaceholderDataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PointerOverPlaceholderDataProperty, (object)value);
		}
	}

	public Geometry UnsetData
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return (Geometry)((DependencyObject)this).GetValue(UnsetDataProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(UnsetDataProperty, (object)value);
		}
	}

	protected override Freezable CreateInstanceCore()
	{
		return (Freezable)(object)new RatingItemPathInfo();
	}
}
