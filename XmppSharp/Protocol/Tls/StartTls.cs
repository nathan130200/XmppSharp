using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", Namespaces.Tls)]
public class StartTls : XmppElement
{
    public StartTls() : base("starttls", Namespaces.Tls)
    {

    }

    public StartTls(TlsPolicy policy) : this()
    {
        Policy = policy;
    }

    public TlsPolicy Policy
    {
        get
        {
            if (HasTag("required", Namespaces.Tls))
                return TlsPolicy.Required;
            else if (HasTag("optional", Namespaces.Tls))
                return TlsPolicy.Optional;
            else
                return TlsPolicy.Unknown;
        }
        set
        {
            RemoveTag("optional");
            RemoveTag("required");

            if (value == TlsPolicy.Optional)
                SetTag("optional", Namespaces.Tls);
            else if (value == TlsPolicy.Required)
                SetTag("required", Namespaces.Tls);
        }
    }
}
