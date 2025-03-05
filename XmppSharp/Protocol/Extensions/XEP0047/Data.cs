using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Extensions;

namespace XmppSharp.Protocol.Extensions.XEP0047;

[XmppTag("data", Namespaces.Ibb)]
public class Data : XmppElement
{
    public Data() : base("data", Namespaces.Ibb)
    {

    }

    public ushort? Seq
    {
        get => (ushort?)this.GetAttributeInt16("seq", 0);
        set => SetAttribute("seq", value);
    }

    public string? SessionId
    {
        get => GetAttribute("sid");
        set => SetAttribute("sid", value);
    }

    public void SetBytes(byte[] buffer)
        => Value = Convert.ToBase64String(buffer);
}