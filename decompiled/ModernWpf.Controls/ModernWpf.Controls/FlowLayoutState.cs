using System.Collections.Generic;
using System.Windows;

namespace ModernWpf.Controls;

public class FlowLayoutState
{
	private readonly List<double> m_lineSizeEstimationBuffer = new List<double>();

	private readonly List<double> m_itemsPerLineEstimationBuffer = new List<double>();

	private static readonly int BufferSize = 100;

	internal FlowLayoutAlgorithm FlowAlgorithm { get; } = new FlowLayoutAlgorithm();

	internal double TotalLineSize { get; private set; }

	internal int TotalLinesMeasured { get; private set; }

	internal double TotalItemsPerLine { get; private set; }

	internal Size SpecialElementDesiredSize { get; set; }

	internal void InitializeForContext(VirtualizingLayoutContext context, IFlowLayoutAlgorithmDelegates callbacks)
	{
		FlowAlgorithm.InitializeForContext(context, callbacks);
		if (m_lineSizeEstimationBuffer.Count == 0)
		{
			m_lineSizeEstimationBuffer.Resize(BufferSize, 0.0);
			m_itemsPerLineEstimationBuffer.Resize(BufferSize, 0.0);
		}
		((ILayoutContextOverrides)context).LayoutStateCore = this;
	}

	internal void UninitializeForContext(VirtualizingLayoutContext context)
	{
		FlowAlgorithm.UninitializeForContext(context);
	}

	internal void OnLineArranged(int startIndex, int countInLine, double lineSize, VirtualizingLayoutContext context)
	{
		if (TotalLinesMeasured == 0 || startIndex + countInLine != context.ItemCount)
		{
			int index = startIndex % m_lineSizeEstimationBuffer.Count;
			if (m_lineSizeEstimationBuffer[index] == 0.0)
			{
				int totalLinesMeasured = TotalLinesMeasured + 1;
				TotalLinesMeasured = totalLinesMeasured;
			}
			TotalLineSize -= m_lineSizeEstimationBuffer[index];
			TotalLineSize += lineSize;
			m_lineSizeEstimationBuffer[index] = lineSize;
			TotalItemsPerLine -= m_itemsPerLineEstimationBuffer[index];
			TotalItemsPerLine += countInLine;
			m_itemsPerLineEstimationBuffer[index] = countInLine;
		}
	}
}
