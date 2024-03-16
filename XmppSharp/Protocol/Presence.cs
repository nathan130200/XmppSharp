using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("presence", Namespace.Client)]
[XmppTag("presence", Namespace.Server)]
[XmppTag("presence", Namespace.Accept)]
[XmppTag("presence", Namespace.Connect)]
public class Presence : Stanza
{
    public Presence() : base("presence")
    {

    }

    public Presence(PresenceType type) : this()
        => Type = type;

    public new PresenceType Type
    {
        get => XmppEnum.FromXml(base.Type, PresenceType.Available);
        set
        {
            if (value == PresenceType.Available)
                base.Type = null;
            else
                base.Type = XmppEnum.ToXml(value);
        }
    }

    public byte Priority
    {
        get
        {
            var priority = GetTag("priority");

            if (priority == null)
                return 0;

            return byte.Parse(priority);
        }
        set
        {
            if (value == 0)
                RemoveTag("priority");
            else
                SetTag("priority", value.ToString());
        }
    }

    public PresenceShow? Show
    {
        get => XmppEnum.FromXml<PresenceShow>(GetTag("show"));
        set
        {
            if (!value.TryGetValue(out var newValue))
                RemoveTag("show");
            else
                SetTag("show", XmppEnum.ToXml(newValue));
        }
    }

    public string? Status
    {
        get => GetTag("status");
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                RemoveTag("status");
            else
                SetTag("status", value);
        }
    }
}
