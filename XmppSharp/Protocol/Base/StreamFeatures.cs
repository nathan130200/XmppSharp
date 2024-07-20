using XmppSharp.Attributes;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:features", Namespaces.Stream)]
public class StreamFeatures : Element
{
	public StreamFeatures() : base("stream:features", Namespaces.Stream)
	{

	}

	public Mechanisms? Mechanisms
	{
		get => this.Child<Mechanisms>();
		set
		{
			this.RemoveTag("mechanisms", Namespaces.Sasl);

			if (value != null)
				this.AddChild(value);
		}
	}

	public StartTls? StartTls
	{
		get => this.Child<StartTls>();
		set
		{
			this.RemoveTag("starttls", Namespaces.Tls);

			if (value != null)
				this.AddChild(value);
		}
	}

	public bool SupportsStartTls
		=> this.HasTag("starttls", Namespaces.Tls);

	public bool SupportsAuthentication
		=> this.HasTag("mechanisms", Namespaces.Sasl);

	public bool SupportsBind
	{
		get => this.HasTag("bind", Namespaces.Bind);
		set
		{
			if (!value)
				this.RemoveTag("bind", Namespaces.Bind);
			else
				this.SetTag("bind", Namespaces.Bind);
		}
	}

	public bool SupportsSession
	{
		get => this.HasTag("session", Namespaces.Session);
		set
		{
			if (!value)
				this.RemoveTag("session", Namespaces.Session);
			else
				this.SetTag("session", Namespaces.Session);
		}
	}
}