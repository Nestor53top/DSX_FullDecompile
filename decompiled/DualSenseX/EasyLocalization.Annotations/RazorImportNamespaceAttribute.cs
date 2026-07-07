using System;

namespace EasyLocalization.Annotations;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class RazorImportNamespaceAttribute : Attribute
{
	[NotNull]
	public string Name { get; private set; }

	public RazorImportNamespaceAttribute([NotNull] string name)
	{
		Name = name;
	}
}
