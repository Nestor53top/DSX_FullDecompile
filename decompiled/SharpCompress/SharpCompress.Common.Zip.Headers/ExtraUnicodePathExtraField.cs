using System;
using System.Text;

namespace SharpCompress.Common.Zip.Headers;

internal class ExtraUnicodePathExtraField : ExtraData
{
	internal byte Version => base.DataBytes[0];

	internal byte[] NameCRC32
	{
		get
		{
			byte[] array = new byte[4];
			Buffer.BlockCopy(base.DataBytes, 1, array, 0, 4);
			return array;
		}
	}

	internal string UnicodeName
	{
		get
		{
			int count = base.Length - 5;
			return Encoding.UTF8.GetString(base.DataBytes, 5, count);
		}
	}
}
