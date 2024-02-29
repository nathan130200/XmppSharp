using XmppSharp.Xml.Dom;

namespace XmppSharp.Protocol.Base;

public abstract class DirectionalElement : Element
{
    protected DirectionalElement(string name) : base(name)
    {
    }

    protected DirectionalElement(string name, string xmlns = null, string text = null) : base(name, xmlns, text)
    {
    }

    internal DirectionalElement()
    {
    }

    public Jid From
    {
        get => this.GetAttributeValue<Jid>("from");
        set => SetAttribute("from", value?.ToString());
    }

    public Jid To
    {
        get => this.GetAttributeValue<Jid>("to");
        set => SetAttribute("to", value?.ToString());
    }

    public void SwitchDirection()
    {
        //var from = From;
        //var to = To;
        //From = to;
        //To = from;

        (From, To) = (To, From);
    }
}