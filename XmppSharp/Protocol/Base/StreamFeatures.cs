using XmppSharp.Attributes;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:features", Namespace.Stream)]
public class StreamFeatures : Element
{
	public StreamFeatures() : base("stream:features", Namespace.Stream)
	{

	}

	public Mechanisms Mechanisms
	{
		get => this.Child<Mechanisms>();
		set
		{
			this.RemoveTag("mechanisms", Namespace.Sasl);

			if (value != null)
				this.AddChild(value);
		}
	}

	public StartTls StartTls
	{
		get => this.Child<StartTls>();
		set
		{
			this.RemoveTag("starttls", Namespace.Tls);

			if (value != null)
				this.AddChild(value);
		}
	}

	public bool SupportsStartTls
		=> this.HasTag("starttls", Namespace.Tls);

	public bool SupportsAuthentication
		=> this.HasTag("mechanisms", Namespace.Sasl);

	public bool SupportsBind
	{
		get => this.HasTag("bind", Namespace.Bind);
		set
		{
			if (!value)
				this.RemoveTag("bind", Namespace.Bind);
			else
				this.SetTag("bind", Namespace.Bind);
		}
	}

	public bool SupportsSession
	{
		get => this.HasTag("session", Namespace.Session);
		set
		{
			if (!value)
				this.RemoveTag("session", Namespace.Session);
			else
				this.SetTag("session", Namespace.Session);
		}
	}
}