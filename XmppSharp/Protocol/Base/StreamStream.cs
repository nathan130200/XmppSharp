namespace XmppSharp.Protocol.Base;

public class StreamStream : DirectionalElement
{
    public StreamStream() : base("stream:stream", Namespaces.Stream)
    {

    }

    public string? Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    public string? Version
    {
        get => GetAttribute("version");
        set => SetAttribute("version", value);
    }

    public string? Language
    {
        get => GetAttribute("xml:lang");
        set => SetAttribute("xml:lang", value);
    }

    public void GenerateId()
        => Id = Guid.NewGuid().ToString("d");

    public override string ToString(bool indented)
        => StartTag();
}
