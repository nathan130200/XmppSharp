using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

public sealed class Element : Container
{
    internal readonly List<Attribute> _attributes = [];

    int _prefixEnd;

    public string Name
    {
        get;
        set
        {
            XmlConvert.VerifyName(value);

            field = value;

            _prefixEnd = value.IndexOf(':');
        }
    }

    public ReadOnlySpan<char> Prefix => _prefixEnd > 0 ? Name.AsSpan(0.._prefixEnd) : [];

    public ReadOnlySpan<char> LocalName => _prefixEnd > 0 ? Name.AsSpan((_prefixEnd + 1)..) : Name;

    public string? Namespace
    {
        get;
        set;
    }

    public Element(string name, string? ns = default)
    {
        XmlConvert.VerifyName(name);

        var prefixEnd = name.IndexOf(':');

        if (prefixEnd > 0)
        {
            var prefix = name.AsSpan(0..prefixEnd);

            if (prefix.IsEmpty || prefix.IsWhiteSpace())
                throw new ArgumentException("Namespace cannot be null or empty.");
        }

        Name = name;

        Namespace = ns;
    }

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

    public Attribute? Attribute(string name)
    {
        lock (_attributes)
            return _attributes.Find(x => x.Name == name);
    }

    public IEnumerable<Attribute> Attributes()
    {
        lock (_attributes)
            return [.. _attributes];
    }

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

    public string? GetAttribute(string name, string? defaultValue = default)
    {
        if (Attribute(name) is { } attr)
            return attr.Value;

        return defaultValue;
    }

    public bool HasAttribute(string name)
    {
        lock (_attributes)
            return _attributes.FindIndex(x => x.Name == name) != -1;
    }

    public bool RemoveAttribute(string name)
    {
        lock (_attributes)
        {
            var entry = _attributes.Find(x => x.Name == name);

            if (entry is null) return false;

            lock (entry)
                RemoveAttributeCore(entry);

            return true;
        }
    }

    internal void RemoveAttributeCore(Attribute? attr)
    {
        if (attr is null) return;

        lock (_attributes)
            _attributes.Remove(attr);

        attr._parent = null;
    }

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

    public bool HasTag(string name, string? ns = default) => Element(name, ns) != null;

    public string? GetTag(string name, string? ns = default)
        => Element(name, ns)?.Value;

    public void SetTag(string name, string? ns = default, object? value = default)
    {
        lock (_children)
        {
            var entry = Element(name, ns);

            if (entry is null)
                AddChild(entry = new Element(name, ns));

            if (value is not null)
                entry.Value = Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

    public void RemoveTag(string name, string? ns = default)
    {
        lock (_children)
        {
            foreach (var node in Elements(name, ns))
                node.Remove();
        }
    }

    public IEnumerable<Element> Descendants()
    {
        List<Element>? result = null;
        BuildDescendantsList(this, ref result);
        return result ?? s_EmptyElementList;
    }

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

    public string? GetNamespaceOfPrefix(string? prefix = default)
    {
        lock (_attributes)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                return _attributes.Find(x => x.IsNamespaceDeclaration && x.LocalName.Equals("xmlns", StringComparison.Ordinal))?.Value;

            return _attributes.Find(x => x.IsNamespaceDeclaration && x.LocalName.Equals(prefix, StringComparison.Ordinal))?.Value;
        }
    }

    public string? GetPrefixOfNamespace(string ns)
    {
        lock (_attributes)
        {
            var attr = _attributes.Find(x => x.IsNamespaceDeclaration && x.Value == ns);

            if (attr is null) return null;

            return attr.Prefix.IsEmpty ? string.Empty : attr.LocalName.ToString();
        }
    }

    public void WriteContent(XmlWriter writer)
    {
        lock (_children)
        {
            foreach (var node in _children)
                node.WriteTo(writer);
        }
    }

    public override void WriteTo(XmlWriter writer)
    {
        lock (this)
        {
            WriteStartElement(writer);
            WriteContent(writer);
            writer.WriteEndElement();
        }
    }

    public sealed override string ToString() => ToString(false);

    const string c_TwoSpaces = "  ";

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
