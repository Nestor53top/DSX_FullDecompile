using System.Collections.Generic;
using System.Xml;

namespace Newtonsoft.Json.Converters;

internal class XmlNodeWrapper : IXmlNode
{
	private readonly XmlNode _node;

	private List<IXmlNode>? _childNodes;

	private List<IXmlNode>? _attributes;

	public object? WrappedNode => _node;

	public XmlNodeType NodeType => _node.NodeType;

	public virtual string? LocalName => _node.LocalName;

	public List<IXmlNode> ChildNodes
	{
		get
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			if (_childNodes == null)
			{
				if (!_node.HasChildNodes)
				{
					_childNodes = XmlNodeConverter.EmptyChildNodes;
				}
				else
				{
					_childNodes = new List<IXmlNode>(_node.ChildNodes.Count);
					foreach (XmlNode childNode in _node.ChildNodes)
					{
						XmlNode node = childNode;
						_childNodes.Add(WrapNode(node));
					}
				}
			}
			return _childNodes;
		}
	}

	protected virtual bool HasChildNodes => _node.HasChildNodes;

	public List<IXmlNode> Attributes
	{
		get
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			if (_attributes == null)
			{
				if (!HasAttributes)
				{
					_attributes = XmlNodeConverter.EmptyChildNodes;
				}
				else
				{
					_attributes = new List<IXmlNode>(((XmlNamedNodeMap)_node.Attributes).Count);
					foreach (XmlAttribute item in (XmlNamedNodeMap)_node.Attributes)
					{
						XmlAttribute node = item;
						_attributes.Add(WrapNode((XmlNode)(object)node));
					}
				}
			}
			return _attributes;
		}
	}

	private bool HasAttributes
	{
		get
		{
			XmlNode node = _node;
			XmlElement val = (XmlElement)(object)((node is XmlElement) ? node : null);
			if (val != null)
			{
				return val.HasAttributes;
			}
			XmlAttributeCollection attributes = _node.Attributes;
			if (attributes == null)
			{
				return false;
			}
			return ((XmlNamedNodeMap)attributes).Count > 0;
		}
	}

	public IXmlNode? ParentNode
	{
		get
		{
			XmlNode node = _node;
			XmlAttribute val = (XmlAttribute)(object)((node is XmlAttribute) ? node : null);
			XmlNode val2 = (XmlNode)((val != null) ? ((object)val.OwnerElement) : ((object)_node.ParentNode));
			if (val2 == null)
			{
				return null;
			}
			return WrapNode(val2);
		}
	}

	public string? Value
	{
		get
		{
			return _node.Value;
		}
		set
		{
			_node.Value = value;
		}
	}

	public string? NamespaceUri => _node.NamespaceURI;

	public XmlNodeWrapper(XmlNode node)
	{
		_node = node;
	}

	internal static IXmlNode WrapNode(XmlNode node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		XmlNodeType nodeType = node.NodeType;
		if ((int)nodeType != 1)
		{
			if ((int)nodeType != 10)
			{
				if ((int)nodeType == 17)
				{
					return new XmlDeclarationWrapper((XmlDeclaration)node);
				}
				return new XmlNodeWrapper(node);
			}
			return new XmlDocumentTypeWrapper((XmlDocumentType)node);
		}
		return new XmlElementWrapper((XmlElement)node);
	}

	public IXmlNode AppendChild(IXmlNode newChild)
	{
		XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
		_node.AppendChild(xmlNodeWrapper._node);
		_childNodes = null;
		_attributes = null;
		return newChild;
	}
}
