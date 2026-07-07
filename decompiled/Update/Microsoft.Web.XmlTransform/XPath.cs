using System;

namespace Microsoft.Web.XmlTransform;

internal sealed class XPath : Locator
{
	protected override string ParentPath => ConstructPath();

	protected override string ConstructPath()
	{
		EnsureArguments(1, 1);
		string text = base.Arguments[0];
		if (!text.StartsWith("/", StringComparison.Ordinal))
		{
			text = AppendStep(base.ParentPath, NextStepNodeTest);
			text = AppendStep(text, base.Arguments[0]);
			text = text.Replace("/./", "/");
		}
		return text;
	}
}
