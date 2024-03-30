using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("success", Namespaces.Sasl)]
public sealed class Success : Element
{
    public Success() : base("success", Namespaces.Sasl)
    {

    }
}
