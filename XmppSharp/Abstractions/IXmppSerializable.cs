using XmppSharp.Dom;

namespace XmppSharp.Abstractions;

public interface IXmppSerializable
{
    Element? Serialize();
}
