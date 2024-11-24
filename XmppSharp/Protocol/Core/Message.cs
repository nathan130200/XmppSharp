using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Extensions.XEP0085;

namespace XmppSharp.Protocol.Core;

[XmppTag("message", Namespaces.Client)]
[XmppTag("message", Namespaces.Server)]
[XmppTag("message", Namespaces.Accept)]
[XmppTag("message", Namespaces.Connect)]
public class Message : Stanza
{
    public Message() : base("message")
    {

    }

    public new MessageType Type
    {
        get => XmppEnum.FromXmlOrDefault(base.Type, MessageType.Normal);
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException(null, nameof(Type));

            base.Type = XmppEnum.ToXml(value);
        }
    }

    public ChatStates? ChatState
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetXmlMapping<ChatStates>())
            {
                if (HasTag(key, Namespaces.ChatStates))
                    return value;
            }

            return default;
        }
        set
        {
            this.Children(e => e.Namespace == Namespaces.ChatStates).Remove();

            if (value.HasValue)
            {
                SetTag(x =>
                {
                    x.TagName = XmppEnum.ToXml(value)!;
                    x.Namespace = Namespaces.ChatStates;
                });
            }
        }
    }

    public string? Body
    {
        get => GetTag("body");
        set
        {
            RemoveTag("body");

            if (value != null)
            {
                SetTag(x =>
                {
                    x.TagName = "body";
                    x.Value = value;
                });
            }
        }
    }
}