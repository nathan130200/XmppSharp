using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[Tag("invite", Namespaces.MucUser)]
public class Invite : DirectionalElement
{
	public Invite() : base("invite", Namespaces.MucUser)
	{

	}

	public string? Reason
	{
		get => GetTag("reason");
		set
		{
			RemoveTag("reason");

			if (value != null)
				SetTag("reason", value: value);
		}
	}
}