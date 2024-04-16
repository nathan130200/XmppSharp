namespace XmppSharp;

public readonly record struct XmlTagInfo : IEquatable<XmlTagInfo>
{
    private readonly string _localName;
    private readonly string _namespaceURI;

    public XmlTagInfo(string localName, string namespaceURI = default)
    {
        _localName = localName;
        _namespaceURI = namespaceURI ?? string.Empty;
    }

    public string LocalName
    {
        get => _localName;
        init => _localName = value;
    }

    public string NamespaceURI
    {
        get => _namespaceURI;
        init => _namespaceURI = value ?? string.Empty;
    }

    public readonly void Deconstruct(out string localName, out string? namespaceURI)
    {
        localName = _localName;
        namespaceURI = _namespaceURI;
    }

    public readonly override int GetHashCode()
        => HashCode.Combine(LocalName, NamespaceURI);

    public readonly bool Equals(XmlTagInfo other)
    {
        return LocalName.Equals(other.LocalName, StringComparison.Ordinal)
            && NamespaceURI.Equals(other.NamespaceURI, StringComparison.Ordinal);
    }

    public static IEqualityComparer<XmlTagInfo> Comparer { get; }
        = EqualityComparer<XmlTagInfo>.Default;
}
