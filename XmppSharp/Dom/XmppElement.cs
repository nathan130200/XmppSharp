using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using XmppSharp.Collections;

namespace XmppSharp.Dom;

[DebuggerDisplay("{StartTag(),nq}")]
public class XmppElement : XmppNode
{
    public XmppName Name { get; set; } = default!;
    public string? Prefix => Name.Prefix;
    public string LocalName => Name.LocalName;

    public XmppNodeList Children { get; private set; }
    public ConcurrentDictionary<string, string> Attributes { get; } = new();
    public XmppElement? this[XmppName name] => Element(name);

    public string TagName
    {
        get => Name;
        set => Name = new(value);
    }

    internal XmppElement()
    {
        Attributes = new();
        Children = new(this);
    }

    public XmppElement(XmppName name, string? namespaceURI = default, object? value = default) : this()
    {
        Name = name;

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
        if (GetType() != other.GetType())
            throw new InvalidOperationException("To perform a copy constructor, both objects must be of the same type.");

        Name = new(other.Name);

        foreach (var (key, value) in other.Attributes)
            Attributes[key] = value;

        lock (other.Children)
        {
            foreach (var node in other.Nodes())
                Children.Add(node.Clone());
        }
    }

    public bool IsRootElement
        => _parent == null;

    public string? Value
    {
        get
        {
            var nodes = Nodes()
                .OfType<XmppText>()
                .Select(x => x.Value);

            if (!nodes.Any())
                return null;

            return string.Concat(nodes);
        }
        set
        {
            RemoveAllChildNodes();
            AddChild(new XmppText(value));
        }
    }

    public void SetValue(object? content, IFormatProvider? format = default)
    {
        format ??= CultureInfo.InvariantCulture;

        if (content != null)
            Value = Convert.ToString(content, format);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddChild(XmppNode? node) => Children.Add(node);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveChild(XmppNode? node) => Children.Remove(node);

    public XmppElement SetAttribute(string name, object? value, string? format = default, IFormatProvider? ifp = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);

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
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);

        lock (Attributes)
            return Attributes.ContainsKey(name);
    }

    public string? GetAttribute(XmppName name)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);
        return Attributes.GetValueOrDefault(name);
    }

    public bool RemoveAttribute(string name)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(name);
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
        ThrowHelper.ThrowIfNull(namespaceURI);
        SetAttribute("xmlns", namespaceURI);
    }

    public void SetNamespace(string prefix, string? namespaceURI)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(prefix);
        ThrowHelper.ThrowIfNull(namespaceURI);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<XmppNode> Nodes() => Children;

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
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);
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
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);
        return LookupElements(this, tagName, namespaceURI, true).FirstOrDefault();
    }

    public bool HasTag(XmppName tagName, string? namespaceURI = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);
        return LookupElements(this, tagName, namespaceURI, true).Any();
    }

    public XmppElement SetTag(XmppName name, string? xmlns = default, object? value = default)
    {
        var e = new XmppElement(name, xmlns, value);
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

    public void ClearAttributes()
    {

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAllChildNodes()
    {
        Children.Clear();
    }
}