namespace XmppSharp.Protocol.Base;

public abstract class Stanza : DirectionalElement
{
    protected Stanza(string name) : base(name)
    {
    }

    protected Stanza(string name, string xmlns = null, string text = null) : base(name, xmlns, text)
    {
    }

    internal Stanza()
    {
    }

    public string Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    public string Version
    {
        get => GetAttribute("version");
        set => SetAttribute("version", value);
    }

    public string Language
    {
        get => GetAttribute("xml:lang");
        set => SetAttribute("xml:lang", value);
    }
}
