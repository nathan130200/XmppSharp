using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

[Tag("session", Namespaces.Session)]
public class Session : XmppElement
{
	public Session() : base("session", Namespaces.Session)
	{

	}
}