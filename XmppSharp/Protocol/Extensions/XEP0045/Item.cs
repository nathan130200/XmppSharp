using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[Tag("item", Namespaces.MucUser)]
[Tag("item", Namespaces.MucAdmin)]
public class Item : XmppElement
{
	public Item() : base("item")
	{

	}

	public Affiliation Affiliation
	{
		get => XmppEnum.ParseOrDefault<Affiliation>(GetAttribute("affiliation"));
		set
		{
			if (!Enum.IsDefined(value))
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

	public Role Role
	{
		get => XmppEnum.ParseOrDefault<Role>(GetAttribute("role"));
		set
		{
			if (!Enum.IsDefined(value))
				RemoveAttribute("role");
			else
				SetAttribute("role", XmppEnum.ToXml(value));
		}
	}

	public Actor? Actor
	{
		get => Element<Actor>();
		set
		{
			Element<Actor>()?.Remove();
			AddChild(value);
		}
	}

	public string? Reason
	{
		get => InnerText;
		set => InnerText = value;
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
