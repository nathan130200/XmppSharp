using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("success", Namespaces.Sasl)]
public class Success : Element
{
    public Success() : base("success", Namespaces.Sasl)
    {

    }
}
