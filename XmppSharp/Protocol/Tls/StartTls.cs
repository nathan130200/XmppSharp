using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", Namespaces.Tls)]
public class StartTls : Element
{
    public StartTls() : base("starttls", Namespaces.Tls)
    {

    }

    public StartTls(StartTlsPolicy policy) : this()
    {
        Policy = policy;
    }

    public StartTlsPolicy Policy
    {
        get
        {
            StartTlsPolicy result;

            if (HasTag("required", Namespaces.Tls))
                result = StartTlsPolicy.Required;
            else
                result = StartTlsPolicy.Optional;

            return result;
        }
        set
        {
            RemoveTag("optional");
            RemoveTag("required");

            if (value == StartTlsPolicy.Required)
                SetTag("required", Namespaces.Tls);
            else
                SetTag("optional", Namespaces.Tls);
        }
    }
}
