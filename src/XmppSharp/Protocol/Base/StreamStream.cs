using System.Xml;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[Tag("stream:stream", Namespaces.Stream)]
public sealed class StreamStream : DirectionalElement
{
	public StreamStream() : base("stream:stream", Namespaces.Stream)
	{
		SetNamespace(string.Empty, Stanza.DefaultNamespace);

		SetAttribute("version", "1.0");
	}

	public string? Id
	{
		get => GetAttribute("id");
		set => SetAttribute("id", value);
	}

	public override void WriteTo(XmlWriter writer)
	{
		WriteStartElement(writer);
	}
}