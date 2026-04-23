using System.ComponentModel;
using System.Xml;

namespace XmppSharp.Dom.Abstractions;

/// <summary>
/// Represents an XML name, consisting of an optional prefix and a local name.
/// </summary>
/// <param name="name">
/// The XML name to parse, which may include a prefix separated by a colon (e.g., "prefix:localName").
/// </param>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class QualifiedName(string name)
{
    /// <summary>
    /// Gets the XML name.
    /// </summary>
    public string Name { get; } = string.Intern(XmlConvert.VerifyName(name));

    readonly int _colonStart = name.IndexOf(':');

    /// <summary>
    /// Indicates whether the XML name has a prefix.
    /// </summary>
    public bool HasPrefix => _colonStart > 0;

    /// <summary>
    /// Gets the local name of the XML name.
    /// </summary>
    public string LocalName => _colonStart > 0 ? name[(_colonStart + 1)..] : Name;

    /// <summary>
    /// Gets the prefix of the XML name, or null if there is no prefix.
    /// </summary>
    public string? Prefix => _colonStart > 0 ? name[0.._colonStart] : default;
}
