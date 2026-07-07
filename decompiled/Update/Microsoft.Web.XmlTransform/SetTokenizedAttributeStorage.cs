using System.Collections.Generic;

namespace Microsoft.Web.XmlTransform;

internal class SetTokenizedAttributeStorage
{
	public List<Dictionary<string, string>> DictionaryList { get; set; }

	public string TokenFormat { get; set; }

	public bool EnableTokenizeParameters { get; set; }

	public bool UseXpathToFormParameter { get; set; }

	public SetTokenizedAttributeStorage()
		: this(4)
	{
	}

	public SetTokenizedAttributeStorage(int capacity)
	{
		DictionaryList = new List<Dictionary<string, string>>(capacity);
		TokenFormat = "$(ReplacableToken_#(" + SetTokenizedAttributes.ParameterAttribute + ")_#(" + SetTokenizedAttributes.TokenNumber + "))";
		EnableTokenizeParameters = false;
		UseXpathToFormParameter = true;
	}
}
