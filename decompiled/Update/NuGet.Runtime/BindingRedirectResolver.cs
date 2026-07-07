using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuGet.Runtime;

internal static class BindingRedirectResolver
{
	public static IEnumerable<AssemblyBinding> GetBindingRedirects(string path)
	{
		return GetBindingRedirects(path, AppDomain.CurrentDomain);
	}

	public static IEnumerable<AssemblyBinding> GetBindingRedirects(string path, AppDomain domain)
	{
		if (path == null)
		{
			throw new ArgumentNullException("path");
		}
		if (domain == null)
		{
			throw new ArgumentNullException("domain");
		}
		return GetBindingRedirects(GetAssemblies(path, domain));
	}

	public static IEnumerable<AssemblyBinding> GetBindingRedirects(IEnumerable<string> assemblyPaths, AppDomain domain)
	{
		if (assemblyPaths == null)
		{
			throw new ArgumentNullException("assemblyPaths");
		}
		if (domain == null)
		{
			throw new ArgumentNullException("domain");
		}
		return GetBindingRedirects(GetAssemblies(assemblyPaths, domain));
	}

	public static IEnumerable<AssemblyBinding> GetBindingRedirects(IEnumerable<IAssembly> assemblies)
	{
		if (assemblies == null)
		{
			throw new ArgumentNullException("assemblies");
		}
		List<IAssembly> list = assemblies.ToList();
		Dictionary<Tuple<string, string>, IAssembly> dictionary = list.ToDictionary(GetUniqueKey);
		HashSet<IAssembly> hashSet = new HashSet<IAssembly>();
		foreach (IAssembly item in list)
		{
			foreach (IAssembly referencedAssembly in item.ReferencedAssemblies)
			{
				Tuple<string, string> uniqueKey = GetUniqueKey(referencedAssembly);
				if (dictionary.TryGetValue(uniqueKey, out var value) && value.Version != referencedAssembly.Version && !string.IsNullOrEmpty(value.PublicKeyToken))
				{
					hashSet.Add(value);
				}
			}
		}
		return hashSet.Select((IAssembly a) => new AssemblyBinding(a));
	}

	private static Tuple<string, string> GetUniqueKey(IAssembly assembly)
	{
		return Tuple.Create(assembly.Name, assembly.PublicKeyToken);
	}

	private static IEnumerable<IAssembly> GetAssemblies(string path, AppDomain domain)
	{
		if (!Directory.Exists(path))
		{
			return Enumerable.Empty<IAssembly>();
		}
		return GetAssemblies(Directory.GetFiles(path, "*.dll"), domain).Concat(GetAssemblies(Directory.GetFiles(path, "*.exe"), domain));
	}

	private static IEnumerable<IAssembly> GetAssemblies(IEnumerable<string> paths, AppDomain domain)
	{
		foreach (string path in paths)
		{
			yield return RemoteAssembly.LoadAssembly(path, domain);
		}
	}
}
