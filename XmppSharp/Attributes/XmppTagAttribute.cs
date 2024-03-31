namespace XmppSharp.Attributes;

using XmppSharp.Dom;
using XmppSharp.Factory;

/// <summary>
/// Attribute responsible for registering elements using <see cref="ElementFactory" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class XmppTagAttribute : Attribute
{
    /// <summary>
    /// Local name of the element (no prefix).
    /// </summary>
    public string LocalName { get; }

    /// <summary>
    /// XML namespace of the element.
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Initialize a new instance of <see cref="XmppTagAttribute"/>.
    /// </summary>
    /// <param name="localName">Local name of the element (no prefix).</param>
    /// <param name="namespace">XML namespace of the element.</param>
    /// <remarks>
    /// You <b>MUST</b> call the <see cref="Element" /> base class constructor to ensure that the element will be populated correctly when parsed by the <see cref="Parser" />.
    /// </remarks>
    public XmppTagAttribute(string localName, string @namespace = default)
    {
        LocalName = localName;
        Namespace = @namespace;
    }
}
