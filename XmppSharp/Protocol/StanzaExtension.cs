using XmppSharp.Dom;

namespace XmppSharp.Protocol;

/// <summary>
/// Implementation class of untyped stanza extension.
/// </summary>
/// <remarks>It can be used at runtime to handle elements that are implicit or unknown to extension factory.</remarks>
public sealed class StanzaExtension : BaseStanzaExtension
{
	internal readonly Element _element;

	/// <summary>
	/// Constructor, assign new element instance.
	/// </summary>
	/// <param name="name">Element qualified name</param>
	/// <param name="ns">Element namespace</param>
	public StanzaExtension(string name, string? ns)
	{
		_element = new Element(name, ns);
	}

	/// <summary>
	/// Constructor from existing element.
	/// </summary>
	public StanzaExtension(Element element)
	{
		ArgumentNullException.ThrowIfNull(element);

		_element = element;
	}

	/// <summary>
	/// Gets the actual instance of the element owned by the extension.
	/// </summary>
	public Element Element => _element;

	/// <inheritdoc cref="Element" />
	public override Element Serialize()
	{
		return _element;
	}
}