namespace ModernWpf.Controls;

internal class IndexRange
{
	private readonly int m_begin = -1;

	private readonly int m_end = -1;

	public int Begin => m_begin;

	public int End => m_end;

	public IndexRange()
	{
	}

	public IndexRange(int begin, int end)
	{
		if (begin > end)
		{
			int num = begin;
			begin = end;
			end = num;
		}
		m_begin = begin;
		m_end = end;
	}

	public bool Contains(int index)
	{
		if (index >= m_begin)
		{
			return index <= m_end;
		}
		return false;
	}

	public bool Split(int splitIndex, ref IndexRange before, ref IndexRange after)
	{
		before = new IndexRange(m_begin, splitIndex);
		if (splitIndex < m_end)
		{
			after = new IndexRange(splitIndex + 1, m_end);
			return true;
		}
		after = new IndexRange();
		return false;
	}

	public bool Intersects(IndexRange other)
	{
		if (m_begin <= other.End)
		{
			return m_end >= other.Begin;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is IndexRange indexRange)
		{
			return this == indexRange;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 17 * 31;
		int begin = m_begin;
		int num2 = (num + begin.GetHashCode()) * 31;
		begin = m_end;
		return num2 + begin.GetHashCode();
	}

	public static bool operator ==(IndexRange lhs, IndexRange rhs)
	{
		if (lhs.m_begin == rhs.m_begin)
		{
			return lhs.m_end == rhs.m_end;
		}
		return false;
	}

	public static bool operator !=(IndexRange lhs, IndexRange rhs)
	{
		return !(lhs == rhs);
	}
}
