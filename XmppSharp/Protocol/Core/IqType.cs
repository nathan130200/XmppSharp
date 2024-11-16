using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Core;

[XmppEnum]
public enum IqType
{
    [XmppMember("error")]
    Error,

    [XmppMember("get")]
    Get,

    [XmppMember("set")]
    Set,

    [XmppMember("result")]
    Result
}