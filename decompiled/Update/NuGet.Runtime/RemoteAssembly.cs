using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NuGet.Runtime;

internal class RemoteAssembly : MarshalByRefObject, IAssembly
{
	private static readonly Dictionary<Tuple<string, string>, Assembly> _assemblyCache = new Dictionary<Tuple<string, string>, Assembly>();

	private readonly List<IAssembly> _referencedAssemblies = new List<IAssembly>();

	public string Name { get; private set; }

	public Version Version { get; private set; }

	public string PublicKeyToken { get; private set; }

	public string Culture { get; private set; }

	public IEnumerable<IAssembly> ReferencedAssemblies => _referencedAssemblies;

	public void Load(string path)
	{
		Tuple<string, string> key = Tuple.Create(Path.GetFileName(path).ToUpperInvariant(), AssemblyName.GetAssemblyName(path).FullName);
		if (!_assemblyCache.TryGetValue(key, out var value))
		{
			value = Assembly.ReflectionOnlyLoadFrom(path);
			_assemblyCache[key] = value;
		}
		CopyAssemblyProperties(value.GetName(), this);
		AssemblyName[] referencedAssemblies = value.GetReferencedAssemblies();
		foreach (AssemblyName assemblyName in referencedAssemblies)
		{
			RemoteAssembly assembly = new RemoteAssembly();
			_referencedAssemblies.Add(CopyAssemblyProperties(assemblyName, assembly));
		}
	}

	private static RemoteAssembly CopyAssemblyProperties(AssemblyName assemblyName, RemoteAssembly assembly)
	{
		assembly.Name = assemblyName.Name;
		assembly.Version = assemblyName.Version;
		assembly.PublicKeyToken = assemblyName.GetPublicKeyTokenString();
		string text = assemblyName.CultureInfo.ToString();
		assembly.Culture = (string.IsNullOrEmpty(text) ? "neutral" : text);
		return assembly;
	}

	internal static IAssembly LoadAssembly(string path, AppDomain domain)
	{
		if (domain != AppDomain.CurrentDomain)
		{
			RemoteAssembly remoteAssembly = domain.CreateInstance<RemoteAssembly>();
			remoteAssembly.Load(path);
			return remoteAssembly;
		}
		RemoteAssembly remoteAssembly2 = new RemoteAssembly();
		remoteAssembly2.Load(path);
		return remoteAssembly2;
	}
}
