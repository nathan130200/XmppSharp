using XmppSharp.Abstractions;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Client;

[StanzaExtension("bind", Namespaces.Bind)]
public sealed class Bind : StanzaExtension
{
	public Bind()
	{

	}

	internal Bind(Element element) : base(element)
	{
		Resource = element.GetTag("resource", Namespaces.Bind);

		Resource = element.GetTag("jid", Namespaces.Bind);
	}

	public string? Resource { get; set; }

	public Jid? Jid { get; set; }

	public override Element Serialize()
	{
		var result = new Element("bind", Namespaces.Bind);

		if (Jid is { } jid)
			result.SetTag("jid", Namespaces.Bind, jid);

		if (Resource is { } resource)
			result.SetTag("resource", Namespaces.Bind, resource);

		return result;
	}
}
