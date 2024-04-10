using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("message", "jabber:client")]
[XmppTag("message", "jabber:server")]
[XmppTag("message", "jabber:component:accept")]
[XmppTag("message", "jabber:component:connect")]
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
        get => this.GetTag("body");
        set
        {
            if (string.IsNullOrEmpty(value))
                this.RemoveTag("body");
            else
                this.SetTag("body", value);
        }
    }

    public bool IsXHtml
        => this.HasTag(Namespace.XHtml + "body");

    public string Subject
    {
        get => this.GetTag("subject");
        set
        {
            if (string.IsNullOrEmpty(value))
                this.RemoveTag("subject");
            else
                this.SetTag("subject", value);
        }
    }

    public string Thread
    {
        get => this.GetTag("thread");
        set
        {
            if (string.IsNullOrEmpty(value))
                this.RemoveTag("thread");
            else
                this.SetTag("thread", value);
        }
    }
}