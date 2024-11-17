using XmppSharp.Attributes;
using XmppSharp.Dom;
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

    public string? Content
    {
        get => Body?.Value;
        set
        {
            Body?.Remove();

            if (value != null)
            {
                Body = new Element("body")
                {
                    Namespace = DefaultNamespace,
                    Value = value
                };
            }
        }
    }

    public new MessageType Type
    {
        get => XmppEnum.FromXml(base.Type, MessageType.Normal);
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException(null, nameof(Type));

            base.Type = XmppEnum.ToXml(value);
        }
    }

    public Element? Subject
    {
        get => this.Child(e => e.TagName == "subject" && !e.HasAttribute("xml:lang"));
        set
        {
            Subject?.Remove();

            if (value != null)
            {
                ThrowHelper.ThrowIfNotEquals("subject", value.TagName);
                value.Namespace ??= DefaultNamespace;
                AddChild(value);
            }
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
                SetTag(XmppEnum.ToXml(value)!, Namespaces.ChatStates);
        }
    }

    public Element? Body
    {
        get => this.Child(e => e.TagName == "body" && !e.HasAttribute("xml:lang"));
        set
        {
            Body?.Remove();

            if (value != null)
            {
                ThrowHelper.ThrowIfNotEquals(value.TagName, "body");
                value.Namespace ??= DefaultNamespace;
                AddChild(value);
            }
        }
    }

    public Element? Thread
    {
        get => this.Child(e => e.TagName == "thread" && !e.HasAttribute("xml:lang"));
        set
        {
            Thread?.Remove();

            if (value != null)
            {
                ThrowHelper.ThrowIfNotEquals(value.TagName, "thread");
                value.Namespace ??= DefaultNamespace;
                AddChild(value);
            }
        }
    }
}