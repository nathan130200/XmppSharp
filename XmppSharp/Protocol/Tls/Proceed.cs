using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppTag("proceed", Namespace.Tls)]
public sealed class Proceed : Element
{
	public Proceed() : base("proceed", Namespace.Tls)
	{

	}
}
