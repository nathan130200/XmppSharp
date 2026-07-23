using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace XmppSharp.Dom.Abstractions;

internal readonly struct QName : IEquatable<QName>
{
	readonly int _hashCode, _colon;

	readonly string _name;

	public readonly string Name => _name;

	public readonly ReadOnlySpan<char> Prefix
		=> _colon > 0 ? _name.AsSpan(0.._colon) : [];

	public readonly ReadOnlySpan<char> LocalName
		=> _colon > 0 ? _name.AsSpan((_colon + 1)..) : _name;

	public QName(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		_name = XmlConvert.VerifyName(name);

		_colon = _name.IndexOf(':');
	}

	public void Deconstruct(out string? prefix, out string localName)
	{
		if (_colon > 0)
		{
			prefix = _name[0.._colon];
			localName = _name[(_colon + 1)..];
		}
		else
		{
			prefix = null;
			localName = _name;
		}
	}

	public readonly override string ToString() => _name;

	public readonly override int GetHashCode() => _hashCode;

	public readonly override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is QName other && Equals(other);

	public bool Equals(QName other)
	{
		if (ReferenceEquals(_name, other._name))
			return true;

		return _name.Equals(other._name, StringComparison.Ordinal);
	}

	public static bool operator ==(QName x, QName y) => x.Equals(y);

	public static bool operator !=(QName x, QName y) => !x.Equals(y);
}