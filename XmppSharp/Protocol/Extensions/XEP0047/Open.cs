using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0047;

[XmppTag("open", Namespaces.Ibb)]
public class Open : XmppElement
{
    public Open() : base("open", Namespaces.Ibb)
    {

    }

    public ushort BlockSize
    {
        get => (ushort)this.GetAttributeInt16("block-size", 4096);
        set => SetAttribute("block-size", value);
    }

    public string? SessionId
    {
        get => GetAttribute("sid");
        set => SetAttribute("sid", value);
    }

    public string? Stanza
    {
        get => GetAttribute("stanza");
        set => SetAttribute("stanza", value);
    }
}