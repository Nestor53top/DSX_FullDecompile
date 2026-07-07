using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlFormatter
{
	private XmlFileInfoDocument document;

	private string originalFileName;

	private LinkedList<string> indents = new LinkedList<string>();

	private LinkedList<string> attributeIndents = new LinkedList<string>();

	private string currentIndent = string.Empty;

	private string currentAttributeIndent;

	private string oneTab;

	private string defaultTab = "\t";

	private XmlNode currentNode;

	private XmlNode previousNode;

	private XmlNode CurrentNode
	{
		get
		{
			return currentNode;
		}
		set
		{
			previousNode = currentNode;
			currentNode = value;
		}
	}

	private XmlNode PreviousNode => previousNode;

	private string PreviousIndent => indents.Last.Value;

	private string CurrentIndent
	{
		get
		{
			if (currentIndent == null)
			{
				currentIndent = ComputeCurrentIndent();
			}
			return currentIndent;
		}
	}

	public string CurrentAttributeIndent
	{
		get
		{
			if (currentAttributeIndent == null)
			{
				currentAttributeIndent = ComputeCurrentAttributeIndent();
			}
			return currentAttributeIndent;
		}
	}

	private string OneTab
	{
		get
		{
			if (oneTab == null)
			{
				oneTab = ComputeOneTab();
			}
			return oneTab;
		}
	}

	public string DefaultTab
	{
		get
		{
			return defaultTab;
		}
		set
		{
			defaultTab = value;
		}
	}

	public static void Format(XmlDocument document)
	{
		if (document is XmlFileInfoDocument parentNode)
		{
			XmlFormatter xmlFormatter = new XmlFormatter(parentNode);
			xmlFormatter.FormatLoop((XmlNode)(object)parentNode);
		}
	}

	private XmlFormatter(XmlFileInfoDocument document)
	{
		this.document = document;
		originalFileName = document.FileName;
	}

	private void FormatLoop(XmlNode parentNode)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected I4, but got Unknown
		for (int i = 0; i < parentNode.ChildNodes.Count; i++)
		{
			XmlNode val = (CurrentNode = parentNode.ChildNodes[i]);
			XmlNodeType nodeType = val.NodeType;
			switch (nodeType - 1)
			{
			case 0:
				i += HandleElement(val);
				break;
			case 12:
				i += HandleWhiteSpace(val);
				break;
			case 5:
			case 7:
				i += EnsureNodeIndent(val, indentBeforeEnd: false);
				break;
			}
		}
	}

	private void FormatAttributes(XmlNode node)
	{
		if (node is IXmlFormattableAttributes xmlFormattableAttributes)
		{
			xmlFormattableAttributes.FormatAttributes(this);
		}
	}

	private int HandleElement(XmlNode node)
	{
		int num = HandleStartElement(node);
		ReorderNewItemsAtEnd(node);
		FormatLoop(node);
		CurrentNode = node;
		return num + HandleEndElement(node);
	}

	private void ReorderNewItemsAtEnd(XmlNode node)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		if (IsNewNode(node))
		{
			return;
		}
		XmlNode val = node.LastChild;
		if (val == null || (int)val.NodeType == 13)
		{
			return;
		}
		XmlNode val2 = null;
		while (val != null)
		{
			XmlNodeType nodeType = val.NodeType;
			if ((int)nodeType != 1)
			{
				if ((int)nodeType == 13)
				{
					val2 = val;
				}
				break;
			}
			if (!IsNewNode(val))
			{
				break;
			}
			val = val.PreviousSibling;
		}
		if (val2 != null)
		{
			node.RemoveChild(val2);
			node.AppendChild(val2);
		}
	}

	private int HandleStartElement(XmlNode node)
	{
		int result = EnsureNodeIndent(node, indentBeforeEnd: false);
		FormatAttributes(node);
		PushIndent();
		return result;
	}

	private int HandleEndElement(XmlNode node)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		PopIndent();
		if (!((XmlElement)node).IsEmpty)
		{
			result = EnsureNodeIndent(node, indentBeforeEnd: true);
		}
		return result;
	}

	private int HandleWhiteSpace(XmlNode node)
	{
		int result = 0;
		if (IsWhiteSpace(PreviousNode))
		{
			XmlNode val = PreviousNode;
			if (FindLastNewLine(node.OuterXml) < 0 && FindLastNewLine(PreviousNode.OuterXml) >= 0)
			{
				val = node;
			}
			val.ParentNode.RemoveChild(val);
			result = -1;
		}
		string indentFromWhiteSpace = GetIndentFromWhiteSpace(node);
		if (indentFromWhiteSpace != null)
		{
			SetIndent(indentFromWhiteSpace);
		}
		return result;
	}

	private int EnsureNodeIndent(XmlNode node, bool indentBeforeEnd)
	{
		int result = 0;
		if (NeedsIndent(node, PreviousNode))
		{
			if (indentBeforeEnd)
			{
				InsertIndentBeforeEnd(node);
			}
			else
			{
				InsertIndentBefore(node);
				result = 1;
			}
		}
		return result;
	}

	private string GetIndentFromWhiteSpace(XmlNode node)
	{
		string outerXml = node.OuterXml;
		int num = FindLastNewLine(outerXml);
		if (num >= 0)
		{
			return outerXml.Substring(num);
		}
		return null;
	}

	private int FindLastNewLine(string whitespace)
	{
		for (int num = whitespace.Length - 1; num >= 0; num--)
		{
			switch (whitespace[num])
			{
			case '\r':
				return num;
			case '\n':
				if (num > 0 && whitespace[num - 1] == '\r')
				{
					return num - 1;
				}
				return num;
			default:
				return -1;
			case '\t':
			case ' ':
				break;
			}
		}
		return -1;
	}

	private void SetIndent(string indent)
	{
		if (currentIndent == null || !currentIndent.Equals(indent))
		{
			currentIndent = indent;
			oneTab = null;
			currentAttributeIndent = null;
		}
	}

	private void PushIndent()
	{
		indents.AddLast(new LinkedListNode<string>(CurrentIndent));
		currentIndent = null;
		attributeIndents.AddLast(new LinkedListNode<string>(currentAttributeIndent));
		currentAttributeIndent = null;
	}

	private void PopIndent()
	{
		if (indents.Count > 0)
		{
			currentIndent = indents.Last.Value;
			indents.RemoveLast();
			currentAttributeIndent = attributeIndents.Last.Value;
			attributeIndents.RemoveLast();
			return;
		}
		throw new InvalidOperationException();
	}

	private bool NeedsIndent(XmlNode node, XmlNode previousNode)
	{
		if (!IsWhiteSpace(previousNode) && !IsText(previousNode))
		{
			if (!IsNewNode(node))
			{
				return IsNewNode(previousNode);
			}
			return true;
		}
		return false;
	}

	private bool IsWhiteSpace(XmlNode node)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if (node != null)
		{
			return (int)node.NodeType == 13;
		}
		return false;
	}

	public bool IsText(XmlNode node)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		if (node != null)
		{
			return (int)node.NodeType == 3;
		}
		return false;
	}

	private bool IsNewNode(XmlNode node)
	{
		if (node != null)
		{
			return document.IsNewNode(node);
		}
		return false;
	}

	private void InsertIndentBefore(XmlNode node)
	{
		node.ParentNode.InsertBefore((XmlNode)(object)((XmlDocument)document).CreateWhitespace(CurrentIndent), node);
	}

	private void InsertIndentBeforeEnd(XmlNode node)
	{
		node.AppendChild((XmlNode)(object)((XmlDocument)document).CreateWhitespace(CurrentIndent));
	}

	private string ComputeCurrentIndent()
	{
		string text = LookAheadForIndent();
		if (text != null)
		{
			return text;
		}
		return PreviousIndent + OneTab;
	}

	private string LookAheadForIndent()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		if (currentNode.ParentNode == null)
		{
			return null;
		}
		foreach (XmlNode childNode in currentNode.ParentNode.ChildNodes)
		{
			XmlNode val = childNode;
			if (IsWhiteSpace(val) && val.NextSibling != null)
			{
				string outerXml = val.OuterXml;
				int num = FindLastNewLine(outerXml);
				if (num >= 0)
				{
					return outerXml.Substring(num);
				}
			}
		}
		return null;
	}

	private string ComputeOneTab()
	{
		if (indents.Count <= 0)
		{
			return DefaultTab;
		}
		LinkedListNode<string> linkedListNode = indents.Last;
		for (LinkedListNode<string> previous = linkedListNode.Previous; previous != null; previous = linkedListNode.Previous)
		{
			if (linkedListNode.Value.StartsWith(previous.Value, StringComparison.Ordinal))
			{
				return linkedListNode.Value.Substring(previous.Value.Length);
			}
			linkedListNode = previous;
		}
		return ConvertIndentToTab(linkedListNode.Value);
	}

	private string ConvertIndentToTab(string indent)
	{
		for (int i = 0; i < indent.Length - 1; i++)
		{
			char c = indent[i];
			if (c != '\n' && c != '\r')
			{
				return indent.Substring(i + 1);
			}
		}
		return DefaultTab;
	}

	private string ComputeCurrentAttributeIndent()
	{
		string text = LookForSiblingIndent(CurrentNode);
		if (text != null)
		{
			return text;
		}
		return CurrentIndent + OneTab;
	}

	private string LookForSiblingIndent(XmlNode currentNode)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		bool flag = true;
		string text = null;
		foreach (XmlNode childNode in currentNode.ParentNode.ChildNodes)
		{
			XmlNode val = childNode;
			if (val == currentNode)
			{
				flag = false;
			}
			else if (val is IXmlFormattableAttributes xmlFormattableAttributes)
			{
				text = xmlFormattableAttributes.AttributeIndent;
			}
			if (!flag && text != null)
			{
				return text;
			}
		}
		return null;
	}
}
