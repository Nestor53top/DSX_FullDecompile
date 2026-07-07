namespace Microsoft.AppCenter.Storage;

internal class StorageCorruptedException : StorageException
{
	private const string DefaultMessage = "The database disk image is malformed.";

	public StorageCorruptedException()
		: base("The database disk image is malformed.")
	{
	}

	public StorageCorruptedException(string message)
		: base(message)
	{
	}
}
