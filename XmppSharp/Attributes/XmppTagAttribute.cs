namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class XmppTagAttribute : Attribute
{
    public string TagName { get; }
    public string NamespaceURI { get; }

    public XmppTagAttribute(string tagName, string namespaceURI)
    {
        TagName = tagName;
        NamespaceURI = namespaceURI;
    }
}
