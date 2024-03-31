using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web;
using System.Xml;

using XmppSharp.Factory;

namespace XmppSharp.Dom;

/// <summary>
/// Represents the class that is used to manipulate XML elements.
/// </summary>
[DebuggerDisplay("{DebugString,nq}")]
public class Element : ICloneable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal string DebugString
        => $"Local Name: \"{LocalName}\", Namespace: \"{GetNamespace(Prefix)}\"";

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly List<Element> _children;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly Dictionary<string, string> _attributes;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal Element _parent;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal string _localName, _prefix, _value;

    internal Element()
    {
        _children = new();
        _attributes = new();
    }

    /// <summary>
    /// Makes a deep copy of the current element.
    /// </summary>
    /// <returns>The complete copy of the element and its respective attributes and child elements.</returns>
    /// <remarks>
    /// This copy is made directly with the element's typing, that is, <see cref="ElementFactory.Create" /> 
    /// will be used to create a copy of the current element, making it possible to cast directly to the desired type.
    /// </remarks>
    public Element Clone()
    {
        var ns = GetNamespace(_prefix);

        // sometimes stanza elements can omit xmlns="..."
        if (string.IsNullOrEmpty(ns) && _localName is "iq" or "message" or "presence")
            ns = Namespaces.Client;

        var result = ElementFactory.Create(_localName, _prefix, ns);

        lock (_attributes)
        {
            foreach (var (key, value) in _attributes)
                result.SetAttribute(key, value);
        }

        lock (_children)
        {
            foreach (var child in _children)
                result.AddChild(child.Clone());
        }

        return result;
    }

    object ICloneable.Clone()
        => Clone();

    /// <summary>
    /// Gets the first child element of this element.
    /// </summary>
    public Element? FirstChild
    {
        get
        {
            lock (_children)
                return _children.Count > 0 ? _children[0] : default;
        }
    }

    /// <summary>
    /// Gets the last child element of this element.
    /// </summary>
    public Element? LastChild
    {
        get
        {
            lock (_children)
                return _children.Count > 0 ? _children[^1] : default;
        }
    }

    protected Element(Element other) : this()
    {
        if (other.GetType() != GetType())
            throw new InvalidOperationException();

        _localName = other._localName;
        _prefix = other._prefix;

        foreach (var (name, value) in other.Attributes)
            _attributes[name] = value;

        foreach (var element in other.Children())
        {
            var child = (Element)Activator.CreateInstance(element.GetType(), element);
            _children.Add(child);
            child._parent = this;
        }

        _value = other._value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Element" />.
    /// </summary>
    /// <param name="name">Qualified name of the element (<c>localName</c> or <c>prefix:localName</c>)</param>
    /// <param name="xmlns">
    /// XML namespace of the element.
    /// <para>Note that if the qualified name has a prefix, the namespace will be assigned to the prefix.</para>
    /// </param>
    /// <param name="text">
    /// Content (text) of the element.
    /// </param>
    public Element(string name, string? xmlns = default, string? text = default) : this()
    {
        var result = Xml.ExtractQualifiedName(name);

        if (xmlns != null)
        {
            if (result.HasPrefix)
                SetNamespace(result.Prefix, xmlns);
            else
                SetNamespace(xmlns);
        }

        _localName = result.LocalName;
        _prefix = result.Prefix;

        if (text != null)
            Content = text;
    }


    /// <summary>
    /// Gets or sets the local part of the element name.
    /// </summary>
    public string LocalName
    {
        get => _localName;
        set
        {
            Require.NotNullOrEmpty(value);
            _localName = value;
        }
    }

    /// <summary>
    /// Gets or sets the prefix part of the element name.
    /// </summary>
    public string? Prefix
    {
        get => _prefix;
        set
        {
            _prefix = string.IsNullOrWhiteSpace(value)
                ? null : value;
        }
    }

    /// <summary>
    /// Gets or sets the content of the element.
    /// </summary>
    public string? Content
    {
        get => _value;
        set => _value = value;
    }

    /// <summary>
    /// Gets or sets the parent element that owns the current element.
    /// </summary>
    public Element Parent
    {
        get => _parent;
        set
        {
            _parent?.RemoveChild(this);
            (_parent = value)?.AddChild(this);
        }
    }

    /// <summary>
    /// Gets the qualified name of the element.
    /// </summary>
    public string TagName
    {
        get
        {
            if (_prefix == null)
                return _localName;

            return string.Concat(_prefix, ':', _localName);
        }
    }

    /// <summary>
    /// Gets or sets the element's default namespace (attribute declared with <c>xmlns</c>)
    /// </summary>
    public string? Namespace
    {
        get => GetNamespace();
        set => SetNamespace(value);
    }

    /// <summary>
    /// Determines whether the current element is the root of the XML tree.
    /// </summary>
    public bool IsRootElement
        => _parent == null;

    /// <summary>
    /// Determines whether the current element is an empty element, that is, it has no content and no child elements.
    /// </summary>
    public bool IsEmptyElement
    {
        get
        {
            lock (_children)
            {
                if (string.IsNullOrEmpty(_value) && _children.Count == 0)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Gets the XML representation of the current element.
    /// </summary>
    /// <param name="indented">Controls whether it will be formatted (recommended for debug) or not (recommended for transport)</param>
    /// <returns>String containing the XML.</returns>
    public string ToString(bool indented)
    {
        var (output, writer) = Xml.CreateXmlWriter(indented);

        using (writer)
            Xml.ToStringXml(this, writer);

        return output.ToString();
    }

    /// <summary>
    /// Removes this element from its parent element to which it belongs.
    /// </summary>
    public void Remove()
    {
        _parent?.RemoveChild(this);
        _parent = null;
    }

    /// <summary>
    /// Adds a child element.
    /// </summary>
    /// <param name="e">Element that will be added.</param>
    /// <remarks>
    /// <para>If the child element already belongs to this parent element, it will not be added.</para>
    /// <para>If the element to be added already has a parent, it will be removed from its parent before being added here.</para>
    /// </remarks>
    public void AddChild(Element e)
    {
        Require.NotNull(e);

        if (e._parent == this)
            return;

        e.Remove();

        lock (_children)
            _children.Add(e);

        e._parent = this;
    }

    /// <summary>
    /// Removes the child element.
    /// </summary>
    /// <param name="e">Element that will be removed.</param>
    /// <remarks>
    /// </remarks>
    public void RemoveChild(Element e)
    {
        Require.NotNull(e);

        if (e._parent != this)
            return;

        lock (_children)
            _children.Remove(e);

        e._parent = null;

        var ns = e._prefix != null
            ? GetNamespace(e._prefix)
            : GetNamespace();

        if (e._prefix != null)
            e.SetNamespace(e._prefix, ns);
        else
            e.SetNamespace(ns);
    }

    /// <summary>
    /// Gets an XML attribute.
    /// </summary>
    /// <param name="name">Qualified attribute name.</param>
    /// <returns>Attribute value or <see langword="null" /> if the attribute does not exist.</returns>
    public string? GetAttribute(string name)
    {
        Require.NotNullOrEmpty(name);

        string result = default;

        lock (_attributes)
            _attributes.TryGetValue(name, out result);

        return result;
    }

    /// <summary>
    /// Defines an XML attribute.
    /// </summary>
    /// <param name="name">Qualified attribute name.</param>
    /// <param name="value">
    /// Attribute value.
    /// <para>
    /// If specified as <see langword="null" /> the attribute will be removed from the element.
    /// </para>
    /// </param>
    public void SetAttribute(string name, string? value)
    {
        Require.NotNullOrEmpty(name);

        lock (_attributes)
        {
            if (value == null)
                _attributes.Remove(name);
            else
                _attributes[name] = value;
        }
    }

    /// <summary>
    /// Determines whether the specified XML attribute exists on the element.
    /// </summary>
    /// <param name="name">Qualified attribute name.</param>
    /// <returns><see langword="true" /> if the attribute exists, otherwise <see langword="false" /></returns>
    public bool HasAttribute(string name)
    {
        Require.NotNullOrEmpty(name);

        lock (_attributes)
            return _attributes.ContainsKey(name);
    }

    /// <summary>
    /// Removes the XML attribute from the element.
    /// </summary>
    /// <param name="name">Qualified attribute name.</param>
    /// <returns><see langword="true" /> if the attribute existed and was removed, otherwise <see langword="false" /></returns>
    public bool RemoveAttribute(string name)
    {
        Require.NotNullOrEmpty(name);

        lock (_attributes)
            return _attributes.Remove(name);
    }

    /// <summary>
    /// Declares the element's default namespace. (<c>xmlns</c>)
    /// </summary>
    /// <param name="uri">URI namespace value.</param>
    public void SetNamespace(string uri)
        => SetAttribute("xmlns", uri);

    /// <summary>
    /// Declares namespace with prefix in the current element.
    /// </summary>
    /// <param name="prefix">Namespace prefix.</param>
    /// <param name="uri">URI namespace value.</param>
    public void SetNamespace(string prefix, string uri)
    {
        Require.NotNullOrEmpty(prefix);
        SetAttribute($"xmlns:{prefix}", uri);
    }

    /// <summary>
    /// Determines whether or not the current element or parent element has a declared namespace, optionally with a prefix.
    /// </summary>
    /// <param name="prefix">Optional namespace prefix.</param>
    /// <returns><see langword="true" /> if the namespace attribute exists, otherwise <see langword="false" /></returns>
    /// <remarks>
    /// Basically it would be the same as calling the function <see cref="HasAttribute(string)" />.
    /// </remarks>
    public bool HasNamespace(string prefix = default)
    {
        if (prefix != null)
            return HasAttribute($"xmlns:{prefix}");

        return HasAttribute("xmlns");
    }

    /// <summary>
    /// Gets the XML attributes declared in the current element.
    /// </summary>
    public IReadOnlyDictionary<string, string> Attributes
    {
        get
        {
            lock (_attributes)
                return _attributes.ToArray().ToDictionary(x => x.Key, x => x.Value);
        }
    }

    // --------------------------------------------------------------------------------------- //

    /// <summary>
    /// Gets all the child elements.
    /// </summary>
    public IEnumerable<Element> Children()
    {
        lock (_children)
            return _children.ToArray();
    }

    /// <summary>
    /// Gets all the child elements typed in <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type that inherits the class <see cref="Element" />.</typeparam>
    public IEnumerable<T> Children<T>() where T : Element
        => Children().OfType<T>();

    /// <summary>
    /// Gets all the child elements based on a search filter.
    /// </summary>
    /// <param name="predicate">Predicate that will be used to filter the elements.</param>
    public IEnumerable<Element> Children(Func<Element, bool> predicate)
        => Children().Where(predicate);

    // --------------------------------------------------------------------------------------- //

    /// <summary>
    /// Gets the first occurrence of the <typeparamref name="T" /> child element.
    /// </summary>
    /// <typeparam name="T">Type that inherits the class <see cref="Element" />.</typeparam>
    public T? Child<T>() where T : Element
        => Children().OfType<T>().FirstOrDefault();

    /// <summary>
    /// Gets the first occurrence of child element based on a search filter.
    /// </summary>
    /// <param name="predicate">Predicate that will be used to filter the elements.</param>
    public Element GetChild(Func<Element, bool> predicate)
        => Children().FirstOrDefault(predicate);

    /// <summary>
    /// Replaces the first occurrence of the specified element.
    /// </summary>
    /// <typeparam name="T">Type that inherits the class <see cref="Element" />.</typeparam>
    /// <param name="element">The new instance of the element that will be added after replacing</param>
    public void ReplaceChild<T>(T element) where T : Element
    {
        foreach (var entry in ElementFactory.GetTags<T>())
            GetChild(entry.HasName)?.Remove();

        if (element != null)
            AddChild(element);
    }

    /// <summary>
    /// Replaces the first occurrence of the specified element.
    /// </summary>
    /// <param name="element">The new instance of the element that will be added after replacing</param>
    public void ReplaceChild(Element element)
    {
        Require.NotNull(element);

        GetChild(element.TagName, element.GetNamespace(element.Prefix))?.Remove();

        if (element != null)
            AddChild(element);
    }

    /// <summary>
    /// Gets the descendent elements as a linear list.
    /// </summary>
    public IReadOnlyList<Element> Descendants()
    {
        var result = new List<Element>();
        GetDescendants(result);
        return result;
    }

    /// <summary>
    /// Gets the descendent elements as a linear list (including this element).
    /// </summary>
    public IReadOnlyList<Element> DescendantsAndSelf()
    {
        var result = new List<Element> { this };
        GetDescendants(result);
        return result;
    }

    void GetDescendants(List<Element> result)
    {
        foreach (var child in Children())
        {
            result.Add(child);
            child.GetDescendants(result);
        }
    }

    /// <summary>
    /// Gets the namespace declared in the element, optionally with prefix.
    /// </summary>
    /// <param name="prefix">Namespace prefix.</param>
    public string GetNamespace(string? prefix = default)
    {
        var attrName = string.IsNullOrWhiteSpace(prefix)
            ? "xmlns" : string.Concat("xmlns:", prefix);

        var value = GetAttribute(attrName);

        if (value != null)
            return value;

        return _parent?.GetNamespace(prefix);
    }

    // ------------------------------------------------------------------------------------

    internal static bool SelectElementImpl(Element e, string name, string xmlns)
    {
        return e.TagName == name
            && (xmlns == null || e.GetNamespace(e.Prefix) == xmlns);
    }

    /// <summary>
    /// Gets the first occurrence of the specified child element.
    /// </summary>
    /// <param name="name">Element qualified name.</param>
    /// <param name="xmlns">Optional element namespace.</param>
    public Element? GetChild(string name, string xmlns = default)
        => GetChild(x => SelectElementImpl(x, name, xmlns));

    /// <summary>
    /// Try to get the first occurrence of the child element <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type of element that inherits the class <see cref="Element" />.</typeparam>
    /// <param name="result">Output variable that will receive the element.</param>
    /// <returns><see langword="true" /> if the element exists and was valid.</returns>
    public bool TryGetChild<T>([NotNullWhen(true)] out T result) where T : Element, new()
    {
        result = Child<T>();
        return result != null;
    }

    /// <summary>
    /// Try to gets the first occurrence of the specified child element.
    /// </summary>
    /// <param name="name">Element qualified name.</param>
    /// <param name="xmlns">Optional element namespace.</param>
    /// <param name="result">Output variable that will receive the element.</param>
    /// <returns><see langword="true" /> if the element exists and was valid.</returns>
    public bool TryGetChild(string name, string xmlns, out Element result)
    {
        result = GetChild(name, xmlns);
        return result != null;
    }

    /// <summary>
    /// Try to gets the first occurrence of the specified child element.
    /// </summary>
    /// <param name="name">Element qualified name.</param>
    /// <param name="result">Output variable that will receive the element.</param>
    /// <returns><see langword="true" /> if the element exists and was valid.</returns>
    public bool TryGetChild(string name, out Element result)
    {
        result = GetChild(name);
        return result != null;
    }

    /// <summary>
    /// Gets the first occurrence of child element based on a search filter.
    /// </summary>
    /// <param name="predicate">Predicate that will be used to filter the elements.</param>
    /// <param name="result">Output variable that will receive the element.</param>
    /// <returns><see langword="true" /> if the element exists and was valid.</returns>
    public bool TryGetChild(Func<Element, bool> predicate, out Element result)
    {
        result = GetChild(predicate);
        return result != null;
    }

    // ------------------------------------------------------------------------------------

    /// <summary>
    /// Determines whether the specified element is contained in children.
    /// </summary>
    /// <param name="name">Qualified element name.</param>
    /// <param name="xmlns">Optional namespace.</param>
    public bool HasTag(string name, string? xmlns = default)
        => GetChild(x => SelectElementImpl(x, name, xmlns)) != null;

    /// <summary>
    /// Directly gets the content of XML tag.
    /// </summary>
    /// <param name="name">Qualified element name.</param>
    /// <param name="xmlns">Optional namespace.</param>
    /// <returns>Element content, or <see langword="null" /> if the element does not exist or if the element has no content.</returns>
    public string GetTag(string name, string? xmlns = default)
        => GetChild(x => SelectElementImpl(x, name, xmlns))?.Content;

    /// <summary>
    /// Directly sets an XML tag with no content, an empty tag.
    /// </summary>
    /// <param name="name">Qualified element name.</param>
    public void SetTag(string name)
        => SetTag(name, null, null);

    /// <summary>
    /// Removes an XML tag from its children.
    /// </summary>
    /// <param name="name">Qualified element name.</param>
    /// <param name="xmlns">Element namespace.</param>
    public void RemoveTag(string name, string xmlns = default)
        => GetChild(x => SelectElementImpl(x, name, xmlns))?.Remove();

    /// <summary>
    /// Directly sets an XML tag with an content.
    /// </summary>
    /// <param name="name">Qualified element name.</param>
    /// <param name="value">Element content.</param>
    public void SetTag(string name, string? value = default)
        => SetTag(name, null, value);

    /// <summary>
    /// Directly sets an XML tag with an content and namespace.
    /// </summary>
    /// <param name="name">Qualified element name.</param>
    /// <param name="xmlns">Element namespace.</param>
    /// <param name="value">Element content.</param>
    public void SetTag(string name, string? xmlns = default, string? value = default)
    {
        var element = GetChild(x => SelectElementImpl(x, name, xmlns));

        if (element != null)
            element.Content = value;
        else
            AddChild(new Element(name, xmlns, value));
    }

    /// <summary>
    /// Gets a string representation of the XML tree without formatting.
    /// </summary>
    public override string ToString()
        => ToString(false);

    /// <summary>
    /// Gets a string representation of the element's opening tag.
    /// </summary>
    public string StartTag()
    {
        var sb = new StringBuilder($"<{XmlConvert.EncodeName(TagName)}");

        foreach (var (name, value) in Attributes)
            sb.AppendFormat(" {0}=\"{1}\"", XmlConvert.EncodeName(name), HttpUtility.HtmlAttributeEncode(value));

        return sb.Append('>').ToString();
    }

    /// <summary>
    /// Gets a string representation of the internal XML content.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string InnerXml
    {
        get
        {
            var (result, writer) = Xml.CreateXmlWriter(false);

            using (writer)
            {
                foreach (var child in Children())
                    Xml.ToStringXml(child, writer);
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// Gets a string representation of the external XML content. Same thing as <see cref="ToString()" />.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string OuterXml
        => ToString(false);

    /// <summary>
    /// Gets the content of the element transformed from base64 into bytes.
    /// </summary>
    public byte[] GetContentFromBase64()
    {
        if (_value == null)
            return Array.Empty<byte>();
        else
            return Convert.FromBase64String(_value);
    }

    /// <summary>
    /// Sets the content of the element transformed to base64 from bytes.
    /// </summary>
    /// <param name="buffer">Bytes that will be transformed into base64.</param>
    public void SetContentAsBase64(byte[]? buffer)
    {
        if (buffer == null)
            _value = null;
        else
            _value = Convert.ToBase64String(buffer);
    }

    /// <summary>
    /// Gets the content of the element transformed from base64 into raw string.
    /// </summary>
    public string GetContentFromBase64String(Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(GetContentFromBase64());

    /// <summary>
    /// Sets the content of the element transformed to base64 from raw string.
    /// </summary>
    /// <param name="value">Raw string that will be transformed into base64.</param>
    public void SetContentAsBase64String(string value, Encoding? encoding = default)
        => SetContentAsBase64((encoding ?? Encoding.UTF8).GetBytes(value));
}
