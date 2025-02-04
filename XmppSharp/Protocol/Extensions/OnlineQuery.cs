using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions;

[XmppTag("query", Namespaces.CryOnline)]
public class OnlineQuery : XmppElement
{
    public OnlineQuery() : base("query", Namespaces.CryOnline)
    {

    }
}
