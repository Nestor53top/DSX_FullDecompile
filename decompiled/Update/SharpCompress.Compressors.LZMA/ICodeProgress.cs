namespace SharpCompress.Compressors.LZMA;

internal interface ICodeProgress
{
	void SetProgress(long inSize, long outSize);
}
