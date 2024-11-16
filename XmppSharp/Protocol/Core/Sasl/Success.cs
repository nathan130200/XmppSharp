using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppTag("success", Namespaces.Sasl)]
public class Success : Element
{
    public Success() : base("success", Namespaces.Sasl)
    {

    }
}
