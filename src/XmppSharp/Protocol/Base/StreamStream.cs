using System.Xml;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[Tag("stream:stream", Namespaces.Stream)]
public sealed class StreamStream : DirectionalElement
{
	public StreamStream() : base("stream:stream", Namespaces.Stream)
	{
		// its safe to set default namespace, later parser can override this.

		SetNamespace(string.Empty, Stanza.DefaultNamespace);
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