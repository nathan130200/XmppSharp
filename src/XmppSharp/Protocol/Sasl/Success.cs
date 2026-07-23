using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[Tag("success", Namespaces.Sasl)]
public sealed class Success() : Element("success", Namespaces.Sasl)
{

}