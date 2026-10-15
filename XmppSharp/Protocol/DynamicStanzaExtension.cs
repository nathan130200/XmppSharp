using XmppSharp.Dom;

namespace XmppSharp.Protocol;

public sealed class DynamicStanzaExtension : StanzaExtension
{
	internal readonly Element _element;

	public DynamicStanzaExtension(string name, string? ns)
	{
		_element = new Element(name, ns);
	}

	public DynamicStanzaExtension(Element element)
	{
		ArgumentNullException.ThrowIfNull(element);

		_element = element;
	}

	public override Element Serialize()
	{
		return _element;
	}
}