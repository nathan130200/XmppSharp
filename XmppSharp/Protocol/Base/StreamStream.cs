using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:stream", "http://etherx.jabber.org/streams")]
public class StreamStream : DirectionalElement
{
	public StreamStream() : base("stream:stream", Namespaces.Stream)
	{

	}

	public string? Id
	{
		get => this.GetAttribute("id");
		set => this.SetAttribute("id", value);
	}

	public string? Language
	{
		get => this.GetAttribute("xml:lang");
		set => this.SetAttribute("xml:lang", value);
	}

	public string? Version
	{
		get => this.GetAttribute("version");
		set => this.SetAttribute("version", value);
	}
}
