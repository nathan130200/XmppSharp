using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Base;

public class DirectionalElement : Element
{
    public DirectionalElement(string name, string xmlns = null, string text = null) : base(name, xmlns, text)
    {
    }

    protected DirectionalElement()
    {
    }

    protected DirectionalElement(Element other) : base(other)
    {
    }

    public Jid? From
    {
        get
        {
            if (!HasAttribute("from"))
                return null;

            if (Jid.TryParse(GetAttribute("from"), out var result))
                return result;

            return null;
        }
        set => SetAttribute("from", value?.ToString());
    }

    public Jid? To
    {
        get
        {
            if (!HasAttribute("to"))
                return null;

            if (Jid.TryParse(GetAttribute("to"), out var result))
                return result;

            return default;
        }
        set => SetAttribute("to", value?.ToString());
    }

    public void SwitchDirection()
        => (From, To) = (To, From);
}
