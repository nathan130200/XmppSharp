using XmppSharp.Abstractions;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

[StanzaExtension("session", Namespaces.Session)]
public sealed class Session : StanzaExtension
{
	public Session()
	{

	}

	internal Session(Element element) : base(element)
	{

	}

	public override Element Serialize() => new("session", Namespaces.Session);
}
