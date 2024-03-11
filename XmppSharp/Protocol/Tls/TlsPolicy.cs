using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppEnum]
public enum TlsPolicy
{
    [XmppEnumMember("optional")]
    Optional,

    [XmppEnumMember("required")]
    Required
}