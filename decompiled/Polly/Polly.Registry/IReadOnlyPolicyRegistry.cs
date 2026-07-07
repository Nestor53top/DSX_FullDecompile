using System.Collections;
using System.Collections.Generic;

namespace Polly.Registry;

public interface IReadOnlyPolicyRegistry<TKey> : IEnumerable<KeyValuePair<TKey, IsPolicy>>, IEnumerable
{
	IsPolicy this[TKey key] { get; }

	int Count { get; }

	TPolicy Get<TPolicy>(TKey key) where TPolicy : IsPolicy;

	bool TryGet<TPolicy>(TKey key, out TPolicy policy) where TPolicy : IsPolicy;

	bool ContainsKey(TKey key);
}
