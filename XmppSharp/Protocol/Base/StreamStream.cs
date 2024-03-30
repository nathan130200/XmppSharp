using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream", Namespaces.Stream)]
public class StreamStream : DirectionalElement
{
    public StreamStream() : base("stream:stream", Namespaces.Stream)
    {

    }

    public string Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    public string Language
    {
        get => GetAttribute("xml:lang");
        set => SetAttribute("xml:lang", value);
    }

    public string Version
    {
        get => GetAttribute("version");
        set => SetAttribute("version", value);
    }
}
