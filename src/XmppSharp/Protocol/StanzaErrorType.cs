using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum StanzaErrorType
{
    [XmppEnumMember("auth")]
    Auth,

    [XmppEnumMember("cancel")]
    Cancel,

    [XmppEnumMember("continue")]
    Continue,

    [XmppEnumMember("modify")]
    Modify,

    [XmppEnumMember("wait")]
    Wait
}
