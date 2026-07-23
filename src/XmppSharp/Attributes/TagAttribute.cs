namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TagAttribute(string name, string ns) : Attribute
{
	public string Name { get; } = name;

	public string Namespace { get; } = ns;
}
