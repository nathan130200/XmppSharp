using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace XmppSharp.Abstractions;

public readonly struct XmlNameInfo : IEquatable<XmlNameInfo>
{
	internal readonly string _qualifiedName;

	public string Prefix { get; }
	public string LocalName { get; }
	public bool HasPrefix => !string.IsNullOrWhiteSpace(Prefix);

	public XmlNameInfo(string name)
	{
		var ofs = name.IndexOf(':');

		if (ofs > 0)
		{
			Prefix = XmlConvert.VerifyName(name[0..ofs]);
			LocalName = XmlConvert.VerifyName(name[(ofs + 1)..]);
			_qualifiedName = name;
		}
		else
		{
			LocalName = XmlConvert.VerifyName(name);
		}

		_qualifiedName = name;
	}

	public override string ToString() => _qualifiedName;

	public override int GetHashCode() => _qualifiedName.GetHashCode();

	public override bool Equals([NotNullWhen(true)] object? obj) => obj is XmlNameInfo name && Equals(name);

	public bool Equals(XmlNameInfo other) => string.Equals(_qualifiedName, other._qualifiedName, StringComparison.Ordinal);

	public static bool operator !=(XmlNameInfo x, XmlNameInfo y) => !(x == y);

	public static bool operator ==(XmlNameInfo x, XmlNameInfo y) => x.Equals(y);

	public static implicit operator string(XmlNameInfo self) => self._qualifiedName;

	public static implicit operator XmlNameInfo(string s)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(s);
		return new XmlNameInfo(s);
	}
}