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
        => Policy = policy;

    public StartTlsPolicy? Policy
    {
        get
        {
            if (HasTag("optional"))
                return StartTlsPolicy.Optional;
            if (HasTag("required"))
                return StartTlsPolicy.Required;

            return null;
        }
        set
        {
            Children().Remove();

            if (value.TryGetValue(out var result))
                SetTag(XmppEnum.ToXmppName(result));
        }
    }
}
