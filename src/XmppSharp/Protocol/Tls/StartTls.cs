using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

[Tag("starttls", Namespaces.Tls)]
public sealed class StartTls() : Element("starttls", Namespaces.Tls)
{
	public StartTls(StartTlsPolicy policy) : this()
	{
		Policy = policy;
	}

	public StartTlsPolicy Policy
	{
		get
		{
			if (HasTag("required", Namespaces.Tls))
				return StartTlsPolicy.Required;

			if (HasTag("optional", Namespaces.Tls))
				return StartTlsPolicy.Optional;

			return default;
		}
		set
		{
			RemoveNodes();

			if (value == StartTlsPolicy.Optional)
				SetTag("optional", Namespaces.Tls);

			if (value == StartTlsPolicy.Required)
				SetTag("required", Namespaces.Tls);
		}
	}
}
