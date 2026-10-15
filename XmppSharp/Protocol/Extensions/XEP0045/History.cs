namespace XmppSharp.Protocol.Extensions.XEP0045;

public sealed record History
{
	public int? MaxChars { get; init; }
	public int? MaxStanzas { get; init; }
	public int? Seconds { get; init; }
	public DateTimeOffset? Since { get; init; }
}