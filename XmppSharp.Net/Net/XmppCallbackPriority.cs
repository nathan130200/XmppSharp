namespace XmppSharp.Net;

/// <summary>
/// Specifies the priority levels for XMPP callback execution.
/// </summary>
public enum XmppCallbackPriority
{
    /// <summary>
    /// Represents a low priority level for callback execution order.
    /// </summary>
    Low,

    /// <summary>
    /// Represents the default priority level for callback execution.
    /// </summary>
    Normal,

    /// <summary>
    /// Represents the highest priority level for callback execution order.
    /// </summary>
    High,
}