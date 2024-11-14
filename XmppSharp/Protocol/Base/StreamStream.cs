namespace XmppSharp.Protocol.Base;

public class StreamStream : Stanza
{
    public StreamStream(StreamStream other) : base(other)
    {

    }

    public StreamStream() : base("stream:stream", Namespaces.Stream)
    {

    }

    public string? Version
    {
        get => GetAttribute("version");
        set => SetAttribute("version", value);
    }
}
