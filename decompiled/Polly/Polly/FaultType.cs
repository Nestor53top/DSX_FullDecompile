namespace Polly;

public enum FaultType
{
	ExceptionHandledByThisPolicy,
	UnhandledException,
	ResultHandledByThisPolicy
}
