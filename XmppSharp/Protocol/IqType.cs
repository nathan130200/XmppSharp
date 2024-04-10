using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

/// <summary>
/// Determines the type of info query being requested.
/// </summary>
[XmppEnum]
public enum IqType
{
    /// <summary>
    /// The stanza reports an error that has occurred regarding processing or delivery of a get or set request.
    /// </summary>
    [XmppMember("error")]
    Error,

    /// <summary>
    /// The stanza provides data that is needed for an operation to be completed, sets new values, replaces existing values, etc.
    /// </summary>
    [XmppMember("set")]
    Set,

    /// <summary>
    /// The stanza requests information, inquires about what data is needed in order to complete further operations, etc.
    /// </summary>
    [XmppMember("get")]
    Get,

    /// <summary>
    /// The stanza is a response to a successful <see cref="Get" /> or <see cref="Set" /> request.
    /// </summary>
    [XmppMember("result")]
    Result
}