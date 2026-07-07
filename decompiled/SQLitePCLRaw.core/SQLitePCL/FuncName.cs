namespace SQLitePCL;

internal class FuncName
{
	public byte[] name { get; private set; }

	public int n { get; private set; }

	public FuncName(byte[] _name, int _n)
	{
		name = _name;
		n = _n;
	}
}
