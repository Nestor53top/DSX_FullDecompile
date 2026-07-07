using System;

namespace EasyLocalization.Annotations;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
public sealed class AspMvcPartialViewAttribute : Attribute
{
}
