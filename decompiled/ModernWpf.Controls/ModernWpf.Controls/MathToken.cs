namespace ModernWpf.Controls;

internal struct MathToken
{
	public MathTokenType Type;

	public char Char;

	public double Value;

	public MathToken(MathTokenType t, char c)
	{
		Type = t;
		Char = c;
		Value = double.NaN;
	}

	public MathToken(MathTokenType t, double d)
	{
		Type = t;
		Value = d;
		Char = '\0';
	}
}
