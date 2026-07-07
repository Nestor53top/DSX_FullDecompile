using System;
using System.Linq;
using System.Reflection;

namespace Squirrel.SimpleSplat;

internal static class AssemblyFinder
{
	public static T AttemptToLoadType<T>(string fullTypeName)
	{
		Type typeFromHandle = typeof(AssemblyFinder);
		AssemblyName[] array = new string[2]
		{
			typeFromHandle.AssemblyQualifiedName.Replace(typeFromHandle.FullName + ", ", ""),
			typeFromHandle.AssemblyQualifiedName.Replace(typeFromHandle.FullName + ", ", "").Replace(".Portable", "")
		}.Select((string x) => new AssemblyName(x)).ToArray();
		foreach (AssemblyName assemblyName in array)
		{
			Type type = Type.GetType(fullTypeName + ", " + assemblyName.FullName, throwOnError: false);
			if (!(type == null))
			{
				return (T)Activator.CreateInstance(type);
			}
		}
		return default(T);
	}
}
