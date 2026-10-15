using System.Diagnostics.CodeAnalysis;
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

	public static explicit operator DynamicStanzaExtension(Element element) => new(element);

	[return: NotNullIfNotNull(nameof(extension))]
	public static implicit operator Element?(DynamicStanzaExtension? extension) => extension?._element;
}