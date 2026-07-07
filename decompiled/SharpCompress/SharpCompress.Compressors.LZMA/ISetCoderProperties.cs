namespace SharpCompress.Compressors.LZMA;

internal interface ISetCoderProperties
{
	void SetCoderProperties(CoderPropID[] propIDs, object[] properties);
}
