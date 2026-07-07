using System;
using System.Threading;

namespace Squirrel;

internal static class Disposable
{
	private class AnonDisposable : IDisposable
	{
		private static readonly Action dummyBlock = delegate
		{
		};

		private Action block;

		public AnonDisposable(Action b)
		{
			block = b;
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref block, dummyBlock)();
		}
	}

	public static IDisposable Create(Action action)
	{
		return new AnonDisposable(action);
	}
}
