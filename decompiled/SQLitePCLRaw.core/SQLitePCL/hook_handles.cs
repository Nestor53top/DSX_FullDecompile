using System;
using System.Collections.Concurrent;

namespace SQLitePCL;

public class hook_handles : IDisposable
{
	private readonly ConcurrentDictionary<byte[], IDisposable> collation;

	private readonly ConcurrentDictionary<FuncName, IDisposable> scalar;

	private readonly ConcurrentDictionary<FuncName, IDisposable> agg;

	public IDisposable update;

	public IDisposable rollback;

	public IDisposable commit;

	public IDisposable trace;

	public IDisposable profile;

	public IDisposable progress;

	public IDisposable authorizer;

	public hook_handles(Func<IntPtr, IntPtr, int, bool> f)
	{
		CompareBuf compareBuf = new CompareBuf(f);
		collation = new ConcurrentDictionary<byte[], IDisposable>(compareBuf);
		scalar = new ConcurrentDictionary<FuncName, IDisposable>(new CompareFuncName(compareBuf));
		agg = new ConcurrentDictionary<FuncName, IDisposable>(new CompareFuncName(compareBuf));
	}

	public bool RemoveScalarFunction(byte[] name, int nargs)
	{
		FuncName key = new FuncName(name, nargs);
		if (scalar.TryRemove(key, out var value))
		{
			value.Dispose();
			return true;
		}
		return false;
	}

	public void AddScalarFunction(byte[] name, int nargs, IDisposable d)
	{
		FuncName key = new FuncName(name, nargs);
		scalar[key] = d;
	}

	public bool RemoveAggFunction(byte[] name, int nargs)
	{
		FuncName key = new FuncName(name, nargs);
		if (agg.TryRemove(key, out var value))
		{
			value.Dispose();
			return true;
		}
		return false;
	}

	public void AddAggFunction(byte[] name, int nargs, IDisposable d)
	{
		FuncName key = new FuncName(name, nargs);
		agg[key] = d;
	}

	public bool RemoveCollation(byte[] name)
	{
		if (collation.TryRemove(name, out var value))
		{
			value.Dispose();
			return true;
		}
		return false;
	}

	public void AddCollation(byte[] name, IDisposable d)
	{
		collation[name] = d;
	}

	public void Dispose()
	{
		foreach (IDisposable value in collation.Values)
		{
			value.Dispose();
		}
		foreach (IDisposable value2 in scalar.Values)
		{
			value2.Dispose();
		}
		foreach (IDisposable value3 in agg.Values)
		{
			value3.Dispose();
		}
		if (update != null)
		{
			update.Dispose();
		}
		if (rollback != null)
		{
			rollback.Dispose();
		}
		if (commit != null)
		{
			commit.Dispose();
		}
		if (trace != null)
		{
			trace.Dispose();
		}
		if (profile != null)
		{
			profile.Dispose();
		}
		if (progress != null)
		{
			progress.Dispose();
		}
		if (authorizer != null)
		{
			authorizer.Dispose();
		}
	}
}
