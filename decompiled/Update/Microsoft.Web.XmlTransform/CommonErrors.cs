using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal static class CommonErrors
{
	internal static void ExpectNoArguments(XmlTransformationLogger log, string transformName, string argumentString)
	{
		if (!string.IsNullOrEmpty(argumentString))
		{
			log.LogWarning(SR.XMLTRANSFORMATION_TransformDoesNotExpectArguments, new object[1] { transformName });
		}
	}

	internal static void WarnIfMultipleTargets(XmlTransformationLogger log, string transformName, XmlNodeList targetNodes, bool applyTransformToAllTargets)
	{
		if (targetNodes.Count > 1)
		{
			log.LogWarning(SR.XMLTRANSFORMATION_TransformOnlyAppliesOnce, new object[1] { transformName });
		}
	}
}
