using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[Tag("actor", Namespaces.MucUser)]
[Tag("actor", Namespaces.MucAdmin)]
public class Actor : XmppElement
{
	public Actor() : base("actor")
	{

	}

	public Actor(Jid? jid) : this()
	{
		Jid = jid;
	}

	public Jid? Jid
	{
		get => GetAttribute("jid");
		set => SetAttribute("jid", value);
	}
}