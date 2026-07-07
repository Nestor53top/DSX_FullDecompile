using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlFileInfoDocument : XmlDocument, IDisposable
{
	private class XmlFileInfoElement : XmlElement, IXmlLineInfo, IXmlFormattableAttributes
	{
		private int lineNumber;

		private int linePosition;

		private bool isOriginal;

		private XmlAttributePreservationDict preservationDict;

		public int LineNumber => lineNumber;

		public int LinePosition => linePosition;

		public bool IsOriginal => isOriginal;

		string IXmlFormattableAttributes.AttributeIndent => preservationDict.GetAttributeNewLineString(null);

		internal XmlFileInfoElement(string prefix, string localName, string namespaceUri, XmlFileInfoDocument document)
			: base(prefix, localName, namespaceUri, (XmlDocument)(object)document)
		{
			lineNumber = document.CurrentLineNumber;
			linePosition = document.CurrentLinePosition;
			isOriginal = document.FirstLoad;
			if (document.PreservationProvider != null)
			{
				preservationDict = document.PreservationProvider.GetDictAtPosition(lineNumber, linePosition - 1);
			}
			if (preservationDict == null)
			{
				preservationDict = new XmlAttributePreservationDict();
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			string text = ((XmlNode)this).Prefix;
			if (!string.IsNullOrEmpty(((XmlNode)this).NamespaceURI))
			{
				text = w.LookupPrefix(((XmlNode)this).NamespaceURI);
				if (text == null)
				{
					text = ((XmlNode)this).Prefix;
				}
			}
			w.WriteStartElement(text, ((XmlNode)this).LocalName, ((XmlNode)this).NamespaceURI);
			if (!(w is XmlAttributePreservingWriter preservingWriter) || preservationDict == null)
			{
				WriteAttributesTo(w);
			}
			else
			{
				WritePreservedAttributesTo(preservingWriter);
			}
			if (((XmlElement)this).IsEmpty)
			{
				w.WriteEndElement();
				return;
			}
			((XmlNode)this).WriteContentTo(w);
			w.WriteFullEndElement();
		}

		private void WriteAttributesTo(XmlWriter w)
		{
			XmlAttributeCollection attributes = ((XmlNode)this).Attributes;
			for (int i = 0; i < ((XmlNamedNodeMap)attributes).Count; i++)
			{
				XmlAttribute val = attributes[i];
				((XmlNode)val).WriteTo(w);
			}
		}

		private void WritePreservedAttributesTo(XmlAttributePreservingWriter preservingWriter)
		{
			preservationDict.WritePreservedAttributes(preservingWriter, ((XmlNode)this).Attributes);
		}

		public bool HasLineInfo()
		{
			return true;
		}

		void IXmlFormattableAttributes.FormatAttributes(XmlFormatter formatter)
		{
			preservationDict.UpdatePreservationInfo(((XmlNode)this).Attributes, formatter);
		}
	}

	private class XmlFileInfoAttribute : XmlAttribute, IXmlLineInfo
	{
		private int lineNumber;

		private int linePosition;

		public int LineNumber => lineNumber;

		public int LinePosition => linePosition;

		internal XmlFileInfoAttribute(string prefix, string localName, string namespaceUri, XmlFileInfoDocument document)
			: base(prefix, localName, namespaceUri, (XmlDocument)(object)document)
		{
			lineNumber = document.CurrentLineNumber;
			linePosition = document.CurrentLinePosition;
		}

		public bool HasLineInfo()
		{
			return true;
		}
	}

	private Encoding _textEncoding;

	private XmlTextReader _reader;

	private XmlAttributePreservationProvider _preservationProvider;

	private bool _firstLoad = true;

	private string _fileName;

	private int _lineNumberOffset;

	private int _linePositionOffset;

	internal bool HasErrorInfo => _reader != null;

	internal string FileName => _fileName;

	private int CurrentLineNumber
	{
		get
		{
			if (_reader == null)
			{
				return 0;
			}
			return _reader.LineNumber + _lineNumberOffset;
		}
	}

	private int CurrentLinePosition
	{
		get
		{
			if (_reader == null)
			{
				return 0;
			}
			return _reader.LinePosition + _linePositionOffset;
		}
	}

	private bool FirstLoad => _firstLoad;

	private XmlAttributePreservationProvider PreservationProvider => _preservationProvider;

	private Encoding TextEncoding
	{
		get
		{
			if (_textEncoding != null)
			{
				return _textEncoding;
			}
			if (((XmlNode)this).HasChildNodes)
			{
				XmlNode firstChild = ((XmlNode)this).FirstChild;
				XmlDeclaration val = (XmlDeclaration)(object)((firstChild is XmlDeclaration) ? firstChild : null);
				if (val != null)
				{
					string encoding = val.Encoding;
					if (encoding.Length > 0)
					{
						return Encoding.GetEncoding(encoding);
					}
				}
			}
			return null;
		}
	}

	public override void Load(string filename)
	{
		LoadFromFileName(filename);
		_firstLoad = false;
	}

	public override void Load(XmlReader reader)
	{
		_reader = (XmlTextReader)(object)((reader is XmlTextReader) ? reader : null);
		if (_reader != null)
		{
			_fileName = ((XmlReader)_reader).BaseURI;
		}
		((XmlDocument)this).Load(reader);
		if (_reader != null)
		{
			_textEncoding = _reader.Encoding;
		}
		_firstLoad = false;
	}

	private void LoadFromFileName(string filename)
	{
		_fileName = filename;
		StreamReader streamReader = null;
		try
		{
			if (((XmlDocument)this).PreserveWhitespace)
			{
				_preservationProvider = new XmlAttributePreservationProvider(filename);
			}
			streamReader = new StreamReader(filename, detectEncodingFromByteOrderMarks: true);
			LoadFromTextReader(streamReader);
		}
		finally
		{
			if (_preservationProvider != null)
			{
				_preservationProvider.Close();
				_preservationProvider = null;
			}
			streamReader?.Close();
		}
	}

	private void LoadFromTextReader(TextReader textReader)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		if (textReader is StreamReader streamReader)
		{
			if (streamReader.BaseStream is FileStream fileStream)
			{
				_fileName = fileStream.Name;
			}
			_textEncoding = GetEncodingFromStream(streamReader.BaseStream);
		}
		_reader = new XmlTextReader(_fileName, textReader);
		((XmlDocument)this).Load((XmlReader)(object)_reader);
		if (_textEncoding == null)
		{
			_textEncoding = _reader.Encoding;
		}
	}

	private Encoding GetEncodingFromStream(Stream stream)
	{
		Encoding result = null;
		if (stream.CanSeek)
		{
			byte[] array = new byte[3];
			stream.Read(array, 0, array.Length);
			if (array[0] == 239 && array[1] == 187 && array[2] == 191)
			{
				result = Encoding.UTF8;
			}
			else if (array[0] == 254 && array[1] == byte.MaxValue)
			{
				result = Encoding.BigEndianUnicode;
			}
			else if (array[0] == byte.MaxValue && array[1] == 254)
			{
				result = Encoding.Unicode;
			}
			else if (array[0] == 43 && array[1] == 47 && array[2] == 118)
			{
				result = Encoding.UTF7;
			}
			stream.Seek(0L, SeekOrigin.Begin);
		}
		return result;
	}

	internal XmlNode CloneNodeFromOtherDocument(XmlNode element)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		XmlTextReader reader = _reader;
		string fileName = _fileName;
		XmlNode result = null;
		try
		{
			IXmlLineInfo val = (IXmlLineInfo)(object)((element is IXmlLineInfo) ? element : null);
			if (val != null)
			{
				_reader = new XmlTextReader((TextReader)new StringReader(element.OuterXml));
				_lineNumberOffset = val.LineNumber - 1;
				_linePositionOffset = val.LinePosition - 2;
				_fileName = ((XmlNode)element.OwnerDocument).BaseURI;
				result = ((XmlDocument)this).ReadNode((XmlReader)(object)_reader);
			}
			else
			{
				_fileName = null;
				_reader = null;
				result = ((XmlDocument)this).ReadNode((XmlReader)new XmlTextReader((TextReader)new StringReader(element.OuterXml)));
			}
		}
		finally
		{
			_lineNumberOffset = 0;
			_linePositionOffset = 0;
			_fileName = fileName;
			_reader = reader;
		}
		return result;
	}

	public override void Save(string filename)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		XmlWriter val = null;
		try
		{
			if (((XmlDocument)this).PreserveWhitespace)
			{
				XmlFormatter.Format((XmlDocument)(object)this);
				val = (XmlWriter)(object)new XmlAttributePreservingWriter(filename, TextEncoding);
			}
			else
			{
				XmlTextWriter val2 = new XmlTextWriter(filename, TextEncoding);
				val2.Formatting = (Formatting)1;
				val = (XmlWriter)(object)val2;
			}
			((XmlNode)this).WriteTo(val);
		}
		finally
		{
			if (val != null)
			{
				val.Flush();
				val.Close();
			}
		}
	}

	public override void Save(Stream w)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		XmlWriter val = null;
		try
		{
			if (((XmlDocument)this).PreserveWhitespace)
			{
				XmlFormatter.Format((XmlDocument)(object)this);
				val = (XmlWriter)(object)new XmlAttributePreservingWriter(w, TextEncoding);
			}
			else
			{
				XmlTextWriter val2 = new XmlTextWriter(w, TextEncoding);
				val2.Formatting = (Formatting)1;
				val = (XmlWriter)(object)val2;
			}
			((XmlNode)this).WriteTo(val);
		}
		finally
		{
			if (val != null)
			{
				val.Flush();
			}
		}
	}

	public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
	{
		if (HasErrorInfo)
		{
			return (XmlElement)(object)new XmlFileInfoElement(prefix, localName, namespaceURI, this);
		}
		return ((XmlDocument)this).CreateElement(prefix, localName, namespaceURI);
	}

	public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
	{
		if (HasErrorInfo)
		{
			return (XmlAttribute)(object)new XmlFileInfoAttribute(prefix, localName, namespaceURI, this);
		}
		return ((XmlDocument)this).CreateAttribute(prefix, localName, namespaceURI);
	}

	internal bool IsNewNode(XmlNode node)
	{
		if (FindContainingElement(node) is XmlFileInfoElement xmlFileInfoElement)
		{
			return !xmlFileInfoElement.IsOriginal;
		}
		return false;
	}

	private XmlElement FindContainingElement(XmlNode node)
	{
		while (node != null && !(node is XmlElement))
		{
			node = node.ParentNode;
		}
		return (XmlElement)(object)((node is XmlElement) ? node : null);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_reader != null)
		{
			((XmlReader)_reader).Close();
			_reader = null;
		}
		if (_preservationProvider != null)
		{
			_preservationProvider.Close();
			_preservationProvider = null;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~XmlFileInfoDocument()
	{
		try
		{
			Dispose(disposing: false);
		}
		finally
		{
			((object)this).Finalize();
		}
	}
}
