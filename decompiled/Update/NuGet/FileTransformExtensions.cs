using System;

namespace NuGet;

internal sealed class FileTransformExtensions : IEquatable<FileTransformExtensions>
{
	public string InstallExtension { get; private set; }

	public string UninstallExtension { get; private set; }

	public FileTransformExtensions(string installExtension, string uninstallExtension)
	{
		if (string.IsNullOrEmpty(installExtension))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "installExtension");
		}
		if (string.IsNullOrEmpty(uninstallExtension))
		{
			throw new ArgumentException(CommonResources.Argument_Cannot_Be_Null_Or_Empty, "uninstallExtension");
		}
		InstallExtension = installExtension;
		UninstallExtension = uninstallExtension;
	}

	public bool Equals(FileTransformExtensions other)
	{
		if (string.Equals(InstallExtension, other.InstallExtension, StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(UninstallExtension, other.UninstallExtension, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return InstallExtension.GetHashCode() * 3137 + UninstallExtension.GetHashCode();
	}
}
