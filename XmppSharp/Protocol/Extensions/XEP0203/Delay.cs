using System.Xml;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0203;

/// <summary>
/// Stanza extension for communicating the fact that an XML stanza has been delivered with a delay.
/// </summary>
[StanzaExtension("delay", XMLNS_DELAY)]
public sealed class Delay : BaseStanzaExtension
{
	const string XMLNS_DELAY = "urn:xmpp:delay";

	/// <summary>
	/// The Jabber ID of the entity that originally sent the XML stanza or that delayed the delivery of the stanza 
	/// </summary>
	public Jid? From { get; set; }

	/// <summary>
	/// The time when the XML stanza was originally sent. 
	/// </summary>
	public DateTimeOffset Timestamp { get; set; }

	/// <summary>
	/// Initialize a new instance of <see cref="Delay"/> class.
	/// </summary>
	public Delay()
	{

	}

	/// <inheritdoc />
	internal Delay(Element element)
	{
		From = (string?)element.Attribute("from");

		Timestamp = XmlConvert.ToDateTimeOffset(element.Attribute("stamp")!);

		Description = element.Value;
	}

	/// <summary>
	/// Gets or sets the description of the reason for the delay.
	/// </summary>
	public string? Description
	{
		get;
		set;
	}

	/// <inheritdoc/>
	public override Element Serialize()
	{
		var result = new Element("delay", XMLNS_DELAY);

		result.SetAttribute("from", From);

		result.SetAttribute("stamp", XmlConvert.ToString(Timestamp));

		if (Description != null)
			result.SetValue(Description);

		return result;
	}
}
