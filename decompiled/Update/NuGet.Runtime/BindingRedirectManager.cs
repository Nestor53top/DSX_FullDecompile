using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NuGet.Runtime;

internal class BindingRedirectManager
{
	private static readonly XName AssemblyBindingName = AssemblyBinding.GetQualifiedName("assemblyBinding");

	private static readonly XName DependentAssemblyName = AssemblyBinding.GetQualifiedName("dependentAssembly");

	private readonly IFileSystem _fileSystem;

	private readonly string _configurationPath;

	public BindingRedirectManager(IFileSystem fileSystem, string configurationPath)
	{
		if (fileSystem == null)
		{
			throw new ArgumentNullException("fileSystem");
		}
		if (string.IsNullOrEmpty(configurationPath))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "configurationPath");
		}
		_fileSystem = fileSystem;
		_configurationPath = configurationPath;
	}

	public void AddBindingRedirects(IEnumerable<AssemblyBinding> bindingRedirects)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (bindingRedirects == null)
		{
			throw new ArgumentNullException("bindingRedirects");
		}
		if (!bindingRedirects.Any())
		{
			return;
		}
		XDocument configuration = GetConfiguration();
		XElement val = ((XContainer)configuration.Root).Element(XName.op_Implicit("runtime"));
		if (val == null)
		{
			val = new XElement(XName.op_Implicit("runtime"));
			((XContainer)(object)configuration.Root).AddIndented((XContainer)(object)val);
		}
		ILookup<AssemblyBinding, XElement> assemblyBindings = GetAssemblyBindings(configuration);
		XElement val2 = null;
		foreach (AssemblyBinding bindingRedirect in bindingRedirects)
		{
			if (assemblyBindings.Contains(bindingRedirect))
			{
				IEnumerable<XElement> source = assemblyBindings[bindingRedirect];
				if (source.Any())
				{
					foreach (XElement item in source.Skip(1))
					{
						RemoveElement(item);
					}
					UpdateBindingRedirectElement(source.First(), bindingRedirect);
					continue;
				}
			}
			if (val2 == null)
			{
				val2 = GetAssemblyBindingElement(val);
			}
			((XContainer)(object)val2).AddIndented((XContainer)(object)bindingRedirect.ToXElement());
		}
		Save(configuration);
	}

	public void RemoveBindingRedirects(IEnumerable<AssemblyBinding> bindingRedirects)
	{
		if (bindingRedirects == null)
		{
			throw new ArgumentNullException("bindingRedirects");
		}
		if (!bindingRedirects.Any())
		{
			return;
		}
		XDocument configuration = GetConfiguration();
		ILookup<AssemblyBinding, XElement> assemblyBindings = GetAssemblyBindings(configuration);
		if (!assemblyBindings.Any())
		{
			return;
		}
		foreach (AssemblyBinding bindingRedirect in bindingRedirects)
		{
			if (!assemblyBindings.Contains(bindingRedirect))
			{
				continue;
			}
			foreach (XElement item in assemblyBindings[bindingRedirect])
			{
				RemoveElement(item);
			}
		}
		Save(configuration);
	}

	private static void RemoveElement(XElement element)
	{
		XElement parent = ((XObject)element).Parent;
		((XNode)(object)element).RemoveIndented();
		if (!parent.HasElements)
		{
			((XNode)(object)parent).RemoveIndented();
		}
	}

	private static XElement GetAssemblyBindingElement(XElement runtime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		XElement val = ((XContainer)runtime).Elements(AssemblyBindingName).FirstOrDefault();
		if (val != null)
		{
			return val;
		}
		val = new XElement(AssemblyBindingName);
		((XContainer)(object)runtime).AddIndented((XContainer)(object)val);
		return val;
	}

	private void Save(XDocument document)
	{
		_fileSystem.AddFile(_configurationPath, (Action<Stream>)document.Save);
	}

	private static ILookup<AssemblyBinding, XElement> GetAssemblyBindings(XDocument document)
	{
		XElement val = ((XContainer)document.Root).Element(XName.op_Implicit("runtime"));
		IEnumerable<XElement> source = Enumerable.Empty<XElement>();
		if (val != null)
		{
			source = GetAssemblyBindingElements(val);
		}
		return source.Select((XElement dependentAssemblyElement) => new
		{
			Binding = AssemblyBinding.Parse((XContainer)(object)dependentAssemblyElement),
			Element = dependentAssemblyElement
		}).ToLookup(p => p.Binding, p => p.Element);
	}

	private static IEnumerable<XElement> GetAssemblyBindingElements(XElement runtime)
	{
		return Extensions.Elements<XElement>(((XContainer)runtime).Elements(AssemblyBindingName), DependentAssemblyName);
	}

	private XDocument GetConfiguration()
	{
		return XmlUtility.GetOrCreateDocument(XName.op_Implicit("configuration"), _fileSystem, _configurationPath);
	}

	private static void UpdateBindingRedirectElement(XElement element, AssemblyBinding bindingRedirect)
	{
		XElement obj = ((XContainer)element).Element(AssemblyBinding.GetQualifiedName("bindingRedirect"));
		obj.Attribute(XName.op_Implicit("oldVersion")).SetValue((object)bindingRedirect.OldVersion);
		obj.Attribute(XName.op_Implicit("newVersion")).SetValue((object)bindingRedirect.NewVersion);
	}
}
