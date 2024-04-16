namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class XmppTagAttribute : Attribute
{
    public string LocalName { get; }
    public string NamespaceURI { get; }

    public XmppTagAttribute(string localName, string namespaceURI)
    {
        LocalName = localName;
        NamespaceURI = namespaceURI;
    }
}
