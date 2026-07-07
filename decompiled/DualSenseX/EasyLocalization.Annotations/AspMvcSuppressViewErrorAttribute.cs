using System;

namespace EasyLocalization.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AspMvcSuppressViewErrorAttribute : Attribute
{
}
