using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

[Tag("starttls", Namespaces.Tls)]
public class StartTls : XmppElement
{
	public StartTls() : base("starttls", Namespaces.Tls)
	{

	}

	public StartTls(bool? required) : this()
	{
		Required = required;
	}

	public bool? Required
	{
		get
		{
			if (HasTag("optional", Namespaces.Tls))
				return false;

			if (HasTag("required", Namespaces.Tls))
				return true;

			return null;
		}
		set
		{
			if (value is not bool state)
			{
				RemoveTag("optional", Namespaces.Tls);
				RemoveTag("required", Namespaces.Tls);
			}
			else
			{
				SetTag(state ? "required" : "optional", Namespaces.Tls);
			}
		}
	}
}
