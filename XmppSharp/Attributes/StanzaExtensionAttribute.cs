using System.Xml;

namespace XmppSharp.Attributes;

/// <summary>Represents attribute for register stanza extensions.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class StanzaExtensionAttribute(string localName, string uri) : Attribute
{
	/// <summary>Gets the XML local name of extension.</summary>
	public string LocalName { get; } = XmlConvert.VerifyNCName(localName);

	/// <summary>Gets the XML namespace of extension.</summary>
	public string Namespace { get; } = uri;
}
