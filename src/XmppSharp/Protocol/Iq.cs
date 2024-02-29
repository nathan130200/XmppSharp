using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("iq", Namespaces.Client)]
[XmppTag("iq", Namespaces.Accept)]
[XmppTag("iq", Namespaces.Server)]
public class Iq : Stanza
{
    public Iq() : base("iq", Namespaces.Client)
    {

    }

    public Iq(IqType type) : this()
    {
        Type = type;
    }

    public IqType? Type
    {
        get => this.GetAttributeEnum<IqType>("type");
        set
        {
            if (!value.TryUnwrap(out var result))
                RemoveAttribute("type");
            else
                this.SetAttributeEnum("type", result);
        }
    }
}
