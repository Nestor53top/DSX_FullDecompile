namespace Microsoft.AppCenter.Storage;

internal class StorageFullException : StorageException
{
	private const string DefaultMessage = "The database is full.";

	public StorageFullException()
		: base("The database is full.")
	{
	}

	public StorageFullException(string message)
		: base(message)
	{
	}
}
