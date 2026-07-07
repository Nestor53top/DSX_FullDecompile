namespace SharpCompress.Compressors.Xz;

public enum CheckType : byte
{
	NONE = 0,
	CRC32 = 1,
	CRC64 = 4,
	SHA256 = 10
}
