using System.Collections;
using System.Collections.Generic;

namespace Polly.Registry;

public interface IPolicyRegistry<TKey> : IReadOnlyPolicyRegistry<TKey>, IEnumerable<KeyValuePair<TKey, IsPolicy>>, IEnumerable
{
	new IsPolicy this[TKey key] { get; set; }

	void Add<TPolicy>(TKey key, TPolicy policy) where TPolicy : IsPolicy;

	bool Remove(TKey key);

	void Clear();
}
