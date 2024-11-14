using System.Diagnostics;

namespace XmppSharp.Protocol.Base;

public abstract class Stanza : DirectionalElement
{
    protected Stanza(Stanza other) : base(other)
    {
    }

    protected Stanza(string tagName, string? namespaceURI = null, object? value = null) : base(tagName, namespaceURI, value)
    {

    }

    public string? Id
    {
        get => GetAttribute("id");
        set => SetAttribute("id", value);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? Type
    {
        get => GetAttribute("type");
        set => SetAttribute("type", value);
    }

    public string? Language
    {
        get => GetAttribute("xml:lang");
        set => SetAttribute("xml:lang", value);
    }

    public void GenerateId(IdGenerator? generator = default)
        => Id = (generator ?? IdGenerator.Timestamp).Generate();

    public StanzaError? Error
    {
        get => Child<StanzaError>();
        set
        {
            Child<StanzaError>()?.Remove();

            if (value != null)
                AddChild(value);
        }
    }
}
