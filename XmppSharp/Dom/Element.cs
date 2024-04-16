using System.Globalization;
using System.Text;
using System.Xml;
using XmppSharp.Factory;

namespace XmppSharp.Dom;

public class Element : Node
{
    private string _localName, _prefix;
    private readonly Dictionary<string, string> _attributes = new();
    private readonly List<Node> _childNodes = new();

    Element()
    {

    }

    public Element(string qualifiedName)
    {
        var info = Xml.ExtractQualifiedName(qualifiedName);
        _localName = info.LocalName;
        _prefix = info.HasPrefix ? info.Prefix : null;
    }

    public Element(string qualifiedName, string namespaceURI) : this(qualifiedName)
    {
        if (namespaceURI != null)
        {
            if (_prefix != null)
                SetNamespace(_prefix, namespaceURI);
            else
                SetNamespace(namespaceURI);
        }
    }

    public IEnumerable<Node> Nodes()
    {
        lock (_childNodes)
            return _childNodes.ToArray();
    }

    static void GetDescendantNodes(Element parent, List<Node> result)
    {
        foreach (var node in parent.Nodes())
        {
            result.Add(node);

            if (node is Element e)
                GetDescendantNodes(e, result);
        }
    }

    static void GetDescendantElements(Element parent, List<Element> result)
    {
        foreach (var element in parent.Children())
        {
            result.Add(element);

            GetDescendantElements(element, result);
        }
    }

    public IEnumerable<Node> DescendantNodes()
    {
        var result = new List<Node>();
        GetDescendantNodes(this, result);
        return result;
    }

    public IEnumerable<Node> DescendantNodesAndSelf()
    {
        var result = new List<Node> { this };
        GetDescendantNodes(this, result);
        return result;
    }

    public IEnumerable<Element> Descendants()
    {
        var result = new List<Element>();
        GetDescendantElements(this, result);
        return result;
    }

    public IEnumerable<Element> DescendantsAndSelf()
    {
        var result = new List<Element> { this };
        GetDescendantElements(this, result);
        return result;
    }

    public override string Value
    {
        get => string.Concat(DescendantNodes().OfType<Text>().Select(x => x.Value));
        set
        {
            Nodes().Remove();

            if (value != null)
                AddChild(new Text(value));
        }
    }

    public override string ToString()
        => ToString(false);

    public string ToString(bool indented, char indentChar = ' ', int indentSize = 2)
    {
        using (StringBuilderPool.Rent(out var sb))
        {
            using (var writer = Xml.CreateWriter(indented, sb, indentChar, indentSize))
                WriteTo(writer);

            return sb.ToString();
        }
    }

    public string? DefaultNamespace
    {
        get => GetNamespace();
        set => SetNamespace(value);
    }

    public override Element Clone()
    {
        var elem = ElementFactory.Create(TagName, GetNamespace(Prefix));
        elem._localName = _localName;
        elem._prefix = _prefix;

        lock (_attributes)
        {
            foreach (var (key, value) in _attributes)
                elem._attributes[key] = value;
        }

        lock (_childNodes)
        {
            foreach (var node in _childNodes)
            {
                var childNode = node.Clone();
                elem._childNodes.Add(childNode);
                childNode._parent = elem;
            }
        }

        return elem;
    }

    public override void WriteTo(XmlWriter writer)
    {
        var ns = GetNamespace(_prefix);

        string skipAttribute = "xmlns";

        if (_prefix == null)
            writer.WriteStartElement(_localName, ns);
        else
        {
            writer.WriteStartElement(_prefix, _localName, ns);
            skipAttribute = $"xmlns:{_prefix}";
        }

        lock (_attributes)
        {
            foreach (var (name, value) in _attributes)
            {
                if (skipAttribute == name)
                    continue;

                var info = Xml.ExtractQualifiedName(name);

                if (!info.HasPrefix)
                    writer.WriteAttributeString(name, value);
                else
                    writer.WriteAttributeString(info.Prefix, info.LocalName, info.Prefix switch
                    {
                        "xml" => Namespace.Xml,
                        "xmlns" => Namespace.Xmlns,
                        _ => GetNamespace(info.Prefix) ?? string.Empty
                    });
            }
        }

        lock (_childNodes)
        {
            foreach (var node in _childNodes)
                node.WriteTo(writer);
        }

        writer.WriteEndElement();
    }

    public string LocalName
    {
        get => _localName;
        set
        {
            Require.NotNullOrWhiteSpace(value, nameof(LocalName));
            _localName = value;
        }
    }

    public string Prefix
    {
        get => _prefix;
        set => _prefix = string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public string TagName
        => _prefix == null ? _localName : string.Concat(_prefix, ':', _localName);

    public Node FirstNode
    {
        get
        {
            lock (_childNodes)
                return _childNodes.Count > 0 ? _childNodes[0] : null;
        }
    }

    public Node LastNode
    {
        get
        {
            lock (_childNodes)
                return _childNodes.Count > 0 ? _childNodes[^1] : null;
        }
    }

    public Element FirstChild
    {
        get
        {
            lock (_childNodes)
            {
                for (int i = 0; i < _childNodes.Count; i++)
                {
                    if (_childNodes[i] is Element e)
                        return e;
                }
            }

            return null;
        }
    }

    public Element LastChild
    {
        get
        {
            lock (_childNodes)
            {
                for (int i = _childNodes.Count - 1; i >= 0; i--)
                {
                    if (_childNodes[i] is Element e)
                        return e;
                }
            }

            return null;
        }
    }

    public virtual void AddChild(Node n)
    {
        if (n == null)
            return;

        if (n.Parent != null)
            n = n.Clone();

        lock (_childNodes)
            _childNodes.Add(n);

        n._parent = this;
    }

    public virtual void RemoveChild(Node n)
    {
        if (n._parent != this)
            return;

        lock (_childNodes)
            _childNodes.Remove(n);

        n._parent = null;

        if (n is Element elem)
        {
            var prefix = elem.Prefix;

            if (prefix != null)
                elem.SetNamespace(prefix, GetNamespace(prefix));
            else
                elem.SetNamespace(GetNamespace());
        }
    }

    public string GetNamespace(string? prefix = default)
    {
        string result;

        if (string.IsNullOrWhiteSpace(prefix))
            result = GetAttribute("xmlns");
        else
            result = GetAttribute($"xmlns:{prefix}");

        if (result != null)
            return result;

        return _parent?.GetNamespace(prefix);
    }

    public void SetNamespace(string uri)
        => SetAttribute("xmlns", uri);

    public void SetNamespace(string prefix, string uri)
    {
        Require.NotNullOrWhiteSpace(prefix);
        SetAttribute($"xmlns:{prefix}", uri);
    }

    public string GetAttribute(string name)
    {
        Require.NotNullOrWhiteSpace(name);

        string value;

        lock (_attributes)
            _attributes.TryGetValue(name, out value);

        return value;
    }

    public void SetAttribute(string name, object? value)
    {
        Require.NotNullOrWhiteSpace(name);

        lock (_attributes)
        {
            if (value == null)
                _attributes.Remove(name);
            else
                _attributes[name] = Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

    public void RemoveAttribute(string name)
    {
        Require.NotNullOrWhiteSpace(name);

        lock (_attributes)
            _attributes.Remove(name);
    }

    public bool HasAttribute(string name)
    {
        Require.NotNullOrWhiteSpace(name);

        lock (_attributes)
            return _attributes.ContainsKey(name);
    }

    public IEnumerable<Element> Children()
    {
        lock (_childNodes)
        {
            foreach (var node in _childNodes)
            {
                if (node is Element e)
                    yield return e;
            }
        }
    }

    public IEnumerable<T> Children<T>()
        => Children().OfType<T>();

    public IEnumerable<Element> Children(string? tagName, string namespaceURI)
    {
        Require.NotNull(tagName);

        return Children(x => x.TagName == tagName && x.Prefix == null
                ? x.GetNamespace() == namespaceURI
                : x.GetNamespace(x.Prefix) == namespaceURI);
    }

    public IEnumerable<Element> Children(Func<Element, bool> predicate)
    {
        Require.NotNull(predicate);

        lock (_childNodes)
        {
            foreach (var node in _childNodes)
            {
                if (node is Element child && predicate(child))
                    yield return child;
            }
        }
    }

    public Element Child(string tagName, string? namespaceURI)
    {
        Require.NotNull(tagName);

        return Children(x => x.TagName == tagName && x.Prefix == null
            ? x.GetNamespace() == namespaceURI
            : x.GetNamespace(x.Prefix) == namespaceURI)
                .FirstOrDefault();
    }

    public T Child<T>() where T : Element
        => Children().OfType<T>().FirstOrDefault();

    public Element Child(Func<Element, bool> predicate)
    {
        Require.NotNull(predicate);

        foreach (var element in Children())
        {
            if (predicate(element))
                return element;
        }

        return null;
    }

    public string? GetTag(string tagName, string? namespaceURI = default)
    {
        Require.NotNullOrWhiteSpace(tagName);
        return Child(tagName, namespaceURI)?.Value;
    }

    public void SetTag(string tagName)
    {
        Require.NotNullOrWhiteSpace(tagName);
        AddChild(new Element(tagName));
    }

    public void SetTag(string tagName, object? value)
    {
        Require.NotNullOrWhiteSpace(tagName);

        var elem = new Element(tagName);

        if (value != null)
            elem.Value = Convert.ToString(value, CultureInfo.InvariantCulture);

        AddChild(elem);
    }

    public void SetTag(string tagName, string? namespaceURI, object? value)
    {
        Require.NotNullOrWhiteSpace(tagName);

        var elem = new Element(tagName, namespaceURI);

        if (value != null)
            elem.Value = Convert.ToString(value, CultureInfo.InvariantCulture);

        AddChild(elem);
    }

    public void RemoveTag(string tagName, string? namespaceURI = default)
    {
        Require.NotNullOrWhiteSpace(tagName);
        Children(tagName, namespaceURI).ForEach(n => n.Remove());
    }

    public bool HasTag(string tagName, string? namespaceURI = default)
    {
        Require.NotNullOrWhiteSpace(tagName);
        return Child(tagName, namespaceURI) is not null;
    }
}
