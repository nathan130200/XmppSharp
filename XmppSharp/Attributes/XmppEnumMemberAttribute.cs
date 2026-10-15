namespace XmppSharp.Attributes;

/// <summary>Represents attribute descriptor for enum members.</summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class XmppEnumMemberAttribute(string value) : Attribute
{
	/// <summary>Gets the string value.</summary>
	public string Value { get; } = value;
}