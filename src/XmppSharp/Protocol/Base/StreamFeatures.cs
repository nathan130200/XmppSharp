using XmppSharp.Dom;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Protocol.Base;

public sealed class StreamFeatures : Element
{
	public StreamFeatures() : base("stream:features", Namespaces.Stream)
	{

	}

	public Mechanisms? Mechanisms
	{
		get => Element<Mechanisms>();

		set
		{
			Element<Mechanisms>()?.Remove();

			AddChild(value);
		}
	}
	public bool SupportsBind
	{
		get => HasTag("bind", Namespaces.Bind);

		set
		{
			if (!value)
				RemoveTag("bind", Namespaces.Bind);
			else
				SetTag("bind", Namespaces.Bind);
		}
	}

	public bool SupportsSession
	{
		get => HasTag("session", Namespaces.Session);

		set
		{
			if (!value)
				RemoveTag("session", Namespaces.Session);
			else
				SetTag("session", Namespaces.Session);
		}
	}
}