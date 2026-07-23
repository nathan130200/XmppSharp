using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace XmppSharp.Dom.Abstractions;

internal readonly struct QName : IEquatable<QName>
{
	readonly int _hashCode;

	public readonly string Name;
	public readonly string LocalName;
	public readonly string? Prefix;

	public QName(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		Name = XmlConvert.VerifyName(name);

		var colon = name.IndexOf(':');

		if (colon == -1)
			LocalName = name;
		else
		{
			Prefix = name[..colon];
			LocalName = name[(colon + 1)..];
		}

		_hashCode = HashCode.Combine(Prefix?.GetHashCode(StringComparison.Ordinal), LocalName.GetHashCode(StringComparison.Ordinal));
	}

	public readonly override string ToString() => Name;

	public readonly override int GetHashCode() => _hashCode;

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is QName other && Equals(other);

	public bool Equals(QName other)
	{
		if (ReferenceEquals(Name, other.Name))
			return true;

		return Name.Equals(other.Name, StringComparison.Ordinal);
	}

	public static implicit operator bool(QName name) => name.Name != null;

	public static bool operator ==(QName x, QName y) => x.Equals(y);

	public static bool operator !=(QName x, QName y) => !x.Equals(y);
}