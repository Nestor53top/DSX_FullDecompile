using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class SimpleStackPanel : Panel
{
	public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SimpleStackPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)(Orientation)1, (FrameworkPropertyMetadataOptions)1));

	public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register("Spacing", typeof(double), typeof(SimpleStackPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)0.0, (FrameworkPropertyMetadataOptions)1));

	public Orientation Orientation
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Orientation)((DependencyObject)this).GetValue(OrientationProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(OrientationProperty, (object)value);
		}
	}

	public double Spacing
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(SpacingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(SpacingProperty, (object)value);
		}
	}

	protected override bool HasLogicalOrientation => true;

	protected override Orientation LogicalOrientation => Orientation;

	protected override Size MeasureOverride(Size constraint)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		Size result = default(Size);
		UIElementCollection internalChildren = ((Panel)this).InternalChildren;
		Size val = constraint;
		bool flag = (int)Orientation == 0;
		double spacing = Spacing;
		bool flag2 = false;
		if (flag)
		{
			((Size)(ref val)).Width = double.PositiveInfinity;
		}
		else
		{
			((Size)(ref val)).Height = double.PositiveInfinity;
		}
		int i = 0;
		for (int count = internalChildren.Count; i < count; i++)
		{
			UIElement val2 = internalChildren[i];
			if (val2 != null)
			{
				bool flag3 = (int)val2.Visibility != 2;
				if (flag3 && !flag2)
				{
					flag2 = true;
				}
				val2.Measure(val);
				Size desiredSize = val2.DesiredSize;
				if (flag)
				{
					((Size)(ref result)).Width = ((Size)(ref result)).Width + ((flag3 ? spacing : 0.0) + ((Size)(ref desiredSize)).Width);
					((Size)(ref result)).Height = Math.Max(((Size)(ref result)).Height, ((Size)(ref desiredSize)).Height);
				}
				else
				{
					((Size)(ref result)).Width = Math.Max(((Size)(ref result)).Width, ((Size)(ref desiredSize)).Width);
					((Size)(ref result)).Height = ((Size)(ref result)).Height + ((flag3 ? spacing : 0.0) + ((Size)(ref desiredSize)).Height);
				}
			}
		}
		if (flag)
		{
			((Size)(ref result)).Width = ((Size)(ref result)).Width - (flag2 ? spacing : 0.0);
		}
		else
		{
			((Size)(ref result)).Height = ((Size)(ref result)).Height - (flag2 ? spacing : 0.0);
		}
		return result;
	}

	protected override Size ArrangeOverride(Size arrangeSize)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Invalid comparison between Unknown and I4
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		UIElementCollection internalChildren = ((Panel)this).InternalChildren;
		bool flag = (int)Orientation == 0;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(arrangeSize);
		double num = 0.0;
		double spacing = Spacing;
		int i = 0;
		for (int count = internalChildren.Count; i < count; i++)
		{
			UIElement val2 = internalChildren[i];
			if (val2 != null)
			{
				Size desiredSize;
				if (flag)
				{
					((Rect)(ref val)).X = ((Rect)(ref val)).X + num;
					desiredSize = val2.DesiredSize;
					num = (((Rect)(ref val)).Width = ((Size)(ref desiredSize)).Width);
					double height = ((Size)(ref arrangeSize)).Height;
					desiredSize = val2.DesiredSize;
					((Rect)(ref val)).Height = Math.Max(height, ((Size)(ref desiredSize)).Height);
				}
				else
				{
					((Rect)(ref val)).Y = ((Rect)(ref val)).Y + num;
					desiredSize = val2.DesiredSize;
					num = (((Rect)(ref val)).Height = ((Size)(ref desiredSize)).Height);
					double width2 = ((Size)(ref arrangeSize)).Width;
					desiredSize = val2.DesiredSize;
					((Rect)(ref val)).Width = Math.Max(width2, ((Size)(ref desiredSize)).Width);
				}
				if ((int)val2.Visibility != 2)
				{
					num += spacing;
				}
				val2.Arrange(val);
			}
		}
		return arrangeSize;
	}
}
