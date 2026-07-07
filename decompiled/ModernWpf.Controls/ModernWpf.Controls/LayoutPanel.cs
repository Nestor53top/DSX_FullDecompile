using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ModernWpf.Controls;

public class LayoutPanel : Panel
{
	public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(Layout), typeof(LayoutPanel), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLayoutChanged)));

	public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(LayoutPanel), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Thickness(0.0, 0.0, 0.0, 0.0), (FrameworkPropertyMetadataOptions)1));

	private LayoutContext m_layoutContext;

	private Layout m_layout;

	public Layout Layout
	{
		get
		{
			return m_layout;
		}
		set
		{
			((DependencyObject)this).SetValue(LayoutProperty, (object)value);
		}
	}

	public Thickness Padding
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return (Thickness)((DependencyObject)this).GetValue(PaddingProperty);
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((DependencyObject)this).SetValue(PaddingProperty, (object)value);
		}
	}

	internal object LayoutState { get; set; }

	private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		((LayoutPanel)(object)d).OnLayoutChanged((Layout)((DependencyPropertyChangedEventArgs)(ref e)).OldValue, (Layout)((DependencyPropertyChangedEventArgs)(ref e)).NewValue);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		Thickness padding = Padding;
		double num = ((Thickness)(ref padding)).Left + ((Thickness)(ref padding)).Right;
		double num2 = ((Thickness)(ref padding)).Top + ((Thickness)(ref padding)).Bottom;
		Size val = availableSize;
		((Size)(ref val)).Width = ((Size)(ref val)).Width - num;
		((Size)(ref val)).Height = ((Size)(ref val)).Height - num2;
		((Size)(ref val)).Width = Math.Max(0.0, ((Size)(ref val)).Width);
		((Size)(ref val)).Height = Math.Max(0.0, ((Size)(ref val)).Height);
		Layout layout = Layout;
		Size result;
		if (layout != null)
		{
			Size val2 = layout.Measure(m_layoutContext, val);
			((Size)(ref val2)).Width = ((Size)(ref val2)).Width + num;
			((Size)(ref val2)).Height = ((Size)(ref val2)).Height + num2;
			result = val2;
		}
		else
		{
			Size val3 = default(Size);
			foreach (UIElement child in ((Panel)this).Children)
			{
				child.Measure(val);
				Size desiredSize = child.DesiredSize;
				((Size)(ref val3)).Width = Math.Max(((Size)(ref val3)).Width, ((Size)(ref desiredSize)).Width);
				((Size)(ref val3)).Height = Math.Max(((Size)(ref val3)).Height, ((Size)(ref desiredSize)).Height);
			}
			result = val3;
			((Size)(ref result)).Width = ((Size)(ref result)).Width + num;
			((Size)(ref result)).Height = ((Size)(ref result)).Height + num2;
		}
		return result;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		Size result = finalSize;
		Thickness padding = Padding;
		double num = ((Thickness)(ref padding)).Left + ((Thickness)(ref padding)).Right;
		double num2 = ((Thickness)(ref padding)).Top + ((Thickness)(ref padding)).Bottom;
		double left = ((Thickness)(ref padding)).Left;
		double top = ((Thickness)(ref padding)).Top;
		Size finalSize2 = finalSize;
		((Size)(ref finalSize2)).Width = ((Size)(ref finalSize2)).Width - num;
		((Size)(ref finalSize2)).Height = ((Size)(ref finalSize2)).Height - num2;
		((Size)(ref finalSize2)).Width = Math.Max(0.0, ((Size)(ref finalSize2)).Width);
		((Size)(ref finalSize2)).Height = Math.Max(0.0, ((Size)(ref finalSize2)).Height);
		Layout layout = Layout;
		if (layout != null)
		{
			Size val = layout.Arrange(m_layoutContext, finalSize2);
			((Size)(ref val)).Width = ((Size)(ref val)).Width + num;
			((Size)(ref val)).Height = ((Size)(ref val)).Height + num2;
			if (left != 0.0 || top != 0.0)
			{
				foreach (UIElement child in ((Panel)this).Children)
				{
					FrameworkElement val3 = (FrameworkElement)((child is FrameworkElement) ? child : null);
					if (val3 != null)
					{
						Rect layoutSlot = LayoutInformation.GetLayoutSlot(val3);
						((Rect)(ref layoutSlot)).X = ((Rect)(ref layoutSlot)).X + left;
						((Rect)(ref layoutSlot)).Y = ((Rect)(ref layoutSlot)).Y + top;
						((UIElement)val3).Arrange(layoutSlot);
					}
				}
			}
			result = val;
		}
		else
		{
			Rect val4 = default(Rect);
			((Rect)(ref val4))._002Ector(left, top, ((Size)(ref finalSize2)).Width, ((Size)(ref finalSize2)).Height);
			foreach (UIElement child2 in ((Panel)this).Children)
			{
				child2.Arrange(val4);
			}
		}
		return result;
	}

	private void OnLayoutChanged(Layout oldValue, Layout newValue)
	{
		if (m_layoutContext == null)
		{
			m_layoutContext = new LayoutPanelLayoutContext(this);
		}
		if (oldValue != null)
		{
			oldValue.UninitializeForContext(m_layoutContext);
			oldValue.MeasureInvalidated -= InvalidateMeasureForLayout;
			oldValue.ArrangeInvalidated -= InvalidateArrangeForLayout;
		}
		m_layout = newValue;
		if (newValue != null)
		{
			newValue.InitializeForContext(m_layoutContext);
			newValue.MeasureInvalidated += InvalidateMeasureForLayout;
			newValue.ArrangeInvalidated += InvalidateArrangeForLayout;
		}
		((UIElement)this).InvalidateMeasure();
	}

	private void InvalidateMeasureForLayout(Layout sender, object args)
	{
		((UIElement)this).InvalidateMeasure();
	}

	private void InvalidateArrangeForLayout(Layout sender, object args)
	{
		((UIElement)this).InvalidateArrange();
	}
}
