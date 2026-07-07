using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace NuGet;

internal static class XmlUtility
{
	public static XDocument LoadSafe(string filePath)
	{
		XmlReaderSettings val = CreateSafeSettings();
		XmlReader val2 = XmlReader.Create(filePath, val);
		try
		{
			return XDocument.Load(val2);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	public static XDocument LoadSafe(Stream input)
	{
		XmlReaderSettings val = CreateSafeSettings();
		return XDocument.Load(XmlReader.Create(input, val));
	}

	public static XDocument LoadSafe(Stream input, bool ignoreWhiteSpace)
	{
		XmlReaderSettings val = CreateSafeSettings(ignoreWhiteSpace);
		return XDocument.Load(XmlReader.Create(input, val));
	}

	public static XDocument LoadSafe(Stream input, LoadOptions options)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		XmlReaderSettings val = CreateSafeSettings();
		return XDocument.Load(XmlReader.Create(input, val), options);
	}

	private static XmlReaderSettings CreateSafeSettings(bool ignoreWhiteSpace = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		return new XmlReaderSettings
		{
			XmlResolver = null,
			DtdProcessing = (DtdProcessing)0,
			IgnoreWhitespace = ignoreWhiteSpace
		};
	}

	internal static XDocument GetOrCreateDocument(XName rootName, IFileSystem fileSystem, string path)
	{
		if (fileSystem.FileExists(path))
		{
			try
			{
				return GetDocument(fileSystem, path);
			}
			catch (FileNotFoundException)
			{
				return CreateDocument(rootName, fileSystem, path);
			}
		}
		return CreateDocument(rootName, fileSystem, path);
	}

	public static XDocument CreateDocument(XName rootName, IFileSystem fileSystem, string path)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		XDocument val = new XDocument(new object[1] { (object)new XElement(rootName) });
		fileSystem.AddFile(path, (Action<Stream>)val.Save);
		return val;
	}

	internal static XDocument GetDocument(IFileSystem fileSystem, string path)
	{
		using Stream input = fileSystem.OpenFile(path);
		return LoadSafe(input, (LoadOptions)1);
	}

	internal static bool TryParseDocument(string content, out XDocument document)
	{
		document = null;
		try
		{
			document = XDocument.Parse(content);
			return true;
		}
		catch (XmlException)
		{
			return false;
		}
	}
}
