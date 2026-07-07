namespace NuGet;

internal class HashCodeCombiner
{
	private long _combinedHash64 = 5381L;

	public int CombinedHash => _combinedHash64.GetHashCode();

	public void AddInt32(int i)
	{
		_combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
	}

	public void AddObject(object o)
	{
		int i = o?.GetHashCode() ?? 0;
		AddInt32(i);
	}
}
