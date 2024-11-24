using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Extensions.XEP0045;
using XmppSharp.Protocol.Extensions.XEP0172;

namespace XmppSharp.Protocol.Core;

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

    public Presence(PresenceType type, PresenceShow? show = default, byte? priority = default) : this()
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
            {
                SetTag(x =>
                {
                    x.TagName = "show";
                    x.Value = XmppEnum.ToXml(value);
                });
            }
        }
    }

    // priority: -1, 0~255
    // where -1 exclude from bare JID routing
    // 0~255 client priority in bare JID routing

    public int? Priority
    {
        get
        {
            if (int.TryParse(GetTag("priority"), out var result))
                return Math.Clamp(result, -1, 255);

            return null;
        }
        set
        {
            RemoveTag("priority");

            if (value.HasValue)
            {
                var priority = Math.Clamp((int)value, -1, 255);

                SetTag(x =>
                {
                    x.TagName = "priority";
                    x.SetValue(priority);
                });
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
            {
                SetTag(x =>
                {
                    x.TagName = "status";
                    x.Value = value;
                });
            }
        }
    }

    public MucUser? User
    {
        get => Child<MucUser>();
        set
        {
            Child<MucUser>()?.Remove();
            AddChild(value);
        }
    }

    public Nickname? Nickname
    {
        get => Child<Nickname>();
        set
        {
            Child<Nickname>()?.Remove();
            AddChild(value);
        }
    }
}
