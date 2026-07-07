using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NuGet;

internal class XmlTransformer : IPackageFileTransformer
{
	private readonly IDictionary<XName, Action<XElement, XElement>> _nodeActions;

	public XmlTransformer(IDictionary<XName, Action<XElement, XElement>> nodeActions)
	{
		_nodeActions = nodeActions;
	}

	public virtual void TransformFile(IPackageFile file, string targetPath, IProjectSystem projectSystem)
	{
		XElement xml = GetXml(file, projectSystem);
		XDocument orCreateDocument = XmlUtility.GetOrCreateDocument(xml.Name, projectSystem, targetPath);
		orCreateDocument.Root.MergeWith(xml, _nodeActions);
		projectSystem.AddFile(targetPath, (Action<Stream>)orCreateDocument.Save);
	}

	public virtual void RevertFile(IPackageFile file, string targetPath, IEnumerable<IPackageFile> matchingFiles, IProjectSystem projectSystem)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		XElement xml = GetXml(file, projectSystem);
		XDocument orCreateDocument = XmlUtility.GetOrCreateDocument(xml.Name, projectSystem, targetPath);
		XElement target = matchingFiles.Select((IPackageFile f) => GetXml(f, projectSystem)).Aggregate(new XElement(xml.Name), (XElement left, XElement right) => left.MergeWith(right, _nodeActions));
		orCreateDocument.Root.Except(xml.Except(target));
		using Stream stream = projectSystem.CreateFile(targetPath);
		orCreateDocument.Save(stream);
	}

	private static XElement GetXml(IPackageFile file, IProjectSystem projectSystem)
	{
		return XElement.Parse(Preprocessor.Process(file, projectSystem), (LoadOptions)1);
	}
}
