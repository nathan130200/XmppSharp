using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.XEP0085;

[XmppEnum]
public enum ChatStates
{
    /// <summary>
    /// User is actively participating in the chat session.
    /// </summary>
    [XmppMember("active")]
    Active,

    /// <summary>
    /// User has not been actively participating in the chat session.
    /// </summary>
    [XmppMember("inactive")]
    Inactive,

    /// <summary>
    /// User has effectively ended their participation in the chat session.
    /// </summary>
    [XmppMember("gone")]
    Gone,

    /// <summary>
    /// User is composing a message.
    /// </summary>
    [XmppMember("composing")]
    Composing,

    /// <summary>
    /// User had been composing but now has stopped.
    /// </summary>
    [XmppMember("paused")]
    Paused
}
