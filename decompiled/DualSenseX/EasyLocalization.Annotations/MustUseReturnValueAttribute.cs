using System;

namespace EasyLocalization.Annotations;

[AttributeUsage(AttributeTargets.Method)]
public sealed class MustUseReturnValueAttribute : Attribute
{
	[CanBeNull]
	public string Justification { get; private set; }

	public MustUseReturnValueAttribute()
	{
	}

	public MustUseReturnValueAttribute([NotNull] string justification)
	{
		Justification = justification;
	}
}
