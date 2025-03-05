﻿using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Extensions.XEP0045;
using XmppSharp.Protocol.Extensions.XEP0172;

namespace XmppSharp.Protocol;

[XmppTag("precense", Namespaces.Client)]
[XmppTag("precense", Namespaces.Server)]
[XmppTag("precense", Namespaces.Connect)]
[XmppTag("precense", Namespaces.Accept)]
public class Presence : Stanza
{
    public Presence() : base("presence", Namespaces.Client)
    {

    }

    public Presence(Presence other) : base(other)
    {

    }

    public Presence(PresenceType type, PresenceShow? show = default, int? priority = default) : this()
    {
        Type = type;
        Show = show;
        Priority = priority;
    }

    public new PresenceType Type
    {
        get => XmppEnum.FromXmlOrDefault(base.Type, PresenceType.Available);
        set
        {
            if (value == PresenceType.Available)
                base.Type = null;
            else
            {
                if (!Enum.IsDefined(value))
                    throw new ArgumentException(default, nameof(Type));

                base.Type = XmppEnum.ToXml(value);
            }
        }
    }

    public PresenceShow? Show
    {
        get
        {
            if (!HasTag("show"))
                return null;

            return XmppEnum.FromXml<PresenceShow>(GetTag("show"));
        }
        set
        {
            RemoveTag("show");

            if (value.HasValue)
                SetTag("show", value: XmppEnum.ToXml(value));
        }
    }

    public int? Priority
    {
        get
        {
            if (int.TryParse(GetTag("priority"), out var result))
                return Math.Clamp(result, -1, 255);

            return default;
        }
        set
        {
            RemoveTag("priority");

            if (value.HasValue)
            {
                var priority = Math.Clamp((int)value, -1, 255);
                SetTag("priority", value: priority);
            }
        }
    }

    public string? Status
    {
        get => GetTag("status");
        set
        {
            RemoveTag("status");

            if (value != null)
                SetTag("status", value: value);
        }
    }

    public MucUser? User
    {
        get => Element<MucUser>();
        set
        {
            Element<MucUser>()?.Remove();
            AddChild(value);
        }
    }

    public Nickname? Nickname
    {
        get => Element<Nickname>();
        set
        {
            Element<Nickname>()?.Remove();
            AddChild(value);
        }
    }
}
