using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

/// <summary>
/// Defines the type of an error condition. This is used in the <see cref="Error"/> class to specify the type of error that occurred.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Indicates an error related to authentication.
    /// </summary>
    /// <remarks>This type of error occurs when there is a failure in the authentication process, such as invalid credentials or an expired session.</remarks>
    [XmppEnumMember("auth")]
    Auth = 1,

    /// <summary>
    /// Indicates that the action was canceled.
    /// </summary>
    /// <remarks>This type of error occurs when a request is intentionally aborted.</remarks>
    [XmppEnumMember("cancel")]
    Cancel,

    /// <summary>
    /// Indicates that the action should continue.
    /// </summary>
    /// <remarks>This type of error occurs when a request can proceed despite an error condition.</remarks>
    [XmppEnumMember("continue")]
    Continue,

    /// <summary>
    /// Indicates that the action should be modified.
    /// </summary>
    /// <remarks>This type of error occurs when a request needs to be adjusted before it can be processed.</remarks>
    [XmppEnumMember("modify")]
    Modify,

    /// <summary>
    /// Indicates that the action should be delayed.
    /// </summary>
    /// <remarks>This type of error occurs when a request needs to wait before it can be processed.</remarks>
    [XmppEnumMember("wait")]
    Wait
}
