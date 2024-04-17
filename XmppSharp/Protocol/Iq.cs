using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("iq", Namespace.Client)]
[XmppTag("iq", Namespace.Server)]
[XmppTag("iq", Namespace.Accept)]
[XmppTag("iq", Namespace.Connect)]
public class Iq : Stanza
{
	public Iq() : base("iq", Namespace.Client)
	{

	}

	public Iq(IqType type) : this()
		=> this.Type = type;

	public new IqType Type
	{
		get => XmppEnum.ParseOrThrow<IqType>(base.Type);
		set => base.Type = value.ToXmppName();
	}

	public Element Query
	{
		get
		{
			_ = this.TryGetChild("query", Namespace.CryOnline, out Element result)
				|| this.TryGetChild("bind", Namespace.Bind, out result)
				|| this.TryGetChild("session", Namespace.Session, out result)
				|| this.TryGetChild("query", Namespace.DiscoInfo, out result)
				|| this.TryGetChild("query", Namespace.DiscoItems, out result)
				|| this.TryGetChild("ping", Namespace.Ping, out result);

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