namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TagAttribute : Attribute
{
	public string Name { get; }
	public string Namespace { get; }

	public TagAttribute(string name, string? @namespace)
	{
		Name = name;
		Namespace = @namespace ?? string.Empty;
	}
}
