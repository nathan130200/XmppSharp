using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0047;

[XmppTag("close", Namespaces.Ibb)]
public class Close : XmppElement
{
    public Close() : base("close", Namespaces.Ibb)
    {

    }

    public string? SessionId
    {
        get => GetAttribute("sid");
        set => SetAttribute("sid", value);
    }
}
