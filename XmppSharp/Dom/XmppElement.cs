using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;

namespace XmppSharp.Dom;

[DebuggerDisplay("{StartTag,nq}")]
internal class XmppElementDebuggerProxy
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly XmppElement? _element;

    public XmppElementDebuggerProxy(XmppElement? e)
        => _element = e;

    public string? TagName => _element?.TagName;
    public string? StartTag => _element?.StartTag();
    public string? EndTag => _element?.EndTag();
    public IReadOnlyDictionary<string, string>? Attributes => _element?.Attributes;
    public IEnumerable<XmppNode>? ChildNodes => _element?.Nodes();
}

[DebuggerTypeProxy(typeof(XmppElementDebuggerProxy))]
public class XmppElement : XmppNode
{
    private string _localName;
    private string? _prefix;

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
            Throw.IfNullOrWhiteSpace(value);
            _localName = value;
        }
    }

    public XmppElement? this[string tagName] => Element(tagName);
    public XmppElement? this[string tagName, string? xmlns] => Element(tagName, xmlns);

    public string TagName
    {
        get
        {
            if (_prefix == null)
                return _localName;

            return string.Concat(_prefix, ':', _localName);
        }
        set
        {
            Throw.IfNullOrWhiteSpace(value);

            var hasPrefix = Xml.ExtractQualifiedName(value, out var prefix, out var localName);

            if (hasPrefix)
                Prefix = prefix;

            LocalName = localName;
        }
    }

    protected internal readonly List<XmppNode> _children;
    protected internal readonly Dictionary<string, string> _attributes;

    internal XmppElement()
    {
        _children = new List<XmppNode>();
        _attributes = new Dictionary<string, string>(StringComparer.Ordinal);
    }

    public XmppElement(string tagName, string? namespaceURI = default, object? value = default) : this()
    {
        Throw.IfNull(tagName);

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

    protected internal virtual XmppNode TransformNode(XmppNode node)
    {
        return node;
    }

    public bool IsRootElement
        => _parent == null;

    public string? Value
    {
        get
        {
            var nodes = from node in Nodes()
                        where node is XmppText
                        select node;

            if (!nodes.Any())
                return null;

            return string.Concat(nodes);
        }
        set
        {
            RemoveNodes();
            AddChild(new XmppText(value));
        }
    }

    public void SetValue(object? content, IFormatProvider? format = default)
    {
        format ??= CultureInfo.InvariantCulture;

        if (content != null)
            Value = Convert.ToString(content, format);
    }

    public void AddChild(XmppNode? node)
    {
        if (node is null)
            return;

        if (node._parent is not null)
            node = node.Clone();

        lock (_children)
        {
            node._parent = this;
            _children.Add(TransformNode(node));
        }
    }

    public void RemoveChild(XmppNode? node)
    {
        if (node?._parent != this)
            return;

        lock (_children)
        {
            node._parent = null;
            _children.Remove(node);
        }
    }

    public IEnumerable<XmppElement> Descendants()
    {
        var result = new List<XmppElement>();
        CollectAllElements(this, result, false);
        return result;
    }

    public IEnumerable<XmppElement> DescendantsAndSelf()
    {
        var result = new List<XmppElement>();
        CollectAllElements(this, result, true);
        return result;
    }

    public IEnumerable<XmppNode> DescendantNodes()
    {
        var result = new List<XmppNode>();
        CollectAllNodes(this, result, false);
        return result;
    }

    public IEnumerable<XmppNode> DescendantNodesAndSelf()
    {
        var result = new List<XmppNode>();
        CollectAllNodes(this, result, true);
        return result;
    }

    static void CollectAllNodes(XmppElement parent, List<XmppNode> result, bool includeSelf = false)
    {
        if (includeSelf)
            result.Add(parent);

        foreach (var child in parent.Nodes())
        {
            result.Add(child);

            if (child is XmppElement e)
                CollectAllNodes(e, result, false);
        }
    }

    static void CollectAllElements(XmppElement parent, List<XmppElement> result, bool includeSelf = false)
    {
        if (includeSelf)
            result.Add(parent);

        foreach (var child in parent.Elements())
        {
            result.Add(child);
            CollectAllElements(child, result, false);
        }
    }

    public void SetAttribute(string name, object? value, IFormatProvider? ifp = default)
    {
        Throw.IfNullOrWhiteSpace(name);

        ifp ??= CultureInfo.InvariantCulture;

        lock (_attributes)
        {
            if (value is null)
                _attributes.Remove(name);
            else
                _attributes[name] = Convert.ToString(value, ifp ?? CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }

    public override XmppNode Clone()
    {
        var result = (XmppElement)Activator.CreateInstance(GetType())!;

        lock (_attributes)
        {
            foreach (var (key, value) in _attributes)
                result._attributes[key] = value;
        }

        foreach (var node in Nodes())
            result.AddChild(node.Clone());

        result.Value = Value;

        return result;
    }

    public IReadOnlyDictionary<string, string> Attributes
    {
        get
        {
            lock (_attributes)
                return _attributes.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public bool HasAttribute(string name)
    {
        Throw.IfNullOrWhiteSpace(name);

        lock (_attributes)
            return _attributes.ContainsKey(name);
    }

    public string? GetAttribute(string name, string? defaultValue = default)
    {
        Throw.IfNullOrWhiteSpace(name);

        lock (_attributes)
            return _attributes.GetValueOrDefault(name, defaultValue!);
    }

    public bool RemoveAttribute(string name)
    {
        Throw.IfNullOrWhiteSpace(name);

        lock (_attributes)
            return _attributes.Remove(name, out _);
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
        Throw.IfNull(namespaceURI);
        SetAttribute("xmlns", namespaceURI);
    }

    public void SetNamespace(string prefix, string? namespaceURI)
    {
        Throw.IfNullOrWhiteSpace(prefix);
        Throw.IfNull(namespaceURI);
        SetAttribute($"xmlns:{prefix}", namespaceURI);
    }

    public XmppNode? FirstNode => Nodes().FirstOrDefault();
    public XmppNode? LastNode => Nodes().LastOrDefault();
    public XmppElement? FirstChild => Elements().FirstOrDefault();
    public XmppElement? LastChild => Elements().LastOrDefault();

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

    public IEnumerable<XmppNode> Nodes()
    {
        lock (_children)
            return _children.ToArray();
    }

    public void WriteTo(TextWriter textWriter, XmppFormatting formatting = XmppFormatting.Default)
    {
        using var writer = Xml.CreateWriter(textWriter, formatting);
        Xml.WriteTree(this, writer);
    }

    public void WriteTo(XmlWriter writer)
        => Xml.WriteTree(this, writer);

    public void Save(Stream stream, XmppFormatting formatting = XmppFormatting.Default)
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

    public virtual string ToString(bool indented)
    {
        var sb = new StringBuilder();

        var formatting = XmppFormatting.Default;

        if (indented)
            formatting |= XmppFormatting.Indented;

        using (var sw = new StringWriter(sb))
            WriteTo(sw, formatting);

        return sb.ToString();
    }

    public IEnumerable<XmppElement> Elements()
        => Nodes().OfType<XmppElement>();

    public IEnumerable<XmppElement> Elements(string tagName, string? namespaceURI = default)
    {
        Throw.IfNullOrWhiteSpace(tagName);
        return FindElements(this, tagName, namespaceURI, false);
    }

    static IEnumerable<XmppElement> FindElements(XmppElement self, string tagName, string? ns, bool findOne)
    {
        foreach (var element in self.Elements())
        {
            if (element.TagName == tagName && (ns == null || ns == element.Namespace))
            {
                yield return element;

                if (findOne)
                    yield break;
            }
        }
    }

    public XmppElement? Element(string tagName, string? namespaceURI = default)
    {
        Throw.IfNullOrWhiteSpace(tagName);
        return FindElements(this, tagName, namespaceURI, true).FirstOrDefault();
    }

    public XmppElement? Element(Func<XmppElement, bool> match)
        => Elements().FirstOrDefault(match);

    public bool HasTag(string tagName, string? namespaceURI = default)
    {
        Throw.IfNullOrWhiteSpace(tagName);
        return FindElements(this, tagName, namespaceURI, true).Any();
    }

    public XmppElement SetTag(string tagName, string? xmlns = default, object? value = default)
    {
        Throw.IfNullOrWhiteSpace(tagName);

        var e = new XmppElement(tagName, xmlns, value);
        AddChild(e);
        return e;
    }

    public void RemoveTag(string tagName, string? namespaceURI = default)
        => Element(tagName, namespaceURI)?.Remove();

    public void RemoveTags(string tagName, string? namespaceURI = default)
        => Elements(tagName, namespaceURI).Remove();

    public string? GetTag(string tagName, string? namespaceURI = default)
        => Element(tagName, namespaceURI)?.Value;

    public IEnumerable<string> GetTags(string tagName, string? namespaceURI = default)
        => Elements(tagName, namespaceURI).Select(x => x.Value!);

    public T? Element<T>() where T : XmppElement
        => Elements().OfType<T>().FirstOrDefault();

    public IEnumerable<T> Elements<T>() where T : XmppElement
        => Elements().OfType<T>();

    public void RemoveAttributes()
    {
        lock (_attributes)
            _attributes.Clear();
    }

    public void RemoveAll()
    {
        RemoveAttributes();
        RemoveNodes();
    }

    public void RemoveNodes()
    {
        lock (_children)
        {
            foreach (var item in _children)
                item._parent = null;

            _children.Clear();
        }
    }
}