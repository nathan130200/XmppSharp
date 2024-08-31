using System.Diagnostics;

namespace XmppSharp.XpNet;

public readonly record struct CharPair(
	char First, char Second);

[DebuggerDisplay("{ToString(),nq}")]
public class Token
{
	public int TokenEnd { get; set; } = -1;
	public int NameEnd { get; set; } = -1;
	public char RefChar1 { get; set; } = (char)0;
	public char RefChar2 { get; set; } = (char)0;

	public CharPair GetRefCharPair()
		=> new(RefChar1, RefChar2);

	public override string ToString()
		=> $"Token(token_end={TokenEnd}, name_end={NameEnd}, char1={(int)RefChar1:X2}, char2={(int)RefChar2:X2})";
}