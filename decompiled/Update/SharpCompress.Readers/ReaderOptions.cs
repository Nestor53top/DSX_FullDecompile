using SharpCompress.Common;

namespace SharpCompress.Readers;

internal class ReaderOptions : OptionsBase
{
	public bool LookForHeader { get; set; }

	public string Password { get; set; }
}
