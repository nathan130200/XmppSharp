using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Defines the type of an IQ stanza in the XMPP protocol.
/// </summary>
public enum IqType
{
    /// <summary>
    /// Indicates that the IQ stanza is of type "error". This type is used to indicate that an error occurred while processing a previous IQ request.
    /// </summary>
    [XmppEnumMember("error")]
    Error,

    /// <summary>
    /// Indicates that the IQ stanza is of type "get". This type is used to request information from another entity in the XMPP network.
    /// </summary>
    [XmppEnumMember("get")]
    Get,

    /// <summary>
    /// Indicates that the IQ stanza is of type "set". This type is used to set or update information on another entity in the XMPP network.
    /// </summary>
    [XmppEnumMember("set")]
    Set,

    /// <summary>
    /// Indicates that the IQ stanza is of type "result". This type is used to indicate a successful response to a previous IQ request.
    /// </summary>
    [XmppEnumMember("result")]
    Result
}
