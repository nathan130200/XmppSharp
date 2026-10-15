using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

/// <summary>
/// Represents a XML element.
/// </summary>
public sealed class Element : Container
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal readonly List<Attribute> _attributes = [];

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	int _prefixEnd;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	string _name;

	/// <summary>
	/// Gets or sets the qualified name of this node.
	/// </summary>
	public string Name
	{
		get => _name;
		set
		{
			_name = XmlConvert.VerifyName(value);

			_prefixEnd = _name.IndexOf(':');
		}
	}

	/// <summary>
	/// Gets a child element.
	/// </summary>
	/// <param name="name">Element qualified name.</param>
	/// <param name="ns">Element namespace URI.</param>
	public Element? this[string name, string? ns = default]
		=> Element(name, ns);

	/// <summary>
	/// Gets the prefix part from this element name.
	/// </summary>
	public ReadOnlySpan<char> Prefix => _prefixEnd > 0 ? Name.AsSpan(0.._prefixEnd) : [];

	/// <summary>
	/// Gets the local name part from this element name.
	/// </summary>
	public ReadOnlySpan<char> LocalName => _prefixEnd > 0 ? Name.AsSpan((_prefixEnd + 1)..) : Name;

	/// <summary>
	/// Gets or sets the namespace of this element.
	/// </summary>
	public string? Namespace
	{
		get;
		set;
	}

	/// <summary>
	/// Initialize a new instance of <see cref="Element"/> class.
	/// </summary>
	/// <param name="name">Element qualified name.</param>
	/// <param name="ns">Element namespace.</param>
	public Element(string name, string? ns = default)
	{
		_name = XmlConvert.VerifyName(name);

		_prefixEnd = name.IndexOf(':');

		if (_prefixEnd > 0)
		{
			var prefix = name.AsSpan(0.._prefixEnd);

			if (prefix.IsEmpty || prefix.IsWhiteSpace())
				throw new ArgumentException("Namespace cannot be null or empty.");
		}

		Namespace = ns;
	}

	/// <inheritdoc />
	public override Node Clone()
	{
		lock (this)
		{
			var result = new Element(Name, Namespace);

			lock (_attributes)
			{
				foreach (var attribute in _attributes)
				{
					var temp = attribute.Clone();
					result._attributes.Add(temp);
					temp._parent = result;
				}
			}

			return result;
		}
	}

	/// <summary>
	/// Gets the first child element.
	/// </summary>
	public Element? FirstChild
	{
		get
		{
			lock (_children)
			{
				var span = CollectionsMarshal.AsSpan(_children);

				for (int i = 0; i < span.Length; i++)
				{
					if (span[i] is Element e)
						return e;
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Gets the last child element.
	/// </summary>
	public Element? LastChild
	{
		get
		{
			lock (_children)
			{
				var span = CollectionsMarshal.AsSpan(_children);

				for (int i = span.Length - 1; i >= 0; i--)
				{
					if (span[i] is Element e)
						return e;
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Add child node to current element.
	/// </summary>
	/// <param name="node">XML node to add.</param>
	/// <remarks>
	/// If the node already have an owner, the node will be cloned.
	/// </remarks>
	public void AddChild(Node? node)
	{
		if (node is null) return;

		var syncObj = node;

		lock (syncObj)
		{
			if (node._parent != null)
				node = node.Clone();

			lock (_children)
			{
				_children.Add(node);

				node._parent = this;
			}
		}
	}

	/// <summary>
	/// Remove child node from current element.
	/// </summary>
	/// <param name="node">XML node to remove.</param>
	public void RemoveChild(Node? node)
	{
		if (node is null) return;

		var syncObj = node;

		lock (syncObj)
		{
			if (node._parent != this) return;

			lock (_children)
				_children.Remove(node);

			node._parent = null;
		}
	}

	/// <summary>
	/// Gets an XML attribute.
	/// </summary>
	public Attribute? Attribute(string name)
	{
		lock (_attributes)
			return _attributes.Find(x => x.Name == name);
	}

	/// <summary>
	/// Gets an list of XML attributes in current element.
	/// </summary>
	public IEnumerable<Attribute> Attributes()
	{
		lock (_attributes)
			return [.. _attributes];
	}

	/// <summary>
	/// Sets an XML attribute.
	/// </summary>
	/// <param name="name">XML attribute name.</param>
	/// <param name="value">XML attribute value.</param>
	/// <remarks>
	/// <list type="bullet">
	/// <item>If <paramref name="value"/> is <see langword="null" /> and existing attribute is found, it will be removed.</item>
	/// <item>The <paramref name="value"/> will be converted to string representation in invariant culture.</item>
	/// </list>
	/// </remarks>
	public void SetAttribute(string name, object? value)
	{
		lock (_attributes)
		{
			var attr = Attribute(name);

			if (value is null)
			{
				attr?.Remove();
				return;
			}

			if (attr != null)
				attr.SetValue(value);
			else
			{
				_attributes.Add(new Attribute(name, value)
				{
					_parent = this
				});
			}
		}
	}

	/// <summary>
	/// Gets an XML attribute value.
	/// </summary>
	/// <param name="name">XML attribute name.</param>
	/// <param name="defaultValue">Optional fallback value.</param>
	/// <returns>XML attribute value if found; otherwise <paramref name="defaultValue"/>.</returns>
	public string? GetAttribute(string name, string? defaultValue = default)
	{
		if (Attribute(name) is { } attr)
			return attr.Value;

		return defaultValue;
	}

	/// <summary>
	/// Check if XML attribute exists.
	/// </summary>
	/// <param name="name">XML attribute name.</param>
	/// <returns><see langword="true"/> if exists; otherwise <see langword="false" />.</returns>
	public bool HasAttribute(string name)
	{
		lock (_attributes)
			return _attributes.FindIndex(x => x.Name == name) != -1;
	}

	/// <summary>
	/// Removes an XML attribute.
	/// </summary>
	/// <param name="name">XML attribute name.</param>
	/// <returns><see langword="true" /> if was removed; otherwise <see langword="false"/></returns>
	public bool RemoveAttribute(string name)
	{
		lock (_attributes)
		{
			var entry = _attributes.Find(x => x.Name == name);

			if (entry is null) return false;

			lock (entry)
				RemoveAttribute(entry);

			return true;
		}
	}

	internal void RemoveAttribute(Attribute? attr)
	{
		if (attr is null) return;

		lock (_attributes)
			_attributes.Remove(attr);

		attr._parent = null;
	}

	/// <summary>
	/// Sets the content of this XML element.
	/// </summary>
	/// <param name="value">Text content to set.</param>
	/// <remarks>The <paramref name="value"/> will converted to string representation in invariant culture.</remarks>
	public void SetValue(object? value)
	{
		RemoveNodes();

		if (value is not null)
			Value = Convert.ToString(value, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Gets or sets text content of this XML element.
	/// </summary>
	/// <remarks>
	/// Setting this value, will remove any existing node from its children.
	/// </remarks>
	[NotNull]
	public string? Value
	{
		get
		{
			var values = from node in DescendantNodes()
						 where node is Text
						 select ((Text)node).Value;

			return string.Concat(values);
		}
		set
		{
			RemoveNodes();

			if (value != null)
				AddChild(new Text(value));
		}
	}

	/// <summary>
	/// Quickly determine whether a child element exists.
	/// </summary>
	/// <param name="name">XML qualified element name</param>
	/// <param name="ns">Namespace URI</param>
	/// <remarks>If <paramref name="ns"/> parameter is <see langword="null" /> it will inherit the namespace of the current element.</remarks>
	public bool HasTag(string name, string? ns = default) => Element(name, ns) != null;

	/// <summary>
	/// Quickly retrieve the text content of the child element.
	/// </summary>
	/// <param name="name">XML qualified element name</param>
	/// <param name="ns">Namespace URI</param>
	/// <returns><see langword="null" /> if element does not exists or text content is not set; otherwise the text content.</returns>
	/// <remarks>If <paramref name="ns"/> parameter is <see langword="null" /> it will inherit the namespace of the current element.</remarks>
	public string? GetTag(string name, string? ns = default)
		=> Element(name, ns)?.Value;

	/// <summary>
	/// Quickly sets or updates a child element with a text content.
	/// </summary>
	/// <param name="name">XML qualified element name</param>
	/// <param name="ns">Namespace URI</param>
	/// <param name="value">Optional value to set.</param>
	/// <remarks>
	/// <para>If an element matching with <paramref name="name"/> and <paramref name="ns"/> already exists, its content will be updated; otherwise, a new element will be created.</para>
	/// <para>If <paramref name="ns"/> parameter is <see langword="null" /> it will inherit the namespace of the current element.</para>
	/// </remarks>
	public void SetTag(string name, string? ns = default, object? value = default)
	{
		lock (_children)
		{
			var entry = Element(name, ns);

			if (entry is null)
				AddChild(entry = new Element(name, ns ?? Namespace));

			if (value is not null)
				entry.Value = Convert.ToString(value, CultureInfo.InvariantCulture);
		}
	}

	/// <summary>
	/// Quickly removes child elements.
	/// </summary>
	/// <param name="name">XML qualified element name</param>
	/// <param name="ns">Namespace URI</param>
	/// <remarks>
	/// <para>It will remove all occurrences of <paramref name="name"/> and <paramref name="ns"/>.</para>
	/// <para>If <paramref name="ns"/> parameter is <see langword="null" /> it will inherit the namespace of the current element.</para>
	/// </remarks>
	public void RemoveTag(string name, string? ns = default)
	{
		lock (_children)
		{
			foreach (var node in Elements(name, ns))
				node.Remove();
		}
	}


	/// <summary>
	/// Returns a collection of the descendant elements for this document or element, in document order.
	/// </summary>
	public IEnumerable<Element> Descendants()
	{
		List<Element>? result = null;
		BuildDescendantsList(this, ref result);
		return result ?? s_EmptyElementList;
	}


	/// <summary>
	/// Returns a collection of this element and the descendant elements for this document or element, recursively in the document order.
	/// </summary>
	public IEnumerable<Element> DescendantsAndSelf()
	{
		yield return this;

		foreach (var node in Descendants())
			yield return node;
	}

	static void BuildDescendantsList(Element source, ref List<Element>? result)
	{
		foreach (var node in source.Nodes())
		{
			if (node is Element element)
			{
				result ??= [];

				result.Add(element);

				BuildDescendantsList(element, ref result);
			}
		}
	}

	/// <summary>
	/// Write start tag.
	/// </summary>
	/// <param name="writer">XML writer instance.</param>
	public void WriteStartElement(XmlWriter writer)
	{
		if (Prefix.IsEmpty)
			writer.WriteStartElement(Name, Namespace);
		else
			writer.WriteStartElement(Prefix.ToString(), LocalName.ToString(), Namespace);

		lock (_attributes)
		{
			foreach (var attribute in _attributes)
				attribute.WriteTo(writer);
		}
	}

	/// <summary>
	/// Write childs nodes.
	/// </summary>
	/// <param name="writer">XML writer instance.</param>
	public void WriteContent(XmlWriter writer)
	{
		lock (_children)
		{
			foreach (var node in _children)
				node.WriteTo(writer);
		}
	}

	/// <summary>
	/// Recursively find the namespace declaration of given prefix.
	/// </summary>
	/// <param name="prefix">Prefix to match. For default namespace, use <see cref="string.Empty"/>.</param>
	/// <returns>Declared namespaced if found; otherwise <see langword="null" /></returns>
	public string? GetNamespaceOfPrefix(string? prefix = default)
	{
		if (GetNamespaceOfPrefixCore(prefix) is { } inAtts)
			return inAtts;

		if (Prefix.SequenceEqual(prefix) && Namespace is { } inScope)
			return inScope;

		return _parent?.GetNamespaceOfPrefix(prefix);
	}

	string? GetNamespaceOfPrefixCore(string? prefix = default)
	{
		lock (_attributes)
		{
			if (string.IsNullOrWhiteSpace(prefix))
				return _attributes.Find(x => x.IsNamespaceDeclaration && x.LocalName.SequenceEqual("xmlns"))?.Value;

			return _attributes.Find(x => x.IsNamespaceDeclaration && x.LocalName.SequenceEqual(prefix))?.Value;
		}
	}

	/// <summary>
	/// Recursively find the prefix that qualified the given namespace.
	/// </summary>
	/// <param name="ns">Namespace to match.</param>
	/// <returns>Prefix if found; otherwise <see langword="null" /></returns>
	public string? GetPrefixOfNamespace(string ns)
	{
		ArgumentNullException.ThrowIfNull(ns);

		if (GetPrefixOfNamespaceCore(ns) is { } inAtts)
			return inAtts;

		if (string.Equals(Namespace, ns))
			return Prefix.ToString();

		return _parent?.GetPrefixOfNamespace(ns);
	}

	string? GetPrefixOfNamespaceCore(string ns)
	{
		lock (_attributes)
		{
			var attr = _attributes.Find(x => x.IsNamespaceDeclaration && x.Value == ns);

			if (attr is null) return null;

			return attr.Prefix.IsEmpty ? string.Empty : attr.LocalName.ToString();
		}
	}

	/// <inheritdoc/>
	public override void WriteTo(XmlWriter writer)
	{
		lock (this)
		{
			WriteStartElement(writer);
			WriteContent(writer);
			writer.WriteEndElement();
		}
	}

	/// <summary>
	/// Gets the string representation of this XML element.
	/// </summary>
	public sealed override string ToString() => ToString(false);

	const string c_TwoSpaces = "  ";

	/// <summary>
	/// Gets the string representation of this XML element, optionally formatted.
	/// </summary>
	/// <param name="indent">Determines whether the final XML will be formatted.</param>
	public string ToString(bool indent)
	{
		var sb = new StringBuilder();

		var options = new XmlWriterSettings
		{
			Indent = indent,
			IndentChars = c_TwoSpaces,
			WriteEndDocumentOnClose = false,
			ConformanceLevel = ConformanceLevel.Fragment,
			OmitXmlDeclaration = true,
			NamespaceHandling = NamespaceHandling.OmitDuplicates,
			Encoding = Encoding.UTF8
		};

		using (var writer = XmlWriter.Create(sb, options))
			WriteTo(writer);

		return sb.ToString();
	}
}
