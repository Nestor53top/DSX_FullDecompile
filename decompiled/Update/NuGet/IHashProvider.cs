using System.IO;

namespace NuGet;

internal interface IHashProvider
{
	byte[] CalculateHash(Stream stream);

	byte[] CalculateHash(byte[] data);

	bool VerifyHash(byte[] data, byte[] hash);
}
