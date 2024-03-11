using System.Security.Cryptography;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Base;

public class Stanza : DirectionalElement
{
    public Stanza(string name, string xmlns = null, string text = null) : base(name, xmlns, text)
    {
    }

    protected Stanza()
    {
    }

    protected Stanza(Element other) : base(other)
    {
    }

    public string Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    public string Type
    {
        get => GetAttribute("type");
        set => SetAttribute("type", value);
    }

    public string Language
    {
        get => GetAttribute("xml:lang");
        set => SetAttribute("xml:lang", value);
    }

    public void GenerateId()
    {
        Span<byte> buffer = stackalloc byte[8];
        RandomNumberGenerator.Fill(buffer);
        Id = Convert.ToHexString(buffer).ToLowerInvariant();
    }

    public Error Error
    {
        get => Child<Error>();
        set => ReplaceChild(value);
    }
}