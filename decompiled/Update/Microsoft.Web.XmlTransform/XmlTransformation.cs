using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.Web.XmlTransform;

internal class XmlTransformation : IServiceProvider, IDisposable
{
	internal static readonly string TransformNamespace = "http://schemas.microsoft.com/XML-Document-Transform";

	internal static readonly string SupressWarnings = "SupressWarnings";

	private string transformFile;

	private XmlDocument xmlTransformation;

	private XmlDocument xmlTarget;

	private XmlTransformableDocument xmlTransformable;

	private XmlTransformationLogger logger;

	private NamedTypeFactory namedTypeFactory;

	private ServiceContainer transformationServiceContainer = new ServiceContainer();

	private ServiceContainer documentServiceContainer;

	private bool hasTransformNamespace;

	public bool HasTransformNamespace => hasTransformNamespace;

	public XmlTransformation(string transformFile)
		: this(transformFile, isTransformAFile: true, null)
	{
	}

	public XmlTransformation(string transform, IXmlTransformationLogger logger)
		: this(transform, isTransformAFile: true, logger)
	{
	}

	public XmlTransformation(string transform, bool isTransformAFile, IXmlTransformationLogger logger)
	{
		transformFile = transform;
		this.logger = new XmlTransformationLogger(logger);
		xmlTransformation = (XmlDocument)(object)new XmlFileInfoDocument();
		if (isTransformAFile)
		{
			xmlTransformation.Load(transform);
		}
		else
		{
			xmlTransformation.LoadXml(transform);
		}
		InitializeTransformationServices();
		PreprocessTransformDocument();
	}

	public XmlTransformation(Stream transformStream, IXmlTransformationLogger logger)
	{
		this.logger = new XmlTransformationLogger(logger);
		transformFile = string.Empty;
		xmlTransformation = (XmlDocument)(object)new XmlFileInfoDocument();
		xmlTransformation.Load(transformStream);
		InitializeTransformationServices();
		PreprocessTransformDocument();
	}

	private void InitializeTransformationServices()
	{
		namedTypeFactory = new NamedTypeFactory(transformFile);
		transformationServiceContainer.AddService(namedTypeFactory.GetType(), namedTypeFactory);
		transformationServiceContainer.AddService(logger.GetType(), logger);
	}

	private void InitializeDocumentServices(XmlDocument document)
	{
		documentServiceContainer = new ServiceContainer();
		if (document is IXmlOriginalDocumentService)
		{
			documentServiceContainer.AddService(typeof(IXmlOriginalDocumentService), document);
		}
	}

	private void ReleaseDocumentServices()
	{
		if (documentServiceContainer != null)
		{
			documentServiceContainer.RemoveService(typeof(IXmlOriginalDocumentService));
			documentServiceContainer = null;
		}
	}

	private void PreprocessTransformDocument()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		hasTransformNamespace = false;
		foreach (XmlAttribute item in ((XmlNode)xmlTransformation).SelectNodes("//namespace::*"))
		{
			XmlAttribute val = item;
			if (((XmlNode)val).Value.Equals(TransformNamespace, StringComparison.Ordinal))
			{
				hasTransformNamespace = true;
				break;
			}
		}
		if (!hasTransformNamespace)
		{
			return;
		}
		XmlNamespaceManager val2 = new XmlNamespaceManager((XmlNameTable)new NameTable());
		val2.AddNamespace("xdt", TransformNamespace);
		XmlNodeList val3 = ((XmlNode)xmlTransformation).SelectNodes("//xdt:*", val2);
		foreach (XmlNode item2 in val3)
		{
			XmlNode val4 = item2;
			XmlElement val5 = (XmlElement)(object)((val4 is XmlElement) ? val4 : null);
			if (val5 == null)
			{
				continue;
			}
			XmlElementContext xmlElementContext = null;
			try
			{
				string localName;
				if ((localName = ((XmlNode)val5).LocalName) != null && localName == "Import")
				{
					xmlElementContext = CreateElementContext(null, val5);
					PreprocessImportElement(xmlElementContext);
				}
				else
				{
					logger.LogWarning((XmlNode)(object)val5, SR.XMLTRANSFORMATION_UnknownXdtTag, ((XmlNode)val5).Name);
				}
			}
			catch (Exception ex)
			{
				if (xmlElementContext != null)
				{
					ex = WrapException(ex, xmlElementContext);
				}
				logger.LogErrorFromException(ex);
				throw new XmlTransformationException(SR.XMLTRANSFORMATION_FatalTransformSyntaxError, ex);
			}
			finally
			{
				xmlElementContext = null;
			}
		}
	}

	public void AddTransformationService(Type serviceType, object serviceInstance)
	{
		transformationServiceContainer.AddService(serviceType, serviceInstance);
	}

	public void RemoveTransformationService(Type serviceType)
	{
		transformationServiceContainer.RemoveService(serviceType);
	}

	public bool Apply(XmlDocument xmlTarget)
	{
		if (this.xmlTarget == null)
		{
			logger.HasLoggedErrors = false;
			this.xmlTarget = xmlTarget;
			xmlTransformable = xmlTarget as XmlTransformableDocument;
			try
			{
				if (hasTransformNamespace)
				{
					InitializeDocumentServices(xmlTarget);
					TransformLoop(xmlTransformation);
				}
				else
				{
					logger.LogMessage(MessageType.Normal, "The expected namespace {0} was not found in the transform file", TransformNamespace);
				}
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}
			finally
			{
				ReleaseDocumentServices();
				this.xmlTarget = null;
				xmlTransformable = null;
			}
			return !logger.HasLoggedErrors;
		}
		return false;
	}

	private void TransformLoop(XmlDocument xmlSource)
	{
		TransformLoop(new XmlNodeContext((XmlNode)(object)xmlSource));
	}

	private void TransformLoop(XmlNodeContext parentContext)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		foreach (XmlNode childNode in parentContext.Node.ChildNodes)
		{
			XmlNode val = childNode;
			XmlElement val2 = (XmlElement)(object)((val is XmlElement) ? val : null);
			if (val2 != null)
			{
				XmlElementContext context = CreateElementContext(parentContext as XmlElementContext, val2);
				try
				{
					HandleElement(context);
				}
				catch (Exception ex)
				{
					HandleException(ex, context);
				}
			}
		}
	}

	private XmlElementContext CreateElementContext(XmlElementContext parentContext, XmlElement element)
	{
		return new XmlElementContext(parentContext, element, xmlTarget, this);
	}

	private void HandleException(Exception ex)
	{
		logger.LogErrorFromException(ex);
	}

	private void HandleException(Exception ex, XmlNodeContext context)
	{
		HandleException(WrapException(ex, context));
	}

	private Exception WrapException(Exception ex, XmlNodeContext context)
	{
		return XmlNodeException.Wrap(ex, context.Node);
	}

	private void HandleElement(XmlElementContext context)
	{
		string argumentString;
		Transform transform = context.ConstructTransform(out argumentString);
		if (transform != null)
		{
			bool supressWarnings = logger.SupressWarnings;
			XmlNode namedItem = ((XmlNamedNodeMap)((XmlNode)context.Element).Attributes).GetNamedItem(SupressWarnings, TransformNamespace);
			XmlAttribute val = (XmlAttribute)(object)((namedItem is XmlAttribute) ? namedItem : null);
			if (val != null)
			{
				bool supressWarnings2 = Convert.ToBoolean(((XmlNode)val).Value, CultureInfo.InvariantCulture);
				logger.SupressWarnings = supressWarnings2;
			}
			try
			{
				OnApplyingTransform();
				transform.Execute(context, argumentString);
				OnAppliedTransform();
			}
			catch (Exception ex)
			{
				HandleException(ex, context);
			}
			finally
			{
				logger.SupressWarnings = supressWarnings;
			}
		}
		TransformLoop((XmlNodeContext)context);
	}

	private void OnApplyingTransform()
	{
		if (xmlTransformable != null)
		{
			xmlTransformable.OnBeforeChange();
		}
	}

	private void OnAppliedTransform()
	{
		if (xmlTransformable != null)
		{
			xmlTransformable.OnAfterChange();
		}
	}

	private void PreprocessImportElement(XmlElementContext context)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		string text = null;
		string text2 = null;
		string text3 = null;
		foreach (XmlAttribute item in (XmlNamedNodeMap)((XmlNode)context.Element).Attributes)
		{
			XmlAttribute val = item;
			if (((XmlNode)val).NamespaceURI.Length == 0)
			{
				switch (((XmlNode)val).Name)
				{
				case "assembly":
					text = ((XmlNode)val).Value;
					continue;
				case "namespace":
					text2 = ((XmlNode)val).Value;
					continue;
				case "path":
					text3 = ((XmlNode)val).Value;
					continue;
				}
			}
			throw new XmlNodeException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_ImportUnknownAttribute, new object[1] { ((XmlNode)val).Name }), (XmlNode)(object)val);
		}
		if (text != null && text3 != null)
		{
			throw new XmlNodeException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_ImportAttributeConflict, new object[0]), (XmlNode)(object)context.Element);
		}
		if (text == null && text3 == null)
		{
			throw new XmlNodeException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_ImportMissingAssembly, new object[0]), (XmlNode)(object)context.Element);
		}
		if (text2 == null)
		{
			throw new XmlNodeException(string.Format(CultureInfo.CurrentCulture, SR.XMLTRANSFORMATION_ImportMissingNamespace, new object[0]), (XmlNode)(object)context.Element);
		}
		if (text != null)
		{
			namedTypeFactory.AddAssemblyRegistration(text, text2);
		}
		else
		{
			namedTypeFactory.AddPathRegistration(text3, text2);
		}
	}

	public object GetService(Type serviceType)
	{
		object obj = null;
		if (documentServiceContainer != null)
		{
			obj = documentServiceContainer.GetService(serviceType);
		}
		if (obj == null)
		{
			obj = transformationServiceContainer.GetService(serviceType);
		}
		return obj;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (transformationServiceContainer != null)
		{
			transformationServiceContainer.Dispose();
			transformationServiceContainer = null;
		}
		if (documentServiceContainer != null)
		{
			documentServiceContainer.Dispose();
			documentServiceContainer = null;
		}
		if (xmlTransformable != null)
		{
			xmlTransformable.Dispose();
			xmlTransformable = null;
		}
		if (xmlTransformation is XmlFileInfoDocument)
		{
			(xmlTransformation as XmlFileInfoDocument).Dispose();
			xmlTransformation = null;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~XmlTransformation()
	{
		Dispose(disposing: false);
	}
}
