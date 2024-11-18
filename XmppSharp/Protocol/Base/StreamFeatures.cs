using XmppSharp.Dom;
using XmppSharp.Protocol.Core.Sasl;
using XmppSharp.Protocol.Core.Tls;

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
            Child<StartTls>()?.Remove();
            AddChild(value);
        }
    }

    public Mechanisms? Mechanisms
    {
        get => Child<Mechanisms>();
        set
        {
            Child<Mechanisms>()?.Remove();
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
            {
                SetTag(x =>
                {
                    x.TagName = "bind";
                    x.Namespace = Namespaces.Bind;
                });
            }
        }
    }

    public bool SupportSession
    {
        get => HasTag("session", Namespaces.Session);
        set
        {
            RemoveTag("session", Namespaces.Session);

            if (value)
            {
                SetTag(x =>
                {
                    x.TagName = "session";
                    x.Namespace = Namespaces.Session;
                });
            }
        }
    }
}
