using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppTag("success", Namespaces.Sasl)]
public class Success : XmppElement
{
    public Success() : base("success", Namespaces.Sasl)
    {

    }
}
