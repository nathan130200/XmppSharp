using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("presence", "jabber:client")]
[XmppTag("presence", "jabber:server")]
[XmppTag("presence", "jabber:component:accept")]
[XmppTag("presence", "jabber:component:connect")]
public class Presence : Stanza
{
    public Presence() : base(Namespace.Client + "presence")
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
            var value = this.GetTag("priority");

            if (sbyte.TryParse(value, out var result))
                return result;

            return 0;
        }
        set
        {
            if (value == 0)
                this.RemoveTag("priority");
            else
                this.SetTag("priority", value);
        }
    }

    public PresenceShow Show
    {
        get => XmppEnum.ParseOrDefault(this.GetTag("show"), PresenceShow.Online);
        set
        {
            if (value == PresenceShow.Online)
                this.RemoveTag("show");
            else
                this.SetTag("show", value.ToXmppName());
        }
    }

    public string? Status
    {
        get => this.GetTag("status");
        set
        {
            if (value == null)
                this.RemoveTag("status");
            else
                this.SetTag("status", value);
        }
    }
}
