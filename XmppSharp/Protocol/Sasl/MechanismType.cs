using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppEnum]
public enum MechanismType
{
    Unspecified,

    [XmppEnumMember("PLAIN")]
    Plain,

    [XmppEnumMember("EXTERNAL")]
    External,

    [XmppEnumMember("SCRAM-SHA-1-PLUS")]
    ScramSha1Plus,

    [XmppEnumMember("SCRAM-SHA-1")]
    ScramSha1,

    [XmppEnumMember("DIGEST-MD5")]
    DigestMd5
}
