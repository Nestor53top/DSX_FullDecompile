using System;
using System.Collections.Generic;

namespace Squirrel.SimpleSplat;

public interface IDependencyResolver : IDisposable
{
	object GetService(Type serviceType, string contract = null);

	IEnumerable<object> GetServices(Type serviceType, string contract = null);
}
