using XmppSharp.Attributes;

namespace XmppSharp.Protocol;

[XmppEnum]
public enum IqType
{
    [XmppEnumMember("error")]
    Error,

    [XmppEnumMember("set")]
    Set,

    [XmppEnumMember("get")]
    Get,

    [XmppEnumMember("result")]
    Result,
}
