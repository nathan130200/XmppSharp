namespace XmppSharp.XpNet;

public class TokenException : Exception
{
	public TokenException()
	{
	}

	public TokenException(string? message) : base(message)
	{
	}

	public TokenException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}

public class EmptyTokenException : TokenException
{

}

public class EndOfPrologException : TokenException
{

}

public class ExtensibleTokenException : TokenException
{
	public TokenType Type { get; }

	public ExtensibleTokenException(TokenType type)
		=> Type = type;
}

public class InvalidTokenException : TokenException
{
	public int Offset { get; set; }
	public InvalidTokenType Type { get; }

	public InvalidTokenException(int offset, InvalidTokenType type)
	{
		Offset = offset;
		Type = type;
	}

	public InvalidTokenException(int offset) : this(offset, InvalidTokenType.IllegalChar)
	{

	}
}

public class PartialCharException : TokenException
{
	public int LeadByteIndex { get; }

	public PartialCharException(int leadByteIndex)
		=> LeadByteIndex = leadByteIndex;
}

public class PartialTokenException : TokenException
{

}

public enum InvalidTokenType
{
	IllegalChar,
	XmlTarget,
	DuplicatedAttribute
}