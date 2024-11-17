using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol.Core;

[XmppTag("iq", Namespaces.Client)]
[XmppTag("iq", Namespaces.Accept)]
[XmppTag("iq", Namespaces.Connect)]
[XmppTag("iq", Namespaces.Server)]
public class Iq : Stanza
{
    public Iq(Iq other) : base(other)
    {

    }

    public Iq() : base("iq", Namespaces.Client)
    {

    }

    public Iq(IqType type) : this()
    {
        Type = type;
    }

    public new IqType Type
    {
        get
        {
            return base.Type switch
            {
                "get" => IqType.Get,
                "set" => IqType.Set,
                "result" => IqType.Result,
                "error" => IqType.Error,
                _ => throw new ArgumentException(default, nameof(Type))
            };
        }
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException(default, nameof(Type));

            base.Type = XmppEnum.ToXml(value);
        }
    }
}