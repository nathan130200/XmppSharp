namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class XmppEnumMemberAttribute(string name) : Attribute
{
	public string Name { get; } = name;
}
