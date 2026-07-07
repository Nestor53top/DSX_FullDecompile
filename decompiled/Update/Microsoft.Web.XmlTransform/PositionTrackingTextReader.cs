using System.IO;

namespace Microsoft.Web.XmlTransform;

internal class PositionTrackingTextReader : TextReader
{
	private const int newlineCharacter = 10;

	private TextReader internalReader;

	private int lineNumber = 1;

	private int linePosition = 1;

	private int characterPosition = 1;

	public PositionTrackingTextReader(TextReader textReader)
	{
		internalReader = textReader;
	}

	public override int Read()
	{
		int num = internalReader.Read();
		UpdatePosition(num);
		return num;
	}

	public override int Peek()
	{
		return internalReader.Peek();
	}

	public bool ReadToPosition(int lineNumber, int linePosition)
	{
		while (this.lineNumber < lineNumber && Peek() != -1)
		{
			ReadLine();
		}
		while (this.linePosition < linePosition && Peek() != -1)
		{
			Read();
		}
		if (this.lineNumber == lineNumber)
		{
			return this.linePosition == linePosition;
		}
		return false;
	}

	public bool ReadToPosition(int characterPosition)
	{
		while (this.characterPosition < characterPosition && Peek() != -1)
		{
			Read();
		}
		return this.characterPosition == characterPosition;
	}

	private void UpdatePosition(int character)
	{
		if (character == 10)
		{
			lineNumber++;
			linePosition = 1;
		}
		else
		{
			linePosition++;
		}
		characterPosition++;
	}
}
