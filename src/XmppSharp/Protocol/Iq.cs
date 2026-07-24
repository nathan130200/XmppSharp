using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[Tag("iq", Namespaces.Client)]
[Tag("iq", Namespaces.Server)]
[Tag("iq", Namespaces.Component)]
public sealed class Iq() : Stanza("iq")
{
    public IqType Type
    {
        get => XmppEnum<IqType>.Parse(GetAttribute("type")!);
        set => SetAttribute("type", XmppEnum<IqType>.GetName(value));
    }
}