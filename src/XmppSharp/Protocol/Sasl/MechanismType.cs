using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppEnum]
public enum MechanismType
{
    [XmppEnumMember("PLAIN")]
    Plain,

    [XmppEnumMember("DIGEST-MD5")]
    DigestMd5,

    [XmppEnumMember("SCRAM-SHA-1")]
    ScramSha1,

    [XmppEnumMember("EXTERNAL")]
    External,
}