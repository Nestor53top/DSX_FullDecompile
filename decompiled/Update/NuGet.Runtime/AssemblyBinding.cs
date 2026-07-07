using System;
using System.Xml.Linq;

namespace NuGet.Runtime;

internal class AssemblyBinding : IEquatable<AssemblyBinding>
{
	private const string Namespace = "urn:schemas-microsoft-com:asm.v1";

	private string _oldVersion;

	private string _culture;

	public string Name { get; private set; }

	public string Culture
	{
		get
		{
			return _culture ?? "neutral";
		}
		set
		{
			_culture = value;
		}
	}

	public string PublicKeyToken { get; private set; }

	public string ProcessorArchitecture { get; private set; }

	public string NewVersion { get; private set; }

	public string OldVersion
	{
		get
		{
			return _oldVersion ?? ("0.0.0.0-" + NewVersion);
		}
		set
		{
			_oldVersion = value;
		}
	}

	public Version AssemblyNewVersion { get; private set; }

	public string CodeBaseHref { get; private set; }

	public string CodeBaseVersion { get; private set; }

	public string PublisherPolicy { get; private set; }

	internal AssemblyBinding()
	{
	}

	public AssemblyBinding(IAssembly assembly)
	{
		Name = assembly.Name;
		PublicKeyToken = assembly.PublicKeyToken;
		NewVersion = assembly.Version.ToString();
		AssemblyNewVersion = assembly.Version;
		Culture = assembly.Culture;
	}

	public XElement ToXElement()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Expected O, but got Unknown
		XElement val = new XElement(GetQualifiedName("dependentAssembly"), new object[2]
		{
			(object)new XElement(GetQualifiedName("assemblyIdentity"), new object[4]
			{
				(object)new XAttribute(XName.op_Implicit("name"), (object)Name),
				(object)new XAttribute(XName.op_Implicit("publicKeyToken"), (object)PublicKeyToken),
				(object)new XAttribute(XName.op_Implicit("culture"), (object)Culture),
				(object)new XAttribute(XName.op_Implicit("processorArchitecture"), (object)(ProcessorArchitecture ?? string.Empty))
			}),
			(object)new XElement(GetQualifiedName("bindingRedirect"), new object[2]
			{
				(object)new XAttribute(XName.op_Implicit("oldVersion"), (object)OldVersion),
				(object)new XAttribute(XName.op_Implicit("newVersion"), (object)NewVersion)
			})
		});
		if (!string.IsNullOrEmpty(PublisherPolicy))
		{
			((XContainer)val).Add((object)new XElement(GetQualifiedName("publisherPolicy"), (object)new XAttribute(XName.op_Implicit("apply"), (object)PublisherPolicy)));
		}
		if (!string.IsNullOrEmpty(CodeBaseHref))
		{
			((XContainer)val).Add((object)new XElement(GetQualifiedName("codeBase"), new object[2]
			{
				(object)new XAttribute(XName.op_Implicit("href"), (object)CodeBaseHref),
				(object)new XAttribute(XName.op_Implicit("version"), (object)CodeBaseVersion)
			}));
		}
		val.RemoveAttributes((XAttribute a) => string.IsNullOrEmpty(a.Value));
		return val;
	}

	public override string ToString()
	{
		return ((object)ToXElement()).ToString();
	}

	public static AssemblyBinding Parse(XContainer dependentAssembly)
	{
		if (dependentAssembly == null)
		{
			throw new ArgumentNullException("dependentAssembly");
		}
		AssemblyBinding assemblyBinding = new AssemblyBinding();
		XElement val = dependentAssembly.Element(GetQualifiedName("assemblyIdentity"));
		if (val != null)
		{
			assemblyBinding.Name = val.Attribute(XName.op_Implicit("name")).Value;
			assemblyBinding.Culture = val.GetOptionalAttributeValue("culture");
			assemblyBinding.PublicKeyToken = val.GetOptionalAttributeValue("publicKeyToken");
			assemblyBinding.ProcessorArchitecture = val.GetOptionalAttributeValue("processorArchitecture");
		}
		XElement val2 = dependentAssembly.Element(GetQualifiedName("bindingRedirect"));
		if (val2 != null)
		{
			assemblyBinding.OldVersion = val2.Attribute(XName.op_Implicit("oldVersion")).Value;
			assemblyBinding.NewVersion = val2.Attribute(XName.op_Implicit("newVersion")).Value;
		}
		XElement val3 = dependentAssembly.Element(GetQualifiedName("codeBase"));
		if (val3 != null)
		{
			assemblyBinding.CodeBaseHref = val3.Attribute(XName.op_Implicit("href")).Value;
			assemblyBinding.CodeBaseVersion = val3.Attribute(XName.op_Implicit("version")).Value;
		}
		XElement val4 = dependentAssembly.Element(GetQualifiedName("publisherPolicy"));
		if (val4 != null)
		{
			assemblyBinding.PublisherPolicy = val4.Attribute(XName.op_Implicit("apply")).Value;
		}
		return assemblyBinding;
	}

	public static XName GetQualifiedName(string name)
	{
		return XName.Get(name, "urn:schemas-microsoft-com:asm.v1");
	}

	public bool Equals(AssemblyBinding other)
	{
		if (SafeEquals(Name, other.Name) && SafeEquals(PublicKeyToken, other.PublicKeyToken) && SafeEquals(Culture, other.Culture))
		{
			return SafeEquals(ProcessorArchitecture, other.ProcessorArchitecture);
		}
		return false;
	}

	private static bool SafeEquals(object a, object b)
	{
		if (a != null && b != null)
		{
			return a.Equals(b);
		}
		if (a == null && b == null)
		{
			return true;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is AssemblyBinding other)
		{
			return Equals(other);
		}
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		HashCodeCombiner hashCodeCombiner = new HashCodeCombiner();
		hashCodeCombiner.AddObject(Name);
		hashCodeCombiner.AddObject(PublicKeyToken);
		hashCodeCombiner.AddObject(Culture);
		hashCodeCombiner.AddObject(ProcessorArchitecture);
		return hashCodeCombiner.CombinedHash;
	}
}
