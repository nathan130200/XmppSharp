namespace XmppSharp;

public readonly record struct XmlTagInfo : IEquatable<XmlTagInfo>
{
	private readonly string _localName;
	private readonly string _namespaceURI;

	public XmlTagInfo(string localName, string namespaceURI = default)
	{
		this._localName = localName;
		this._namespaceURI = namespaceURI ?? string.Empty;
	}

	public string LocalName
	{
		get => this._localName;
		init => this._localName = value;
	}

	public string NamespaceURI
	{
		get => this._namespaceURI;
		init => this._namespaceURI = value ?? string.Empty;
	}

	public readonly void Deconstruct(out string localName, out string? namespaceURI)
	{
		localName = this._localName;
		namespaceURI = this._namespaceURI;
	}

	public readonly override int GetHashCode()
		=> HashCode.Combine(this.LocalName, this.NamespaceURI);

	public readonly bool Equals(XmlTagInfo other)
	{
		return this.LocalName.Equals(other.LocalName, StringComparison.Ordinal)
			&& this.NamespaceURI.Equals(other.NamespaceURI, StringComparison.Ordinal);
	}

	public static IEqualityComparer<XmlTagInfo> Comparer { get; }
		= EqualityComparer<XmlTagInfo>.Default;
}
