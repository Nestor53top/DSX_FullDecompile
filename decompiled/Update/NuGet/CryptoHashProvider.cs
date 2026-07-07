using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using NuGet.Resources;

namespace NuGet;

internal class CryptoHashProvider : IHashProvider
{
	private const string SHA512HashAlgorithm = "SHA512";

	private const string SHA256HashAlgorithm = "SHA256";

	private readonly string _hashAlgorithm;

	private static bool AllowOnlyFipsAlgorithms => ReadFipsConfigValue();

	public CryptoHashProvider()
		: this(null)
	{
	}

	public CryptoHashProvider(string hashAlgorithm)
	{
		if (string.IsNullOrEmpty(hashAlgorithm))
		{
			hashAlgorithm = "SHA512";
		}
		else if (!hashAlgorithm.Equals("SHA512", StringComparison.OrdinalIgnoreCase) && !hashAlgorithm.Equals("SHA256", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, NuGetResources.UnsupportedHashAlgorithm, new object[1] { hashAlgorithm }), "hashAlgorithm");
		}
		_hashAlgorithm = hashAlgorithm;
	}

	public byte[] CalculateHash(Stream stream)
	{
		using HashAlgorithm hashAlgorithm = GetHashAlgorithm();
		return hashAlgorithm.ComputeHash(stream);
	}

	public byte[] CalculateHash(byte[] data)
	{
		using HashAlgorithm hashAlgorithm = GetHashAlgorithm();
		return hashAlgorithm.ComputeHash(data);
	}

	public bool VerifyHash(byte[] data, byte[] hash)
	{
		return Enumerable.SequenceEqual(CalculateHash(data), hash);
	}

	private HashAlgorithm GetHashAlgorithm()
	{
		if (_hashAlgorithm.Equals("SHA256", StringComparison.OrdinalIgnoreCase))
		{
			if (!AllowOnlyFipsAlgorithms)
			{
				return new SHA256Managed();
			}
			return new SHA256CryptoServiceProvider();
		}
		if (!AllowOnlyFipsAlgorithms)
		{
			return new SHA512Managed();
		}
		return new SHA512CryptoServiceProvider();
	}

	private static bool ReadFipsConfigValue()
	{
		Type typeFromHandle = typeof(CryptoConfig);
		if (typeFromHandle != null)
		{
			PropertyInfo property = typeFromHandle.GetProperty("AllowOnlyFipsAlgorithms", BindingFlags.Static | BindingFlags.Public);
			if (property != null)
			{
				return (bool)property.GetValue(null, null);
			}
		}
		return false;
	}
}
