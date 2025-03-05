using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XmppSharp.Collections;

namespace XmppSharp.Dom;

[DebuggerDisplay("{StartTag(),nq}")]
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
    public IEnumerable<XmppNode>? Children => _element?.Nodes();
}

[DebuggerTypeProxy(typeof(XmppElementDebuggerProxy))]
public class XmppElement : XmppNode
{
    public XmppName QualifiedName { get; set; } = default!;
    public string? Prefix => QualifiedName.Prefix;
    public string LocalName => QualifiedName.LocalName;

    public XmppElement? this[XmppName name] => Element(name);

    public string TagName
    {
        get => QualifiedName;
        set => QualifiedName = new(value);
    }

    internal readonly List<XmppNode> _children;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConcurrentDictionary<string, string> Attributes { get; }

    internal XmppElement()
    {
        _children = new List<XmppNode>();
        Attributes = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
    }

    public XmppElement(XmppName tagName, string? namespaceURI = default, object? value = default) : this()
    {
        Throw.IfNull(tagName);

        QualifiedName = tagName;

        if (namespaceURI != null)
        {
            if (Prefix != null)
                SetNamespace(Prefix, namespaceURI);
            else
                SetNamespace(namespaceURI);
        }

        SetValue(value);
    }

    public XmppElement(XmppElement other) : this()
    {
        QualifiedName = new(other.QualifiedName);

        foreach (var (key, value) in other.Attributes)
            Attributes[key] = value;

        foreach (var node in other.Nodes())
            _children.Add(node.Clone());
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
            _children.Add(node);
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

    public IEnumerable<XmppElement> Descendants(bool includeSelf = false)
    {
        var result = new List<XmppElement>();
        BuildDescendantElements(this, result, includeSelf);
        return result;
    }

    public IEnumerable<XmppElement> DescendantNodes(bool includeSelf = false)
    {
        var result = new List<XmppElement>();
        BuildDescendantElements(this, result, includeSelf);
        return result;
    }

    static void BuildDescendantNodes(XmppElement parent, List<XmppNode> result, bool includeSelf = false)
    {
        if (includeSelf)
            result.Add(parent);

        foreach (var child in parent.Nodes())
        {
            result.Add(child);

            if (child is XmppElement e)
                BuildDescendantNodes(e, result, false);
        }
    }

    static void BuildDescendantElements(XmppElement parent, List<XmppElement> result, bool includeSelf = false)
    {
        if (includeSelf)
            result.Add(parent);

        foreach (var child in parent.Elements())
        {
            result.Add(child);
            BuildDescendantElements(child, result, false);
        }
    }

    public XmppElement SetAttribute(string name, object? value, string? format = default, IFormatProvider? ifp = default)
    {
        Throw.IfStringNullOrWhiteSpace(name);

        ifp ??= CultureInfo.InvariantCulture;

        if (value is null)
            RemoveAttribute(name);
        else
        {
            string str;
            if (value is IFormattable fmt) str = fmt.ToString(format, ifp);
            else if (value is IConvertible conv) str = conv.ToString(ifp);
            else str = Convert.ToString(value, ifp) ?? string.Empty;
            Attributes[name] = str;
        }

        return this;
    }

    public override XmppNode Clone()
    {
        var result = XmppElementFactory.Create(TagName, Namespace);

        foreach (var (key, value) in Attributes)
            result.Attributes[key] = value;

        foreach (var node in Nodes())
            result.AddChild(node.Clone());

        result.Value = Value;

        return result;
    }

    public bool HasAttribute(string name)
    {
        Throw.IfStringNullOrWhiteSpace(name);
        return Attributes.ContainsKey(name);
    }

    public string? GetAttribute(XmppName name)
    {
        Throw.IfStringNullOrWhiteSpace(name);
        return Attributes.GetValueOrDefault(name);
    }

    public bool RemoveAttribute(string name)
    {
        Throw.IfStringNullOrWhiteSpace(name);
        return Attributes.Remove(name, out _);
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
        Throw.IfStringNullOrWhiteSpace(prefix);
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

    public string ToString(bool indented)
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

    public IEnumerable<XmppElement> Elements(XmppName tagName, string? namespaceURI = default)
    {
        Throw.IfNull(tagName);
        return LookupElements(this, tagName, namespaceURI, false);
    }

    static IEnumerable<XmppElement> LookupElements(XmppElement self, XmppName tagName, string? ns, bool once)
    {
        foreach (var element in self.Elements())
        {
            if (element.TagName == tagName && (ns == null || ns == element.Namespace))
            {
                yield return element;

                if (once)
                    yield break;
            }
        }
    }

    public XmppElement? Element(XmppName tagName, string? namespaceURI = default)
    {
        Throw.IfNull(tagName);
        return LookupElements(this, tagName, namespaceURI, true).FirstOrDefault();
    }

    public bool HasTag(XmppName tagName, string? namespaceURI = default)
    {
        Throw.IfNull(tagName);
        return LookupElements(this, tagName, namespaceURI, true).Any();
    }

    public XmppElement SetTag(XmppName name, string? xmlns = default, object? value = default)
    {
        Throw.IfNull(name);

        var e = new XmppElement(name, xmlns, value);
        AddChild(e);
        return e;
    }

    public void RemoveTag(XmppName tagName, string? namespaceURI = default)
        => Element(tagName, namespaceURI)?.Remove();

    public void RemoveTags(XmppName tagName, string? namespaceURI = default)
        => Elements(tagName, namespaceURI).Remove();

    public string? GetTag(XmppName tagName, string? namespaceURI = default)
        => Element(tagName, namespaceURI)?.Value;

    public IEnumerable<string> GetTags(XmppName tagName, string? namespaceURI = default)
        => Elements(tagName, namespaceURI).Select(x => x.Value!);

    public T? Element<T>() where T : XmppElement
        => Elements().OfType<T>().FirstOrDefault();

    public IEnumerable<T> Elements<T>() where T : XmppElement
        => Elements().OfType<T>();

    public void RemoveAttributes()
        => Attributes.Clear();

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