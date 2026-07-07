using System.Runtime.Versioning;

namespace NuGet;

internal interface IProjectSystem : IFileSystem, IPropertyProvider
{
	FrameworkName TargetFramework { get; }

	string ProjectName { get; }

	bool IsBindingRedirectSupported { get; }

	void AddReference(string referencePath);

	void AddFrameworkReference(string name);

	bool ReferenceExists(string name);

	void RemoveReference(string name);

	bool IsSupportedFile(string path);

	string ResolvePath(string path);

	void AddImport(string targetFullPath, ProjectImportLocation location);

	void RemoveImport(string targetFullPath);

	bool FileExistsInProject(string path);
}
