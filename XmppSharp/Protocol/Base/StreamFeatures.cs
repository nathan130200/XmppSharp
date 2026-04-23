using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents the stream features advertised by the server during the initial stream negotiation phase of an XMPP connection.
/// </summary>
[Tag("features", Namespaces.Stream)]
[Tag("stream:features", Namespaces.Stream)]
public class StreamFeatures : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamFeatures"/> class.
    /// </summary>
    public StreamFeatures() : base("stream:features", Namespaces.Stream)
    {

    }

    /// <summary>
    /// Gets or sets the <see cref="StartTls"/> element, which indicates whether the server supports TLS encryption for the connection.
    /// </summary>
    public StartTls? StartTls
    {
        get => Element<StartTls>();
        set
        {
            Element<StartTls>()?.Remove();
            AddChild(value);
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="Mechanisms"/> element, which lists the authentication mechanisms supported by the server for SASL authentication.
    /// </summary>
    public Mechanisms? Mechanisms
    {
        get => Element<Mechanisms>();
        set
        {
            Element<Mechanisms>()?.Remove();
            AddChild(value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the server supports resource binding, which is required for establishing a complete XMPP session after authentication.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the server supports session establishment, which is an optional step in the XMPP connection process that allows clients to maintain state across multiple connections.
    /// </summary>
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
