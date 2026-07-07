using System.Collections.Generic;
using System.Linq;

namespace HidSharp.Reports;

public class Indexes
{
	private static readonly Indexes _unset = new Indexes();

	public virtual int Count => 0;

	public static Indexes Unset => _unset;

	public bool ContainsValue(uint value)
	{
		int elementIndex;
		return TryGetIndexFromValue(value, out elementIndex);
	}

	public IEnumerable<uint> GetAllValues()
	{
		return Enumerable.Range(0, Count).SelectMany((int index) => GetValuesFromIndex(index));
	}

	public virtual bool TryGetIndexFromValue(uint value, out int elementIndex)
	{
		elementIndex = -1;
		return false;
	}

	public virtual IEnumerable<uint> GetValuesFromIndex(int elementIndex)
	{
		yield break;
	}
}
