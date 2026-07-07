using System;
using System.Threading;

namespace Squirrel.SimpleSplat;

internal sealed class ActionDisposable : IDisposable
{
	private Action block;

	public static IDisposable Empty => new ActionDisposable(delegate
	{
	});

	public ActionDisposable(Action block)
	{
		this.block = block;
	}

	public void Dispose()
	{
		Interlocked.Exchange(ref block, delegate
		{
		})();
	}
}
