using System;

namespace HidSharp.Experimental;

internal abstract class BleDiscovery : IDisposable
{
	public abstract void StopDiscovery();

	void IDisposable.Dispose()
	{
		StopDiscovery();
	}
}
