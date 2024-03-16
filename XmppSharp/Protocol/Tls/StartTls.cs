using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", Namespace.Tls)]
public class StartTls : Element
{
    public StartTls() : base("starttls", Namespace.Tls)
    {

    }

    public StartTls(TlsPolicy policy) : this()
        => Policy = policy;

    public TlsPolicy? Policy
    {
        get
        {
            if (HasTag("optional"))
                return TlsPolicy.Optional;
            if (HasTag("required"))
                return TlsPolicy.Required;

            return null;
        }
        set
        {
            Children().Remove();

            if (value.TryGetValue(out var result))
                SetTag(XmppEnum.ToXml(result));
        }
    }
}
