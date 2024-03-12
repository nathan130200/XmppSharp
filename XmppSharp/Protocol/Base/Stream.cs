using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream", Namespace.Stream)]
public class Stream : DirectionalElement
{
    public Stream() : base("stream:stream", Namespace.Stream)
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
