using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppTag("proceed", Namespaces.Tls)]
public sealed class Proceed : Element
{
	public Proceed() : base("proceed", Namespaces.Tls)
	{

	}
}
