namespace XmppSharp.Protocol.Base;

public abstract class Stanza : Element
{
    protected Stanza(string qualifiedName) : base(qualifiedName)
    {
    }

    protected Stanza(string qualifiedName, string namespaceURI) : base(qualifiedName, namespaceURI)
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
        => Id = Guid.NewGuid().ToString("d");

    public StanzaError Error
    {
        get => Child<StanzaError>();
        set
        {
            if (value == null)
                RemoveTag("error", Namespace.Stanzas);
            else
                AddChild(value);
        }
    }
}