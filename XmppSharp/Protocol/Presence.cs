using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("presence", Namespace.Client)]
[XmppTag("presence", Namespace.Server)]
[XmppTag("presence", Namespace.Accept)]
[XmppTag("presence", Namespace.Connect)]
public class Presence : Stanza
{
    public Presence() : base("presence", Namespace.Client)
    {

    }

    public Presence(PresenceType type) : this()
        => Type = type;

    public new PresenceType Type
    {
        get => XmppEnum.ParseOrDefault(base.Type, PresenceType.Available);
        set
        {
            if (value == PresenceType.Available)
                base.Type = null;
            else
                base.Type = value.ToXmppName();
        }
    }

    public sbyte Priority
    {
        get
        {
            var value = GetTag("priority");

            if (sbyte.TryParse(value, out var result))
                return result;

            return 0;
        }
        set
        {
            if (value == 0)
                RemoveTag("priority");
            else
                SetTag("priority", value);
        }
    }

    public PresenceShow Show
    {
        get => XmppEnum.ParseOrDefault(GetTag("show"), PresenceShow.Online);
        set
        {
            if (value == PresenceShow.Online)
                RemoveTag("show");
            else
                SetTag("show", value.ToXmppName());
        }
    }

    public string? Status
    {
        get => GetTag("status");
        set
        {
            if (value == null)
                RemoveTag("status");
            else
                SetTag("status", value);
        }
    }
}
