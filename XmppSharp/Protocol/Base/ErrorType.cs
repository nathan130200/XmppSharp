using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppEnum]
public enum ErrorType
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
