using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core;

/// <summary>
/// This is an implementation of a session extension.
/// </summary>
[StanzaExtension("session", Namespaces.Session)]
public sealed class Session : BaseStanzaExtension
{
	/// <summary>
	/// Initialize a new instance of <see cref="Session"/> class.
	/// </summary>
	public Session()
	{

	}

	internal Session(Element element) : base(element)
	{

	}

	/// <inheritdoc/>
	public override Element Serialize() => new("session", Namespaces.Session);
}
