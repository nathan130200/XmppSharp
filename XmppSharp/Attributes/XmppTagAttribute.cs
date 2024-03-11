namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class XmppTagAttribute : Attribute
{
    public string LocalName { get; }
    public string Namespace { get; }

    public XmppTagAttribute(string localName, string @namespace = default)
    {
        LocalName = localName;
        Namespace = @namespace;
    }
}
