using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol;

/// <summary>
/// An abstraction of a info-query stanza.
/// </summary>
public class Iq : Stanza
{
	static readonly XmppEnumMapping<IqType> s_TypeParser = XmppEnumMapping<IqType>.Shared;

	/// <summary>
	/// Initialize a new instance of <see cref="Iq"/> class.
	/// </summary>
	/// <param name="type">The type of iq.</param>
	public Iq(IqType type)
	{
		if (!Enum.IsDefined(type))
			throw new ArgumentOutOfRangeException(nameof(type));

		Type = type;
	}

	/// <summary>
	/// Initialize a new instance of <see cref="Iq"/> class from given element.
	/// </summary>
	public Iq(Element element) : base(element)
	{
		Type = s_TypeParser.Parse(element.Attribute("type"));

		foreach (var child in element.Elements())
			AddExtension(child);
	}

	/// <summary>
	/// Gets or sets the IQ type.
	/// </summary>
	public IqType Type
	{
		get;
		set;
	}

	/// <inheritdoc />
	public override Element Serialize()
	{
		var result = new Element("iq", DefaultNamespace);

		result.SetAttribute("id", Id);
		result.SetAttribute("type", s_TypeParser.GetName(Type));
		result.SetAttribute("from", From);
		result.SetAttribute("to", To);

		foreach (var ext in GetExtensions())
			result.AddChild(ext.Serialize());

		return result;
	}
}