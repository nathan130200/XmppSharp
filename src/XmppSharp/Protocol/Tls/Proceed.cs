using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

[Tag("proceed", Namespaces.Tls)]
public sealed class Proceed() : Element("proceed", Namespaces.Tls)
{

}
