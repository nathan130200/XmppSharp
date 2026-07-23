using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

[Tag("session", Namespaces.Session)]
public sealed class Session() : Element("session", Namespaces.Session)
{

}