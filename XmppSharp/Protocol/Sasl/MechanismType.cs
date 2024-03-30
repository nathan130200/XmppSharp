using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

[XmppEnum]
public enum MechanismType
{
	[XmppMember("PLAIN")]
	Plain,
	
	[XmppMember("EXTERNAL")]
	External,
	
	[XmppMember("SCRAM-SHA-1-PLUS")]
	ScramSha1Plus,
	
	[XmppMember("SCRAM-SHA-1")]
	ScramSha1,
	
	[XmppMember("DIGEST-MD5")]
	DigestMD5
}