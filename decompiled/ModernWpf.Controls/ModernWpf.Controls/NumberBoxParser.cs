using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ModernWpf.Controls;

internal class NumberBoxParser
{
	private const string c_numberBoxOperators = "+-*/^";

	private static readonly MathToken[] s_emptyTokens = new MathToken[0];

	private static IList<MathToken> GetTokens(string input, INumberBoxNumberFormatter numberParser)
	{
		List<MathToken> list = new List<MathToken>();
		bool flag = true;
		while (input.Length > 0)
		{
			char c = input[0];
			if (c != ' ')
			{
				if (flag)
				{
					if (c == '(')
					{
						list.Add(new MathToken(MathTokenType.Parenthesis, c));
					}
					else
					{
						var (d, num) = GetNextNumber(input, numberParser);
						if (num <= 0)
						{
							return s_emptyTokens;
						}
						list.Add(new MathToken(MathTokenType.Numeric, d));
						input = input.SafeSubstring(num - 1);
						flag = false;
					}
				}
				else if ("+-*/^".IndexOf(c) >= 0)
				{
					list.Add(new MathToken(MathTokenType.Operator, c));
					flag = true;
				}
				else
				{
					if (c != ')')
					{
						return s_emptyTokens;
					}
					list.Add(new MathToken(MathTokenType.Parenthesis, c));
				}
			}
			input = input.SafeSubstring(1);
		}
		return list;
	}

	private static (double, int) GetNextNumber(string input, INumberBoxNumberFormatter numberParser)
	{
		Match match = new Regex("^-?([^-+/*\\(\\)\\^\\s]+)").Match(input);
		if (match.Success)
		{
			int length = match.Value.Length;
			double? num = numberParser.ParseDouble(input.Substring(0, length));
			if (num.HasValue)
			{
				return (num.Value, length);
			}
		}
		return (double.NaN, 0);
	}

	private static int GetPrecedenceValue(char c)
	{
		int result = 0;
		switch (c)
		{
		case '*':
		case '/':
			result = 1;
			break;
		case '^':
			result = 2;
			break;
		}
		return result;
	}

	private static IList<MathToken> ConvertInfixToPostfix(IList<MathToken> infixTokens)
	{
		List<MathToken> list = new List<MathToken>();
		Stack<MathToken> stack = new Stack<MathToken>();
		foreach (MathToken infixToken in infixTokens)
		{
			if (infixToken.Type == MathTokenType.Numeric)
			{
				list.Add(infixToken);
			}
			else if (infixToken.Type == MathTokenType.Operator)
			{
				while (stack.Count > 0)
				{
					MathToken mathToken = stack.Peek();
					if (mathToken.Type == MathTokenType.Parenthesis || GetPrecedenceValue(mathToken.Char) < GetPrecedenceValue(infixToken.Char))
					{
						break;
					}
					list.Add(stack.Pop());
				}
				stack.Push(infixToken);
			}
			else
			{
				if (infixToken.Type != MathTokenType.Parenthesis)
				{
					continue;
				}
				if (infixToken.Char == '(')
				{
					stack.Push(infixToken);
					continue;
				}
				while (stack.Count > 0 && stack.Peek().Char != '(')
				{
					list.Add(stack.Pop());
				}
				if (stack.Count == 0)
				{
					return s_emptyTokens;
				}
				stack.Pop();
			}
		}
		while (stack.Count > 0)
		{
			if (stack.Peek().Type == MathTokenType.Parenthesis)
			{
				return s_emptyTokens;
			}
			list.Add(stack.Pop());
		}
		return list;
	}

	private static double? ComputePostfixExpression(IList<MathToken> tokens)
	{
		Stack<double> stack = new Stack<double>();
		foreach (MathToken token in tokens)
		{
			if (token.Type == MathTokenType.Operator)
			{
				if (stack.Count < 2)
				{
					return null;
				}
				double num = stack.Pop();
				double num2 = stack.Pop();
				double item;
				switch (token.Char)
				{
				case '-':
					item = num2 - num;
					break;
				case '+':
					item = num + num2;
					break;
				case '*':
					item = num * num2;
					break;
				case '/':
					if (num == 0.0)
					{
						return double.NaN;
					}
					item = num2 / num;
					break;
				case '^':
					item = Math.Pow(num2, num);
					break;
				default:
					return null;
				}
				stack.Push(item);
			}
			else if (token.Type == MathTokenType.Numeric)
			{
				stack.Push(token.Value);
			}
		}
		if (stack.Count != 1)
		{
			return null;
		}
		return stack.Peek();
	}

	public static double? Compute(string expr, INumberBoxNumberFormatter numberParser)
	{
		IList<MathToken> tokens = GetTokens(expr, numberParser);
		if (tokens.Count > 0)
		{
			IList<MathToken> list = ConvertInfixToPostfix(tokens);
			if (list.Count > 0)
			{
				return ComputePostfixExpression(list);
			}
		}
		return null;
	}
}
