using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("message", Namespace.Client)]
[XmppTag("message", Namespace.Server)]
[XmppTag("message", Namespace.Accept)]
[XmppTag("message", Namespace.Connect)]
public class Message : Stanza
{
    public Message() : base("message", Namespace.Client)
    {

    }

    public Message(MessageType type) : this()
        => Type = type;

    public new MessageType Type
    {
        get => XmppEnum.ParseOrDefault(base.Type, MessageType.Normal);
        set
        {
            if (value == MessageType.Normal)
                base.Type = null;
            else
                base.Type = value.ToXmppName();
        }
    }

    public string Body
    {
        get => GetTag("body");
        set
        {
            if (string.IsNullOrEmpty(value))
                RemoveTag("body");
            else
                SetTag("body", value);
        }
    }

    public bool IsXHtml
        => HasTag("body", Namespace.XHtml);

    public string Subject
    {
        get => GetTag("subject");
        set
        {
            if (string.IsNullOrEmpty(value))
                RemoveTag("subject");
            else
                SetTag("subject", value);
        }
    }

    public string Thread
    {
        get => GetTag("thread");
        set
        {
            if (string.IsNullOrEmpty(value))
                RemoveTag("thread");
            else
                SetTag("thread", value);
        }
    }
}