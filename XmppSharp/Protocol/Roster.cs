using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol;

[XmppTag("query", Namespaces.IqRoster)]
public class Roster : XmppElement
{
    public Roster() : base("query", Namespaces.IqRoster)
    {

    }

    public IEnumerable<RosterItem> Items
    {
        get => Elements<RosterItem>();
        set
        {
            Elements<RosterItem>().Remove();

            foreach (var item in value)
                AddChild(item);
        }
    }

    public Roster AddRosterItem(RosterItem item)
    {
        AddChild(item);
        return this;
    }

    public Roster AddRosterItem(Jid jid, string? name = default, RosterSubscriptionType? subscription = default)
    {
        AddChild(new RosterItem(jid, name, subscription));
        return this;
    }

    public string? Ver
    {
        get => GetAttribute("ver");
        set => SetAttribute("ver", value);
    }
}
