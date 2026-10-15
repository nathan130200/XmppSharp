using System.Xml;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

/// <summary>
/// An abstraction of a MUC query.
/// </summary>
[StanzaExtension("x", Namespaces.Muc)]
public sealed class Muc : BaseStanzaExtension
{
	/// <summary>
	/// Initialize a new instance of <see cref="Muc"/> class.
	/// </summary>
	public Muc()
	{

	}

	internal Muc(Element element) : base(element)
	{
		Password = element.GetTag("password");

		if (element.Element("history") is { } history)
		{
			History = new()
			{
				MaxChars = history.Attribute("maxchars"),

				MaxStanzas = history.Attribute("maxstanzas"),

				Seconds = history.Attribute("seconds"),

				Since = history.Attribute("since") is { } since
					? XmlConvert.ToDateTimeOffset(since)
					: null
			};
		}
	}

	/// <summary>
	/// Gets or sets room history to request.
	/// </summary>
	public History? History
	{
		get;
		set;
	}

	/// <summary>
	/// Gets or sets room password to join.
	/// </summary>
	public string? Password
	{
		get;
		set;
	}

	/// <inheritdoc />
	public override Element Serialize()
	{
		var result = new Element("x", Namespaces.Muc);

		if (Password is { } password)
			result.SetTag("password", value: password);

		if (History is { } history)
		{
			result.C("history", x =>
			{
				x.SetAttribute("maxchars", history.MaxChars);

				x.SetAttribute("maxstanzas", history.MaxStanzas);

				x.SetAttribute("seconds", history.Seconds);

				if (history.Since is { } since)
					x.SetAttribute("since", XmlConvert.ToString(since));
			});
		}

		return result;
	}
}