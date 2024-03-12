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
