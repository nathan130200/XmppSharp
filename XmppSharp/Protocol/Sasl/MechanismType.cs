using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// Enumerates the possible SASL (Simple Authentication and Security Layer) mechanisms supported by XMPP.
/// <para>The <b><see cref="Unspecified"/></b> value serves as a fallback during parsing when a mechanism is not explicitly identified.</para>
/// </summary>
[XmppEnum]
public enum MechanismType
{
	/// <summary>
	/// A fallback value used during parsing when a mechanism cannot be explicitly identified.
	/// </summary>
	Unspecified,

	/// <summary>
	/// Denotes the PLAIN SASL mechanism, which transmits authentication credentials in a base64 encoded string (insecure).
	/// </summary>
	[XmppMember("PLAIN")]
	Plain,

	/// <summary>
	/// Denotes the EXTERNAL SASL mechanism, which relies on external security context (not widely used).
	/// </summary>
	[XmppMember("EXTERNAL")]
	External,

	/// <summary>
	/// Denotes the SCRAM-SHA-1-PLUS SASL mechanism, a secure mechanism with improved security features over SCRAM-SHA-1.
	/// </summary>
	[XmppMember("SCRAM-SHA-1-PLUS")]
	ScramSha1Plus,

	/// <summary>
	/// Denotes the SCRAM-SHA-1 SASL mechanism, a secure mechanism using the SHA-1 hashing algorithm.
	/// </summary>
	[XmppMember("SCRAM-SHA-1")]
	ScramSha1,

	/// <summary>
	/// Denotes the DIGEST-MD5 SASL mechanism, a legacy mechanism using MD5 hashing (considered insecure).
	/// </summary>
	[XmppMember("DIGEST-MD5")]
	DigestMD5,
}