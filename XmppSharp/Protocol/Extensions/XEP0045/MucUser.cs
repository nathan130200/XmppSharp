using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[XmppTag("x", Namespaces.MucUser)]
public class MucUser : Element
{
    public MucUser() : base("x", Namespaces.MucUser)
    {

    }

    public Decline? Decline
    {
        get => Child<Decline>();
        set
        {
            Child<Decline>()?.Remove();
            AddChild(value);
        }
    }

    public Destroy? Destroy
    {
        get => Child<Destroy>();
        set
        {
            Child<Destroy>()?.Remove();
            AddChild(value);
        }
    }

    public IEnumerable<Invite> Invites
    {
        get => Children<Invite>();
        set
        {
            Children<Invite>().Remove();

            foreach (var item in value)
                AddChild(item);
        }
    }

    public Item? Item
    {
        get => Child<Item>();
        set
        {
            Child<Item>()?.Remove();
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
        get => Children<Status>();
        set
        {
            Children<Status>().Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }
}
