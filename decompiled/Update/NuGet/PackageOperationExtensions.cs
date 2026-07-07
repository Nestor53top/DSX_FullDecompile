using System;
using System.Collections.Generic;
using System.Linq;

namespace NuGet;

internal static class PackageOperationExtensions
{
	private sealed class IndexedPackageOperation
	{
		public int Index { get; set; }

		public PackageOperation Operation { get; set; }

		public IndexedPackageOperation(int index, PackageOperation operation)
		{
			Index = index;
			Operation = operation;
		}
	}

	public static IList<PackageOperation> Reduce(this IEnumerable<PackageOperation> operations)
	{
		Dictionary<object, List<IndexedPackageOperation>> dictionary = operations.Select((PackageOperation o, int index) => new IndexedPackageOperation(index, o)).ToLookup((IndexedPackageOperation o) => GetOperationKey(o.Operation)).ToDictionary((IGrouping<object, IndexedPackageOperation> g) => g.Key, (IGrouping<object, IndexedPackageOperation> g) => g.ToList());
		foreach (PackageOperation operation in operations)
		{
			object opposingOperationKey = GetOpposingOperationKey(operation);
			if (dictionary.ContainsKey(opposingOperationKey))
			{
				List<IndexedPackageOperation> list = dictionary[opposingOperationKey];
				list.RemoveAt(0);
				if (!list.Any())
				{
					dictionary.Remove(opposingOperationKey);
				}
			}
		}
		return dictionary.SelectMany((KeyValuePair<object, List<IndexedPackageOperation>> o) => o.Value).ToList().Reorder();
	}

	private static IList<PackageOperation> Reorder(this List<IndexedPackageOperation> operations)
	{
		operations.Sort((IndexedPackageOperation a, IndexedPackageOperation b) => a.Index - b.Index);
		List<IndexedPackageOperation> list = new List<IndexedPackageOperation>();
		for (int num = 0; num < operations.Count; num++)
		{
			IndexedPackageOperation indexedPackageOperation = operations[num];
			if (indexedPackageOperation.Operation.Package.IsSatellitePackage())
			{
				list.Add(indexedPackageOperation);
				operations.RemoveAt(num);
				num--;
			}
		}
		if (list.Count > 0)
		{
			operations.InsertRange(0, list.Where((IndexedPackageOperation s) => s.Operation.Action == PackageAction.Uninstall));
			operations.AddRange(list.Where((IndexedPackageOperation s) => s.Operation.Action == PackageAction.Install));
		}
		return operations.Select((IndexedPackageOperation o) => o.Operation).ToList();
	}

	private static object GetOperationKey(PackageOperation operation)
	{
		return Tuple.Create(operation.Action, operation.Package.Id, operation.Package.Version);
	}

	private static object GetOpposingOperationKey(PackageOperation operation)
	{
		return Tuple.Create((operation.Action == PackageAction.Install) ? PackageAction.Uninstall : PackageAction.Install, operation.Package.Id, operation.Package.Version);
	}
}
