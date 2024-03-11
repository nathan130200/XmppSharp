using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Tls;

[XmppTag("starttls", Namespace.Tls)]
public class StartTls : Element
{
    public StartTls() : base("starttls", Namespace.Tls)
    {

    }

    public StartTls(TlsPolicy policy)
        => Policy = policy;

    public TlsPolicy? Policy
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetValues<TlsPolicy>())
            {
                if (HasTag(name))
                    return value;
            }

            return null;
        }
        set
        {
            if (Policy.TryUnwrap(out var oldValue))
                RemoveTag(XmppEnum.ToXml(oldValue));

            if (value.TryUnwrap(out var result))
                SetTag(XmppEnum.ToXml(result));
        }
    }
}
