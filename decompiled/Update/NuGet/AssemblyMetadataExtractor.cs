using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NuGet.Runtime;

namespace NuGet;

internal static class AssemblyMetadataExtractor
{
	private sealed class MetadataExtractor : MarshalByRefObject
	{
		private class AssemblyResolver
		{
			private readonly string _lookupPath;

			public AssemblyResolver(string assemblyPath)
			{
				_lookupPath = Path.GetDirectoryName(assemblyPath);
			}

			public Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
			{
				AssemblyName assemblyName = new AssemblyName(AppDomain.CurrentDomain.ApplyPolicy(args.Name));
				string text = Path.Combine(_lookupPath, assemblyName.Name + ".dll");
				if (!File.Exists(text))
				{
					return Assembly.ReflectionOnlyLoad(assemblyName.FullName);
				}
				return Assembly.ReflectionOnlyLoadFrom(text);
			}
		}

		public AssemblyMetadata GetMetadata(string path)
		{
			AssemblyResolver assemblyResolver = new AssemblyResolver(path);
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += assemblyResolver.ReflectionOnlyAssemblyResolve;
			try
			{
				Assembly assembly = Assembly.ReflectionOnlyLoadFrom(path);
				AssemblyName name = assembly.GetName();
				IList<CustomAttributeData> customAttributes = CustomAttributeData.GetCustomAttributes(assembly);
				if (!SemanticVersion.TryParse(GetAttributeValueOrDefault<AssemblyInformationalVersionAttribute>(customAttributes), out var value))
				{
					value = new SemanticVersion(name.Version);
				}
				return new AssemblyMetadata(GetProperties(customAttributes))
				{
					Name = name.Name,
					Version = value,
					Title = GetAttributeValueOrDefault<AssemblyTitleAttribute>(customAttributes),
					Company = GetAttributeValueOrDefault<AssemblyCompanyAttribute>(customAttributes),
					Description = GetAttributeValueOrDefault<AssemblyDescriptionAttribute>(customAttributes),
					Copyright = GetAttributeValueOrDefault<AssemblyCopyrightAttribute>(customAttributes)
				};
			}
			finally
			{
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= assemblyResolver.ReflectionOnlyAssemblyResolve;
			}
		}

		private static string GetAttributeValueOrDefault<T>(IList<CustomAttributeData> attributes) where T : Attribute
		{
			foreach (CustomAttributeData attribute in attributes)
			{
				if (attribute.Constructor.DeclaringType == typeof(T))
				{
					string text = attribute.ConstructorArguments[0].Value.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
			}
			return null;
		}

		private static Dictionary<string, string> GetProperties(IList<CustomAttributeData> attributes)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			string attributeName = typeof(AssemblyMetadataAttribute).FullName;
			foreach (CustomAttributeData item in attributes.Where((CustomAttributeData x) => x.Constructor.DeclaringType.FullName == attributeName && x.ConstructorArguments.Count == 2))
			{
				string text = item.ConstructorArguments[0].Value.ToString();
				string value = item.ConstructorArguments[1].Value.ToString();
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(value))
				{
					dictionary[text] = value;
				}
			}
			return dictionary;
		}
	}

	public static AssemblyMetadata GetMetadata(string assemblyPath)
	{
		AppDomainSetup appDomainSetup = new AppDomainSetup
		{
			ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
		};
		AppDomain domain = AppDomain.CreateDomain("metadata", AppDomain.CurrentDomain.Evidence, appDomainSetup);
		try
		{
			return domain.CreateInstance<MetadataExtractor>().GetMetadata(assemblyPath);
		}
		finally
		{
			AppDomain.Unload(domain);
		}
	}

	public static void ExtractMetadata(PackageBuilder builder, string assemblyPath)
	{
		AssemblyMetadata metadata = GetMetadata(assemblyPath);
		builder.Version = metadata.Version;
		builder.Title = metadata.Title;
		builder.Description = metadata.Description;
		builder.Copyright = metadata.Copyright;
		if (!builder.Authors.Any() && !string.IsNullOrEmpty(metadata.Company))
		{
			builder.Authors.Add(metadata.Company);
		}
		builder.Properties.AddRange(metadata.Properties);
		if (builder.Properties.ContainsKey("id"))
		{
			builder.Id = builder.Properties["id"];
		}
		else
		{
			builder.Id = metadata.Name;
		}
	}
}
