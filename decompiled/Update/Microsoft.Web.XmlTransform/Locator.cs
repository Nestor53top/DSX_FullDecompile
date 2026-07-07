using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal abstract class Locator
{
	private string argumentString;

	private IList<string> arguments;

	private string parentPath;

	private XmlElementContext context;

	private XmlTransformationLogger logger;

	protected virtual string ParentPath => parentPath;

	protected XmlNode CurrentElement => (XmlNode)(object)context.Element;

	protected virtual string NextStepNodeTest
	{
		get
		{
			if (!string.IsNullOrEmpty(CurrentElement.NamespaceURI) && string.IsNullOrEmpty(CurrentElement.Prefix))
			{
				return "_defaultNamespace:" + CurrentElement.LocalName;
			}
			return CurrentElement.Name;
		}
	}

	protected virtual XPathAxis NextStepAxis => XPathAxis.Child;

	protected XmlTransformationLogger Log
	{
		get
		{
			if (logger == null)
			{
				logger = context.GetService<XmlTransformationLogger>();
				if (logger != null)
				{
					logger.CurrentReferenceNode = (XmlNode)(object)context.LocatorAttribute;
				}
			}
			return logger;
		}
	}

	protected string ArgumentString => argumentString;

	protected IList<string> Arguments
	{
		get
		{
			if (arguments == null && argumentString != null)
			{
				arguments = XmlArgumentUtility.SplitArguments(argumentString);
			}
			return arguments;
		}
	}

	protected virtual string ConstructPath()
	{
		return AppendStep(ParentPath, NextStepAxis, NextStepNodeTest, ConstructPredicate());
	}

	protected string AppendStep(string basePath, string stepNodeTest)
	{
		return AppendStep(basePath, XPathAxis.Child, stepNodeTest, string.Empty);
	}

	protected string AppendStep(string basePath, XPathAxis stepAxis, string stepNodeTest)
	{
		return AppendStep(basePath, stepAxis, stepNodeTest, string.Empty);
	}

	protected string AppendStep(string basePath, string stepNodeTest, string predicate)
	{
		return AppendStep(basePath, XPathAxis.Child, stepNodeTest, predicate);
	}

	protected string AppendStep(string basePath, XPathAxis stepAxis, string stepNodeTest, string predicate)
	{
		return EnsureTrailingSlash(basePath) + GetAxisString(stepAxis) + stepNodeTest + EnsureBracketedPredicate(predicate);
	}

	protected virtual string ConstructPredicate()
	{
		return string.Empty;
	}

	protected void EnsureArguments()
	{
		EnsureArguments(1);
	}

	protected void EnsureArguments(int min)
	{
		if (Arguments == null || Arguments.Count < min)
		{
			throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_RequiresMinimumArguments, new object[2]
			{
				GetType().Name,
				min
			}));
		}
	}

	protected void EnsureArguments(int min, int max)
	{
		if (min == max && (Arguments == null || Arguments.Count != min))
		{
			throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_RequiresExactArguments, new object[2]
			{
				GetType().Name,
				min
			}));
		}
		EnsureArguments(min);
		if (Arguments.Count > max)
		{
			throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_TooManyArguments, new object[1] { GetType().Name }));
		}
	}

	internal string ConstructPath(string parentPath, XmlElementContext context, string argumentString)
	{
		string result = string.Empty;
		if (this.parentPath == null && this.context == null && this.argumentString == null)
		{
			try
			{
				this.parentPath = parentPath;
				this.context = context;
				this.argumentString = argumentString;
				result = ConstructPath();
			}
			finally
			{
				this.parentPath = null;
				this.context = null;
				this.argumentString = null;
				arguments = null;
				ReleaseLogger();
			}
		}
		return result;
	}

	internal string ConstructParentPath(string parentPath, XmlElementContext context, string argumentString)
	{
		string empty = string.Empty;
		if (this.parentPath == null && this.context == null && this.argumentString == null)
		{
			try
			{
				this.parentPath = parentPath;
				this.context = context;
				this.argumentString = argumentString;
				empty = ParentPath;
			}
			finally
			{
				this.parentPath = null;
				this.context = null;
				this.argumentString = null;
				arguments = null;
				ReleaseLogger();
			}
		}
		return empty;
	}

	private void ReleaseLogger()
	{
		if (logger != null)
		{
			logger.CurrentReferenceNode = null;
			logger = null;
		}
	}

	private string GetAxisString(XPathAxis stepAxis)
	{
		return stepAxis switch
		{
			XPathAxis.Child => string.Empty, 
			XPathAxis.Descendant => "descendant::", 
			XPathAxis.Parent => "parent::", 
			XPathAxis.Ancestor => "ancestor::", 
			XPathAxis.FollowingSibling => "following-sibling::", 
			XPathAxis.PrecedingSibling => "preceding-sibling::", 
			XPathAxis.Following => "following::", 
			XPathAxis.Preceding => "preceding::", 
			XPathAxis.Self => "self::", 
			XPathAxis.DescendantOrSelf => "/", 
			XPathAxis.AncestorOrSelf => "ancestor-or-self::", 
			_ => string.Empty, 
		};
	}

	private string EnsureTrailingSlash(string basePath)
	{
		if (!basePath.EndsWith("/", StringComparison.Ordinal))
		{
			basePath += "/";
		}
		return basePath;
	}

	private string EnsureBracketedPredicate(string predicate)
	{
		if (string.IsNullOrEmpty(predicate))
		{
			return string.Empty;
		}
		if (!predicate.StartsWith("[", StringComparison.Ordinal))
		{
			predicate = "[" + predicate;
		}
		if (!predicate.EndsWith("]", StringComparison.Ordinal))
		{
			predicate += "]";
		}
		return predicate;
	}
}
