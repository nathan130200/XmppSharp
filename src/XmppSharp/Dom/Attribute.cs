using System.Globalization;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

public class Attribute : IXmlNode
{
	internal QName _attributeName;

	internal Element? _parent;

	public Attribute(string name, object? value)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		_attributeName = new(XmlConvert.VerifyName(name));

		Value = Convert.ToString(value, CultureInfo.InvariantCulture);
	}

	public Element? Parent => _parent;

	public bool IsNamespaceDeclaration => _attributeName.LocalName.Equals("xmlns", StringComparison.Ordinal)
		|| _attributeName.Prefix.Equals("xmlns", StringComparison.Ordinal);

	public string Name => _attributeName.Name;

	public ReadOnlySpan<char> LocalName => _attributeName.LocalName;

	public ReadOnlySpan<char> Prefix => _attributeName.Prefix;

	public string? Value { get; set; }

	public Attribute Clone() => new(_attributeName.Name, Value);

	IXmlNode IXmlNode.Clone() => Clone();

	void IXmlNode.Remove() => _parent?.RemoveAttributeInternal(this);

	public void WriteTo(XmlWriter writer)
	{
		var (prefix, localName) = _attributeName;

		if (prefix == null)
			writer.WriteAttributeString(localName, Value);
		else
		{
			var ns = _attributeName.Prefix switch
			{
				"xml" => Namespaces.Xml,
				"xmlns" => Namespaces.Xmlns,
				_ => _parent?.GetNamespace(_attributeName.Prefix)
			};

			writer.WriteAttributeString(prefix, localName, ns, Value);
		}
	}
}
