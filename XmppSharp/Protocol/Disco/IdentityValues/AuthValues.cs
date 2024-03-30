using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Disco.IdentityValues;

/// <summary>
/// The auth category consists of server components that provide authentication services within a server implementation.
/// </summary>
[XmppEnum]
public enum AuthValues
{
    /// <summary>
    /// A server authentication component other than one of the registered types.
    /// </summary>
    [XmppMember("generic")]
    Generic,

    /// <summary>
    /// A server component that authenticates based on external certificates.
    /// </summary>
    [XmppMember("cert")]
    Cert,

    /// <summary>
    /// A server component that authenticates against an LDAP database.
    /// </summary>
    [XmppMember("ldap")]
    Ldap,

    /// <summary>
    /// A server component that authenticates against an NT domain.
    /// </summary>
    [XmppMember("ntlm")]
    Ntlm,

    /// <summary>
    /// A server component that authenticates against a PAM system.
    /// </summary>
    [XmppMember("pam")]
    Pam,

    /// <summary>
    /// A server component that authenticates against a Radius system.
    /// </summary>
    [XmppMember("radius")]
    Radius
}