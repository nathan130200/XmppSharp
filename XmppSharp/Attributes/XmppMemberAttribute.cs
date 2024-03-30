namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class XmppMemberAttribute : Attribute
{
    public string Name { get; }

    public XmppMemberAttribute(string name)
        => Name = name;
}
