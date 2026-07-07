using System;

namespace NuGet;

internal interface IOperationAwareRepository
{
	IDisposable StartOperation(string operation, string mainPackageId, string mainPackageVersion);
}
