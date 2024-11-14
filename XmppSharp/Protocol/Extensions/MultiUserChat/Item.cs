using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.MultiUserChat;

[XmppTag("item", Namespaces.MucUser)]
[XmppTag("item", Namespaces.MucAdmin)]
public class Item : Element
{
    public Item() : base("item")
    {

    }

    public Affiliation? Affiliation
    {
        get => XmppEnum.FromXmlOrDefault<Affiliation>(GetAttribute("affiliation"));
        set
        {
            if (!value.HasValue)
                RemoveAttribute("affiliation");
            else
                SetAttribute("affiliation", XmppEnum.ToXml(value));
        }
    }

    public Jid? Jid
    {
        get => GetAttribute("jid");
        set => SetAttribute("jid", value);
    }

    public string? Nickname
    {
        get => GetAttribute("nickname");
        set => SetAttribute("nickname", value);
    }

    public Role? Role
    {
        get => XmppEnum.FromXmlOrDefault<Role>(GetAttribute("role"));
        set
        {
            if (!value.HasValue)
                RemoveAttribute("role");
            else
                SetAttribute("role", XmppEnum.ToXml(value));
        }
    }

    public Actor? Actor
    {
        get => Child<Actor>();
        set
        {
            Child<Actor>()?.Remove();
            AddChild(value);
        }
    }

    public string? Reason
    {
        get => Value;
        set => Value = value;
    }

    public bool Continue
    {
        get => HasTag("continue");
        set
        {
            if (!value)
                RemoveTag("continue");
            else
                SetTag("continue");
        }
    }
}
