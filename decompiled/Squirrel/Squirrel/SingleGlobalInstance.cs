using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Squirrel.SimpleSplat;

namespace Squirrel;

internal sealed class SingleGlobalInstance : IDisposable, IEnableLogger
{
	private IDisposable handle;

	public SingleGlobalInstance(string key, TimeSpan timeOut)
	{
		if (ModeDetector.InUnitTestRunner())
		{
			return;
		}
		string path = Path.Combine(Path.GetTempPath(), ".squirrel-lock-" + key);
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		FileStream fh = null;
		while (stopwatch.Elapsed < timeOut)
		{
			try
			{
				fh = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete);
				fh.Write(new byte[4] { 186, 173, 240, 13 }, 0, 4);
			}
			catch (Exception exception)
			{
				this.Log().WarnException("Failed to grab lockfile, will retry: " + path, exception);
				Thread.Sleep(250);
				continue;
			}
			break;
		}
		stopwatch.Stop();
		if (fh == null)
		{
			throw new Exception("Couldn't acquire lock, is another instance running");
		}
		handle = Disposable.Create(delegate
		{
			fh.Dispose();
			File.Delete(path);
		});
	}

	public void Dispose()
	{
		if (!ModeDetector.InUnitTestRunner())
		{
			Interlocked.Exchange(ref handle, null)?.Dispose();
		}
	}

	~SingleGlobalInstance()
	{
		if (handle == null)
		{
			return;
		}
		throw new AbandonedMutexException("Leaked a Mutex!");
	}
}
