using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Core.Sasl;
using XmppSharp.Protocol.Core.Tls;

namespace XmppSharp.Protocol.Base;

[XmppTag("stream:features", Namespaces.Stream)]
public class StreamFeatures : XmppElement
{
    public StreamFeatures(StreamFeatures other) : base(other)
    {

    }

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

    public bool SupportBind
    {
        get => HasTag("bind", Namespaces.Bind);
        set
        {
            RemoveTag("bind", Namespaces.Bind);

            if (value)
                SetTag("bind", Namespaces.Bind);
        }
    }

    public bool SupportSession
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
