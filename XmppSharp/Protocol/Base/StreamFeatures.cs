using XmppSharp.Dom;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

public class StreamFeatures : Element
{
    public StreamFeatures(StreamFeatures other) : base(other)
    {

    }

    public StreamFeatures() : base("stream:features", Namespaces.Stream)
    {

    }

    public StartTls? StartTls
    {
        get => Child<StartTls>();
        set
        {
            StartTls?.Remove();

            if (value != null)
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

    public Mechanisms? Mechanisms
    {
        get => Child<Mechanisms>();
        set
        {
            Mechanisms?.Remove();

            if (value != null)
                AddChild(value);
        }
    }
}
