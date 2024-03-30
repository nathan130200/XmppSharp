using System.Diagnostics.CodeAnalysis;
using XmppSharp.Dom;

namespace XmppSharp.Factory;

public readonly struct XmlTagName
{
    public string LocalName { get; }
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