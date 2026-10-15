using System.Xml;

namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class StanzaExtensionAttribute(string localName, string uri) : Attribute
{
    public string LocalName { get; } = XmlConvert.VerifyNCName(localName);

    public string Namespace { get; } = uri;
}
