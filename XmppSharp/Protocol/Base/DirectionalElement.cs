namespace XmppSharp.Protocol.Base;

public abstract class DirectionalElement : Element
{
    protected DirectionalElement(string qualifiedName) : base(qualifiedName)
    {
    }

    protected DirectionalElement(string qualifiedName, string namespaceURI) : base(qualifiedName, namespaceURI)
    {
    }

    public Jid From
    {
        get
        {
            var jid = GetAttribute("from");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set => SetAttribute("from", value?.ToString());
    }

    public Jid To
    {
        get
        {
            var jid = GetAttribute("to");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set => SetAttribute("to", value?.ToString());
    }

    public void SwitchDirection()
        => (From, To) = (To, From);
}
