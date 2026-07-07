using System;

namespace NuGet;

public interface IOperationAwareRepository
{
	IDisposable StartOperation(string operation, string mainPackageId, string mainPackageVersion);
}
