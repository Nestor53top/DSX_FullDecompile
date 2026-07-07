using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal sealed class DisposableAction : IDisposable
{
	public static readonly DisposableAction NoOp = new DisposableAction(delegate
	{
	});

	private Action _action;

	public DisposableAction(Action action)
	{
		_action = action;
	}

	public static IDisposable All(params IDisposable[] tokens)
	{
		return new DisposableAction(delegate
		{
			IDisposable[] array = tokens;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Dispose();
			}
		});
	}

	public static IDisposable All(IEnumerable<IDisposable> tokens)
	{
		return All(tokens.ToArray());
	}

	public void Dispose()
	{
		_action();
	}
}
