using System.Buffers;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace XmppSharp.Protocol.Base;

public abstract class Stanza : DirectionalElement
{
    public Stanza(XElement other) : base(other)
    {
    }

    public Stanza(XName name) : base(name)
    {
    }

    public Stanza(XStreamingElement other) : base(other)
    {
    }

    public Stanza(XName name, object? content) : base(name, content)
    {
    }

    public Stanza(XName name, params object?[] content) : base(name, content)
    {
    }

    public string Id
    {
        get => this.GetAttribute("id");
        set => this.SetAttribute("id", value);
    }

    public string Type
    {
        get => this.GetAttribute("type");
        set => this.SetAttribute("type", value);
    }

    public string Language
    {
        get => this.GetAttribute(XNamespace.Xml + "lang");
        set => this.SetAttribute(XNamespace.Xml + "lang", value);
    }

    public void GenerateId()
    {
        byte[] buf = default;

        try
        {
            buf = ArrayPool<byte>.Shared.Rent(8);
            RandomNumberGenerator.Fill(buf);
            Id = string.Concat("stanza-", buf.ToHex());
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    public StanzaError Error
    {
        get => this.Element<StanzaError>();
        set
        {
            if (value == null)
                this.RemoveTag(Namespace.Stanzas + "error");
            else
                Add(value);
        }
    }
}