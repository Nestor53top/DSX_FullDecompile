using System.Globalization;
using System.Text;

namespace NuGet;

internal class VersionSpec : IVersionSpec
{
	public SemanticVersion MinVersion { get; set; }

	public bool IsMinInclusive { get; set; }

	public SemanticVersion MaxVersion { get; set; }

	public bool IsMaxInclusive { get; set; }

	public VersionSpec()
	{
	}

	public VersionSpec(SemanticVersion version)
	{
		IsMinInclusive = true;
		IsMaxInclusive = true;
		MinVersion = version;
		MaxVersion = version;
	}

	public override string ToString()
	{
		if (MinVersion != null && IsMinInclusive && MaxVersion == null && !IsMaxInclusive)
		{
			return MinVersion.ToString();
		}
		if (MinVersion != null && MaxVersion != null && MinVersion == MaxVersion && IsMinInclusive && IsMaxInclusive)
		{
			return "[" + MinVersion?.ToString() + "]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(IsMinInclusive ? '[' : '(');
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}, {1}", new object[2] { MinVersion, MaxVersion });
		stringBuilder.Append(IsMaxInclusive ? ']' : ')');
		return stringBuilder.ToString();
	}
}
