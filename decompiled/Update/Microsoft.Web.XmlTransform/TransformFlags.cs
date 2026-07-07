using System;

namespace Microsoft.Web.XmlTransform;

[Flags]
internal enum TransformFlags
{
	None = 0,
	ApplyTransformToAllTargetNodes = 1,
	UseParentAsTargetNode = 2
}
