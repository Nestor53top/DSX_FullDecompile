using System.Management;

namespace Microsoft.AppCenter;

internal class ManagmentClassFactory : IManagmentClassFactory
{
	private static ManagmentClassFactory _instanceField;

	internal static ManagmentClassFactory Instance => _instanceField ?? (_instanceField = new ManagmentClassFactory());

	private ManagmentClassFactory()
	{
	}

	public ManagementClass GetComputerSystemClass()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		return new ManagementClass("Win32_ComputerSystem");
	}

	public ManagementClass GetOperatingSystemClass()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		return new ManagementClass("Win32_OperatingSystem");
	}
}
