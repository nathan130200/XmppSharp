using System.Diagnostics.CodeAnalysis;
using XmppSharp.Dom;

namespace XmppSharp.Factory;

/// <summary>
/// Represents an XML element's name, consisting of a local name and an optional namespace.
/// </summary>
public readonly struct XmlTagName
{
    /// <summary>
    /// The local name of the XML tag.
    /// </summary>
    public string LocalName { get; }

    /// <summary>
    /// The namespace of the XML tag, or null if no namespace is specified.
    /// </summary>
    public string? Namespace { get; }

    internal XmlTagName(string localName, string? @namespace)
    {
        LocalName = localName;
        Namespace = @namespace;
    }

    public override int GetHashCode()
    {
        if (Namespace == null)
            return LocalName.GetHashCode();

        return HashCode.Combine(LocalName, Namespace);
    }

    public override bool Equals(object obj)
    {
        if (obj is not XmlTagName other)
            return false;

        return string.Equals(LocalName, other.LocalName, StringComparison.Ordinal)
            && string.Equals(Namespace, other.Namespace, StringComparison.Ordinal);
    }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(Namespace))
            return LocalName;

        return string.Concat('{', Namespace, '}', LocalName);
    }

    public static bool operator ==(XmlTagName lhs, XmlTagName rhs)
        => lhs.Equals(rhs);

    public static bool operator !=(XmlTagName lhs, XmlTagName rhs)
        => !(lhs == rhs);

    /// <summary>
    /// Provides equality comparison functionality for <see cref="XmlTagName"/> instances.
    /// </summary>
    public static IEqualityComparer<XmlTagName> Comparer { get; }
        = new XmlNameEqualityComparer();

    internal bool HasName(Element e)
    {
        return e.LocalName == LocalName
            && (e.Namespace == null || e.Namespace == Namespace);
    }
}

class XmlNameEqualityComparer : IEqualityComparer<XmlTagName>
{
    public bool Equals(XmlTagName x, XmlTagName y)
        => x.Equals(y);

    public int GetHashCode([DisallowNull] XmlTagName obj)
        => obj.GetHashCode();
}