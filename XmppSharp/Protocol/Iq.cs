using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol;

public class Iq : Stanza
{
	static readonly XmppEnumMapping<IqType> s_TypeParser = XmppEnumMapping<IqType>.Shared;

	public Iq()
	{

	}

	public Iq(IqType type) : this()
	{
		Type = type;
	}

	internal Iq(Element element) : base(element)
	{
		Type = s_TypeParser.Parse(element.Attribute("type"));

		foreach (var child in element.Elements())
			TryAddExtension(child);
	}

	public IqType Type { get; set; }

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