using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol;

[Tag("item", Namespaces.IqRoster)]
public class RosterItem : XmppElement
{
	public RosterItem() : base("item", Namespaces.IqRoster)
	{

	}

	public RosterItem(Jid? jid, string? name, RosterSubscriptionType subscription = default) : this()
	{
		Jid = jid;
		ItemName = name;
		Subscription = subscription;
	}

	public IEnumerable<string> Groups
	{
		get => Elements("group").Select(x => x.InnerText!);
		set
		{
			Elements("group").Remove();

			foreach (var groupName in value)
				SetTag("group", value: groupName);
		}
	}

	public Jid? Jid
	{
		get => GetAttribute("jid");
		set => SetAttribute("jid", value);
	}

	public string? ItemName
	{
		get => GetAttribute("name");
		set => SetAttribute("name", value);
	}

	public RosterSubscriptionType Subscription
	{
		get => XmppEnum.ParseOrDefault<RosterSubscriptionType>(GetAttribute("subscription"));
		set => SetAttribute("subscription", value.ToXmlOrDefault());
	}

	public bool? Approved
	{
		get => this.GetAttributeBool("approved");
		set => this.SetAttributeBool("approved", value);
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
