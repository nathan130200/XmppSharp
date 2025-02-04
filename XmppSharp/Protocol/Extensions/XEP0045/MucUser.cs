using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("x", Namespaces.MucUser)]
public class MucUser : XmppElement
{
    public MucUser() : base("x", Namespaces.MucUser)
    {

    }

    public Decline? Decline
    {
        get => Element<Decline>();
        set
        {
            Element<Decline>()?.Remove();
            AddChild(value);
        }
    }

    public Destroy? Destroy
    {
        get => Element<Destroy>();
        set
        {
            Element<Destroy>()?.Remove();
            AddChild(value);
        }
    }

    public IEnumerable<Invite> Invites
    {
        get => Elements<Invite>();
        set
        {
            Elements<Invite>().Remove();

            foreach (var item in value)
                AddChild(item);
        }
    }

    public Item? Item
    {
        get => Element<Item>();
        set
        {
            Element<Item>()?.Remove();
            AddChild(value);
        }
    }

    public string? Password
    {
        get => GetTag("password");
        set
        {
            RemoveTag("password");

            if (value != null)
                SetTag("password", value: value);
        }
    }

    public IEnumerable<Status> Statuses
    {
        get => Elements<Status>();
        set
        {
            Elements<Status>().Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
