namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class XmppMemberAttribute : Attribute
{
    public string Value { get; }

    public XmppMemberAttribute(string value)
    {
        Value = value;
    }
}
