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

            foreach (var groupName in value)
            {
                SetTag(x =>
                {
                    x.TagName = "group";
                    x.Value = groupName;
                });
            }
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
        get => XmppEnum.FromXmlOrDefault(GetAttribute("subscription"), RosterSubscriptionType.None);
        set => SetAttribute("subscription", XmppEnum.ToXml(value));
    }

    public bool? Approved
    {
        get => this.GetAttribute<bool>("approved");
        set
        {
            if (!value.HasValue)
                RemoveAttribute("approved");
            else
                SetAttribute("approved", (bool)value ? 1 : 0);
        }
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
