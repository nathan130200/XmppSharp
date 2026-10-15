using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0092;

/// <summary>
/// This extension provides a standard way to exchange information about the software version used by the entities. 
/// </summary>
[StanzaExtension("query", XMLNS_VERSION)]
public sealed class SoftwareVersion : BaseStanzaExtension
{
	const string XMLNS_VERSION = "jabber:iq:version";


	/// <inheritdoc/>
	public SoftwareVersion()
	{
	}

	/// <inheritdoc/>
	public SoftwareVersion(Element element) : base(element)
	{

	}

	/// <summary>
	/// The natural-language name of the software.
	/// </summary>
	public string? Name
	{
		get;
		set;
	}

	/// <summary>
	/// The operating system of the queried entity.
	/// </summary>
	public string? OperatingSystem
	{
		get;
		set;
	}

	/// <summary>
	/// The specific version of the software.
	/// </summary>
	public string? Version
	{
		get;
		set;
	}

	/// <inheritdoc/>
	public override Element Serialize()
	{
		var result = new Element("query", XMLNS_VERSION);

		if (Name is { } name)
			result.SetTag("name", value: name);

		if (OperatingSystem is { } os)
			result.SetTag("os", value: os);

		if (Version is { } version)
			result.SetTag("version", value: version);

		return result;
	}
}
