using System.Xml.Linq;

namespace XmppSharp.Protocol.Base;

public abstract class DirectionalElement : XElement
{
    public DirectionalElement(XElement other) : base(other)
    {
    }

    public DirectionalElement(XName name) : base(name)
    {
    }

    public DirectionalElement(XStreamingElement other) : base(other)
    {
    }

    public DirectionalElement(XName name, object? content) : base(name, content)
    {
    }

    public DirectionalElement(XName name, params object?[] content) : base(name, content)
    {
    }

    public Jid From
    {
        get
        {
            var jid = this.GetAttribute("from");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set => this.SetAttribute("from", value?.ToString());
    }

    public Jid To
    {
        get
        {
            var jid = this.GetAttribute("to");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set => this.SetAttribute("to", value?.ToString());
    }

    public void SwitchDirection()
        => (From, To) = (To, From);
}
