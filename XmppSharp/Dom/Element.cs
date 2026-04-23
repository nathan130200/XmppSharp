using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using XmppSharp.Dom.Abstractions;
using XmppSharp.Xml;

namespace XmppSharp.Dom;

/// <summary>
/// Represents an XML element in the DOM. An element can contain attributes and child nodes, which can be other elements or text nodes.
/// </summary>
public class Element : Container
{
    internal readonly Dictionary<string, string> _attributes = [];

    internal QualifiedName _elementName;

    /// <summary>
    /// Gets or sets the tag name of the element.
    /// </summary>
    public string TagName
    {
        get => _elementName.Name;
        internal set => _elementName = new(value);
    }

    /// <summary>
    /// Gets or sets the namespace URI of the element.
    /// </summary>
    public string? NamespaceUri
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the first child element of this element, or null if there are no child elements.
    /// </summary>
    public Element? FirstChild
        => Elements().FirstOrDefault();

    /// <summary>
    /// Gets the last child element of this element, or null if there are no child elements.
    /// </summary>
    public Element? LastChild
        => Elements().LastOrDefault();

    /// <summary>
    /// Initializes a new instance of the Element class with the specified tag name, optional namespace URI, and optional value.
    /// </summary>
    /// <param name="name">The tag name of the element.</param>
    /// <param name="namespaceUri">The namespace URI of the element.</param>
    /// <param name="value">The value of the element.</param>
    public Element(string name, string? namespaceUri = default, object? value = default)
    {
        _elementName = new(name);

        if (_elementName.HasPrefix)
            ArgumentException.ThrowIfNullOrWhiteSpace(namespaceUri);

        if (namespaceUri != null)
            NamespaceUri = string.Intern(namespaceUri);

        SetValue(value);
    }

    /// <summary>
    /// Gets or sets the concatenated text content of this element and all its descendant nodes.
    /// <para>
    /// Setting this property will remove all existing child nodes and replace them with a single text node containing the specified value.
    /// </para>
    /// </summary>
    [NotNull]
    public string? InnerText
    {
        get
        {
            return string.Concat(from n in DescendantNodes()
                                 where n is IXmlContent
                                 select ((IXmlContent)n).Value);
        }
        set
        {
            RemoveNodes();

            if (value != null)
            {
                if (LastNode is Text text)
                    text.Value += value;
                else
                    AddChild(new Text(value));
            }
        }
    }

    /// <summary>
    /// Adds a child node to this element.
    /// </summary>
    /// <param name="node">
    /// The node to add as a child of this element.
    /// </param>
    /// <remarks>
    /// If the node already has a parent, it will be cloned before being added to this element to avoid modifying the original node's parent-child relationships.
    /// </remarks>
    public void AddChild(Node? node)
    {
        if (node is null) return;

        if (node._parent != null)
            node = (Node)node.Clone();

        lock (_children)
        {
            node._parent = this;

            _children.Add(node);
        }
    }

    /// <summary>
    /// Removes a child node from this element.
    /// </summary>
    /// <param name="node">The node to remove from this element.</param>
    public void RemoveChild(Node? node)
    {
        if (node?._parent != this)
            return;

        lock (_children)
        {
            node._parent = null;

            _children.Remove(node);
        }

    }

    /// <summary>
    /// Removes all attributes from this element.
    /// </summary>
    public void RemoveAttributes()
    {
        lock (_attributes)
            _attributes.Clear();
    }

    /// <summary>
    /// Removes all child nodes and attributes from this element.
    /// </summary>
    public void RemoveAll()
    {
        RemoveNodes();
        RemoveAttributes();
    }

    /// <summary>
    /// Sets an attribute on this element.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="value">The value of the attribute. If the value is null, the attribute will be removed.</param>
    /// <param name="format_provider">The format provider to use when converting the value to a string.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void SetAttribute(string name, object? value, IFormatProvider? format_provider = default)
    {
        if (value is null)
        {
            RemoveAttribute(name);
            return;
        }

        lock (_attributes)
        {
            _attributes[name] = value as string
                ?? Convert.ToString(value, format_provider
                    ?? CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }

    /// <summary>
    /// Gets the value of an attribute on this element. If the attribute does not exist, the default value will be returned.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="defaultValue">The default value to return if the attribute does not exist.</param>
    /// <returns>The value of the attribute, or the default value if the attribute does not exist.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string? GetAttribute(string name, string? defaultValue = default)
    {
        lock (_attributes)
            return _attributes.GetValueOrDefault(name, defaultValue!);
    }

    /// <summary>
    /// Gets the value of an attribute on this element and attempts to parse it.
    /// </summary>
    /// <typeparam name="T">The type to parse the attribute value as.</typeparam>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="format_provider">The format provider to use when parsing the attribute value.</param>
    /// <returns>The parsed value of the attribute, or null if the attribute does not exist or cannot be parsed.</returns>
    [OverloadResolutionPriority(0)]
    public T? GetAttribute<T>(string name, IFormatProvider? format_provider = default) where T : struct, IParsable<T>
    {
        if (T.TryParse(GetAttribute(name), format_provider ?? CultureInfo.InvariantCulture, out var result))
            return result;

        return null;
    }

    /// <summary>
    /// Gets the value of an attribute on this element and attempts to parse it.
    /// </summary>
    /// <typeparam name="T">The type to parse the attribute value as.</typeparam>
    /// <param name="name">The name of the attribute.</param>
    /// <param name="default_value">The default value to return if the attribute does not exist or cannot be parsed.</param>
    /// <param name="format_provider">The format provider to use when parsing the attribute value.</param>
    /// <returns>The parsed value of the attribute, or the default value if the attribute does not exist or cannot be parsed.</returns>
    [OverloadResolutionPriority(1)]
    public T GetAttribute<T>(string name, T default_value = default!, IFormatProvider? format_provider = default) where T : IParsable<T>
    {
        if (T.TryParse(GetAttribute(name), format_provider ?? CultureInfo.InvariantCulture, out var result))
            return result;

        return default_value;
    }

    /// <summary>
    /// Determines whether this element has an attribute with the specified name.
    /// </summary>
    /// <param name="name">The name of the attribute.</param>
    /// <returns>True if the attribute exists; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool HasAttribute(string name)
    {
        lock (_attributes)
            return _attributes.ContainsKey(name);
    }

    /// <summary>
    /// Removes an attribute with the specified name from this element.
    /// </summary>
    /// <param name="name">The name of the attribute to remove.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void RemoveAttribute(string name)
    {
        lock (_attributes)
            _attributes.Remove(name);
    }

    /// <summary>
    /// Sets the text content of this element to the string representation of the specified value.
    /// </summary>
    /// <param name="value">The value to set as the text content of this element.</param>
    /// <param name="format_provider">The format provider to use when converting the value to a string.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void SetValue(object? value, IFormatProvider? format_provider = null)
        => InnerText = Convert.ToString(value, format_provider ?? CultureInfo.InvariantCulture);

    /// <summary>
    /// Determines whether this element has a child element with the specified tag name and optional namespace URI.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="namespaceUri">The namespace URI of the tag.</param>
    /// <returns>True if the element has a child with the specified tag name and namespace URI; otherwise, false.</returns>
    public bool HasTag(string name, string? namespaceUri = default) => Element(name, namespaceUri) != null;

    /// <summary>
    /// Gets the text content of the first child element with the specified tag name and optional namespace URI.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="namespaceUri">The namespace URI of the tag.</param>
    /// <param name="defaultValue">The default value to return if the tag does not exist.</param>
    /// <returns>The text content of the first child element with the specified tag name and namespace URI, or the default value if no such element exists.</returns>
    public string? GetTag(string name, string? namespaceUri = default, string? defaultValue = null)
        => Element(name, namespaceUri)?.InnerText ?? defaultValue;

    /// <summary>
    /// Sets the text content of the first child element with the specified tag name and optional namespace URI.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="namespaceUri">The namespace URI of the tag.</param>
    /// <param name="value">The value to set as the text content of the tag.</param>
    public void SetTag(string name, string? namespaceUri = default, object? value = default)
    {
        var element = Element(name, namespaceUri);

        if (element == null)
        {
            element = ElementFactory.Create(name, namespaceUri);
            AddChild(element);
        }

        element.SetValue(value);
    }

    /// <summary>
    /// Removes all child elements with the specified tag name and optional namespace URI from this element.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="namespaceUri">The namespace URI of the tag.</param>
    public void RemoveTag(string name, string? namespaceUri = default)
        => Elements(name, namespaceUri).Remove();

    /// <summary>
    /// Gets a snapshot of the attributes of this element.
    /// </summary>
    /// <returns>An enumerable collection of key-value pairs representing the attributes of this element.</returns>
    public IEnumerable<KeyValuePair<string, string>> Attributes()
    {
        lock (_attributes)
            return [.. _attributes];
    }

    /// <summary>
    /// Writes the start tag of this element, including its attributes and namespace declarations, to the specified DomWriter.
    /// </summary>
    /// <param name="writer">The DomWriter to write the start tag to.</param>
    protected void WriteStartElement(DomWriter writer)
    {
        QualifiedName? attrName;

        var attributes = Attributes();

        foreach (var (key, value) in attributes)
        {
            attrName = new(key);

            if (attrName.Name == "xmlns")
            {
                writer.AppendNamespace(string.Empty, value);
            }
            else if (attrName.HasPrefix && attrName.Prefix == "xmlns")
            {
                var colonStart = key.IndexOf(':');
                var prefix = key[(colonStart + 1)..];
                writer.AppendNamespace(prefix, value);
            }
        }

        if (!_elementName.HasPrefix)
            writer.WriteStartElement(default, _elementName.Name, NamespaceUri);
        else
            writer.WriteStartElement(_elementName.Prefix, _elementName.LocalName, NamespaceUri);

        foreach (var (key, value) in attributes)
        {
            attrName = new(key);

            if (!attrName.HasPrefix)
                writer.WriteAttribute(default, attrName.Name, value);
            else
                writer.WriteAttribute(attrName.Prefix, attrName.LocalName, value);
        }
    }

    /// <inheritdoc/>
    public override void WriteTo(DomWriter writer)
    {
        writer.BeginNamespaceScope();

        WriteStartElement(writer);

        foreach (var node in Nodes())
            node.WriteTo(writer);

        writer.WriteEndElement();

        writer.EndNamespaceScope();
    }

    /// <inheritdoc/>
    public sealed override string ToString() => ToString(false);

    /// <summary>
    /// Returns a XML string representation of this element.
    /// </summary>
    /// <param name="indent">Indicates whether to indent the XML output.</param>
    /// <returns>A string containing the XML representation of this element.</returns>
    public string ToString(bool indent)
    {
        var sb = new StringBuilder();

        using (var writer = new DomWriter(sb, indent))
            WriteTo(writer);

        return sb.ToString();
    }

    /// <inheritdoc/>
    public override Node Clone()
    {
        var result = ElementFactory.Create(TagName, NamespaceUri);

        foreach (var (key, value) in Attributes())
            result._attributes[key] = value;

        foreach (var node in Nodes())
            result.AddChild(node.Clone());

        return result;
    }
}