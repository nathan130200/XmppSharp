namespace XmppSharp.Protocol.Extensions.XEP0030;

public class Item(Jid jid, string? name = default, string? node = default)
{
	public Jid Jid { get; } = jid;

	public string? Name { get; } = name;

	public string? Node { get; } = node;
}