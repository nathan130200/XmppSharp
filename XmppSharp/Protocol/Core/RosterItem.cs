using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core;

[XmppTag("item", Namespaces.IqRoster)]
public class RosterItem : Element
{
    public RosterItem() : base("item", Namespaces.IqRoster)
    {

    }

    public RosterItem(Jid? jid, string? name, RosterSubscriptionType? subscription) : this()
    {
        Jid = jid;
        Name = name;
        Subscription = subscription;
    }

    public IEnumerable<string> Groups
    {
        get => Children("group").Select(x => x.Value!);
        set
        {
            Children("group").Remove();

            foreach (var item in value.Where(x => x != null))
                SetTag("group", value: item);
        }
    }

    public Jid? Jid
    {
        get => GetAttribute("jid");
        set => SetAttribute("jid", value);
    }

    public string? Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }

    public RosterSubscriptionType? Subscription
    {
        get => XmppEnum.FromXml(GetAttribute("subscription"), RosterSubscriptionType.None);
        set => SetAttribute("subscription", XmppEnum.ToXml(value));
    }

    public bool? Approved
    {
        get => this.GetAttribute<bool>("approved");
        set => SetAttribute("approved", value);
    }

    public bool Ask
    {
        get => GetAttribute("ask") == "subscribe";
        set
        {
            if (!value)
                RemoveAttribute("ask");
            else
                SetAttribute("ask", "subscribe");
        }
    }
}
