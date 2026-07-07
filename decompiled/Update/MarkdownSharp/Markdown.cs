using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownSharp;

internal class Markdown
{
	private enum TokenType
	{
		Text,
		Tag
	}

	private struct Token(TokenType type, string value)
	{
		public TokenType Type = type;

		public string Value = value;
	}

	private const string _version = "1.13";

	private string _emptyElementSuffix = " />";

	private bool _linkEmails = true;

	private bool _strictBoldItalic;

	private bool _autoNewlines;

	private bool _autoHyperlink;

	private bool _encodeProblemUrlCharacters;

	private const int _nestDepth = 6;

	private const int _tabWidth = 4;

	private const string _markerUL = "[*+-]";

	private const string _markerOL = "\\d+[.]";

	private static readonly Dictionary<string, string> _escapeTable;

	private static readonly Dictionary<string, string> _invertedEscapeTable;

	private static readonly Dictionary<string, string> _backslashEscapeTable;

	private readonly Dictionary<string, string> _urls = new Dictionary<string, string>();

	private readonly Dictionary<string, string> _titles = new Dictionary<string, string>();

	private readonly Dictionary<string, string> _htmlBlocks = new Dictionary<string, string>();

	private int _listLevel;

	private static string AutoLinkPreventionMarker;

	private static Regex _newlinesLeadingTrailing;

	private static Regex _newlinesMultiple;

	private static Regex _leadingWhitespace;

	private static Regex _htmlBlockHash;

	private static string _nestedBracketsPattern;

	private static string _nestedParensPattern;

	private static Regex _linkDef;

	private static Regex _blocksHtml;

	private static Regex _htmlTokens;

	private static Regex _anchorRef;

	private static Regex _anchorInline;

	private static Regex _anchorRefShortcut;

	private static Regex _imagesRef;

	private static Regex _imagesInline;

	private static Regex _headerSetext;

	private static Regex _headerAtx;

	private static Regex _horizontalRules;

	private static string _wholeList;

	private static Regex _listNested;

	private static Regex _listTopLevel;

	private static Regex _codeBlock;

	private static Regex _codeSpan;

	private static Regex _bold;

	private static Regex _strictBold;

	private static Regex _italic;

	private static Regex _strictItalic;

	private static Regex _blockquote;

	private const string _charInsideUrl = "[-A-Z0-9+&@#/%?=~_|\\[\\]\\(\\)!:,\\.;\u001a]";

	private const string _charEndingUrl = "[-A-Z0-9+&@#/%=~_|\\[\\])]";

	private static Regex _autolinkBare;

	private static Regex _endCharRegex;

	private static Regex _outDent;

	private static Regex _codeEncoder;

	private static Regex _amps;

	private static Regex _angles;

	private static Regex _backslashEscapes;

	private static Regex _unescapes;

	private static readonly char[] _problemUrlChars;

	public string EmptyElementSuffix
	{
		get
		{
			return _emptyElementSuffix;
		}
		set
		{
			_emptyElementSuffix = value;
		}
	}

	public bool LinkEmails
	{
		get
		{
			return _linkEmails;
		}
		set
		{
			_linkEmails = value;
		}
	}

	public bool StrictBoldItalic
	{
		get
		{
			return _strictBoldItalic;
		}
		set
		{
			_strictBoldItalic = value;
		}
	}

	public bool AutoNewLines
	{
		get
		{
			return _autoNewlines;
		}
		set
		{
			_autoNewlines = value;
		}
	}

	public bool AutoHyperlink
	{
		get
		{
			return _autoHyperlink;
		}
		set
		{
			_autoHyperlink = value;
		}
	}

	public bool EncodeProblemUrlCharacters
	{
		get
		{
			return _encodeProblemUrlCharacters;
		}
		set
		{
			_encodeProblemUrlCharacters = value;
		}
	}

	public string Version => "1.13";

	public Markdown()
	{
	}

	public Markdown(MarkdownOptions options)
	{
		_autoHyperlink = options.AutoHyperlink;
		_autoNewlines = options.AutoNewlines;
		_emptyElementSuffix = options.EmptyElementSuffix;
		_encodeProblemUrlCharacters = options.EncodeProblemUrlCharacters;
		_linkEmails = options.LinkEmails;
		_strictBoldItalic = options.StrictBoldItalic;
	}

	static Markdown()
	{
		AutoLinkPreventionMarker = "\u001aP";
		_newlinesLeadingTrailing = new Regex("^\\n+|\\n+\\z", RegexOptions.Compiled);
		_newlinesMultiple = new Regex("\\n{2,}", RegexOptions.Compiled);
		_leadingWhitespace = new Regex("^[ ]*", RegexOptions.Compiled);
		_htmlBlockHash = new Regex("\u001aH\\d+H", RegexOptions.Compiled);
		_linkDef = new Regex($"\r\n                        ^[ ]{{0,{3}}}\\[([^\\[\\]]+)\\]:  # id = $1\r\n                          [ ]*\r\n                          \\n?                   # maybe *one* newline\r\n                          [ ]*\r\n                        <?(\\S+?)>?              # url = $2\r\n                          [ ]*\r\n                          \\n?                   # maybe one newline\r\n                          [ ]*\r\n                        (?:\r\n                            (?<=\\s)             # lookbehind for whitespace\r\n                            [\"(]\r\n                            (.+?)               # title = $3\r\n                            [\")]\r\n                            [ ]*\r\n                        )?                      # title is optional\r\n                        (?:\\n+|\\Z)", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_blocksHtml = new Regex(GetBlockPattern(), RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
		_htmlTokens = new Regex("\r\n            (<!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->)|        # match <!-- foo -->\r\n            (<\\?.*?\\?>)|                 # match <?foo?> " + RepeatString("\r\n            (<[A-Za-z\\/!$](?:[^<>]|", 6) + RepeatString(")*>)", 6) + " # match <tag> and </tag>", RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_anchorRef = new Regex($"\r\n            (                               # wrap whole match in $1\r\n                \\[\r\n                    ({GetNestedBracketsPattern()})                   # link text = $2\r\n                \\]\r\n\r\n                [ ]?                        # one optional space\r\n                (?:\\n[ ]*)?                 # one optional newline followed by spaces\r\n\r\n                \\[\r\n                    (.*?)                   # id = $3\r\n                \\]\r\n            )", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_anchorInline = new Regex($"\r\n                (                           # wrap whole match in $1\r\n                    \\[\r\n                        ({GetNestedBracketsPattern()})               # link text = $2\r\n                    \\]\r\n                    \\(                      # literal paren\r\n                        [ ]*\r\n                        ({GetNestedParensPattern()})               # href = $3\r\n                        [ ]*\r\n                        (                   # $4\r\n                        (['\"])           # quote char = $5\r\n                        (.*?)               # title = $6\r\n                        \\5                  # matching quote\r\n                        [ ]*                # ignore any spaces between closing quote and )\r\n                        )?                  # title is optional\r\n                    \\)\r\n                )", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_anchorRefShortcut = new Regex("\r\n            (                               # wrap whole match in $1\r\n              \\[\r\n                 ([^\\[\\]]+)                 # link text = $2; can't contain [ or ]\r\n              \\]\r\n            )", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_imagesRef = new Regex("\r\n                    (               # wrap whole match in $1\r\n                    !\\[\r\n                        (.*?)       # alt text = $2\r\n                    \\]\r\n\r\n                    [ ]?            # one optional space\r\n                    (?:\\n[ ]*)?     # one optional newline followed by spaces\r\n\r\n                    \\[\r\n                        (.*?)       # id = $3\r\n                    \\]\r\n\r\n                    )", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_imagesInline = new Regex($"\r\n              (                     # wrap whole match in $1\r\n                !\\[\r\n                    (.*?)           # alt text = $2\r\n                \\]\r\n                \\s?                 # one optional whitespace character\r\n                \\(                  # literal paren\r\n                    [ ]*\r\n                    ({GetNestedParensPattern()})           # href = $3\r\n                    [ ]*\r\n                    (               # $4\r\n                    (['\"])       # quote char = $5\r\n                    (.*?)           # title = $6\r\n                    \\5              # matching quote\r\n                    [ ]*\r\n                    )?              # title is optional\r\n                \\)\r\n              )", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_headerSetext = new Regex("\r\n                ^(.+?)\r\n                [ ]*\r\n                \\n\r\n                (=+|-+)     # $1 = string of ='s or -'s\r\n                [ ]*\r\n                \\n+", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_headerAtx = new Regex("\r\n                ^(\\#{1,6})  # $1 = string of #'s\r\n                [ ]*\r\n                (.+?)       # $2 = Header text\r\n                [ ]*\r\n                \\#*         # optional closing #'s (not counted)\r\n                \\n+", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_horizontalRules = new Regex("\r\n            ^[ ]{0,3}         # Leading space\r\n                ([-*_])       # $1: First marker\r\n                (?>           # Repeated marker group\r\n                    [ ]{0,2}  # Zero, one, or two spaces.\r\n                    \\1        # Marker character\r\n                ){2,}         # Group repeated at least twice\r\n                [ ]*          # Trailing spaces\r\n                $             # End of line.\r\n            ", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_wholeList = string.Format("\r\n            (                               # $1 = whole list\r\n              (                             # $2\r\n                [ ]{{0,{1}}}\r\n                ({0})                       # $3 = first list item marker\r\n                [ ]+\r\n              )\r\n              (?s:.+?)\r\n              (                             # $4\r\n                  \\z\r\n                |\r\n                  \\n{{2,}}\r\n                  (?=\\S)\r\n                  (?!                       # Negative lookahead for another list item marker\r\n                    [ ]*\r\n                    {0}[ ]+\r\n                  )\r\n              )\r\n            )", string.Format("(?:{0}|{1})", "[*+-]", "\\d+[.]"), 3);
		_listNested = new Regex("^" + _wholeList, RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_listTopLevel = new Regex("(?:(?<=\\n\\n)|\\A\\n?)" + _wholeList, RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_codeBlock = new Regex(string.Format("\r\n                    (?:\\n\\n|\\A\\n?)\r\n                    (                        # $1 = the code block -- one or more lines, starting with a space\r\n                    (?:\r\n                        (?:[ ]{{{0}}})       # Lines must start with a tab-width of spaces\r\n                        .*\\n+\r\n                    )+\r\n                    )\r\n                    ((?=^[ ]{{0,{0}}}[^ \\t\\n])|\\Z) # Lookahead for non-space at line-start, or end of doc", 4), RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_codeSpan = new Regex("\r\n                    (?<![\\\\`])   # Character before opening ` can't be a backslash or backtick\r\n                    (`+)      # $1 = Opening run of `\r\n                    (?!`)     # and no more backticks -- match the full run\r\n                    (.+?)     # $2 = The code block\r\n                    (?<!`)\r\n                    \\1\r\n                    (?!`)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_bold = new Regex("(\\*\\*|__) (?=\\S) (.+?[*_]*) (?<=\\S) \\1", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_strictBold = new Regex("(^|[\\W_])(?:(?!\\1)|(?=^))(\\*|_)\\2(?=\\S)(.*?\\S)\\2\\2(?!\\2)(?=[\\W_]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
		_italic = new Regex("(\\*|_) (?=\\S) (.+?) (?<=\\S) \\1", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		_strictItalic = new Regex("(^|[\\W_])(?:(?!\\1)|(?=^))(\\*|_)(?=\\S)((?:(?!\\2).)*?\\S)\\2(?!\\2)(?=[\\W_]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
		_blockquote = new Regex("\r\n            (                           # Wrap whole match in $1\r\n                (\r\n                ^[ ]*>[ ]?              # '>' at the start of a line\r\n                    .+\\n                # rest of the first line\r\n                (.+\\n)*                 # subsequent consecutive lines\r\n                \\n*                     # blanks\r\n                )+\r\n            )", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
		_autolinkBare = new Regex("(<|=\")?\\b(https?|ftp)(://[-A-Z0-9+&@#/%?=~_|\\[\\]\\(\\)!:,\\.;\u001a]*[-A-Z0-9+&@#/%=~_|\\[\\])])(?=$|\\W)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		_endCharRegex = new Regex("[-A-Z0-9+&@#/%=~_|\\[\\])]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		_outDent = new Regex("^[ ]{1," + 4 + "}", RegexOptions.Multiline | RegexOptions.Compiled);
		_codeEncoder = new Regex("&|<|>|\\\\|\\*|_|\\{|\\}|\\[|\\]", RegexOptions.Compiled);
		_amps = new Regex("&(?!((#[0-9]+)|(#[xX][a-fA-F0-9]+)|([a-zA-Z][a-zA-Z0-9]*));)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		_angles = new Regex("<(?![A-Za-z/?\\$!])", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		_unescapes = new Regex("\u001aE\\d+E", RegexOptions.Compiled);
		_problemUrlChars = "\"'*()[]$:".ToCharArray();
		_escapeTable = new Dictionary<string, string>();
		_invertedEscapeTable = new Dictionary<string, string>();
		_backslashEscapeTable = new Dictionary<string, string>();
		string text = "";
		string text2 = "\\`*_{}[]()>#+-.!/";
		for (int i = 0; i < text2.Length; i++)
		{
			string text3 = text2[i].ToString();
			string hashKey = GetHashKey(text3, isHtmlBlock: false);
			_escapeTable.Add(text3, hashKey);
			_invertedEscapeTable.Add(hashKey, text3);
			_backslashEscapeTable.Add("\\" + text3, hashKey);
			text = text + Regex.Escape("\\" + text3) + "|";
		}
		_backslashEscapes = new Regex(text.Substring(0, text.Length - 1), RegexOptions.Compiled);
	}

	public string Transform(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		Setup();
		text = Normalize(text);
		text = HashHTMLBlocks(text);
		text = StripLinkDefinitions(text);
		text = RunBlockGamut(text);
		text = Unescape(text);
		Cleanup();
		return text + "\n";
	}

	private string RunBlockGamut(string text, bool unhash = true)
	{
		text = DoHeaders(text);
		text = DoHorizontalRules(text);
		text = DoLists(text);
		text = DoCodeBlocks(text);
		text = DoBlockQuotes(text);
		text = HashHTMLBlocks(text);
		text = FormParagraphs(text, unhash);
		return text;
	}

	private string RunSpanGamut(string text)
	{
		text = DoCodeSpans(text);
		text = EscapeSpecialCharsWithinTagAttributes(text);
		text = EscapeBackslashes(text);
		text = DoImages(text);
		text = DoAnchors(text);
		text = DoAutoLinks(text);
		text = text.Replace(AutoLinkPreventionMarker, "://");
		text = EncodeAmpsAndAngles(text);
		text = DoItalicsAndBold(text);
		text = DoHardBreaks(text);
		return text;
	}

	private string FormParagraphs(string text, bool unhash = true)
	{
		string[] array = _newlinesMultiple.Split(_newlinesLeadingTrailing.Replace(text, ""));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].StartsWith("\u001aH"))
			{
				if (!unhash)
				{
					continue;
				}
				int num = 50;
				bool keepGoing = true;
				while (keepGoing && num > 0)
				{
					keepGoing = false;
					array[i] = _htmlBlockHash.Replace(array[i], delegate(Match match)
					{
						keepGoing = true;
						return _htmlBlocks[match.Value];
					});
					num--;
				}
			}
			else
			{
				array[i] = _leadingWhitespace.Replace(RunSpanGamut(array[i]), "<p>") + "</p>";
			}
		}
		return string.Join("\n\n", array);
	}

	private void Setup()
	{
		_urls.Clear();
		_titles.Clear();
		_htmlBlocks.Clear();
		_listLevel = 0;
	}

	private void Cleanup()
	{
		Setup();
	}

	private static string GetNestedBracketsPattern()
	{
		if (_nestedBracketsPattern == null)
		{
			_nestedBracketsPattern = RepeatString("\r\n                    (?>              # Atomic matching\r\n                       [^\\[\\]]+      # Anything other than brackets\r\n                     |\r\n                       \\[\r\n                           ", 6) + RepeatString(" \\]\r\n                    )*", 6);
		}
		return _nestedBracketsPattern;
	}

	private static string GetNestedParensPattern()
	{
		if (_nestedParensPattern == null)
		{
			_nestedParensPattern = RepeatString("\r\n                    (?>              # Atomic matching\r\n                       [^()\\s]+      # Anything other than parens or whitespace\r\n                     |\r\n                       \\(\r\n                           ", 6) + RepeatString(" \\)\r\n                    )*", 6);
		}
		return _nestedParensPattern;
	}

	private string StripLinkDefinitions(string text)
	{
		return _linkDef.Replace(text, LinkEvaluator);
	}

	private string LinkEvaluator(Match match)
	{
		string key = match.Groups[1].Value.ToLowerInvariant();
		_urls[key] = EncodeAmpsAndAngles(match.Groups[2].Value);
		if (match.Groups[3] != null && match.Groups[3].Length > 0)
		{
			_titles[key] = match.Groups[3].Value.Replace("\"", "&quot;");
		}
		return "";
	}

	private static string GetBlockPattern()
	{
		string newValue = "ins|del";
		string newValue2 = "p|div|h[1-6]|blockquote|pre|table|dl|ol|ul|address|script|noscript|form|fieldset|iframe|math";
		string text = "\r\n            (?>\ufeff  \ufeff  \ufeff  \ufeff              # optional tag attributes\r\n              \\s\ufeff  \ufeff  \ufeff              # starts with whitespace\r\n              (?>\r\n                [^>\"/]+\ufeff              # text outside quotes\r\n              |\r\n                /+(?!>)\ufeff  \ufeff              # slash not followed by >\r\n              |\r\n                \"[^\"]*\"\ufeff  \ufeff          # text inside double quotes (tolerate >)\r\n              |\r\n                '[^']*'\ufeff                  # text inside single quotes (tolerate >)\r\n              )*\r\n            )?\ufeff\r\n            ";
		string text2 = RepeatString("\r\n                (?>\r\n                  [^<]+\ufeff  \ufeff  \ufeff          # content without tag\r\n                |\r\n                  <\\2\ufeff  \ufeff  \ufeff          # nested opening tag\r\n                    " + text + "       # attributes\r\n                  (?>\r\n                      />\r\n                  |\r\n                      >", 6) + ".*?" + RepeatString("\r\n                      </\\2\\s*>\ufeff          # closing nested tag\r\n                  )\r\n                  |\ufeff  \ufeff  \ufeff  \ufeff\r\n                  <(?!/\\2\\s*>           # other tags with a different name\r\n                  )\r\n                )*", 6);
		string newValue3 = text2.Replace("\\2", "\\3");
		return "\r\n            (?>\r\n                  (?>\r\n                    (?<=\\n)     # Starting at the beginning of a line\r\n                    |           # or\r\n                    \\A\\n?       # the beginning of the doc\r\n                  )\r\n                  (             # save in $1\r\n\r\n                    # Match from `\\n<tag>` to `</tag>\\n`, handling nested tags\r\n                    # in between.\r\n\r\n                        <($block_tags_b_re)   # start tag = $2\r\n                        $attr>                # attributes followed by > and \\n\r\n                        $content              # content, support nesting\r\n                        </\\2>                 # the matching end tag\r\n                        [ ]*                  # trailing spaces\r\n                        (?=\\n+|\\Z)            # followed by a newline or end of document\r\n\r\n                  | # Special version for tags of group a.\r\n\r\n                        <($block_tags_a_re)   # start tag = $3\r\n                        $attr>[ ]*\\n          # attributes followed by >\r\n                        $content2             # content, support nesting\r\n                        </\\3>                 # the matching end tag\r\n                        [ ]*                  # trailing spaces\r\n                        (?=\\n+|\\Z)            # followed by a newline or end of document\r\n\r\n                  | # Special case just for <hr />. It was easier to make a special\r\n                    # case than to make the other regex more complicated.\r\n\r\n                        [ ]{0,$less_than_tab}\r\n                        <hr\r\n                        $attr                 # attributes\r\n                        /?>                   # the matching end tag\r\n                        [ ]*\r\n                        (?=\\n{2,}|\\Z)         # followed by a blank line or end of document\r\n\r\n                  | # Special case for standalone HTML comments:\r\n\r\n                      (?<=\\n\\n|\\A)            # preceded by a blank line or start of document\r\n                      [ ]{0,$less_than_tab}\r\n                      (?s:\r\n                        <!--(?:|(?:[^>-]|-[^>])(?:[^-]|-[^-])*)-->\r\n                      )\r\n                      [ ]*\r\n                      (?=\\n{2,}|\\Z)            # followed by a blank line or end of document\r\n\r\n                  | # PHP and ASP-style processor instructions (<? and <%)\r\n\r\n                      [ ]{0,$less_than_tab}\r\n                      (?s:\r\n                        <([?%])                # $4\r\n                        .*?\r\n                        \\4>\r\n                      )\r\n                      [ ]*\r\n                      (?=\\n{2,}|\\Z)            # followed by a blank line or end of document\r\n\r\n                  )\r\n            )".Replace("$less_than_tab", 3.ToString()).Replace("$block_tags_b_re", newValue2).Replace("$block_tags_a_re", newValue)
			.Replace("$attr", text)
			.Replace("$content2", newValue3)
			.Replace("$content", text2);
	}

	private string HashHTMLBlocks(string text)
	{
		return _blocksHtml.Replace(text, HtmlEvaluator);
	}

	private string HtmlEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		string hashKey = GetHashKey(value, isHtmlBlock: true);
		_htmlBlocks[hashKey] = value;
		return "\n\n" + hashKey + "\n\n";
	}

	private static string GetHashKey(string s, bool isHtmlBlock)
	{
		char c = (isHtmlBlock ? 'H' : 'E');
		return "\u001a" + c + Math.Abs(s.GetHashCode()) + c;
	}

	private List<Token> TokenizeHTML(string text)
	{
		int num = 0;
		int num2 = 0;
		List<Token> list = new List<Token>();
		foreach (Match item in _htmlTokens.Matches(text))
		{
			num2 = item.Index;
			if (num < num2)
			{
				list.Add(new Token(TokenType.Text, text.Substring(num, num2 - num)));
			}
			list.Add(new Token(TokenType.Tag, item.Value));
			num = num2 + item.Length;
		}
		if (num < text.Length)
		{
			list.Add(new Token(TokenType.Text, text.Substring(num, text.Length - num)));
		}
		return list;
	}

	private string DoAnchors(string text)
	{
		text = _anchorRef.Replace(text, AnchorRefEvaluator);
		text = _anchorInline.Replace(text, AnchorInlineEvaluator);
		text = _anchorRefShortcut.Replace(text, AnchorRefShortcutEvaluator);
		return text;
	}

	private string SaveFromAutoLinking(string s)
	{
		return s.Replace("://", AutoLinkPreventionMarker);
	}

	private string AnchorRefEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		string text = SaveFromAutoLinking(match.Groups[2].Value);
		string text2 = match.Groups[3].Value.ToLowerInvariant();
		if (text2 == "")
		{
			text2 = text.ToLowerInvariant();
		}
		if (_urls.ContainsKey(text2))
		{
			string url = _urls[text2];
			url = EncodeProblemUrlChars(url);
			url = EscapeBoldItalic(url);
			string text3 = "<a href=\"" + url + "\"";
			if (_titles.ContainsKey(text2))
			{
				string s = AttributeEncode(_titles[text2]);
				s = AttributeEncode(EscapeBoldItalic(s));
				text3 = text3 + " title=\"" + s + "\"";
			}
			return text3 + ">" + text + "</a>";
		}
		return value;
	}

	private string AnchorRefShortcutEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		string text = SaveFromAutoLinking(match.Groups[2].Value);
		string key = Regex.Replace(text.ToLowerInvariant(), "[ ]*\\n[ ]*", " ");
		if (_urls.ContainsKey(key))
		{
			string url = _urls[key];
			url = EncodeProblemUrlChars(url);
			url = EscapeBoldItalic(url);
			string text2 = "<a href=\"" + url + "\"";
			if (_titles.ContainsKey(key))
			{
				string s = AttributeEncode(_titles[key]);
				s = EscapeBoldItalic(s);
				text2 = text2 + " title=\"" + s + "\"";
			}
			return text2 + ">" + text + "</a>";
		}
		return value;
	}

	private string AnchorInlineEvaluator(Match match)
	{
		string arg = SaveFromAutoLinking(match.Groups[2].Value);
		string value = match.Groups[3].Value;
		string value2 = match.Groups[6].Value;
		value = EncodeProblemUrlChars(value);
		value = EscapeBoldItalic(value);
		if (value.StartsWith("<") && value.EndsWith(">"))
		{
			value = value.Substring(1, value.Length - 2);
		}
		string text = $"<a href=\"{value}\"";
		if (!string.IsNullOrEmpty(value2))
		{
			value2 = AttributeEncode(value2);
			value2 = EscapeBoldItalic(value2);
			text += $" title=\"{value2}\"";
		}
		return text + $">{arg}</a>";
	}

	private string DoImages(string text)
	{
		text = _imagesRef.Replace(text, ImageReferenceEvaluator);
		text = _imagesInline.Replace(text, ImageInlineEvaluator);
		return text;
	}

	private string EscapeImageAltText(string s)
	{
		s = EscapeBoldItalic(s);
		s = Regex.Replace(s, "[\\[\\]()]", (Match m) => _escapeTable[m.ToString()]);
		return s;
	}

	private string ImageReferenceEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		string value2 = match.Groups[2].Value;
		string text = match.Groups[3].Value.ToLowerInvariant();
		if (text == "")
		{
			text = value2.ToLowerInvariant();
		}
		if (_urls.ContainsKey(text))
		{
			string url = _urls[text];
			string title = null;
			if (_titles.ContainsKey(text))
			{
				title = _titles[text];
			}
			return ImageTag(url, value2, title);
		}
		return value;
	}

	private string ImageInlineEvaluator(Match match)
	{
		string value = match.Groups[2].Value;
		string text = match.Groups[3].Value;
		string value2 = match.Groups[6].Value;
		if (text.StartsWith("<") && text.EndsWith(">"))
		{
			text = text.Substring(1, text.Length - 2);
		}
		return ImageTag(text, value, value2);
	}

	private string ImageTag(string url, string altText, string title)
	{
		altText = EscapeImageAltText(AttributeEncode(altText));
		url = EncodeProblemUrlChars(url);
		url = EscapeBoldItalic(url);
		string text = $"<img src=\"{url}\" alt=\"{altText}\"";
		if (!string.IsNullOrEmpty(title))
		{
			title = AttributeEncode(EscapeBoldItalic(title));
			text += $" title=\"{title}\"";
		}
		return text + _emptyElementSuffix;
	}

	private string DoHeaders(string text)
	{
		text = _headerSetext.Replace(text, SetextHeaderEvaluator);
		text = _headerAtx.Replace(text, AtxHeaderEvaluator);
		return text;
	}

	private string SetextHeaderEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		int num = (match.Groups[2].Value.StartsWith("=") ? 1 : 2);
		return string.Format("<h{1}>{0}</h{1}>\n\n", RunSpanGamut(value), num);
	}

	private string AtxHeaderEvaluator(Match match)
	{
		string value = match.Groups[2].Value;
		int length = match.Groups[1].Value.Length;
		return string.Format("<h{1}>{0}</h{1}>\n\n", RunSpanGamut(value), length);
	}

	private string DoHorizontalRules(string text)
	{
		return _horizontalRules.Replace(text, "<hr" + _emptyElementSuffix + "\n");
	}

	private string DoLists(string text, bool isInsideParagraphlessListItem = false)
	{
		text = ((_listLevel <= 0) ? _listTopLevel.Replace(text, GetListEvaluator()) : _listNested.Replace(text, GetListEvaluator(isInsideParagraphlessListItem)));
		return text;
	}

	private MatchEvaluator GetListEvaluator(bool isInsideParagraphlessListItem = false)
	{
		return delegate(Match match)
		{
			string value = match.Groups[1].Value;
			string text = (Regex.IsMatch(match.Groups[3].Value, "[*+-]") ? "ul" : "ol");
			string arg = ProcessListItems(value, (text == "ul") ? "[*+-]" : "\\d+[.]", isInsideParagraphlessListItem);
			return string.Format("<{0}>\n{1}</{0}>\n", text, arg);
		};
	}

	private string ProcessListItems(string list, string marker, bool isInsideParagraphlessListItem = false)
	{
		_listLevel++;
		list = Regex.Replace(list, "\\n{2,}\\z", "\n");
		string pattern = string.Format("(^[ ]*)                    # leading whitespace = $1\r\n                ({0}) [ ]+                 # list marker = $2\r\n                ((?s:.+?)                  # list item text = $3\r\n                (\\n+))\r\n                (?= (\\z | \\1 ({0}) [ ]+))", marker);
		bool lastItemHadADoubleNewline = false;
		MatchEvaluator matchEvaluator = delegate(Match match)
		{
			string value = match.Groups[3].Value;
			bool flag = value.EndsWith("\n\n");
			if (flag || value.Contains("\n\n") || lastItemHadADoubleNewline)
			{
				value = RunBlockGamut(Outdent(value) + "\n", unhash: false);
			}
			else
			{
				value = DoLists(Outdent(value), isInsideParagraphlessListItem: true);
				value = value.TrimEnd(new char[1] { '\n' });
				if (!isInsideParagraphlessListItem)
				{
					value = RunSpanGamut(value);
				}
			}
			lastItemHadADoubleNewline = flag;
			return $"<li>{value}</li>\n";
		};
		list = Regex.Replace(list, pattern, matchEvaluator.Invoke, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
		_listLevel--;
		return list;
	}

	private string DoCodeBlocks(string text)
	{
		text = _codeBlock.Replace(text, CodeBlockEvaluator);
		return text;
	}

	private string CodeBlockEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		value = EncodeCode(Outdent(value));
		value = _newlinesLeadingTrailing.Replace(value, "");
		return "\n\n<pre><code>" + value + "\n</code></pre>\n\n";
	}

	private string DoCodeSpans(string text)
	{
		return _codeSpan.Replace(text, CodeSpanEvaluator);
	}

	private string CodeSpanEvaluator(Match match)
	{
		string value = match.Groups[2].Value;
		value = Regex.Replace(value, "^[ ]*", "");
		value = Regex.Replace(value, "[ ]*$", "");
		value = EncodeCode(value);
		value = SaveFromAutoLinking(value);
		return "<code>" + value + "</code>";
	}

	private string DoItalicsAndBold(string text)
	{
		if (_strictBoldItalic)
		{
			text = _strictBold.Replace(text, "$1<strong>$3</strong>");
			text = _strictItalic.Replace(text, "$1<em>$3</em>");
		}
		else
		{
			text = _bold.Replace(text, "<strong>$2</strong>");
			text = _italic.Replace(text, "<em>$2</em>");
		}
		return text;
	}

	private string DoHardBreaks(string text)
	{
		text = ((!_autoNewlines) ? Regex.Replace(text, " {2,}\\n", $"<br{_emptyElementSuffix}\n") : Regex.Replace(text, "\\n", $"<br{_emptyElementSuffix}\n"));
		return text;
	}

	private string DoBlockQuotes(string text)
	{
		return _blockquote.Replace(text, BlockQuoteEvaluator);
	}

	private string BlockQuoteEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		value = Regex.Replace(value, "^[ ]*>[ ]?", "", RegexOptions.Multiline);
		value = Regex.Replace(value, "^[ ]+$", "", RegexOptions.Multiline);
		value = RunBlockGamut(value);
		value = Regex.Replace(value, "^", "  ", RegexOptions.Multiline);
		value = Regex.Replace(value, "(\\s*<pre>.+?</pre>)", BlockQuoteEvaluator2, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
		value = $"<blockquote>\n{value}\n</blockquote>";
		string hashKey = GetHashKey(value, isHtmlBlock: true);
		_htmlBlocks[hashKey] = value;
		return "\n\n" + hashKey + "\n\n";
	}

	private string BlockQuoteEvaluator2(Match match)
	{
		return Regex.Replace(match.Groups[1].Value, "^  ", "", RegexOptions.Multiline);
	}

	private static string handleTrailingParens(Match match)
	{
		if (match.Groups[1].Success)
		{
			return match.Value;
		}
		string value = match.Groups[2].Value;
		string text = match.Groups[3].Value;
		if (!text.EndsWith(")"))
		{
			return "<" + value + text + ">";
		}
		int num = 0;
		foreach (Match item in Regex.Matches(text, "[()]"))
		{
			num = ((!(item.Value == "(")) ? (num - 1) : ((num <= 0) ? 1 : (num + 1)));
		}
		string tail = "";
		if (num < 0)
		{
			text = Regex.Replace(text, "\\){1," + -num + "}$", delegate(Match m)
			{
				tail = m.Value;
				return "";
			});
		}
		if (tail.Length > 0)
		{
			char c = text[text.Length - 1];
			if (!_endCharRegex.IsMatch(c.ToString()))
			{
				tail = c + tail;
				text = text.Substring(0, text.Length - 1);
			}
		}
		return "<" + value + text + ">" + tail;
	}

	private string DoAutoLinks(string text)
	{
		if (_autoHyperlink)
		{
			text = _autolinkBare.Replace(text, handleTrailingParens);
		}
		text = Regex.Replace(text, "<((https?|ftp):[^'\">\\s]+)>", HyperlinkEvaluator);
		if (_linkEmails)
		{
			string pattern = "<\r\n                      (?:mailto:)?\r\n                      (\r\n                        [-.\\w]+\r\n                        \\@\r\n                        [-a-z0-9]+(\\.[-a-z0-9]+)*\\.[a-z]+\r\n                      )\r\n                      >";
			text = Regex.Replace(text, pattern, EmailEvaluator, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}
		return text;
	}

	private string HyperlinkEvaluator(Match match)
	{
		string value = match.Groups[1].Value;
		return $"<a href=\"{EscapeBoldItalic(EncodeProblemUrlChars(value))}\">{value}</a>";
	}

	private string EmailEvaluator(Match match)
	{
		string text = Unescape(match.Groups[1].Value);
		text = "mailto:" + text;
		text = EncodeEmailAddress(text);
		text = string.Format("<a href=\"{0}\">{0}</a>", text);
		return Regex.Replace(text, "\">.+?:", "\">");
	}

	private string Outdent(string block)
	{
		return _outDent.Replace(block, "");
	}

	private string EncodeEmailAddress(string addr)
	{
		StringBuilder stringBuilder = new StringBuilder(addr.Length * 5);
		Random random = new Random();
		foreach (char c in addr)
		{
			int num = random.Next(1, 100);
			if ((num > 90 || c == ':') && c != '@')
			{
				stringBuilder.Append(c);
			}
			else if (num < 45)
			{
				stringBuilder.AppendFormat("&#x{0:x};", (int)c);
			}
			else
			{
				stringBuilder.AppendFormat("&#{0};", (int)c);
			}
		}
		return stringBuilder.ToString();
	}

	private string EncodeCode(string code)
	{
		return _codeEncoder.Replace(code, EncodeCodeEvaluator);
	}

	private string EncodeCodeEvaluator(Match match)
	{
		return match.Value switch
		{
			"&" => "&amp;", 
			"<" => "&lt;", 
			">" => "&gt;", 
			_ => _escapeTable[match.Value], 
		};
	}

	private string EncodeAmpsAndAngles(string s)
	{
		s = _amps.Replace(s, "&amp;");
		s = _angles.Replace(s, "&lt;");
		return s;
	}

	private string EscapeBackslashes(string s)
	{
		return _backslashEscapes.Replace(s, EscapeBackslashesEvaluator);
	}

	private string EscapeBackslashesEvaluator(Match match)
	{
		return _backslashEscapeTable[match.Value];
	}

	private string Unescape(string s)
	{
		return _unescapes.Replace(s, UnescapeEvaluator);
	}

	private string UnescapeEvaluator(Match match)
	{
		return _invertedEscapeTable[match.Value];
	}

	private string EscapeBoldItalic(string s)
	{
		s = s.Replace("*", _escapeTable["*"]);
		s = s.Replace("_", _escapeTable["_"]);
		return s;
	}

	private static string AttributeEncode(string s)
	{
		return s.Replace(">", "&gt;").Replace("<", "&lt;").Replace("\"", "&quot;");
	}

	private string EncodeProblemUrlChars(string url)
	{
		if (!_encodeProblemUrlCharacters)
		{
			return url;
		}
		StringBuilder stringBuilder = new StringBuilder(url.Length);
		for (int i = 0; i < url.Length; i++)
		{
			char c = url[i];
			bool flag = Array.IndexOf(_problemUrlChars, c) != -1;
			if (flag && c == ':' && i < url.Length - 1)
			{
				flag = url[i + 1] != '/' && (url[i + 1] < '0' || url[i + 1] > '9');
			}
			if (flag)
			{
				stringBuilder.Append("%" + $"{(byte)c:x}");
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private string EscapeSpecialCharsWithinTagAttributes(string text)
	{
		List<Token> list = TokenizeHTML(text);
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		foreach (Token item in list)
		{
			string text2 = item.Value;
			if (item.Type == TokenType.Tag)
			{
				text2 = text2.Replace("\\", _escapeTable["\\"]);
				if (_autoHyperlink && text2.StartsWith("<!"))
				{
					text2 = text2.Replace("/", _escapeTable["/"]);
				}
				text2 = Regex.Replace(text2, "(?<=.)</?code>(?=.)", _escapeTable["`"]);
				text2 = EscapeBoldItalic(text2);
			}
			stringBuilder.Append(text2);
		}
		return stringBuilder.ToString();
	}

	private string Normalize(string text)
	{
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		StringBuilder stringBuilder2 = new StringBuilder();
		bool flag = false;
		for (int i = 0; i < text.Length; i++)
		{
			switch (text[i])
			{
			case '\n':
				if (flag)
				{
					stringBuilder.Append((object?)stringBuilder2);
				}
				stringBuilder.Append('\n');
				stringBuilder2.Length = 0;
				flag = false;
				break;
			case '\r':
				if (i < text.Length - 1 && text[i + 1] != '\n')
				{
					if (flag)
					{
						stringBuilder.Append((object?)stringBuilder2);
					}
					stringBuilder.Append('\n');
					stringBuilder2.Length = 0;
					flag = false;
				}
				break;
			case '\t':
			{
				int num = 4 - stringBuilder2.Length % 4;
				for (int j = 0; j < num; j++)
				{
					stringBuilder2.Append(' ');
				}
				break;
			}
			default:
				if (!flag && text[i] != ' ')
				{
					flag = true;
				}
				stringBuilder2.Append(text[i]);
				break;
			case '\u001a':
				break;
			}
		}
		if (flag)
		{
			stringBuilder.Append((object?)stringBuilder2);
		}
		stringBuilder.Append('\n');
		return stringBuilder.Append("\n\n").ToString();
	}

	private static string RepeatString(string text, int count)
	{
		StringBuilder stringBuilder = new StringBuilder(text.Length * count);
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}
}
