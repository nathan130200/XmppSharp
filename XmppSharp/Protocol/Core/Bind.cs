using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core;

/// <summary>
/// This is an implementation of a resource binding extension.
/// </summary>
[StanzaExtension("bind", Namespaces.Bind)]
public sealed class Bind : BaseStanzaExtension
{
	/// <inheritdoc/>
	public Bind()
	{

	}

	internal Bind(Element element) : base(element)
	{
		Resource = element.GetTag("resource", Namespaces.Bind);

		Jid = element.GetTag("jid", Namespaces.Bind);
	}

	/// <summary>
	/// Gets or sets the resource.
	/// </summary>
	public string? Resource { get; set; }

	/// <summary>
	/// Gets or sets the assigned JID.
	/// </summary>
	public Jid? Jid { get; set; }

	/// <inheritdoc/>
	public override Element Serialize()
	{
		var result = new Element("bind", Namespaces.Bind);

		if (Jid is { } jid)
			result.SetTag("jid", value: jid);

		if (Resource is { } resource)
			result.SetTag("resource", value: resource);

		return result;
	}
}
