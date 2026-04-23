namespace XmppSharp.Attributes;

/// <summary>
/// Indicates the XML element name and namespace URI associated with a class, used for constructing XML elements in XMPP.
/// </summary>
/// <param name="name">The name of the XML element.</param>
/// <param name="uri">The namespace URI associated with the XML element.</param>
/// <remarks>
/// An class can be decorated with multiple <see cref="TagAttribute"/> attributes to specify different XML element names and namespaces for different contexts.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TagAttribute(string name, string uri) : Attribute
{
    /// <summary>
    /// Gets the name of the XML element.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the namespace URI of the XML element.
    /// </summary>
    public string NamespaceUri { get; } = uri;
}
