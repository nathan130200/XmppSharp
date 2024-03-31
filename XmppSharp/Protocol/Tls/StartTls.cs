using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

/// <summary>
/// Represents a "starttls" element used to initiate TLS (Transport Layer Security) negotiation within XMPP.
/// This element indicates the intent for both the client and server to transition the existing connection to a secure TLS session.
/// </summary>
[XmppTag("starttls", Namespaces.Tls)]
public sealed class StartTls : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartTls"/> class with a default policy of null.
    /// </summary>
    public StartTls() : base("starttls", Namespaces.Tls)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartTls"/> class with the specified policy.
    /// </summary>
    /// <param name="policy">The policy to include in the "starttls" element, indicating whether TLS is optional or required.</param>
    public StartTls(StartTlsPolicy policy) : this()
        => Policy = policy;

    /// <summary>
    /// Gets or sets the TLS policy indicating whether TLS is optional or required.
    /// </summary>
    public StartTlsPolicy? Policy
    {
        get
        {
            if (HasTag("optional"))
                return StartTlsPolicy.Optional;
            if (HasTag("required"))
                return StartTlsPolicy.Required;

            return null;
        }
        set
        {
            Children().Remove();

            if (value.TryUnwrap(out var result))
                SetTag(result.ToXmppName());
        }
    }
}
