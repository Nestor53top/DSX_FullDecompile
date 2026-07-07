using System.IO;
using System.Text;

namespace Microsoft.Web.XmlTransform;

internal class WhitespaceTrackingTextReader(TextReader reader) : PositionTrackingTextReader(reader)
{
	private StringBuilder precedingWhitespace = new StringBuilder();

	public string PrecedingWhitespace => precedingWhitespace.ToString();

	public override int Read()
	{
		int num = base.Read();
		UpdateWhitespaceTracking(num);
		return num;
	}

	private void UpdateWhitespaceTracking(int character)
	{
		if (char.IsWhiteSpace((char)character))
		{
			AppendWhitespaceCharacter(character);
		}
		else
		{
			ResetWhitespaceString();
		}
	}

	private void AppendWhitespaceCharacter(int character)
	{
		precedingWhitespace.Append((char)character);
	}

	private void ResetWhitespaceString()
	{
		precedingWhitespace = new StringBuilder();
	}
}
