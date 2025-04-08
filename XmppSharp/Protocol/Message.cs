﻿using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Extensions.XEP0085;

namespace XmppSharp.Protocol;

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
        set => base.Type = XmppEnum.ToXmlOrThrow(value);
    }

    public ChatStates? ChatState
    {
        get
        {
            foreach (var (key, value) in XmppEnum.XmlMapping<ChatStates>())
            {
                if (HasTag(key, Namespaces.ChatStates))
                    return value;
            }

            return default;
        }
        set
        {
            this.Elements(e => e.Namespace == Namespaces.ChatStates).Remove();

            if (value.HasValue)
                SetTag(XmppEnum.ToXml(value)!, Namespaces.ChatStates);
        }
    }

    public string? Body
    {
        get => GetTag("body");
        set
        {
            RemoveTag("body");

            if (value != null)
                SetTag("body", value: value);
        }
    }
}