using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace XmppSharp.Dom;

[DebuggerDisplay("{StartTag(),nq}")]
[DebuggerTypeProxy(typeof(ElementTypeProxy))]
[DefaultMember("TagName")]
public class Element : Node
{
    #region Debugger Type Proxy

    class ElementTypeProxy
    {
        private readonly Element? _element;

        public ElementTypeProxy(Element? e)
            => _element = e;

        public string? TagName => _element?.TagName;
        public string? Namespace => _element?.Namespace;
        public string? DefaultNamespace => _element?.GetNamespace();
        public IEnumerable<Node>? Children => _element?.Nodes();
        public IReadOnlyDictionary<string, string>? Attributes => _element?.Attributes;

        public Node? FirstNode => _element?.FirstNode;
        public Node? LastNode => _element?.LastNode;

        public Element? FirstChild => _element?.FirstChild;
        public Element? LastChild => _element?.LastChild;
        public string? StartTag => _element?.StartTag();
        public string? EndTag => _element?.EndTag();
    }

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _localName = default!;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string? _prefix;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private List<Node> _children;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Dictionary<string, string> _attributes;

    ~Element()
    {
        _localName = null!;
        _prefix = null!;

        _children.Clear();
        _children = null!;

        _attributes?.Clear();
        _attributes = null!;

        _parent = null;
    }

    Element()
    {
        _children = new();
        _attributes = new();
    }

    public Element(string tagName, string? namespaceURI = default, object? value = default)
    {
        _children = new();
        _attributes = new();

        TagName = tagName;

        if (namespaceURI != null)
        {
            if (Prefix != null)
                SetNamespace(Prefix, namespaceURI);
            else
                SetNamespace(namespaceURI);
        }

        SetValue(value);
    }

    public Element(Element other)
    {
        if (GetType() != other.GetType())
            throw new InvalidOperationException("To perform a copy constructor, both objects must be of the same type.");

        _children = new();
        _attributes = new();
        _localName = other._localName;
        _prefix = other._prefix;

        foreach (var (key, value) in other.Attributes)
            _attributes[key] = value;

        lock (other._children)
        {
            foreach (var node in other.Nodes())
                _children.Add(node.Clone());
        }

    }

    public bool IsRootElement
        => _parent == null;

    public string? Value
    {
        get
        {
            var nodes = Nodes()
                .OfType<Text>()
                .Select(x => x.Value);

            if (!nodes.Any())
                return null;

            return string.Concat(nodes);
        }
        set
        {
            RemoveAllChildNodes();
            AddChild(new Text(value));
        }
    }

    public void SetValue(object? content, IFormatProvider? format = default)
    {
        format ??= CultureInfo.InvariantCulture;

        if (content != null)
            Value = Convert.ToString(content, format);
    }

    public string? Prefix
    {
        get => _prefix;
        set => _prefix = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public string LocalName
    {
        get => _localName;
        set
        {
            ThrowHelper.ThrowIfNullOrWhiteSpace(value, nameof(LocalName));
            _localName = value;
        }
    }

    public string TagName
    {
        get => _prefix == null ? _localName : string.Concat(_prefix, ':', _localName);
        set
        {
            ThrowHelper.ThrowIfNullOrWhiteSpace(value, nameof(TagName));
            var ofs = value.IndexOf(':');

            if (ofs > 0)
                Prefix = value[0..ofs];

            LocalName = value[(ofs + 1)..];
        }
    }

    public void InsertBefore(Node newNode, Node referenceNode)
    {
        ThrowHelper.ThrowIfNull(newNode);
        ThrowHelper.ThrowIfNull(referenceNode);

        if (newNode._parent is not null)
            newNode = newNode.Clone();

        lock (_children)
        {
            var ofs = _children.IndexOf(referenceNode);

            if (ofs > 0)
                ofs--;

            _children.Insert(ofs, newNode);
            newNode._parent = this;
        }
    }

    public void InsertAfter(Node newNode, Node referenceNode)
    {
        ThrowHelper.ThrowIfNull(newNode);
        ThrowHelper.ThrowIfNull(referenceNode);

        if (newNode._parent is not null)
            newNode = newNode.Clone();

        lock (_children)
        {
            var ofs = _children.IndexOf(referenceNode);

            if (ofs == _children.Count - 1)
                _children.Add(newNode);
            else
                _children.Insert(ofs + 1, newNode);

            newNode._parent = this;
        }
    }

    public void ReplaceWith(Node newNode)
    {
        ThrowHelper.ThrowIfNull(newNode);

        if (_parent is null) return;

        if (newNode._parent is not null)
            newNode = newNode.Clone();

        lock (_parent._children)
        {
            var ofs = _parent._children.IndexOf(this);
            _parent._children[ofs] = newNode;
            newNode._parent = _parent;
            _parent = null;
        }
    }

    public void AddChild(Node? node)
    {
        if (node == null)
            return;

        if (node._parent != null)
            node = node.Clone();

        lock (_children)
            _children.Add(node);

        node._parent = this;
    }

    public void RemoveChild(Node? node)
    {
        if (node == null)
            return;

        if (node._parent != this)
            return;

        if (node is Element element)
            element.SetNamespace(GetNamespace(element.Prefix));

        lock (_children)
        {
            _children.Remove(node);
            node._parent = null;
        }
    }

    public Element SetAttribute(string name, object? value, string? format = default, IFormatProvider? formatter = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);

        if (value is null)
            RemoveAttribute(name);
        else
        {
            formatter ??= CultureInfo.InvariantCulture;

            lock (_attributes)
            {
                string? rawValue;
                if (value is IFormattable fmt) rawValue = fmt.ToString(format, formatter);
                else if (value is IConvertible conv) rawValue = conv.ToString(formatter);
                else rawValue = Convert.ToString(value, formatter);
                _attributes[name] = rawValue ?? string.Empty;
            }
        }

        return this;
    }

    public override Node Clone()
    {
        var result = ElementFactory.CreateElement(TagName, Namespace);

        foreach (var (key, value) in Attributes)
            result._attributes[key] = value;

        foreach (var node in Nodes())
            result.AddChild(node.Clone());

        result.Value = Value;

        return result;
    }

    public bool HasAttribute(string name)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);

        lock (_attributes)
            return _attributes.ContainsKey(name);
    }

    public string? GetAttribute(string name)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);

        lock (_attributes)
            return _attributes.GetValueOrDefault(name);
    }

    public void RemoveAttribute(string name)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);

        lock (_attributes)
            _attributes.Remove(name);
    }

    public string? GetNamespace(string? prefix = default)
    {
        var result = GetAttribute(string.IsNullOrWhiteSpace(prefix) ? "xmlns" : $"xmlns:{prefix}");

        if (result != null)
            return result;

        return _parent?.GetNamespace(prefix);
    }

    public void SetNamespace(string? namespaceURI)
    {
        ThrowHelper.ThrowIfNull(namespaceURI);
        SetAttribute("xmlns", namespaceURI);
    }

    public void SetNamespace(string prefix, string? namespaceURI)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(prefix);
        ThrowHelper.ThrowIfNull(namespaceURI);
        SetAttribute($"xmlns:{prefix}", namespaceURI);
    }

    public Node? FirstNode => Nodes().FirstOrDefault();
    public Node? LastNode => Nodes().LastOrDefault();
    public Element? FirstChild => Children().FirstOrDefault();
    public Element? LastChild => Children().LastOrDefault();

    public string? DefaultNamespace
    {
        get => GetNamespace();
        set => SetNamespace(value);
    }

    public string? Namespace
    {
        get => GetNamespace(Prefix);
        set
        {
            if (Prefix == null)
                SetNamespace(value);
            else
                SetNamespace(Prefix, value);
        }
    }

    public IEnumerable<Node> Nodes()
    {
        Node[] result;

        lock (_children)
            result = _children.ToArray();

        return result.ToList();
    }

    public IReadOnlyDictionary<string, string> Attributes
    {
        get
        {
            KeyValuePair<string, string>[] result;

            lock (_attributes)
                result = _attributes.ToArray();

            return result.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public void WriteTo(TextWriter textWriter, XmlFormatting formatting = XmlFormatting.Default)
    {
        using var writer = Xml.CreateWriter(textWriter, formatting);
        Xml.WriteTree(this, writer);
    }

    public void WriteTo(XmlWriter writer)
        => Xml.WriteTree(this, writer);

    public void Save(Stream stream, XmlFormatting formatting = XmlFormatting.Default)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8, -1, true);
        WriteTo(writer, formatting);
    }

    public string StartTag()
    {
        var sb = new StringBuilder($"<{Xml.EncodeName(TagName)}");

        foreach (var (key, value) in Attributes)
            sb.AppendFormat(" {0}=\"{1}\"", Xml.EncodeName(key), Xml.EscapeAttribute(value));

        return sb.Append('>').ToString();
    }

    public string EndTag()
        => $"</{Xml.EncodeName(TagName)}>";

    public override string ToString() => ToString(false);

    public string ToString(bool indented)
    {
        var sb = new StringBuilder();

        var formatting = XmlFormatting.Default;

        if (indented)
            formatting |= XmlFormatting.Indented;

        using (var sw = new StringWriter(sb))
            WriteTo(sw, formatting);

        return sb.ToString();
    }

    public IEnumerable<Element> Children()
        => Nodes().OfType<Element>();

    public IEnumerable<Element> Children(string tagName, string? namespaceURI = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);
        return ChildrenInternal(tagName, namespaceURI, false);
    }

    IEnumerable<Element> ChildrenInternal(string tagName, string? ns, bool once)
    {
        lock (_children)
        {
            foreach (var element in Children())
            {
                if (element.TagName == tagName && (ns == null || ns == element.Namespace))
                {
                    yield return element;

                    if (once)
                        yield break;
                }
            }
        }
    }

    public Element? Child(string tagName, string? namespaceURI = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);
        return ChildrenInternal(tagName, namespaceURI, true).FirstOrDefault();
    }

    public bool HasTag(string tagName, string? namespaceURI = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);
        return ChildrenInternal(tagName, namespaceURI, true).Any();
    }

    public void SetTag(Action<Element> action)
    {
        ThrowHelper.ThrowIfNull(action);

        var element = new Element();
        action(element);

        ThrowHelper.ThrowIfNullOrWhiteSpace(element.TagName);

        AddChild(element);
    }

    public void RemoveTag(string tagName, string? namespaceURI = default)
        => Child(tagName, namespaceURI)?.Remove();

    public void RemoveTags(string tagName, string? namespaceURI = default)
        => Children(tagName, namespaceURI).Remove();

    public string? GetTag(string tagName, string? namespaceURI = default)
        => Child(tagName, namespaceURI)?.Value;

    public IEnumerable<string?> GetTags(string tagName, string? namespaceURI = default)
        => Children(tagName, namespaceURI).Select(x => x.Value);

    public T? Child<T>() where T : Element
        => Children().OfType<T>().FirstOrDefault();

    public IEnumerable<T> Children<T>() where T : Element
        => Children().OfType<T>();

    public void ClearAttributes()
    {
        lock (_attributes)
        {
            var allKeys = _attributes.Keys.Where(x => !x.Contains("xmlns"));

            foreach (var key in allKeys)
                _attributes.Remove(key);
        }
    }

    public void RemoveAllChildNodes()
    {
        Node[] items;

        lock (_children)
        {
            items = _children.ToArray();
            _children.Clear();
        }

        foreach (var item in items)
            item._parent = null;
    }
}
