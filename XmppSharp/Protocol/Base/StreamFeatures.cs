using XmppSharp.Attributes;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:features", Namespace.Stream)]
public class StreamFeatures : Element
{
    public StreamFeatures() : base("stream:features", Namespace.Stream)
    {

    }

    public Mechanisms Mechanisms
    {
        get => Child<Mechanisms>();
        set
        {
            RemoveTag("mechanisms", Namespace.Sasl);

            if (value != null)
                AddChild(value);
        }
    }

    public StartTls StartTls
    {
        get => Child<StartTls>();
        set
        {
            RemoveTag("starttls", Namespace.Tls);

            if (value != null)
                AddChild(value);
        }
    }

    public bool SupportsStartTls
        => HasTag("starttls", Namespace.Tls);

    public bool SupportsAuthentication
        => HasTag("mechanisms", Namespace.Sasl);

    public bool SupportsBind
    {
        get => HasTag("bind", Namespace.Bind);
        set
        {
            Action<string, string> fn = !value ? RemoveTag : SetTag;
            fn("bind", Namespace.Bind);
        }
    }

    public bool SupportsSession
    {
        get => HasTag("session", Namespace.Session);
        set
        {
            Action<string, string> fn = !value ? RemoveTag : SetTag;
            fn("session", Namespace.Session);
        }
    }
}