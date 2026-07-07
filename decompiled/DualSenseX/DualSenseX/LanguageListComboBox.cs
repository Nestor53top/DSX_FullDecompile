namespace DualSenseX;

public class LanguageListComboBox
{
	public string LanguageNameShort { get; set; }

	public string LanguageName { get; set; }

	public string LanguageImage { get; set; }

	public override string ToString()
	{
		return LanguageName;
	}
}
