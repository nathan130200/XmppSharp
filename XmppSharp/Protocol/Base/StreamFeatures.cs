using XmppSharp.Attributes;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[XmppTag("features", Namespaces.Stream)]
public class StreamFeatures : Element
{
    public StreamFeatures() : base("stream:features", Namespaces.Stream)
    {

    }

    public Mechanisms Mechanisms
    {
        get => Child<Mechanisms>();
        set => ReplaceChild<Mechanisms>(value);
    }

    public StartTls StartTls
    {
        get => Child<StartTls>();
        set => ReplaceChild(value);
    }

    public Bind Bind
    {
        get => Child<Bind>();
        set => ReplaceChild(value);
    }

    public Session Session
    {
        get => Child<Session>();
        set => ReplaceChild(value);
    }

    public bool SupportsStartTls
        => StartTls != null;

    public bool SupportsBind
        => Bind != null;

    public bool SupportsSaslAuth
        => Mechanisms != null;
}