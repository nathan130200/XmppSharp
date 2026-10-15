using System.Xml;
using XmppSharp.Abstractions;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0203;

[StanzaExtension("delay", Namespaces.Delay)]
public sealed class Delay : StanzaExtension
{
	public Jid? From { get; set; }

	public DateTimeOffset Timestamp { get; set; }

	public Delay()
	{

	}

	internal Delay(Element element)
	{
		From = (string?)element.Attribute("from");
		Timestamp = XmlConvert.ToDateTimeOffset(element.Attribute("stamp")!);
	}

	public override Element Serialize()
	{
		var result = new Element("delay", Namespaces.Delay);
		result.SetAttribute("from", From);
		result.SetAttribute("stamp", XmlConvert.ToString(Timestamp));
		return result;
	}
}
