using System;
using System.Collections.Generic;

namespace ModernWpf.Controls;

public sealed class IndexPath
{
	private readonly List<int> m_path = new List<int>();

	public IndexPath(int index)
	{
		m_path.Add(index);
	}

	public IndexPath(int groupIndex, int itemIndex)
	{
		m_path.Add(groupIndex);
		m_path.Add(itemIndex);
	}

	public IndexPath(IList<int> indices)
	{
		if (indices != null)
		{
			for (int i = 0; i < indices.Count; i++)
			{
				m_path.Add(indices[i]);
			}
		}
	}

	public static IndexPath CreateFrom(int index)
	{
		return new IndexPath(index);
	}

	public static IndexPath CreateFrom(int groupIndex, int itemIndex)
	{
		return new IndexPath(groupIndex, itemIndex);
	}

	public static IndexPath CreateFromIndices(IList<int> indices)
	{
		return new IndexPath(indices);
	}

	public int GetSize()
	{
		return m_path.Count;
	}

	public int GetAt(int index)
	{
		return m_path[index];
	}

	public int CompareTo(IndexPath other)
	{
		int num = 0;
		int count = m_path.Count;
		int count2 = other.m_path.Count;
		if (count == 0 || count2 == 0)
		{
			num = count - count2;
		}
		else
		{
			for (int i = 0; i < Math.Min(count, count2); i++)
			{
				if (m_path[i] < other.m_path[i])
				{
					num = -1;
					break;
				}
				if (m_path[i] > other.m_path[i])
				{
					num = 1;
					break;
				}
			}
			num = ((num == 0) ? (count - count2) : num);
		}
		if (num != 0)
		{
			num = ((num > 0) ? 1 : (-1));
		}
		return num;
	}

	public override string ToString()
	{
		string text = "R";
		foreach (int item in m_path)
		{
			text += ".";
			text += item;
		}
		return text;
	}

	internal bool IsValid()
	{
		bool result = true;
		for (int i = 0; i < m_path.Count; i++)
		{
			if (m_path[i] < 0)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	internal IndexPath CloneWithChildIndex(int childIndex)
	{
		return new IndexPath(new List<int>(m_path) { childIndex });
	}
}
