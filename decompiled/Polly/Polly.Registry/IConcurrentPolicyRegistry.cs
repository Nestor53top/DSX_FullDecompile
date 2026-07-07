using System;
using System.Collections;
using System.Collections.Generic;

namespace Polly.Registry;

public interface IConcurrentPolicyRegistry<TKey> : IPolicyRegistry<TKey>, IReadOnlyPolicyRegistry<TKey>, IEnumerable<KeyValuePair<TKey, IsPolicy>>, IEnumerable
{
	bool TryAdd<TPolicy>(TKey key, TPolicy policy) where TPolicy : IsPolicy;

	bool TryRemove<TPolicy>(TKey key, out TPolicy policy) where TPolicy : IsPolicy;

	bool TryUpdate<TPolicy>(TKey key, TPolicy newPolicy, TPolicy comparisonPolicy) where TPolicy : IsPolicy;

	TPolicy GetOrAdd<TPolicy>(TKey key, Func<TKey, TPolicy> policyFactory) where TPolicy : IsPolicy;

	TPolicy GetOrAdd<TPolicy>(TKey key, TPolicy policy) where TPolicy : IsPolicy;

	TPolicy AddOrUpdate<TPolicy>(TKey key, Func<TKey, TPolicy> addPolicyFactory, Func<TKey, TPolicy, TPolicy> updatePolicyFactory) where TPolicy : IsPolicy;

	TPolicy AddOrUpdate<TPolicy>(TKey key, TPolicy addPolicy, Func<TKey, TPolicy, TPolicy> updatePolicyFactory) where TPolicy : IsPolicy;
}
