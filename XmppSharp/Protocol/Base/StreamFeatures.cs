using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Represents the "features" element within an XMPP stream, received from the server after initiating a connection.
/// This element provides information about the features and extensions supported by the server for the current session.
/// </summary>
[XmppTag("features", Namespaces.Stream)]
public class StreamFeatures : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamFeatures"/> class.
    /// </summary>
    public StreamFeatures() : base("stream:features", Namespaces.Stream)
    {

    }

    /// <summary>
    /// Gets or sets the child element containing supported SASL authentication mechanisms (<see cref="Mechanisms"/>).
    /// A null value indicates that the server does not support SASL authentication.
    /// </summary>
    public Mechanisms Mechanisms
    {
        get => Child<Mechanisms>();
        set => ReplaceChild<Mechanisms>(value);
    }

    /// <summary>
    /// Gets or sets the child element containing information about TLS security layer support (<see cref="StartTls"/>).
    /// A null value indicates that the server does not support TLS.
    /// </summary>
    public StartTls StartTls
    {
        get => Child<StartTls>();
        set => ReplaceChild(value);
    }

    /// <summary>
    /// Gets or sets the child element containing information about resource binding (<see cref="Bind"/>).
    /// A null value indicates that the server does not support resource binding.
    /// </summary>
    public Bind Bind
    {
        get => Child<Bind>();
        set => ReplaceChild(value);
    }

    /// <summary>
    /// Gets or sets the child element containing session resumption information (<see cref="Session"/>).
    /// </summary>
    public Session Session
    {
        get => Child<Session>();
        set => ReplaceChild(value);
    }

    /// <summary>
    /// Gets a value indicating whether the server supports the STARTTLS extension for securing the connection.
    /// </summary>
    public bool SupportsStartTls
        => StartTls != null;

    /// <summary>
    /// Gets a value indicating whether the server supports resource binding.
    /// </summary>
    public bool SupportsBind
        => Bind != null;

    /// <summary>
    /// Gets a value indicating whether the server supports SASL authentication mechanisms.
    /// </summary>
    public bool SupportsSaslAuth
        => Mechanisms != null;
}