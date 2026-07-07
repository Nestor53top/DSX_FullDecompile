using System;
using System.Security.Cryptography;
using System.Text;

namespace NuGet;

internal static class EncryptionUtility
{
	private static readonly byte[] _entropyBytes = Encoding.UTF8.GetBytes("NuGet");

	internal static string EncryptString(string value)
	{
		return Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(value), _entropyBytes, (DataProtectionScope)0));
	}

	internal static string DecryptString(string encryptedString)
	{
		byte[] bytes = ProtectedData.Unprotect(Convert.FromBase64String(encryptedString), _entropyBytes, (DataProtectionScope)0);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string GenerateUniqueToken(string caseInsensitiveKey)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(caseInsensitiveKey.ToUpperInvariant());
		return Convert.ToBase64String(new CryptoHashProvider("SHA256").CalculateHash(bytes)).ToUpperInvariant();
	}
}
