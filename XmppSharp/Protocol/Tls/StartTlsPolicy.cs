using System.Runtime.Serialization;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Tls;

[XmppEnum]
public enum StartTlsPolicy
{
    [DataMember(Name = nameof(Optional))]
    [XmppMember("optional")]
    Optional,

    [DataMember(Name = nameof(Required))]
    [XmppMember("required")]
    Required
}