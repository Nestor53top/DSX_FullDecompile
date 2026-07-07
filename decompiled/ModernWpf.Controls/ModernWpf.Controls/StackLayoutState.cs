using System;
using System.Collections.Generic;

namespace ModernWpf.Controls;

public class StackLayoutState
{
	private readonly List<double> m_estimationBuffer = new List<double>();

	private const int BufferSize = 100;

	internal FlowLayoutAlgorithm FlowAlgorithm { get; } = new FlowLayoutAlgorithm();

	internal double TotalElementSize { get; private set; }

	internal double MaxArrangeBounds { get; private set; }

	internal int TotalElementsMeasured { get; private set; }

	internal void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
	{
		FlowAlgorithm.InitializeForContext(context, callbacks);
		if (m_estimationBuffer.Count == 0)
		{
			m_estimationBuffer.Resize(100, 0.0);
		}
		((ILayoutContextOverrides)context).LayoutStateCore = this;
	}

	internal void UninitializeForContext(VirtualizingLayoutContext context)
	{
		FlowAlgorithm.UninitializeForContext(context);
	}

	internal void OnElementMeasured(int elementIndex, double majorSize, double minorSize)
	{
		int index = elementIndex % m_estimationBuffer.Count;
		if (m_estimationBuffer[index] == 0.0)
		{
			TotalElementsMeasured++;
		}
		TotalElementSize -= m_estimationBuffer[index];
		TotalElementSize += majorSize;
		m_estimationBuffer[index] = majorSize;
		MaxArrangeBounds = Math.Max(MaxArrangeBounds, minorSize);
	}

	internal void OnMeasureStart()
	{
		MaxArrangeBounds = 0.0;
	}
}
