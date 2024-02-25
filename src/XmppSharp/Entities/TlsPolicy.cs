namespace Jabber.Entities;

public enum TlsPolicy
{
    /// <summary>
    /// No encryption required.
    /// </summary>
    None,

    /// <summary>
    /// Server offers encryption and the client decides whether to use it or not.
    /// </summary>
    Optional,

    /// <summary>
    /// Server offers encryption and its mandatory.
    /// </summary>
    Required
}