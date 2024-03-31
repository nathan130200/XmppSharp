namespace XmppSharp.Attributes;

/// <summary>
/// Declaring an enum with this attribute gives the advantage of being able to use <see cref="XmppEnum"/> auxiliary functions to parse/serialize the enum's fields.
/// <para>
/// <list type="bullet">
/// <item><see cref="XmppEnum.GetNames"/></item>
/// <item><see cref="XmppEnum.GetValues"/></item>
/// <item><see cref="XmppEnum.ParseOrDefault"/></item>
/// <item><see cref="XmppEnum.ParseOrThrow"/></item>
/// <item><see cref="XmppEnum.Parse"/></item>
/// <item><see cref="XmppEnum.ToXmppName"/></item>
/// </list>
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
public sealed class XmppEnumAttribute : Attribute
{

}
