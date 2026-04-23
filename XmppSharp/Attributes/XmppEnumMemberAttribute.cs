namespace XmppSharp.Attributes;


/// <summary>
/// Defines an attribute to specify the XML string representation of an enum member for serialization and deserialization purposes.
/// </summary>
/// <param name="name">
/// The XML string representation of this enum member.
/// </param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class XmppEnumMemberAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the XML string representation of the enum member associated with this attribute.
    /// </summary>
    public string Name { get; } = name;
}
