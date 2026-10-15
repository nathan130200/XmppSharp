using Jabber.Dom;

namespace Jabber.Abstractions;

public interface IXmppSerializable
{
    Element? Serialize();
}
