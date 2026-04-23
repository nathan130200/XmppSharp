using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Tls;

/// <summary>
/// The <![CDATA[<starttls>]]> element is used in two contexts:
/// <list type="bullet">
/// <item>It is sent by the server in the stream features to indicate that it supports TLS and to specify the policy for using TLS (optional or required).</item>
/// <item>It is sent by the client to the server to request the start of TLS negotiation.</item>
/// </list>
/// </summary>
[Tag("starttls", Namespaces.Tls)]
public class StartTls : Element
{

    /// <summary>
    /// Initializes a new instance of the <see cref="StartTls"/> class.
    /// </summary>
    public StartTls() : base("starttls", Namespaces.Tls)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartTls"/> class with the specified policy.
    /// </summary>
    /// <param name="policy">The TLS policy to apply.</param>
    public StartTls(StartTlsPolicy policy) : this()
    {
        Policy = policy;
    }

    /// <summary>
    /// Gets or sets the TLS policy for the server.
    /// </summary>
    public StartTlsPolicy Policy
    {
        get
        {
            if (HasTag("optional", Namespaces.Tls))
                return StartTlsPolicy.Optional;

            if (HasTag("required", Namespaces.Tls))
                return StartTlsPolicy.Required;

            return 0;
        }
        set
        {
            RemoveTag("optional", Namespaces.Tls);
            RemoveTag("required", Namespaces.Tls);

            if (Enum.IsDefined(value))
                SetTag(value.ToXml(), Namespaces.Tls);
        }
    }
}
