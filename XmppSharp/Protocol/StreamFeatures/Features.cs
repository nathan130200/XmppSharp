using XmppSharp.Attributes;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Tls;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.StreamFeatures;

[XmppTag("features", Namespace.Stream)]
public class Features : Element
{
    public Features() : base("stream:features", Namespace.Stream)
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