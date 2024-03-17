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
        get
        {
            if (base.Type == null)
                return MessageType.Normal;

            return XmppEnum.FromXml(base.Type, MessageType.Normal);
        }
        set
        {
            if (value == MessageType.Normal)
                base.Type = null;
            else
                base.Type = XmppEnum.ToXml(value);
        }
    }

    public string Body
    {
        get
        {
            if (!HasTag("body"))
                return default;

            var innerText = GetChild("body")
                .Descendants()
                .Select(x => x.Value);

            return string.Concat(innerText);
        }
        set
        {
            if (string.IsNullOrEmpty(value))
                RemoveTag("body");
            else
                SetTag("body", value);
        }
    }

    public bool IsHtml
        => HasTag("body", "http://www.w3.org/1999/xhtml");

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