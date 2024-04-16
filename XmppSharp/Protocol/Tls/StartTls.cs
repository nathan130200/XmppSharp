using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", Namespace.Tls)]
public sealed class StartTls : Element
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
            RemoveTag("optional");
            RemoveTag("required");

            if (value.TryGetValue(out var policy))
                SetTag(policy.ToXmppName());
        }
    }
}
