using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernWpf.Controls.Primitives;

public static class TreeViewItemHelper
{
	public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(TreeViewItemHelper), new PropertyMetadata(new PropertyChangedCallback(OnIsEnabledChanged)));

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly DependencyProperty CollapsedGlyphProperty = DependencyProperty.RegisterAttached("CollapsedGlyph", typeof(string), typeof(TreeViewItemHelper), new PropertyMetadata((object)"\ue76c"));

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly DependencyProperty ExpandedGlyphProperty = DependencyProperty.RegisterAttached("ExpandedGlyph", typeof(string), typeof(TreeViewItemHelper), new PropertyMetadata((object)"\ue70d"));

	public static readonly DependencyProperty CollapsedPathProperty = DependencyProperty.RegisterAttached("CollapsedPath", typeof(Geometry), typeof(TreeViewItemHelper));

	public static readonly DependencyProperty ExpandedPathProperty = DependencyProperty.RegisterAttached("ExpandedPath", typeof(Geometry), typeof(TreeViewItemHelper));

	public static readonly DependencyProperty GlyphBrushProperty = DependencyProperty.RegisterAttached("GlyphBrush", typeof(Brush), typeof(TreeViewItemHelper), (PropertyMetadata)null);

	public static readonly DependencyProperty GlyphOpacityProperty = DependencyProperty.RegisterAttached("GlyphOpacity", typeof(double), typeof(TreeViewItemHelper), new PropertyMetadata((object)1.0));

	public static readonly DependencyProperty GlyphSizeProperty = DependencyProperty.RegisterAttached("GlyphSize", typeof(double), typeof(TreeViewItemHelper), new PropertyMetadata((object)12.0));

	private static readonly DependencyPropertyKey IndentationPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Indentation", typeof(Thickness), typeof(TreeViewItemHelper), (PropertyMetadata)null);

	public static readonly DependencyProperty IndentationProperty = IndentationPropertyKey.DependencyProperty;

	public static bool GetIsEnabled(TreeViewItem treeViewItem)
	{
		return (bool)((DependencyObject)treeViewItem).GetValue(IsEnabledProperty);
	}

	public static void SetIsEnabled(TreeViewItem treeViewItem, bool value)
	{
		((DependencyObject)treeViewItem).SetValue(IsEnabledProperty, (object)value);
	}

	private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		TreeViewItem val = (TreeViewItem)d;
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			((UIElement)val).IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnTreeViewItemIsVisibleChanged);
			if (((UIElement)val).IsVisible)
			{
				UpdateIndentation(val);
			}
		}
		else
		{
			((UIElement)val).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnTreeViewItemIsVisibleChanged);
			((DependencyObject)val).ClearValue(IndentationPropertyKey);
		}
	}

	private static void OnTreeViewItemIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		if ((bool)((DependencyPropertyChangedEventArgs)(ref e)).NewValue)
		{
			UpdateIndentation((TreeViewItem)sender);
		}
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string GetCollapsedGlyph(TreeViewItem treeViewItem)
	{
		return (string)((DependencyObject)treeViewItem).GetValue(CollapsedGlyphProperty);
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void SetCollapsedGlyph(TreeViewItem treeViewItem, string value)
	{
		((DependencyObject)treeViewItem).SetValue(CollapsedGlyphProperty, (object)value);
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string GetExpandedGlyph(TreeViewItem treeViewItem)
	{
		return (string)((DependencyObject)treeViewItem).GetValue(ExpandedGlyphProperty);
	}

	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void SetExpandedGlyph(TreeViewItem treeViewItem, string value)
	{
		((DependencyObject)treeViewItem).SetValue(ExpandedGlyphProperty, (object)value);
	}

	public static Geometry GetCollapsedPath(TreeViewItem treeViewItem)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Geometry)((DependencyObject)treeViewItem).GetValue(CollapsedPathProperty);
	}

	public static void SetCollapsedPath(TreeViewItem treeViewItem, Geometry value)
	{
		((DependencyObject)treeViewItem).SetValue(CollapsedPathProperty, (object)value);
	}

	public static Geometry GetExpandedPath(TreeViewItem treeViewItem)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Geometry)((DependencyObject)treeViewItem).GetValue(ExpandedPathProperty);
	}

	public static void SetExpandedPath(TreeViewItem treeViewItem, Geometry value)
	{
		((DependencyObject)treeViewItem).SetValue(ExpandedPathProperty, (object)value);
	}

	public static Brush GetGlyphBrush(TreeViewItem treeViewItem)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return (Brush)((DependencyObject)treeViewItem).GetValue(GlyphBrushProperty);
	}

	public static void SetGlyphBrush(TreeViewItem treeViewItem, Brush value)
	{
		((DependencyObject)treeViewItem).SetValue(GlyphBrushProperty, (object)value);
	}

	public static double GetGlyphOpacity(TreeViewItem treeViewItem)
	{
		return (double)((DependencyObject)treeViewItem).GetValue(GlyphOpacityProperty);
	}

	public static void SetGlyphOpacity(TreeViewItem treeViewItem, double value)
	{
		((DependencyObject)treeViewItem).SetValue(GlyphOpacityProperty, (object)value);
	}

	public static double GetGlyphSize(TreeViewItem treeViewItem)
	{
		return (double)((DependencyObject)treeViewItem).GetValue(GlyphSizeProperty);
	}

	public static void SetGlyphSize(TreeViewItem treeViewItem, double value)
	{
		((DependencyObject)treeViewItem).SetValue(GlyphSizeProperty, (object)value);
	}

	public static Thickness GetIndentation(TreeViewItem treeViewItem)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return (Thickness)((DependencyObject)treeViewItem).GetValue(IndentationProperty);
	}

	private static void SetIndentation(TreeViewItem treeViewItem, Thickness value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((DependencyObject)treeViewItem).SetValue(IndentationPropertyKey, (object)value);
	}

	private static void UpdateIndentation(TreeViewItem item)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		SetIndentation(item, new Thickness((double)(GetDepth(item) * 16), 0.0, 0.0, 0.0));
	}

	private static int GetDepth(TreeViewItem item)
	{
		int num = 0;
		while (true)
		{
			ItemsControl obj = ItemsControl.ItemsControlFromItemContainer((DependencyObject)(object)item);
			TreeViewItem val = (TreeViewItem)(object)((obj is TreeViewItem) ? obj : null);
			if (val == null)
			{
				break;
			}
			num++;
			item = val;
		}
		return num;
	}
}
