using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", Namespaces.Tls)]
public sealed class StartTls : Element
{
	public StartTls() : base("starttls", Namespaces.Tls)
	{

	}

	public StartTls(TlsPolicy policy) : this()
		=> this.Policy = policy;

	public TlsPolicy? Policy
	{
		get
		{
			if (this.HasTag("optional"))
				return TlsPolicy.Optional;

			if (this.HasTag("required"))
				return TlsPolicy.Required;

			return null;
		}
		set
		{
			this.RemoveTag("optional");
			this.RemoveTag("required");

			if (value.TryGetValue(out var policy))
				this.SetTag(policy.ToXmppName());
		}
	}
}
