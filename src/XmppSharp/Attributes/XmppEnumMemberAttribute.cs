namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class XmppEnumMemberAttribute : Attribute
{
    public string Value { get; }

    public XmppEnumMemberAttribute(string name)
        => Value = name;
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
public sealed class XmppTagAttribute : Attribute
{
    public string Name { get; }
    public string Namespace { get; }

    public XmppTagAttribute(string name)
        => Name = name;

    public XmppTagAttribute(string name, string xmlns) : this(name)
        => Namespace = xmlns;
}