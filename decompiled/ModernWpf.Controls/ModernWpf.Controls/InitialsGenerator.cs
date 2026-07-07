using System.Linq;

namespace ModernWpf.Controls;

internal class InitialsGenerator
{
	public static string InitialsFromDisplayName(string contactDisplayName)
	{
		if (GetCharacterType(contactDisplayName) == CharacterType.Standard)
		{
			string source = contactDisplayName;
			StripTrailingBrackets(ref source);
			string[] array = Split(source, ' ');
			if (array.Length == 1)
			{
				return GetFirstFullCharacter(array.First()).ToUpper();
			}
			if (array.Length > 1)
			{
				string str = array.First();
				string str2 = array.Last();
				return (GetFirstFullCharacter(str) + GetFirstFullCharacter(str2)).ToUpper();
			}
			return string.Empty;
		}
		return string.Empty;
	}

	public static CharacterType GetCharacterType(string str)
	{
		CharacterType characterType = CharacterType.Other;
		for (int i = 0; i < 3 && i < str.Length && str[i] != 0 && str[i] != '\ufeff'; i++)
		{
			switch (GetCharacterType(str[i]))
			{
			case CharacterType.Glyph:
				characterType = CharacterType.Glyph;
				break;
			case CharacterType.Symbolic:
				if (characterType != CharacterType.Glyph)
				{
					characterType = CharacterType.Symbolic;
				}
				break;
			case CharacterType.Standard:
				if (characterType != CharacterType.Glyph && characterType != CharacterType.Symbolic)
				{
					characterType = CharacterType.Standard;
				}
				break;
			}
		}
		return characterType;
	}

	public static CharacterType GetCharacterType(char character)
	{
		if (character >= 'ɐ' && character <= 'ʯ')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0600' && character <= 'ۿ')
		{
			return CharacterType.Glyph;
		}
		if (character >= 'ݐ' && character <= 'ݿ')
		{
			return CharacterType.Glyph;
		}
		if (character >= 'ࢠ' && character <= '\u08ff')
		{
			return CharacterType.Glyph;
		}
		if (character >= 'ﭐ' && character <= '﷿')
		{
			return CharacterType.Glyph;
		}
		if (character >= 'ﹰ' && character <= '\ufeff')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0900' && character <= 'ॿ')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\ua8e0' && character <= '\ua8ff')
		{
			return CharacterType.Glyph;
		}
		if (character >= 'ঀ' && character <= '\u09ff')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0a00' && character <= '\u0a7f')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0a80' && character <= '\u0aff')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0b00' && character <= '\u0b7f')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0b80' && character <= '\u0bff')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0c00' && character <= '౿')
		{
			return CharacterType.Glyph;
		}
		if (character >= 'ಀ' && character <= '\u0cff')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0d00' && character <= 'ൿ')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0d80' && character <= '\u0dff')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0e00' && character <= '\u0e7f')
		{
			return CharacterType.Glyph;
		}
		if (character >= '\u0e80' && character <= '\u0eff')
		{
			return CharacterType.Glyph;
		}
		if (character >= '一' && character <= '鿿')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '㐀' && character <= '䶿')
		{
			return CharacterType.Symbolic;
		}
		if (character >= 131072 && character <= 173791)
		{
			return CharacterType.Symbolic;
		}
		if (character >= 173824 && character <= 177983)
		{
			return CharacterType.Symbolic;
		}
		if (character >= 177984 && character <= 178207)
		{
			return CharacterType.Symbolic;
		}
		if (character >= '⺀' && character <= '\u2eff')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '\u3000' && character <= '〿')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '㇀' && character <= '㇯')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '㈀' && character <= '㋿')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '㌀' && character <= '㏿')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '豈' && character <= '\ufaff')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '︰' && character <= '\ufe4f')
		{
			return CharacterType.Symbolic;
		}
		if (character >= 194560 && character <= 195103)
		{
			return CharacterType.Symbolic;
		}
		if (character >= 'Ͱ' && character <= 'Ͽ')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '\u0590' && character <= '\u05ff')
		{
			return CharacterType.Symbolic;
		}
		if (character >= '\u0530' && character <= '֏')
		{
			return CharacterType.Symbolic;
		}
		if (character > '\0' && character <= '\u007f')
		{
			return CharacterType.Standard;
		}
		if (character >= '\u0080' && character <= 'ÿ')
		{
			return CharacterType.Standard;
		}
		if (character >= 'Ā' && character <= 'ſ')
		{
			return CharacterType.Standard;
		}
		if (character >= 'ƀ' && character <= 'ɏ')
		{
			return CharacterType.Standard;
		}
		if (character >= 'Ⱡ' && character <= 'Ɀ')
		{
			return CharacterType.Standard;
		}
		if (character >= '\ua720' && character <= 'ꟿ')
		{
			return CharacterType.Standard;
		}
		if (character >= 'ꬰ' && character <= '\uab6f')
		{
			return CharacterType.Standard;
		}
		if (character >= 'Ḁ' && character <= 'ỿ')
		{
			return CharacterType.Standard;
		}
		if (character >= 'Ѐ' && character <= 'ӿ')
		{
			return CharacterType.Standard;
		}
		if (character >= 'Ԁ' && character <= 'ԯ')
		{
			return CharacterType.Standard;
		}
		if (character >= '\u0300' && character <= '\u036f')
		{
			return CharacterType.Standard;
		}
		return CharacterType.Other;
	}

	private static string[] Split(string source, char delim, int maxIterations = 25)
	{
		return source.Split(new char[1] { delim }, maxIterations);
	}

	private static void StripTrailingBrackets(ref string source)
	{
		string[] array = new string[3] { "{}", "()", "[]" };
		if (source.Length == 0)
		{
			return;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (source[source.Length - 1] == text[1])
			{
				int num = source.LastIndexOf(text[0]);
				if (num != -1)
				{
					source = source.Remove(num);
					break;
				}
			}
		}
	}

	private static string GetFirstFullCharacter(string str)
	{
		int num = 0;
		while (num < str.Length)
		{
			char c = str[num];
			if (c >= '!' && c <= '/')
			{
				num++;
				continue;
			}
			if (c >= ':' && c <= '@')
			{
				num++;
				continue;
			}
			if (c < '{' || c > '~')
			{
				break;
			}
			num++;
		}
		if (num >= str.Length)
		{
			num = 0;
		}
		int i;
		for (i = num + 1; i < str.Length; i++)
		{
			char c2 = str[i];
			if (c2 < '\u0300' || c2 > '\u036f')
			{
				break;
			}
		}
		int length = i - num;
		return str.SafeSubstring(num, length);
	}
}
