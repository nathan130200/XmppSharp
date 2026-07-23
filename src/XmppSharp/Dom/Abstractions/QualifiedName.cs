using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace XmppSharp.Dom.Abstractions;

internal readonly struct QualifiedName : IEquatable<QualifiedName>
{
	readonly int _hashCode;
	readonly string _str;

	public readonly string LocalName;

	public readonly string? Prefix;

	public QualifiedName(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		_str = XmlConvert.VerifyName(name);

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

	public readonly override string ToString() => _str;

	public static implicit operator string(QualifiedName name) => name._str;

	public readonly override int GetHashCode() => _hashCode;

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is QualifiedName other && Equals(other);

	public bool Equals(QualifiedName other)
	{
		if (ReferenceEquals(_str, other._str))
			return true;

		return _str.Equals(other._str, StringComparison.Ordinal);
	}

	public static implicit operator bool(QualifiedName name) => name._str != null;

	public static bool operator ==(QualifiedName x, QualifiedName y) => x.Equals(y);

	public static bool operator !=(QualifiedName x, QualifiedName y) => !x.Equals(y);
}