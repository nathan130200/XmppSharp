namespace XmppSharp.Attributes;

using System.Xml.Linq;
using XmppSharp.Factory;

/// <summary>
/// Attribute responsible for registering elements using <see cref="ElementFactory" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class XmppTagAttribute : Attribute
{
    /// <summary>
    /// Name of the XML element.
    /// </summary>
    public XName Name { get; }

    /// <summary>
    /// Initialize a new instance of <see cref="XmppTagAttribute"/>.
    /// </summary>
    /// <param name="localName">Name of the element (without prefix).</param>
    /// <param name="namespace">XML namespace of the element.</param>
    public XmppTagAttribute(string localName, string @namespace = default)
        => Name = XName.Get(localName, @namespace);
}
