using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernWpf.Controls.Primitives;

public class ColumnMajorUniformToLargestGridLayout : NonVirtualizingLayout
{
	public static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register("ColumnSpacing", typeof(double), typeof(ColumnMajorUniformToLargestGridLayout), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColumnSpacingPropertyChanged)));

	public static readonly DependencyProperty RowSpacingProperty = DependencyProperty.Register("RowSpacing", typeof(double), typeof(ColumnMajorUniformToLargestGridLayout), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRowSpacingPropertyChanged)));

	public static readonly DependencyProperty MaxColumnsProperty = DependencyProperty.Register("MaxColumns", typeof(int), typeof(ColumnMajorUniformToLargestGridLayout), (PropertyMetadata)new FrameworkPropertyMetadata((object)1, new PropertyChangedCallback(OnMaxColumnsPropertyChanged)), new ValidateValueCallback(ValidateMaxColumns));

	private int m_actualColumnCount = 1;

	private Size m_largestChildSize;

	private bool m_testHooksEnabled;

	private int m_rows = -1;

	private int m_columns = -1;

	private int m_largerColumns = -1;

	public double ColumnSpacing
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(ColumnSpacingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ColumnSpacingProperty, (object)value);
		}
	}

	public double RowSpacing
	{
		get
		{
			return (double)((DependencyObject)this).GetValue(RowSpacingProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(RowSpacingProperty, (object)value);
		}
	}

	public int MaxColumns
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(MaxColumnsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(MaxColumnsProperty, (object)value);
		}
	}

	internal event TypedEventHandler<ColumnMajorUniformToLargestGridLayout, object> LayoutChanged;

	private static void OnColumnSpacingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ColumnMajorUniformToLargestGridLayout)(object)sender).OnColumnSpacingPropertyChanged(args);
	}

	private void OnColumnSpacingPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		InvalidateMeasure();
	}

	private static void OnRowSpacingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ColumnMajorUniformToLargestGridLayout)(object)sender).OnRowSpacingPropertyChanged(args);
	}

	private void OnRowSpacingPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		InvalidateMeasure();
	}

	private static void OnMaxColumnsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((ColumnMajorUniformToLargestGridLayout)(object)sender).OnMaxColumnsPropertyChanged(args);
	}

	private void OnMaxColumnsPropertyChanged(DependencyPropertyChangedEventArgs args)
	{
		InvalidateMeasure();
	}

	private static bool ValidateMaxColumns(object value)
	{
		return (int)value > 0;
	}

	protected override Size MeasureOverride(NonVirtualizingLayoutContext context, Size availableSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<UIElement> children = context.Children;
		if (children != null && children.Count > 0)
		{
			m_largestChildSize = calculateLargestChildSize();
			m_actualColumnCount = CalculateColumns(children.Count, ((Size)(ref m_largestChildSize)).Width, ((Size)(ref availableSize)).Width);
			int num = (int)Math.Ceiling((double)children.Count / (double)m_actualColumnCount);
			return new Size(((Size)(ref m_largestChildSize)).Width * (double)m_actualColumnCount + ColumnSpacing * (double)(m_actualColumnCount - 1), ((Size)(ref m_largestChildSize)).Height * (double)num + RowSpacing * (double)(num - 1));
		}
		return new Size(0.0, 0.0);
		Size calculateLargestChildSize()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			double num2 = 0.0;
			double num3 = 0.0;
			foreach (UIElement item in children)
			{
				item.Measure(availableSize);
				Size desiredSize = item.DesiredSize;
				if (((Size)(ref desiredSize)).Width > num2)
				{
					num2 = ((Size)(ref desiredSize)).Width;
				}
				if (((Size)(ref desiredSize)).Height > num3)
				{
					num3 = ((Size)(ref desiredSize)).Height;
				}
			}
			return new Size(num2, num3);
		}
	}

	protected override Size ArrangeOverride(NonVirtualizingLayoutContext context, Size finalSize)
	{
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<UIElement> children = context.Children;
		if (children != null)
		{
			int count = children.Count;
			int num = (int)Math.Floor((double)count / (double)m_actualColumnCount);
			int num2 = count % m_actualColumnCount;
			double columnSpacing = ColumnSpacing;
			double rowSpacing = RowSpacing;
			double num3 = 0.0;
			double num4 = 0.0;
			int num5 = 0;
			int num6 = 0;
			foreach (UIElement item in children)
			{
				Size desiredSize = item.DesiredSize;
				item.Arrange(new Rect(num3, num4, ((Size)(ref desiredSize)).Width, ((Size)(ref desiredSize)).Height));
				if (num6 < num2)
				{
					if (num5 % (num + 1) == num)
					{
						num3 += ((Size)(ref m_largestChildSize)).Width + columnSpacing;
						num4 = 0.0;
						num6++;
					}
					else
					{
						num4 += ((Size)(ref m_largestChildSize)).Height + rowSpacing;
					}
				}
				else if ((num5 - num2 * (num + 1)) % num == num - 1)
				{
					num3 += ((Size)(ref m_largestChildSize)).Width + columnSpacing;
					num4 = 0.0;
					num6++;
				}
				else
				{
					num4 += ((Size)(ref m_largestChildSize)).Height + rowSpacing;
				}
				num5++;
			}
			if (m_testHooksEnabled && (m_largerColumns != num2 || m_columns != num6 || m_rows != num))
			{
				m_largerColumns = num2;
				m_columns = num6;
				m_rows = num;
				this.LayoutChanged?.Invoke(this, null);
			}
		}
		return finalSize;
	}

	private int CalculateColumns(int childCount, double maxItemWidth, double availableWidth)
	{
		double num = ColumnSpacing + maxItemWidth;
		int num2 = Math.Min(MaxColumns, childCount);
		if (num < double.Epsilon)
		{
			return num2;
		}
		double num3 = availableWidth - maxItemWidth;
		double num4 = Math.Max(0.0, Math.Floor(num3 / num));
		double num5 = Math.Min(num2, num4 + 1.0);
		return Math.Max(1, (int)num5);
	}

	internal void SetTestHooksEnabled(bool enabled)
	{
		m_testHooksEnabled = enabled;
	}

	internal int GetRows()
	{
		return m_rows;
	}

	internal int GetColumns()
	{
		return m_columns;
	}

	internal int GetLargerColumns()
	{
		return m_largerColumns;
	}
}
