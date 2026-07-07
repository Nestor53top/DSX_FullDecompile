using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlAttributePreservingWriter : XmlWriter
{
	private class AttributeTextWriter : TextWriter
	{
		private enum State
		{
			Writing,
			WaitingForAttributeLeadingSpace,
			ReadingAttribute,
			Buffering,
			FlushingBuffer,
			WritingComment
		}

		private State state;

		private StringBuilder writeBuffer;

		private TextWriter baseWriter;

		private string leadingWhitespace;

		private int lineNumber = 1;

		private int linePosition = 1;

		private int maxLineLength = 160;

		private string newLineString = "\r\n";

		public string AttributeLeadingWhitespace
		{
			set
			{
				leadingWhitespace = value;
			}
		}

		public string AttributeNewLineString
		{
			get
			{
				return newLineString;
			}
			set
			{
				newLineString = value;
			}
		}

		public int MaxLineLength
		{
			get
			{
				return maxLineLength;
			}
			set
			{
				maxLineLength = value;
			}
		}

		public override Encoding Encoding => baseWriter.Encoding;

		public AttributeTextWriter(TextWriter baseWriter)
			: base(CultureInfo.InvariantCulture)
		{
			this.baseWriter = baseWriter;
		}

		public void StartAttribute()
		{
			ChangeState(State.WaitingForAttributeLeadingSpace);
		}

		public void EndAttribute()
		{
			WriteQueuedAttribute();
		}

		public void StartComment()
		{
			ChangeState(State.WritingComment);
		}

		public void EndComment()
		{
			ChangeState(State.Writing);
		}

		public override void Write(char value)
		{
			UpdateState(value);
			switch (state)
			{
			case State.WaitingForAttributeLeadingSpace:
				if (value == ' ')
				{
					ChangeState(State.ReadingAttribute);
					break;
				}
				goto case State.Writing;
			case State.Writing:
			case State.FlushingBuffer:
			case State.WritingComment:
				ReallyWriteCharacter(value);
				break;
			case State.ReadingAttribute:
			case State.Buffering:
				writeBuffer.Append(value);
				break;
			}
		}

		private void UpdateState(char value)
		{
			if (state == State.WritingComment)
			{
				return;
			}
			switch (value)
			{
			case ' ':
				if (state == State.Writing)
				{
					ChangeState(State.Buffering);
				}
				break;
			case '>':
				if (state == State.Buffering)
				{
					string text = writeBuffer.ToString();
					if (text.EndsWith(" /", StringComparison.Ordinal))
					{
						writeBuffer.Remove(text.LastIndexOf(' '), 1);
					}
					ChangeState(State.Writing);
				}
				break;
			default:
				if (state == State.Buffering)
				{
					ChangeState(State.Writing);
				}
				break;
			case '/':
				break;
			}
		}

		private void ChangeState(State newState)
		{
			if (this.state != newState)
			{
				State state = this.state;
				this.state = newState;
				if (StateRequiresBuffer(newState))
				{
					CreateBuffer();
				}
				else if (StateRequiresBuffer(state))
				{
					FlushBuffer();
				}
			}
		}

		private bool StateRequiresBuffer(State state)
		{
			if (state != State.Buffering)
			{
				return state == State.ReadingAttribute;
			}
			return true;
		}

		private void CreateBuffer()
		{
			if (writeBuffer == null)
			{
				writeBuffer = new StringBuilder();
			}
		}

		private void FlushBuffer()
		{
			if (writeBuffer != null)
			{
				State state = this.state;
				try
				{
					this.state = State.FlushingBuffer;
					Write(writeBuffer.ToString());
					writeBuffer = null;
				}
				finally
				{
					this.state = state;
				}
			}
		}

		private void ReallyWriteCharacter(char value)
		{
			baseWriter.Write(value);
			if (value == '\n')
			{
				lineNumber++;
				linePosition = 1;
			}
			else
			{
				linePosition++;
			}
		}

		private void WriteQueuedAttribute()
		{
			if (leadingWhitespace != null)
			{
				writeBuffer.Insert(0, leadingWhitespace);
				leadingWhitespace = null;
			}
			else
			{
				int num = linePosition + writeBuffer.Length + 1;
				if (num > MaxLineLength)
				{
					writeBuffer.Insert(0, AttributeNewLineString);
				}
				else
				{
					writeBuffer.Insert(0, ' ');
				}
			}
			ChangeState(State.Writing);
		}

		public override void Flush()
		{
			baseWriter.Flush();
		}

		public override void Close()
		{
			baseWriter.Close();
		}
	}

	private XmlTextWriter xmlWriter;

	private AttributeTextWriter textWriter;

	public override WriteState WriteState => ((XmlWriter)xmlWriter).WriteState;

	public XmlAttributePreservingWriter(string fileName, Encoding encoding)
		: this((encoding == null) ? new StreamWriter(fileName) : new StreamWriter(fileName, append: false, encoding))
	{
	}

	public XmlAttributePreservingWriter(Stream w, Encoding encoding)
		: this((encoding == null) ? new StreamWriter(w) : new StreamWriter(w, encoding))
	{
	}

	public XmlAttributePreservingWriter(TextWriter textWriter)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		this.textWriter = new AttributeTextWriter(textWriter);
		xmlWriter = new XmlTextWriter((TextWriter)this.textWriter);
	}

	public void WriteAttributeWhitespace(string whitespace)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if ((int)((XmlWriter)this).WriteState == 3)
		{
			((XmlWriter)this).WriteEndAttribute();
		}
		else if ((int)((XmlWriter)this).WriteState != 2)
		{
			throw new InvalidOperationException();
		}
		textWriter.AttributeLeadingWhitespace = whitespace;
	}

	public void WriteAttributeTrailingWhitespace(string whitespace)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if ((int)((XmlWriter)this).WriteState == 3)
		{
			((XmlWriter)this).WriteEndAttribute();
		}
		else if ((int)((XmlWriter)this).WriteState != 2)
		{
			throw new InvalidOperationException();
		}
		textWriter.Write(whitespace);
	}

	public string SetAttributeNewLineString(string newLineString)
	{
		string attributeNewLineString = textWriter.AttributeNewLineString;
		if (newLineString == null && ((XmlWriter)xmlWriter).Settings != null)
		{
			newLineString = ((XmlWriter)xmlWriter).Settings.NewLineChars;
		}
		if (newLineString == null)
		{
			newLineString = "\r\n";
		}
		textWriter.AttributeNewLineString = newLineString;
		return attributeNewLineString;
	}

	private bool IsOnlyWhitespace(string whitespace)
	{
		foreach (char c in whitespace)
		{
			if (!char.IsWhiteSpace(c))
			{
				return false;
			}
		}
		return true;
	}

	public override void Close()
	{
		((XmlWriter)xmlWriter).Close();
	}

	public override void Flush()
	{
		((XmlWriter)xmlWriter).Flush();
	}

	public override string LookupPrefix(string ns)
	{
		return ((XmlWriter)xmlWriter).LookupPrefix(ns);
	}

	public override void WriteBase64(byte[] buffer, int index, int count)
	{
		((XmlWriter)xmlWriter).WriteBase64(buffer, index, count);
	}

	public override void WriteCData(string text)
	{
		((XmlWriter)xmlWriter).WriteCData(text);
	}

	public override void WriteCharEntity(char ch)
	{
		((XmlWriter)xmlWriter).WriteCharEntity(ch);
	}

	public override void WriteChars(char[] buffer, int index, int count)
	{
		((XmlWriter)xmlWriter).WriteChars(buffer, index, count);
	}

	public override void WriteComment(string text)
	{
		textWriter.StartComment();
		((XmlWriter)xmlWriter).WriteComment(text);
		textWriter.EndComment();
	}

	public override void WriteDocType(string name, string pubid, string sysid, string subset)
	{
		((XmlWriter)xmlWriter).WriteDocType(name, pubid, sysid, subset);
	}

	public override void WriteEndAttribute()
	{
		((XmlWriter)xmlWriter).WriteEndAttribute();
		textWriter.EndAttribute();
	}

	public override void WriteEndDocument()
	{
		((XmlWriter)xmlWriter).WriteEndDocument();
	}

	public override void WriteEndElement()
	{
		((XmlWriter)xmlWriter).WriteEndElement();
	}

	public override void WriteEntityRef(string name)
	{
		((XmlWriter)xmlWriter).WriteEntityRef(name);
	}

	public override void WriteFullEndElement()
	{
		((XmlWriter)xmlWriter).WriteFullEndElement();
	}

	public override void WriteProcessingInstruction(string name, string text)
	{
		((XmlWriter)xmlWriter).WriteProcessingInstruction(name, text);
	}

	public override void WriteRaw(string data)
	{
		((XmlWriter)xmlWriter).WriteRaw(data);
	}

	public override void WriteRaw(char[] buffer, int index, int count)
	{
		((XmlWriter)xmlWriter).WriteRaw(buffer, index, count);
	}

	public override void WriteStartAttribute(string prefix, string localName, string ns)
	{
		textWriter.StartAttribute();
		((XmlWriter)xmlWriter).WriteStartAttribute(prefix, localName, ns);
	}

	public override void WriteStartDocument(bool standalone)
	{
		((XmlWriter)xmlWriter).WriteStartDocument(standalone);
	}

	public override void WriteStartDocument()
	{
		((XmlWriter)xmlWriter).WriteStartDocument();
	}

	public override void WriteStartElement(string prefix, string localName, string ns)
	{
		((XmlWriter)xmlWriter).WriteStartElement(prefix, localName, ns);
	}

	public override void WriteString(string text)
	{
		((XmlWriter)xmlWriter).WriteString(text);
	}

	public override void WriteSurrogateCharEntity(char lowChar, char highChar)
	{
		((XmlWriter)xmlWriter).WriteSurrogateCharEntity(lowChar, highChar);
	}

	public override void WriteWhitespace(string ws)
	{
		((XmlWriter)xmlWriter).WriteWhitespace(ws);
	}
}
