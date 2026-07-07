using System.Collections.Generic;

namespace SQLitePCL;

internal class CompareFuncName : EqualityComparer<FuncName>
{
	private IEqualityComparer<byte[]> _ptrlencmp;

	public CompareFuncName(IEqualityComparer<byte[]> ptrlencmp)
	{
		_ptrlencmp = ptrlencmp;
	}

	public override bool Equals(FuncName p1, FuncName p2)
	{
		if (p1.n != p2.n)
		{
			return false;
		}
		return _ptrlencmp.Equals(p1.name, p2.name);
	}

	public override int GetHashCode(FuncName p)
	{
		return p.n + p.name.Length;
	}
}
