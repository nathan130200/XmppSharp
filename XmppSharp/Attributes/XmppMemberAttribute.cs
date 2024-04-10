namespace XmppSharp.Attributes;
/// <summary>
/// Annotating a field or property with this attribute helps when retrieving metadata associated with it.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class XmppMemberAttribute : Attribute
{
    public string Name { get; }

    public XmppMemberAttribute(string name)
        => Name = name;
}
