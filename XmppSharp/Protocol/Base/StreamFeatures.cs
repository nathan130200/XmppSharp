using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:features", Namespaces.Stream)]
public class StreamFeatures : XmppElement
{
    public StreamFeatures() : base("stream:features", Namespaces.Stream)
    {

    }

    public StartTls? StartTls
    {
        get => Element<StartTls>();
        set
        {
            Element<StartTls>()?.Remove();
            AddChild(value);
        }
    }

    public Mechanisms? Mechanisms
    {
        get => Element<Mechanisms>();
        set
        {
            Element<Mechanisms>()?.Remove();
            AddChild(value);
        }
    }

    public bool SupportsBind
    {
        get => HasTag("bind", Namespaces.Bind);
        set
        {
            RemoveTag("bind", Namespaces.Bind);

            if (value)
                SetTag("bind", Namespaces.Bind);
        }
    }

    public bool SupportsSession
    {
        get => HasTag("session", Namespaces.Session);
        set
        {
            RemoveTag("session", Namespaces.Session);

            if (value)
                SetTag("session", Namespaces.Session);
        }
    }
}
