using System.Globalization;
using System.Text;

namespace NuGet;

internal class Tokenizer
{
	private string _text;

	private int _index;

	public Tokenizer(string text)
	{
		_text = text;
		_index = 0;
	}

	public Token Read()
	{
		if (_index >= _text.Length)
		{
			return null;
		}
		if (_text[_index] == '$')
		{
			_index++;
			return ParseTokenAfterDollarSign();
		}
		return ParseText();
	}

	private static bool IsWordChar(char ch)
	{
		UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
		if (unicodeCategory != UnicodeCategory.LowercaseLetter && unicodeCategory != UnicodeCategory.UppercaseLetter && unicodeCategory != UnicodeCategory.TitlecaseLetter && unicodeCategory != UnicodeCategory.OtherLetter && unicodeCategory != UnicodeCategory.ModifierLetter && unicodeCategory != UnicodeCategory.DecimalDigitNumber)
		{
			return unicodeCategory == UnicodeCategory.ConnectorPunctuation;
		}
		return true;
	}

	private Token ParseTokenAfterDollarSign()
	{
		StringBuilder stringBuilder = new StringBuilder();
		while (_index < _text.Length)
		{
			char c = _text[_index];
			if (c == '$')
			{
				_index++;
				if (stringBuilder.Length == 0)
				{
					return new Token(TokenCategory.Text, "$");
				}
				return new Token(TokenCategory.Variable, stringBuilder.ToString());
			}
			if (IsWordChar(c))
			{
				stringBuilder.Append(c);
				_index++;
				continue;
			}
			stringBuilder.Insert(0, '$');
			stringBuilder.Append(c);
			_index++;
			return new Token(TokenCategory.Text, stringBuilder.ToString());
		}
		stringBuilder.Insert(0, '$');
		return new Token(TokenCategory.Text, stringBuilder.ToString());
	}

	private Token ParseText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		while (_index < _text.Length && _text[_index] != '$')
		{
			stringBuilder.Append(_text[_index]);
			_index++;
		}
		return new Token(TokenCategory.Text, stringBuilder.ToString());
	}
}
