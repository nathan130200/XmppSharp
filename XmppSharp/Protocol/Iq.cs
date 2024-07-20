#pragma warning disable IDE0018

using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("iq", Namespaces.Client)]
[XmppTag("iq", Namespaces.Server)]
[XmppTag("iq", Namespaces.Accept)]
[XmppTag("iq", Namespaces.Connect)]
public class Iq : Stanza
{
	public Iq() : base("iq", Namespaces.Client)
	{

	}

	public Iq(IqType type) : this()
		=> this.Type = type;

	public new IqType Type
	{
		get => XmppEnum.ParseOrThrow<IqType>(base.Type!);
		set => base.Type = value.ToXmppName();
	}

	public Element? Query
	{
		get
		{
			Element? result;

			_ = this.TryGetChild("query", Namespaces.CryOnline, out result)
				|| this.TryGetChild("bind", Namespaces.Bind, out result)
				|| this.TryGetChild("session", Namespaces.Session, out result)
				|| this.TryGetChild("query", Namespaces.DiscoInfo, out result)
				|| this.TryGetChild("query", Namespaces.DiscoItems, out result)
				|| this.TryGetChild("ping", Namespaces.Ping, out result)
				|| this.TryGetChild("vCard", Namespaces.vCard, out result);

			return result;
		}
		set
		{
			this.Query?.Remove();

			if (value != null)
				this.AddChild(value);
		}
	}
}