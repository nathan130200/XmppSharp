using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum IqType
{
    [XmppEnumMember("get")]
    Get,

    [XmppEnumMember("set")]
    Set,

    [XmppEnumMember("result")]
    Result,

    [XmppEnumMember("error")]
    Error
}
