using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlTransformationLogger
{
	private bool hasLoggedErrors;

	private IXmlTransformationLogger externalLogger;

	private XmlNode currentReferenceNode;

	private bool fSupressWarnings;

	internal bool HasLoggedErrors
	{
		get
		{
			return hasLoggedErrors;
		}
		set
		{
			hasLoggedErrors = false;
		}
	}

	internal XmlNode CurrentReferenceNode
	{
		get
		{
			return currentReferenceNode;
		}
		set
		{
			currentReferenceNode = value;
		}
	}

	public bool SupressWarnings
	{
		get
		{
			return fSupressWarnings;
		}
		set
		{
			fSupressWarnings = value;
		}
	}

	internal XmlTransformationLogger(IXmlTransformationLogger logger)
	{
		externalLogger = logger;
	}

	internal void LogErrorFromException(Exception ex)
	{
		hasLoggedErrors = true;
		if (externalLogger != null)
		{
			if (ex is XmlNodeException { HasErrorInfo: not false } ex2)
			{
				externalLogger.LogErrorFromException(ex2, ConvertUriToFileName(ex2.FileName), ex2.LineNumber, ex2.LinePosition);
			}
			else
			{
				externalLogger.LogErrorFromException(ex);
			}
			return;
		}
		throw ex;
	}

	public void LogMessage(string message, params object[] messageArgs)
	{
		if (externalLogger != null)
		{
			externalLogger.LogMessage(message, messageArgs);
		}
	}

	public void LogMessage(MessageType type, string message, params object[] messageArgs)
	{
		if (externalLogger != null)
		{
			externalLogger.LogMessage(type, message, messageArgs);
		}
	}

	public void LogWarning(string message, params object[] messageArgs)
	{
		if (SupressWarnings)
		{
			LogMessage(message, messageArgs);
		}
		else if (CurrentReferenceNode != null)
		{
			LogWarning(CurrentReferenceNode, message, messageArgs);
		}
		else if (externalLogger != null)
		{
			externalLogger.LogWarning(message, messageArgs);
		}
	}

	public void LogWarning(XmlNode referenceNode, string message, params object[] messageArgs)
	{
		if (SupressWarnings)
		{
			LogMessage(message, messageArgs);
		}
		else if (externalLogger != null)
		{
			string file = ConvertUriToFileName(referenceNode.OwnerDocument);
			IXmlLineInfo val = (IXmlLineInfo)(object)((referenceNode is IXmlLineInfo) ? referenceNode : null);
			if (val != null)
			{
				externalLogger.LogWarning(file, val.LineNumber, val.LinePosition, message, messageArgs);
			}
			else
			{
				externalLogger.LogWarning(file, message, messageArgs);
			}
		}
	}

	public void LogError(string message, params object[] messageArgs)
	{
		hasLoggedErrors = true;
		if (CurrentReferenceNode != null)
		{
			LogError(CurrentReferenceNode, message, messageArgs);
			return;
		}
		if (externalLogger != null)
		{
			externalLogger.LogError(message, messageArgs);
			return;
		}
		throw new XmlTransformationException(string.Format(CultureInfo.CurrentCulture, message, messageArgs));
	}

	public void LogError(XmlNode referenceNode, string message, params object[] messageArgs)
	{
		hasLoggedErrors = true;
		if (externalLogger != null)
		{
			string file = ConvertUriToFileName(referenceNode.OwnerDocument);
			IXmlLineInfo val = (IXmlLineInfo)(object)((referenceNode is IXmlLineInfo) ? referenceNode : null);
			if (val != null)
			{
				externalLogger.LogError(file, val.LineNumber, val.LinePosition, message, messageArgs);
			}
			else
			{
				externalLogger.LogError(file, message, messageArgs);
			}
			return;
		}
		throw new XmlNodeException(string.Format(CultureInfo.CurrentCulture, message, messageArgs), referenceNode);
	}

	public void StartSection(string message, params object[] messageArgs)
	{
		if (externalLogger != null)
		{
			externalLogger.StartSection(message, messageArgs);
		}
	}

	public void StartSection(MessageType type, string message, params object[] messageArgs)
	{
		if (externalLogger != null)
		{
			externalLogger.StartSection(type, message, messageArgs);
		}
	}

	public void EndSection(string message, params object[] messageArgs)
	{
		if (externalLogger != null)
		{
			externalLogger.EndSection(message, messageArgs);
		}
	}

	public void EndSection(MessageType type, string message, params object[] messageArgs)
	{
		if (externalLogger != null)
		{
			externalLogger.EndSection(type, message, messageArgs);
		}
	}

	private string ConvertUriToFileName(XmlDocument xmlDocument)
	{
		XmlFileInfoDocument xmlFileInfoDocument = xmlDocument as XmlFileInfoDocument;
		string fileName = ((xmlFileInfoDocument == null) ? ((XmlNode)xmlFileInfoDocument).BaseURI : xmlFileInfoDocument.FileName);
		return ConvertUriToFileName(fileName);
	}

	private string ConvertUriToFileName(string fileName)
	{
		try
		{
			Uri uri = new Uri(fileName);
			if (uri.IsFile && string.IsNullOrEmpty(uri.Host))
			{
				fileName = uri.LocalPath;
			}
		}
		catch (UriFormatException)
		{
		}
		return fileName;
	}
}
