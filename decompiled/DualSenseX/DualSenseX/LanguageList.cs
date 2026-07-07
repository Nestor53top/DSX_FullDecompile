namespace DualSenseX;

public class LanguageList
{
	public string LanguageName { get; set; }

	public string LanguageJsonLink { get; set; }

	public string FlagImageURL { get; set; }

	public override string ToString()
	{
		return LanguageName;
	}
}
