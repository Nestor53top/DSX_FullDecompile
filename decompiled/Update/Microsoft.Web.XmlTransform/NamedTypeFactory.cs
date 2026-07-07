using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Web.XmlTransform;

internal class NamedTypeFactory
{
	private class Registration
	{
		private Assembly assembly;

		private string nameSpace;

		public bool IsValid => assembly != null;

		public string NameSpace => nameSpace;

		public Assembly Assembly => assembly;

		public Registration(Assembly assembly, string nameSpace)
		{
			this.assembly = assembly;
			this.nameSpace = nameSpace;
		}
	}

	private class AssemblyNameRegistration : Registration
	{
		public AssemblyNameRegistration(string assemblyName, string nameSpace)
			: base(Assembly.Load(assemblyName), nameSpace)
		{
		}
	}

	private class PathRegistration : Registration
	{
		public PathRegistration(string path, string nameSpace)
			: base(Assembly.LoadFrom(path), nameSpace)
		{
		}
	}

	private string relativePathRoot;

	private List<Registration> registrations = new List<Registration>();

	internal NamedTypeFactory(string relativePathRoot)
	{
		this.relativePathRoot = relativePathRoot;
		CreateDefaultRegistrations();
	}

	private void CreateDefaultRegistrations()
	{
		AddAssemblyRegistration(GetType().Assembly, GetType().Namespace);
	}

	internal void AddAssemblyRegistration(Assembly assembly, string nameSpace)
	{
		registrations.Add(new Registration(assembly, nameSpace));
	}

	internal void AddAssemblyRegistration(string assemblyName, string nameSpace)
	{
		registrations.Add(new AssemblyNameRegistration(assemblyName, nameSpace));
	}

	internal void AddPathRegistration(string path, string nameSpace)
	{
		if (!Path.IsPathRooted(path))
		{
			path = Path.Combine(Path.GetDirectoryName(relativePathRoot), path);
		}
		registrations.Add(new PathRegistration(path, nameSpace));
	}

	internal ObjectType Construct<ObjectType>(string typeName) where ObjectType : class
	{
		if (!string.IsNullOrEmpty(typeName))
		{
			Type type = GetType(typeName);
			if (type == null)
			{
				throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_UnknownTypeName, new object[2]
				{
					typeName,
					typeof(ObjectType).Name
				}));
			}
			if (!type.IsSubclassOf(typeof(ObjectType)))
			{
				throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_IncorrectBaseType, new object[2]
				{
					type.FullName,
					typeof(ObjectType).Name
				}));
			}
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
			{
				throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_NoValidConstructor, new object[1] { type.FullName }));
			}
			return constructor.Invoke(new object[0]) as ObjectType;
		}
		return null;
	}

	private Type GetType(string typeName)
	{
		Type type = null;
		foreach (Registration registration in registrations)
		{
			if (!registration.IsValid)
			{
				continue;
			}
			Type type2 = registration.Assembly.GetType(registration.NameSpace + "." + typeName);
			if (type2 != null)
			{
				if (!(type == null))
				{
					throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_AmbiguousTypeMatch, new object[1] { typeName }));
				}
				type = type2;
			}
		}
		return type;
	}
}
