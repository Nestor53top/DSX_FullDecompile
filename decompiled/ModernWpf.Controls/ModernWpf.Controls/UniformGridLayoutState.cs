using System;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls;

public class UniformGridLayoutState
{
	internal FlowLayoutAlgorithm FlowAlgorithm { get; } = new FlowLayoutAlgorithm();

	internal double EffectiveItemWidth { get; private set; }

	internal double EffectiveItemHeight { get; private set; }

	internal void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
	{
		FlowAlgorithm.InitializeForContext(context, callbacks);
		((ILayoutContextOverrides)context).LayoutStateCore = this;
	}

	internal void UninitializeForContext(VirtualizingLayoutContext context)
	{
		FlowAlgorithm.UninitializeForContext(context);
	}

	internal void EnsureElementSize(Size availableSize, VirtualizingLayoutContext context, double layoutItemWidth, double LayoutItemHeight, UniformGridLayoutItemsStretch stretch, Orientation orientation, double minRowSpacing, double minColumnSpacing, uint maxItemsPerLine)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (maxItemsPerLine == 0)
		{
			maxItemsPerLine = 1u;
		}
		if (context.ItemCount <= 0)
		{
			return;
		}
		UIElement elementIfRealized = FlowAlgorithm.GetElementIfRealized(0);
		if (elementIfRealized != null)
		{
			elementIfRealized.Measure(availableSize);
			SetSize(elementIfRealized.DesiredSize, layoutItemWidth, LayoutItemHeight, availableSize, stretch, orientation, minRowSpacing, minColumnSpacing, maxItemsPerLine);
			return;
		}
		UIElement orCreateElementAt = context.GetOrCreateElementAt(0, ElementRealizationOptions.ForceCreate);
		if (orCreateElementAt != null)
		{
			orCreateElementAt.Measure(availableSize);
			SetSize(orCreateElementAt.DesiredSize, layoutItemWidth, LayoutItemHeight, availableSize, stretch, orientation, minRowSpacing, minColumnSpacing, maxItemsPerLine);
			context.RecycleElement(orCreateElementAt);
		}
	}

	private void SetSize(Size desiredItemSize, double layoutItemWidth, double LayoutItemHeight, Size availableSize, UniformGridLayoutItemsStretch stretch, Orientation orientation, double minRowSpacing, double minColumnSpacing, uint maxItemsPerLine)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (maxItemsPerLine == 0)
		{
			maxItemsPerLine = 1u;
		}
		EffectiveItemWidth = (double.IsNaN(layoutItemWidth) ? ((Size)(ref desiredItemSize)).Width : layoutItemWidth);
		EffectiveItemHeight = (double.IsNaN(LayoutItemHeight) ? ((Size)(ref desiredItemSize)).Height : LayoutItemHeight);
		double num = (((int)orientation == 0) ? ((Size)(ref availableSize)).Width : ((Size)(ref availableSize)).Height);
		double num2 = (((int)orientation == 1) ? minRowSpacing : minColumnSpacing);
		double num3 = (((int)orientation == 0) ? EffectiveItemWidth : EffectiveItemHeight);
		double num4 = 0.0;
		if (!double.IsInfinity(num))
		{
			uint num5 = Math.Min(maxItemsPerLine, (uint)Math.Max(1.0, num / (num3 + num2)));
			if (num5 == 0)
			{
				num5 = 1u;
			}
			double num6 = (double)num5 * (num3 + num2) - num2;
			num4 = (int)(num - num6) / (int)num5;
		}
		switch (stretch)
		{
		case UniformGridLayoutItemsStretch.Fill:
			if ((int)orientation == 0)
			{
				EffectiveItemWidth += num4;
			}
			else
			{
				EffectiveItemHeight += num4;
			}
			break;
		case UniformGridLayoutItemsStretch.Uniform:
		{
			double num7 = (((int)orientation == 0) ? EffectiveItemHeight : EffectiveItemWidth) * (num4 / num3);
			if ((int)orientation == 0)
			{
				EffectiveItemWidth += num4;
				EffectiveItemHeight += num7;
			}
			else
			{
				EffectiveItemHeight += num4;
				EffectiveItemWidth += num7;
			}
			break;
		}
		}
	}
}
