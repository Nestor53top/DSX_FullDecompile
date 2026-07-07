namespace SharpCompress.Compressors.Deflate;

internal enum FlushType
{
	None,
	Partial,
	Sync,
	Full,
	Finish
}
