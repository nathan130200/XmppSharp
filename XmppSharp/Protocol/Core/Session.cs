using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core;

/// <summary>
/// This is an implementation of a session extension.
/// </summary>
[StanzaExtension("session", Namespaces.Session)]
public sealed class Session : BaseStanzaExtension
{
	/// <inheritdoc/>
	public Session()
	{

	}

	internal Session(Element element) : base(element)
	{

	}

	/// <inheritdoc/>
	public override Element Serialize() => new("session", Namespaces.Session);
}
