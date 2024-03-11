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

    public Jid From
    {
        get => Jid.Parse(GetAttribute("from"), false);
        set => SetAttribute("from", value?.ToString());
    }

    public Jid To
    {
        get => Jid.Parse(GetAttribute("to"), false);
        set => SetAttribute("to", value?.ToString());
    }

    public void SwitchDirection()
        => (From, To) = (To, From);
}
