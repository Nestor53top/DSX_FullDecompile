using System.Threading;

namespace Medallion.Threading.Internal;

internal sealed class RefBox<T> where T : struct
{
	private readonly T _value;

	public ref readonly T Value => ref _value;

	internal RefBox(T value)
	{
		_value = value;
	}
}
internal sealed class RefBox
{
	public static RefBox<T> Create<T>(T value) where T : struct
	{
		return new RefBox<T>(value);
	}

	public static bool TryConsume<T>(ref RefBox<T>? boxRef, out T value) where T : struct
	{
		RefBox<T> refBox = Interlocked.Exchange(ref boxRef, null);
		if (refBox != null)
		{
			value = refBox.Value;
			return true;
		}
		value = default(T);
		return false;
	}
}
