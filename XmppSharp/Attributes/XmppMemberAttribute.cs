namespace XmppSharp.Attributes;

using XmppSharp.Protocol.Tls;

/// <summary>
/// Annotating a field or property with this attribute helps when retrieving metadata associated with it.
/// <para>
/// Example declaring an XMPP enum (using the enum <see cref="StartTlsPolicy"/> as a reference):
/// <code>
/// <![CDATA[
/// [XmppEnum]
/// public enum StartTlsPolicy
/// {
///     [XmppMember("optional")]
///     Optional,
///     
///     [XmppMember("required")]
///     Required
/// }
/// ]]>
/// </code>
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class XmppMemberAttribute : Attribute
{
    public string Name { get; }

    public XmppMemberAttribute(string name)
        => Name = name;
}
