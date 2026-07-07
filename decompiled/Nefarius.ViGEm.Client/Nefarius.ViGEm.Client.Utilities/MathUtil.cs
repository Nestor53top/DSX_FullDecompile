namespace Nefarius.ViGEm.Client.Utilities;

internal class MathUtil
{
	public static int ConvertRange(int originalStart, int originalEnd, int newStart, int newEnd, int value)
	{
		double num = (double)(newEnd - newStart) / (double)(originalEnd - originalStart);
		return (int)((double)newStart + (double)(value - originalStart) * num);
	}
}
