namespace Standard;

internal static class Utility
{
	public static int LOWORD(int i)
	{
		return (short)(i & 0xFFFF);
	}
}
