using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlElementContext : XmlNodeContext
{
	private XmlElementContext parentContext;

	private string xpath;

	private string parentXPath;

	private XmlDocument xmlTargetDoc;

	private IServiceProvider serviceProvider;

	private XmlNode transformNodes;

	private XmlNodeList targetNodes;

	private XmlNodeList targetParents;

	private XmlAttribute transformAttribute;

	private XmlAttribute locatorAttribute;

	private XmlNamespaceManager namespaceManager;

	private static Regex nameAndArgumentsRegex;

	public XmlElement Element
	{
		get
		{
			XmlNode obj = base.Node;
			return (XmlElement)(object)((obj is XmlElement) ? obj : null);
		}
	}

	public string XPath
	{
		get
		{
			if (xpath == null)
			{
				xpath = ConstructXPath();
			}
			return xpath;
		}
	}

	public string ParentXPath
	{
		get
		{
			if (parentXPath == null)
			{
				parentXPath = ConstructParentXPath();
			}
			return parentXPath;
		}
	}

	public int TransformLineNumber
	{
		get
		{
			XmlAttribute obj = transformAttribute;
			IXmlLineInfo val = (IXmlLineInfo)(object)((obj is IXmlLineInfo) ? obj : null);
			if (val != null)
			{
				return val.LineNumber;
			}
			return base.LineNumber;
		}
	}

	public int TransformLinePosition
	{
		get
		{
			XmlAttribute obj = transformAttribute;
			IXmlLineInfo val = (IXmlLineInfo)(object)((obj is IXmlLineInfo) ? obj : null);
			if (val != null)
			{
				return val.LinePosition;
			}
			return base.LinePosition;
		}
	}

	public XmlAttribute TransformAttribute => transformAttribute;

	public XmlAttribute LocatorAttribute => locatorAttribute;

	internal XmlNode TransformNode
	{
		get
		{
			if (transformNodes == null)
			{
				transformNodes = CreateCloneInTargetDocument((XmlNode)(object)Element);
			}
			return transformNodes;
		}
	}

	internal XmlNodeList TargetNodes
	{
		get
		{
			if (targetNodes == null)
			{
				targetNodes = GetTargetNodes(XPath);
			}
			return targetNodes;
		}
	}

	internal XmlNodeList TargetParents
	{
		get
		{
			if (targetParents == null && parentContext != null)
			{
				targetParents = GetTargetNodes(ParentXPath);
			}
			return targetParents;
		}
	}

	private XmlDocument TargetDocument => xmlTargetDoc;

	private Regex NameAndArgumentsRegex
	{
		get
		{
			if (nameAndArgumentsRegex == null)
			{
				nameAndArgumentsRegex = new Regex("\\A\\s*(?<name>\\w+)(\\s*\\((?<arguments>.*)\\))?\\s*\\Z", RegexOptions.Compiled | RegexOptions.Singleline);
			}
			return nameAndArgumentsRegex;
		}
	}

	public XmlElementContext(XmlElementContext parent, XmlElement element, XmlDocument xmlTargetDoc, IServiceProvider serviceProvider)
		: base((XmlNode)(object)element)
	{
		parentContext = parent;
		this.xmlTargetDoc = xmlTargetDoc;
		this.serviceProvider = serviceProvider;
	}

	public T GetService<T>() where T : class
	{
		if (serviceProvider != null)
		{
			return serviceProvider.GetService(typeof(T)) as T;
		}
		return null;
	}

	public Transform ConstructTransform(out string argumentString)
	{
		try
		{
			return CreateObjectFromAttribute<Transform>(out argumentString, out transformAttribute);
		}
		catch (Exception ex)
		{
			throw WrapException(ex);
		}
	}

	private string ConstructXPath()
	{
		try
		{
			string parentPath = ((parentContext == null) ? string.Empty : parentContext.XPath);
			string argumentString;
			Locator locator = CreateLocator(out argumentString);
			return locator.ConstructPath(parentPath, this, argumentString);
		}
		catch (Exception ex)
		{
			throw WrapException(ex);
		}
	}

	private string ConstructParentXPath()
	{
		try
		{
			string parentPath = ((parentContext == null) ? string.Empty : parentContext.XPath);
			string argumentString;
			Locator locator = CreateLocator(out argumentString);
			return locator.ConstructParentPath(parentPath, this, argumentString);
		}
		catch (Exception ex)
		{
			throw WrapException(ex);
		}
	}

	private Locator CreateLocator(out string argumentString)
	{
		Locator locator = CreateObjectFromAttribute<Locator>(out argumentString, out locatorAttribute);
		if (locator == null)
		{
			argumentString = null;
			locator = new DefaultLocator();
		}
		return locator;
	}

	private XmlNode CreateCloneInTargetDocument(XmlNode sourceNode)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		XmlNode result;
		if (TargetDocument is XmlFileInfoDocument xmlFileInfoDocument)
		{
			result = xmlFileInfoDocument.CloneNodeFromOtherDocument(sourceNode);
		}
		else
		{
			XmlReader val = (XmlReader)new XmlTextReader((TextReader)new StringReader(sourceNode.OuterXml));
			result = TargetDocument.ReadNode(val);
		}
		ScrubTransformAttributesAndNamespaces(result);
		return result;
	}

	private void ScrubTransformAttributesAndNamespaces(XmlNode node)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (node.Attributes != null)
		{
			List<XmlAttribute> list = new List<XmlAttribute>();
			foreach (XmlAttribute item in (XmlNamedNodeMap)node.Attributes)
			{
				XmlAttribute val = item;
				if (((XmlNode)val).NamespaceURI == XmlTransformation.TransformNamespace)
				{
					list.Add(val);
				}
				else if (((XmlNode)val).Prefix.Equals("xmlns") || ((XmlNode)val).Name.Equals("xmlns"))
				{
					list.Add(val);
				}
				else
				{
					((XmlNode)val).Prefix = null;
				}
			}
			foreach (XmlAttribute item2 in list)
			{
				node.Attributes.Remove(item2);
			}
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			XmlNode val2 = childNode;
			ScrubTransformAttributesAndNamespaces(val2);
		}
	}

	private XmlNodeList GetTargetNodes(string xpath)
	{
		GetNamespaceManager();
		return ((XmlNode)TargetDocument).SelectNodes(xpath, GetNamespaceManager());
	}

	private Exception WrapException(Exception ex)
	{
		return XmlNodeException.Wrap(ex, (XmlNode)(object)Element);
	}

	private Exception WrapException(Exception ex, XmlNode node)
	{
		return XmlNodeException.Wrap(ex, node);
	}

	private XmlNamespaceManager GetNamespaceManager()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		if (namespaceManager == null)
		{
			XmlNodeList val = ((XmlNode)Element).SelectNodes("namespace::*");
			if (val.Count > 0)
			{
				namespaceManager = new XmlNamespaceManager(((XmlNode)Element).OwnerDocument.NameTable);
				foreach (XmlAttribute item in val)
				{
					XmlAttribute val2 = item;
					string empty = string.Empty;
					int num = ((XmlNode)val2).Name.IndexOf(':');
					empty = ((num < 0) ? "_defaultNamespace" : ((XmlNode)val2).Name.Substring(num + 1));
					namespaceManager.AddNamespace(empty, ((XmlNode)val2).Value);
				}
			}
			else
			{
				namespaceManager = new XmlNamespaceManager(GetParentNameTable());
			}
		}
		return namespaceManager;
	}

	private XmlNameTable GetParentNameTable()
	{
		if (parentContext == null)
		{
			return ((XmlNode)Element).OwnerDocument.NameTable;
		}
		return parentContext.GetNamespaceManager().NameTable;
	}

	private string ParseNameAndArguments(string name, out string arguments)
	{
		arguments = null;
		System.Text.RegularExpressions.Match match = NameAndArgumentsRegex.Match(name);
		if (match.Success)
		{
			if (match.Groups["arguments"].Success)
			{
				CaptureCollection captures = match.Groups["arguments"].Captures;
				if (captures.Count == 1 && !string.IsNullOrEmpty(captures[0].Value))
				{
					arguments = captures[0].Value;
				}
			}
			return match.Groups["name"].Captures[0].Value;
		}
		throw new XmlTransformationException(SR.XMLTRANSFORMATION_BadAttributeValue);
	}

	private ObjectType CreateObjectFromAttribute<ObjectType>(out string argumentString, out XmlAttribute objectAttribute) where ObjectType : class
	{
		XmlNode namedItem = ((XmlNamedNodeMap)((XmlNode)Element).Attributes).GetNamedItem(typeof(ObjectType).Name, XmlTransformation.TransformNamespace);
		objectAttribute = (XmlAttribute)(object)((namedItem is XmlAttribute) ? namedItem : null);
		try
		{
			if (objectAttribute != null)
			{
				string text = ParseNameAndArguments(((XmlNode)objectAttribute).Value, out argumentString);
				if (!string.IsNullOrEmpty(text))
				{
					NamedTypeFactory service = GetService<NamedTypeFactory>();
					return service.Construct<ObjectType>(text);
				}
			}
		}
		catch (Exception ex)
		{
			throw WrapException(ex, (XmlNode)(object)objectAttribute);
		}
		argumentString = null;
		return null;
	}

	internal bool HasTargetNode(out XmlElementContext failedContext, out bool existedInOriginal)
	{
		failedContext = null;
		existedInOriginal = false;
		if (TargetNodes.Count == 0)
		{
			failedContext = this;
			while (failedContext.parentContext != null && failedContext.parentContext.TargetNodes.Count == 0)
			{
				failedContext = failedContext.parentContext;
			}
			existedInOriginal = ExistedInOriginal(failedContext.XPath);
			return false;
		}
		return true;
	}

	internal bool HasTargetParent(out XmlElementContext failedContext, out bool existedInOriginal)
	{
		failedContext = null;
		existedInOriginal = false;
		if (TargetParents.Count == 0)
		{
			failedContext = this;
			while (failedContext.parentContext != null && !string.IsNullOrEmpty(failedContext.parentContext.ParentXPath) && failedContext.parentContext.TargetParents.Count == 0)
			{
				failedContext = failedContext.parentContext;
			}
			existedInOriginal = ExistedInOriginal(failedContext.XPath);
			return false;
		}
		return true;
	}

	private bool ExistedInOriginal(string xpath)
	{
		IXmlOriginalDocumentService service = GetService<IXmlOriginalDocumentService>();
		if (service != null)
		{
			XmlNodeList val = service.SelectNodes(xpath, GetNamespaceManager());
			if (val != null && val.Count > 0)
			{
				return true;
			}
		}
		return false;
	}
}
