using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Defines the type of a message stanza.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// The default message type. It is used when the "type" attribute is not specified in the message stanza.
    /// </summary>
    [XmppEnumMember("normal")]
    Normal,

    /// <summary>
    /// Indicates that an error occurred during message processing.
    /// </summary>
    [XmppEnumMember("error")]
    Error,

    /// <summary>
    /// Indicates a one-to-one chat message.
    /// </summary>
    [XmppEnumMember("chat")]
    Chat,

    /// <summary>
    /// Indicates a message sent to a multi-user chat room (MUC).
    /// </summary>
    [XmppEnumMember("groupchat")]
    GroupChat,

    /// <summary>
    /// Indicates a headline message, which is typically used for news or notifications.
    /// </summary>
    [XmppEnumMember("headline")]
    Headline
}
