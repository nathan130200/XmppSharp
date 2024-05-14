namespace XmppSharp.Expat.Native;

public readonly struct ParsingState
{
	public enum Type
	{
		Initialized,
		Parsing,
		Finished,
		Suspended
	}

	public readonly Type Status;
	public readonly bool IsFinalBuffer;
}
