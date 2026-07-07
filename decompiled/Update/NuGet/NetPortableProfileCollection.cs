using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NuGet;

internal class NetPortableProfileCollection : KeyedCollection<string, NetPortableProfile>
{
	public NetPortableProfileCollection()
		: base((IEqualityComparer<string>?)StringComparer.OrdinalIgnoreCase)
	{
	}

	protected override string GetKeyForItem(NetPortableProfile item)
	{
		return item.Name;
	}
}
