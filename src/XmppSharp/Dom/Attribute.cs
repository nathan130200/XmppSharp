using System.Globalization;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

public class Attribute : IXmlNode
{
	internal QName _name;

	internal Element? _parent;

	public Attribute(string name, object? value)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		_name = new(XmlConvert.VerifyName(name));

		Value = Convert.ToString(value, CultureInfo.InvariantCulture);
	}

	public Element? Parent => _parent;

	public bool IsNamespaceDeclaration => string.Equals(_name.LocalName, "xmlns", StringComparison.Ordinal)
		|| string.Equals(_name.Prefix, "xmlns", StringComparison.Ordinal);

	public string Name => _name.ToString();

	public string LocalName => _name.LocalName;

	public string? Prefix => _name.Prefix;

	public string? Value { get; set; }

	public Attribute Clone() => new(Name, Value);

	IXmlNode IXmlNode.Clone() => Clone();

	void IXmlNode.Remove()
	{
		throw new NotSupportedException();
	}

	public void WriteTo(XmlWriter writer)
	{
		if (_name.Prefix == null)
			writer.WriteAttributeString(_name.LocalName, Value);
		else
		{
			var ns = _name.Prefix switch
			{
				"xml" => Namespaces.Xml,
				"xmlns" => Namespaces.Xmlns,
				_ => _parent?.GetNamespace(_name.Prefix)
			};

			writer.WriteAttributeString(_name.Prefix, _name.LocalName, ns, Value);
		}
	}
}
