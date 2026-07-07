using SharpCompress.Common;

namespace SharpCompress.Readers;

public class ReaderOptions : OptionsBase
{
	public bool LookForHeader { get; set; }

	public string Password { get; set; }
}
