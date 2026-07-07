using System.Runtime.InteropServices;

namespace Medallion.Threading.Internal;

internal static class To<TTo>
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct ValueTaskConversion
	{
	}

	public static ValueTaskConversion ValueTask => default(ValueTaskConversion);
}
