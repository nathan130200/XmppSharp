using Jabber.Abstractions;
using Jabber.Attributes;
using Jabber.Dom;

namespace Jabber.Protocol.Client;

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
