using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[Tag("message", Namespaces.Client)]
[Tag("message", Namespaces.Server)]
[Tag("message", Namespaces.Component)]
public sealed class Message() : Stanza("message")
{
    public MessageType Type
    {
        get => XmppEnum<MessageType>.Parse(GetAttribute("type")!);
        set => SetAttribute("type", XmppEnum<MessageType>.GetName(value));
    }

    public string? Body
    {
        get => GetTag("body", Namespace);
        set
        {
            if (value is null)
                RemoveTag("body", Namespace);
            else
                SetTag("body", Namespace);
        }
    }
}