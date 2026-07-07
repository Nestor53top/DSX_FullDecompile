using System;
using System.IO;
using System.Text;

namespace Microsoft.Web.XmlTransform;

internal class XmlAttributePreservationProvider : IDisposable
{
	private StreamReader streamReader;

	private PositionTrackingTextReader reader;

	public XmlAttributePreservationProvider(string fileName)
	{
		streamReader = new StreamReader(File.OpenRead(fileName));
		reader = new PositionTrackingTextReader(streamReader);
	}

	public XmlAttributePreservationDict GetDictAtPosition(int lineNumber, int linePosition)
	{
		if (reader.ReadToPosition(lineNumber, linePosition))
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int num;
			do
			{
				num = reader.Read();
				if (num == 34)
				{
					flag = !flag;
				}
				stringBuilder.Append((char)num);
			}
			while (num > 0 && ((ushort)num != 62 || flag));
			if (num > 0)
			{
				XmlAttributePreservationDict xmlAttributePreservationDict = new XmlAttributePreservationDict();
				xmlAttributePreservationDict.ReadPreservationInfo(stringBuilder.ToString());
				return xmlAttributePreservationDict;
			}
		}
		return null;
	}

	public void Close()
	{
		Dispose();
		GC.SuppressFinalize(this);
	}

	public void Dispose()
	{
		if (streamReader != null)
		{
			streamReader.Close();
			streamReader = null;
		}
		if (reader != null)
		{
			reader.Dispose();
			reader = null;
		}
	}

	~XmlAttributePreservationProvider()
	{
		Dispose();
	}
}
