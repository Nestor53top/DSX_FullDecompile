using System.Management;

namespace Microsoft.AppCenter;

internal interface IManagmentClassFactory
{
	ManagementClass GetComputerSystemClass();

	ManagementClass GetOperatingSystemClass();
}
