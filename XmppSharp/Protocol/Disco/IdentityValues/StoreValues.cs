using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Disco.IdentityValues;

/// <summary>
/// The store category consists of internal server components that provide data storage and retrieval services.
/// </summary>
[XmppEnum]
public enum StoreValues
{
    /// <summary>
    /// A server component that stores data in a Berkeley database.
    /// </summary>
    [XmppMember("berkeley")]
    Berkeley,

    /// <summary>
    /// A server component that stores data on the file system.
    /// </summary>
    [XmppMember("file")]
    File,

    /// <summary>
    /// A server data storage component other than one of the registered types.
    /// </summary>
    [XmppMember("generic")]
    Generic,

    /// <summary>
    /// A server component that stores data in an LDAP database.
    /// </summary>
    [XmppMember("ldap")]
    Ldap,

    /// <summary>
    /// A server component that stores data in an MySQL database.
    /// </summary>
    [XmppMember("mysql")]
    MySQL,

    /// <summary>
    /// A server component that stores data in an Oracle database.
    /// </summary>
    [XmppMember("oracle")]
    Oracle,

    /// <summary>
    /// A server component that stores data in a PostgreSQL database
    /// </summary>
    [XmppMember("postgres")]
    Postgres
}