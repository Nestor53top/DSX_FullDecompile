namespace Microsoft.AppCenter.Ingestion.Models;

public class ValidationException : IngestionException
{
	public enum Rule
	{
		CannotBeNull,
		CannotBeEmpty,
		MaxItems,
		MinItems,
		MaxLength,
		Pattern,
		InclusiveMinimum,
		InclusiveMaximum
	}

	private const string DefaultMessage = "Validation failed";

	private static string GetRuleString(Rule rule, string extraValue)
	{
		return rule switch
		{
			Rule.CannotBeNull => "Cannot be null", 
			Rule.CannotBeEmpty => "Cannot be empty", 
			Rule.MaxItems => "Number of items exceeded maximum of " + extraValue, 
			Rule.MinItems => "Number of items less than minimum of " + extraValue, 
			Rule.MaxLength => "Maximum length of " + extraValue + " exceeded", 
			Rule.Pattern => "Does not match expected pattern: " + extraValue, 
			Rule.InclusiveMaximum => "Item exceeds maximum value of " + extraValue, 
			Rule.InclusiveMinimum => "Item is less than minimum value of " + extraValue, 
			_ => "Unknown rule", 
		};
	}

	private static string GetErrorString(Rule validationRule, string propertyName, object detail)
	{
		return "Validation failed due to property '" + propertyName + "': " + GetRuleString(validationRule, detail?.ToString());
	}

	public ValidationException()
		: base("Validation failed")
	{
	}

	public ValidationException(string message)
		: base(message)
	{
	}

	public ValidationException(Rule validationRule, string propertyName, object detail = null)
		: base(GetErrorString(validationRule, propertyName, detail))
	{
	}
}
