using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

/// <summary>
/// Represents an XML attribute.
/// </summary>
public sealed class Attribute : IXmlNode
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal Element? _parent;

	/// <inheritdoc/>
	public Element? Parent => _parent;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	readonly int _prefixEnd;

	/// <summary>
	/// Gets the attribute qualified name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets or sets the attribute value.
	/// </summary>
	public string Value { get; set; }

	/// <summary>
	/// Gets the local name part of the attribute name.
	/// </summary>
	public ReadOnlySpan<char> Prefix => _prefixEnd > 0 ? Name.AsSpan(0.._prefixEnd) : [];

	/// <summary>
	/// Gets the prefix part of the attribute name.
	/// </summary>
	public ReadOnlySpan<char> LocalName => _prefixEnd > 0 ? Name.AsSpan((_prefixEnd + 1)..) : Name;

	internal Attribute(string name, object? value)
	{
		XmlConvert.VerifyName(name);

		Name = name;

		_prefixEnd = name.IndexOf(':');

		Value = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
	}

	/// <summary>
	/// Gets a value indicating if this attribute is a namespace declaration.
	/// </summary>
	/// <remarks>
	/// An attribute declaration should be:
	/// <list type="bullet">
	/// <item>The <see cref="Prefix"/> equals to <c>xmlns</c>.</item>
	/// <item>The <see cref="Prefix"/> should be <b>empty</b> and <see cref="LocalName"/> equals to <c>xmlns</c>.</item>
	/// </list>
	/// </remarks>
	public bool IsNamespaceDeclaration => Prefix.Equals("xmlns", StringComparison.Ordinal)
		|| (Prefix.IsEmpty && LocalName.Equals("xmlns", StringComparison.Ordinal));

	/// <summary>
	/// Sets the attribute value from converting <paramref name="value"/> to string representation.
	/// </summary>
	/// <param name="value">The new value of attribute.</param>
	public void SetValue(object? value)
	{
		Value = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
	}

	internal Attribute Clone() => new(Name, Value);

	IXmlNode IXmlNode.Clone() => Clone();

	void IXmlNode.WriteTo(XmlWriter writer) => WriteTo(writer);

	/// <summary>
	/// Remove this attribute from its parent.
	/// </summary>
	public void Remove()
	{
		lock (this)
			_parent?.RemoveAttribute(this);
	}

	internal void WriteTo(XmlWriter writer)
	{
		if (Prefix.IsEmpty)
			writer.WriteAttributeString(Name, Value);
		else
		{
			var prefix = Name[.._prefixEnd];
			var localName = Name[(_prefixEnd + 1)..];

			string? ns;

			if (Prefix.Equals("xml", StringComparison.Ordinal))
				ns = Namespaces.Xml;
			else if (Prefix.Equals("xmlns", StringComparison.Ordinal))
				ns = Namespaces.Xmlns;
			else
				ns = _parent?.GetNamespaceOfPrefix(prefix);

			writer.WriteAttributeString(prefix, localName, ns, Value);
		}
	}

	static T? ParseOrNull<T>(string? value) where T : struct, IParsable<T>
	{
		if (T.TryParse(value, CultureInfo.InvariantCulture, out var result))
			return result;

		return null;
	}

	static T? ParseOrDefault<T>(string? value) where T : IParsable<T>
	{
		if (T.TryParse(value, CultureInfo.InvariantCulture, out var result))
			return result;

		return default;
	}

#pragma warning disable

	public static implicit operator bool(Attribute? a)
	{
		if (a is null) return false;

		if (a.Value.Length == 1)
			return a.Value[0] == '1';

		return bool.TryParse(a.Value, out var result) && result;
	}

	public static implicit operator bool?(Attribute? a)
	{
		if (string.IsNullOrWhiteSpace(a?.Value)) return null;

		if (a.Value.Length == 1)
			return a.Value[0] == '1';

		return bool.TryParse(a.Value, out var result) ? result : null;
	}

	[return: NotNullIfNotNull(nameof(a))]
	public static implicit operator string?(Attribute? a) => a?.Value;

	public static implicit operator sbyte(Attribute? a) => ParseOrDefault<sbyte>(a?.Value);
	public static implicit operator short(Attribute? a) => ParseOrDefault<short>(a?.Value);
	public static implicit operator int(Attribute? a) => ParseOrDefault<int>(a?.Value);
	public static implicit operator long(Attribute? a) => ParseOrDefault<long>(a?.Value);
	public static implicit operator byte(Attribute? a) => ParseOrDefault<byte>(a?.Value);
	public static implicit operator ushort(Attribute? a) => ParseOrDefault<ushort>(a?.Value);
	public static implicit operator uint(Attribute? a) => ParseOrDefault<uint>(a?.Value);
	public static implicit operator ulong(Attribute? a) => ParseOrDefault<ulong>(a?.Value);

	public static implicit operator sbyte?(Attribute? a) => ParseOrNull<sbyte>(a?.Value);
	public static implicit operator short?(Attribute? a) => ParseOrNull<short>(a?.Value);
	public static implicit operator int?(Attribute? a) => ParseOrNull<int>(a?.Value);
	public static implicit operator long?(Attribute? a) => ParseOrNull<long>(a?.Value);
	public static implicit operator byte?(Attribute? a) => ParseOrNull<byte>(a?.Value);
	public static implicit operator ushort?(Attribute? a) => ParseOrNull<ushort>(a?.Value);
	public static implicit operator uint?(Attribute? a) => ParseOrNull<uint>(a?.Value);
	public static implicit operator ulong?(Attribute? a) => ParseOrNull<ulong>(a?.Value);

	public static implicit operator float(Attribute? a) => ParseOrDefault<float>(a?.Value);
	public static implicit operator float?(Attribute? a) => ParseOrNull<float>(a?.Value);
	public static implicit operator double(Attribute? a) => ParseOrDefault<double>(a?.Value);
	public static implicit operator double?(Attribute? a) => ParseOrNull<double>(a?.Value);

#pragma warning restore
}
