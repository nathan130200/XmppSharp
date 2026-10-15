using System.Xml;

namespace Jabber.Dom.Abstractions;

public interface IXmppNode
{
    Element? Parent { get; }

    IXmppNode Clone();

    void WriteTo(XmlWriter writer);
}
