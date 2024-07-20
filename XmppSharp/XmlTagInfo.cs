namespace XmppSharp;

public readonly record struct XmlTagInfo : IEquatable<XmlTagInfo>
{
	public string LocalName { get; init; }
	public string? NamespaceURI { get; init; }

	public XmlTagInfo(string localName, string? namespaceURI = default)
	{
		this.LocalName = localName;
		this.NamespaceURI = namespaceURI ?? string.Empty;
	}

	public readonly void Deconstruct(out string localName, out string? namespaceURI)
	{
		localName = this.LocalName;
		namespaceURI = this.NamespaceURI;
	}

	public readonly override int GetHashCode()
		=> HashCode.Combine(this.LocalName, this.NamespaceURI);

	public readonly bool Equals(XmlTagInfo other)
	{
		return this.LocalName.Equals(other.LocalName, StringComparison.Ordinal)
			&& this.NamespaceURI?.Equals(other.NamespaceURI, StringComparison.Ordinal) == true;
	}

	public static IEqualityComparer<XmlTagInfo> Comparer { get; }
		= EqualityComparer<XmlTagInfo>.Default;
}
