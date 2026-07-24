using System.Xml;

namespace XmppSharp.Protocol.Base;

public sealed class StreamStream : DirectionalElement
{
    public StreamStream() : base("stream:stream", Namespaces.Stream)
    {

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