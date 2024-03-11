namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class XmppEnumMemberAttribute : Attribute
{
    public string Name { get; }

    public XmppEnumMemberAttribute(string name)
        => Name = name;
}


[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
public class XmppEnumAttribute : Attribute
{
    public string Namespace { get; }

    public XmppEnumAttribute(string @namespace = default)
        => Namespace = @namespace;
}
