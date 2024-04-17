using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("item", Namespace.DiscoItems)]
public class Item : Element
{
	public Item() : base("item", Namespace.DiscoItems)
	{

	}

	public Item(Jid? jid, string? name) : this()
	{
		this.Jid = jid;
		this.Name = name;
	}

	public Jid? Jid
	{
		get => this.GetAttribute("jid");
		set => this.SetAttribute("jid", value);
	}

	public string? Name
	{
		get => this.GetAttribute("name");
		set => this.SetAttribute("name", value);
	}

	public string? Node
	{
		get => this.GetAttribute("node");
		set => this.SetAttribute("node", value);
	}
}