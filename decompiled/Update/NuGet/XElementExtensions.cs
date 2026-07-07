using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NuGet;

internal static class XElementExtensions
{
	public static string GetOptionalAttributeValue(this XElement element, string localName, string namespaceName = null)
	{
		XAttribute val = ((!string.IsNullOrEmpty(namespaceName)) ? element.Attribute(XName.Get(localName, namespaceName)) : element.Attribute(XName.op_Implicit(localName)));
		if (val == null)
		{
			return null;
		}
		return val.Value;
	}

	public static string GetOptionalElementValue(this XContainer element, string localName, string namespaceName = null)
	{
		XElement val = (XElement)((!string.IsNullOrEmpty(namespaceName)) ? ((object)element.Element(XName.Get(localName, namespaceName))) : ((object)element.ElementsNoNamespace(localName).FirstOrDefault()));
		if (val == null)
		{
			return null;
		}
		return val.Value;
	}

	public static IEnumerable<XElement> ElementsNoNamespace(this XContainer container, string localName)
	{
		return from e in container.Elements()
			where e.Name.LocalName == localName
			select e;
	}

	public static IEnumerable<XElement> ElementsNoNamespace(this IEnumerable<XContainer> source, string localName)
	{
		return from e in Extensions.Elements<XContainer>(source)
			where e.Name.LocalName == localName
			select e;
	}

	public static XElement Except(this XElement source, XElement target)
	{
		if (target == null)
		{
			return source;
		}
		foreach (XAttribute item in (from e in source.Attributes()
			where AttributeEquals(e, target.Attribute(e.Name))
			select e).ToList())
		{
			item.Remove();
		}
		foreach (XNode item2 in ((XContainer)source).Nodes().ToList())
		{
			XComment val = (XComment)(object)((item2 is XComment) ? item2 : null);
			if (val != null)
			{
				if (HasComment(target, val))
				{
					((XNode)val).Remove();
				}
				continue;
			}
			XElement val2 = (XElement)(object)((item2 is XElement) ? item2 : null);
			if (val2 == null)
			{
				continue;
			}
			XElement val3 = FindElement(target, val2);
			if (val3 != null && !HasConflict(val2, val3))
			{
				val2.Except(val3);
				if (!val2.HasAttributes && !val2.HasElements)
				{
					((XNode)val2).Remove();
					((XNode)val3).Remove();
				}
			}
		}
		return source;
	}

	public static XElement MergeWith(this XElement source, XElement target)
	{
		return source.MergeWith(target, null);
	}

	public static XElement MergeWith(this XElement source, XElement target, IDictionary<XName, Action<XElement, XElement>> nodeActions)
	{
		if (target == null)
		{
			return source;
		}
		foreach (XAttribute item in target.Attributes())
		{
			if (source.Attribute(item.Name) == null)
			{
				((XContainer)source).Add((object)item);
			}
		}
		Queue<XComment> queue = new Queue<XComment>();
		foreach (XNode item2 in ((XContainer)target).Nodes())
		{
			XComment val = (XComment)(object)((item2 is XComment) ? item2 : null);
			if (val != null)
			{
				queue.Enqueue(val);
				continue;
			}
			XElement val2 = (XElement)(object)((item2 is XElement) ? item2 : null);
			if (val2 != null)
			{
				XElement val3 = FindElement(source, val2);
				if (val3 != null)
				{
					AddContents(queue, (Action<XComment>)((XNode)val3).AddBeforeSelf);
				}
				if (val3 != null && !HasConflict(val3, val2))
				{
					val3.MergeWith(val2, nodeActions);
					continue;
				}
				if (nodeActions != null && nodeActions.TryGetValue(val2.Name, out var value))
				{
					value(source, val2);
					continue;
				}
				((XContainer)source).Add((object)val2);
				XElement val4 = ((XContainer)source).Elements().Last();
				AddContents(queue, (Action<XComment>)((XNode)val4).AddBeforeSelf);
			}
		}
		AddContents(queue, (Action<XComment>)((XContainer)source).Add);
		return source;
	}

	private static XElement FindElement(XElement source, XElement targetChild)
	{
		List<XElement> list = ((XContainer)source).Elements(targetChild.Name).ToList();
		list.Sort((XElement a, XElement b) => Compare(targetChild, a, b));
		return list.FirstOrDefault();
	}

	private static bool HasComment(XElement element, XComment comment)
	{
		return ((XContainer)element).Nodes().Any((XNode node) => (int)((XObject)node).NodeType == 8 && ((XComment)node).Value.Equals(comment.Value, StringComparison.Ordinal));
	}

	private static int Compare(XElement target, XElement left, XElement right)
	{
		int num = CountMatches(left, target, AttributeEquals);
		int num2 = CountMatches(right, target, AttributeEquals);
		if (num == num2)
		{
			int value = CountMatches(left, target, (XAttribute a, XAttribute b) => a.Name == b.Name);
			return CountMatches(right, target, (XAttribute a, XAttribute b) => a.Name == b.Name).CompareTo(value);
		}
		return num2.CompareTo(num);
	}

	private static int CountMatches(XElement left, XElement right, Func<XAttribute, XAttribute, bool> matcher)
	{
		return (from la in left.Attributes()
			from ta in right.Attributes()
			where matcher(la, ta)
			select la).Count();
	}

	private static bool HasConflict(XElement source, XElement target)
	{
		Dictionary<XName, string> dictionary = source.Attributes().ToDictionary((XAttribute a) => a.Name, (XAttribute a) => a.Value);
		foreach (XAttribute item in target.Attributes())
		{
			if (dictionary.TryGetValue(item.Name, out var value) && value != item.Value)
			{
				return true;
			}
		}
		return false;
	}

	public static void RemoveAttributes(this XElement element, Func<XAttribute, bool> condition)
	{
		Extensions.Remove((IEnumerable<XAttribute>)element.Attributes().Where(condition).ToList());
		((XContainer)element).Descendants().ToList().ForEach(delegate(XElement e)
		{
			e.RemoveAttributes(condition);
		});
	}

	public static void AddIndented(this XContainer container, XContainer content)
	{
		string text = ((XNode)(object)container).ComputeOneLevelOfIndentation();
		XNode previousNode = ((XNode)container).PreviousNode;
		XText val = (XText)(object)((previousNode is XText) ? previousNode : null);
		string text2 = ((val != null) ? val.Value : Environment.NewLine);
		content.IndentChildrenElements(text2 + text, text);
		AddLeadingIndentation(container, text2, text);
		container.Add((object)content);
		AddTrailingIndentation(container, text2);
	}

	private static void AddTrailingIndentation(XContainer container, string containerIndent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		container.Add((object)new XText(containerIndent));
	}

	private static void AddLeadingIndentation(XContainer container, string containerIndent, string oneIndentLevel)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		bool num = !container.Nodes().Any();
		XNode lastNode = container.LastNode;
		XText val = (XText)(object)((lastNode is XText) ? lastNode : null);
		if (num || val == null)
		{
			container.Add((object)new XText(containerIndent + oneIndentLevel));
		}
		else
		{
			val.Value += oneIndentLevel;
		}
	}

	private static void IndentChildrenElements(this XContainer container, string containerIndent, string oneIndentLevel)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		string text = containerIndent + oneIndentLevel;
		foreach (XElement item in container.Elements())
		{
			((XNode)item).AddBeforeSelf((object)new XText(text));
			((XContainer)(object)item).IndentChildrenElements(text + oneIndentLevel, oneIndentLevel);
		}
		if (container.Elements().Any())
		{
			container.Add((object)new XText(containerIndent));
		}
	}

	public static void RemoveIndented(this XNode element)
	{
		XNode previousNode = element.PreviousNode;
		XText val = (XText)(object)((previousNode is XText) ? previousNode : null);
		XNode nextNode = element.NextNode;
		XText val2 = (XText)(object)((nextNode is XText) ? nextNode : null);
		string text = element.ComputeOneLevelOfIndentation();
		bool num = !element.ElementsAfterSelf().Any();
		element.Remove();
		if (val2 != null && val2.IsWhiteSpace())
		{
			((XNode)val2).Remove();
		}
		if (num && val != null && val.IsWhiteSpace())
		{
			val.Value = val.Value.Substring(0, val.Value.Length - text.Length);
		}
	}

	private static bool IsWhiteSpace(this XText textNode)
	{
		return string.IsNullOrWhiteSpace(textNode.Value);
	}

	private static string ComputeOneLevelOfIndentation(this XNode node)
	{
		int num = node.Ancestors().Count();
		XNode previousNode = node.PreviousNode;
		XText val = (XText)(object)((previousNode is XText) ? previousNode : null);
		if (num == 0 || val == null || !val.IsWhiteSpace())
		{
			return "  ";
		}
		string text = val.Value.Trim(Environment.NewLine.ToCharArray());
		int c = ((text.LastOrDefault() == '\t') ? 9 : 32);
		int count = Math.Max(1, text.Length / num);
		return new string((char)c, count);
	}

	private static bool AttributeEquals(XAttribute source, XAttribute target)
	{
		if (source == null && target == null)
		{
			return true;
		}
		if (source == null || target == null)
		{
			return false;
		}
		if (source.Name == target.Name)
		{
			return source.Value == target.Value;
		}
		return false;
	}

	private static void AddContents<T>(Queue<T> pendingComments, Action<T> action)
	{
		while (pendingComments.Count > 0)
		{
			action(pendingComments.Dequeue());
		}
	}
}
