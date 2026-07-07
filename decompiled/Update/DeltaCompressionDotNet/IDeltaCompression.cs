namespace DeltaCompressionDotNet;

internal interface IDeltaCompression
{
	void CreateDelta(string oldFilePath, string newFilePath, string deltaFilePath);

	void ApplyDelta(string deltaFilePath, string oldFilePath, string newFilePath);
}
