namespace XmppSharp.Abstractions;

public class XmlTagInfo(string name, string @namespace) : IEquatable<XmlTagInfo>
{
	public readonly string Name = name;
	public readonly string Namespace = @namespace;

	public override int GetHashCode()
	{
		return HashCode.Combine(Name, Namespace);
	}

	public override string ToString()
	{
		return string.Concat('{', Namespace, '}', Name);
	}

	public override bool Equals(object? obj) => obj is XmlTagInfo other && Equals(other);

	public bool Equals(XmlTagInfo? other)
	{
		if (other is null) return false;

		if (ReferenceEquals(other, this)) return true;

		return string.Equals(Name, other.Name, StringComparison.Ordinal)
			&& string.Equals(Namespace, other.Namespace, StringComparison.Ordinal);
	}

	public static bool operator !=(XmlTagInfo? x, XmlTagInfo? y) => !(x == y);

	public static bool operator ==(XmlTagInfo? x, XmlTagInfo? y)
	{
		if (x is null && y is null) return true;

		if (x is null || y is null) return false;

		return x.Equals(y);
	}
}