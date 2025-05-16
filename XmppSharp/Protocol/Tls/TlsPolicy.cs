namespace XmppSharp.Protocol.Tls;

public enum TlsPolicy
{
    /// <summary>
    /// Unknown TLS policy.
    /// </summary>
    None,

    /// <summary>
    /// Informs that encryption is recommended, but not mandatory.
    /// </summary>
    Optional,

    /// <summary>
    /// It states that encryption is recommended and mandatory, and will not proceed until a secure communication channel is established.
    /// </summary>
    Required
}