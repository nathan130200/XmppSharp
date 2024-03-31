using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Specifies the types of errors that can be reported within XMPP stanzas (XML messages).
/// Each enum value corresponds to a specific error condition and guides the recipient's response.
/// </summary>
[XmppEnum]
public enum StanzaErrorType
{
    /// <summary>
    /// Indicates an authentication-related error.
    /// </summary>
    [XmppMember("auth")]
    Auth,

    /// <summary>
    /// Signals that the recipient should cancel the current operation.
    /// </summary>
    [XmppMember("cancel")]
    Cancel,

    /// <summary>
    /// Instructs the recipient to continue with the operation, potentially with adjustments.
    /// </summary>
    [XmppMember("continue")]
    Continue,

    /// <summary>
    /// Requests the recipient to modify their request or behavior to proceed.
    /// </summary>
    [XmppMember("modify")]
    Modify,

    /// <summary>
    /// Suggests that the recipient try the operation again after a waiting period.
    /// </summary>
    [XmppMember("wait")]
    Wait
}