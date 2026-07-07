using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Ingestion.Models;

public abstract class CustomProperty
{
	private const int MaxNameLength = 128;

	private const string KeyPattern = "^[a-zA-Z][a-zA-Z0-9\\-_]*$";

	[JsonProperty(PropertyName = "name")]
	public string Name { get; set; }

	public CustomProperty()
	{
	}

	public CustomProperty(string name)
	{
		Name = name;
	}

	public abstract object GetValue();

	public virtual void Validate()
	{
		if (Name == null)
		{
			throw new ValidationException(ValidationException.Rule.CannotBeNull, "Name");
		}
		if (Name != null)
		{
			if (Name.Length > 128)
			{
				throw new ValidationException(ValidationException.Rule.MaxLength, "Name", 128);
			}
			if (!Regex.IsMatch(Name, "^[a-zA-Z][a-zA-Z0-9\\-_]*$"))
			{
				throw new ValidationException(ValidationException.Rule.Pattern, "Name", "^[a-zA-Z][a-zA-Z0-9\\-_]*$");
			}
		}
	}
}
