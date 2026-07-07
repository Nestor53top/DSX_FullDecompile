using System;
using System.Security.Cryptography;
using SharpCompress.Converters;

namespace SharpCompress.Common.Zip;

internal class WinzipAesEncryptionData
{
	private const int RFC2898_ITERATIONS = 1000;

	private byte[] salt;

	private WinzipAesKeySize keySize;

	private byte[] passwordVerifyValue;

	private string password;

	private byte[] generatedVerifyValue;

	internal byte[] IvBytes { get; set; }

	internal byte[] KeyBytes { get; set; }

	private int KeySizeInBytes => KeyLengthInBytes(keySize);

	internal WinzipAesEncryptionData(WinzipAesKeySize keySize, byte[] salt, byte[] passwordVerifyValue, string password)
	{
		this.keySize = keySize;
		this.salt = salt;
		this.passwordVerifyValue = passwordVerifyValue;
		this.password = password;
		Initialize();
	}

	internal static int KeyLengthInBytes(WinzipAesKeySize keySize)
	{
		return keySize switch
		{
			WinzipAesKeySize.KeySize128 => 16, 
			WinzipAesKeySize.KeySize192 => 24, 
			WinzipAesKeySize.KeySize256 => 32, 
			_ => throw new InvalidOperationException(), 
		};
	}

	private void Initialize()
	{
		Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, 1000);
		KeyBytes = rfc2898DeriveBytes.GetBytes(KeySizeInBytes);
		IvBytes = rfc2898DeriveBytes.GetBytes(KeySizeInBytes);
		generatedVerifyValue = rfc2898DeriveBytes.GetBytes(2);
		short @int = DataConverter.LittleEndian.GetInt16(passwordVerifyValue, 0);
		if (password != null)
		{
			short int2 = DataConverter.LittleEndian.GetInt16(generatedVerifyValue, 0);
			if (@int != int2)
			{
				throw new InvalidFormatException("bad password");
			}
		}
	}
}
