namespace SharpCompress.Readers;

public class ExtractionOptions
{
	public bool Overwrite { get; set; }

	public bool ExtractFullPath { get; set; }

	public bool PreserveFileTime { get; set; }

	public bool PreserveAttributes { get; set; }
}
