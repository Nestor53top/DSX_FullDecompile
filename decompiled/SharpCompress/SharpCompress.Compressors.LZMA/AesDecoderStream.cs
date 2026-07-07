using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SharpCompress.Compressors.LZMA.Utilites;

namespace SharpCompress.Compressors.LZMA;

internal class AesDecoderStream : DecoderStream2
{
	private readonly Stream mStream;

	private readonly ICryptoTransform mDecoder;

	private readonly byte[] mBuffer;

	private long mWritten;

	private readonly long mLimit;

	private int mOffset;

	private int mEnding;

	private int mUnderflow;

	private bool isDisposed;

	public override long Position => mWritten;

	public override long Length => mLimit;

	public AesDecoderStream(Stream input, byte[] info, IPasswordProvider pass, long limit)
	{
		mStream = input;
		mLimit = limit;
		if (((int)input.Length & 0xF) != 0)
		{
			throw new NotSupportedException("AES decoder does not support padding.");
		}
		Init(info, out var numCyclesPower, out var salt, out var iv);
		byte[] bytes = Encoding.Unicode.GetBytes(pass.CryptoGetTextPassword());
		byte[] rgbKey = InitKey(numCyclesPower, salt, bytes);
		using (Aes aes = Aes.Create())
		{
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.None;
			mDecoder = aes.CreateDecryptor(rgbKey, iv);
		}
		mBuffer = new byte[4096];
	}

	protected override void Dispose(bool disposing)
	{
		try
		{
			if (!isDisposed)
			{
				isDisposed = true;
				if (disposing)
				{
					mStream.Dispose();
					mDecoder.Dispose();
				}
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (count == 0 || mWritten == mLimit)
		{
			return 0;
		}
		if (mUnderflow > 0)
		{
			return HandleUnderflow(buffer, offset, count);
		}
		if (mEnding - mOffset < 16)
		{
			Buffer.BlockCopy(mBuffer, mOffset, mBuffer, 0, mEnding - mOffset);
			mEnding -= mOffset;
			mOffset = 0;
			do
			{
				int num = mStream.Read(mBuffer, mEnding, mBuffer.Length - mEnding);
				if (num == 0)
				{
					throw new EndOfStreamException();
				}
				mEnding += num;
			}
			while (mEnding - mOffset < 16);
		}
		if (count > mLimit - mWritten)
		{
			count = (int)(mLimit - mWritten);
		}
		if (count < 16)
		{
			return HandleUnderflow(buffer, offset, count);
		}
		if (count > mEnding - mOffset)
		{
			count = mEnding - mOffset;
		}
		int num2 = mDecoder.TransformBlock(mBuffer, mOffset, count & -16, buffer, offset);
		mOffset += num2;
		mWritten += num2;
		return num2;
	}

	private void Init(byte[] info, out int numCyclesPower, out byte[] salt, out byte[] iv)
	{
		byte b = info[0];
		numCyclesPower = b & 0x3F;
		if ((b & 0xC0) == 0)
		{
			salt = new byte[0];
			iv = new byte[0];
			return;
		}
		int num = (b >> 7) & 1;
		int num2 = (b >> 6) & 1;
		if (info.Length == 1)
		{
			throw new InvalidOperationException();
		}
		byte b2 = info[1];
		num += b2 >> 4;
		num2 += b2 & 0xF;
		if (info.Length < 2 + num + num2)
		{
			throw new InvalidOperationException();
		}
		salt = new byte[num];
		for (int i = 0; i < num; i++)
		{
			salt[i] = info[i + 2];
		}
		iv = new byte[16];
		for (int j = 0; j < num2; j++)
		{
			iv[j] = info[j + num + 2];
		}
		if (numCyclesPower <= 24)
		{
			return;
		}
		throw new NotSupportedException();
	}

	private byte[] InitKey(int mNumCyclesPower, byte[] salt, byte[] pass)
	{
		if (mNumCyclesPower == 63)
		{
			byte[] array = new byte[32];
			int i;
			for (i = 0; i < salt.Length; i++)
			{
				array[i] = salt[i];
			}
			for (int j = 0; j < pass.Length; j++)
			{
				if (i >= 32)
				{
					break;
				}
				array[i++] = pass[j];
			}
			return array;
		}
		using SHA256 sHA = SHA256.Create();
		byte[] array2 = new byte[8];
		long num = 1L << mNumCyclesPower;
		for (long num2 = 0L; num2 < num; num2++)
		{
			sHA.TransformBlock(salt, 0, salt.Length, null, 0);
			sHA.TransformBlock(pass, 0, pass.Length, null, 0);
			sHA.TransformBlock(array2, 0, 8, null, 0);
			for (int k = 0; k < 8; k++)
			{
				if (++array2[k] != 0)
				{
					break;
				}
			}
		}
		sHA.TransformFinalBlock(array2, 0, 0);
		return sHA.Hash;
	}

	private int HandleUnderflow(byte[] buffer, int offset, int count)
	{
		if (mUnderflow == 0)
		{
			int inputCount = (mEnding - mOffset) & -16;
			mUnderflow = mDecoder.TransformBlock(mBuffer, mOffset, inputCount, mBuffer, mOffset);
		}
		if (count > mUnderflow)
		{
			count = mUnderflow;
		}
		Buffer.BlockCopy(mBuffer, mOffset, buffer, offset, count);
		mWritten += count;
		mOffset += count;
		mUnderflow -= count;
		return count;
	}
}
