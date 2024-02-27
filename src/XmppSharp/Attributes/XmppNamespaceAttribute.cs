using System.Xml.Linq;

namespace XmppSharp.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class XmppNamespaceAttribute : Attribute
{
	public XNamespace Namespace { get; }
	public string? Prefix { get; init; }

	public XmppNamespaceAttribute(string @namespace)
	{
		Namespace = @namespace;
	}
}
