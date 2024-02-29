using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;
using XmppSharp.Factory;

namespace XmppSharp.Xml.Dom;

public class Element
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _localName, _prefix;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly List<Element> _children;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, string> _attributes;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Element _parent;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _value;


    internal Element()
    {
        _children = new List<Element>();
        _attributes = new Dictionary<string, string>();
    }

    public Element(Element other) : this()
    {
        _localName = other._localName;
        _prefix = other._prefix;
        _value = other._value;

        foreach (var element in other.Elements())
        {
            var newElement = ElementFactory.CreateElement(element.Name, element.Namespace);

            foreach (var (key, value) in element.Attributes())
                newElement._attributes[key] = value;

            _children.Add(newElement);
        }

        foreach (var (key, value) in other.Attributes())
            _attributes[key] = value;
    }

    public Element(string name) : this()
    {
        Name = name;
    }

    public Element(string name, string xmlns = default, string text = default) : this(name)
    {
        if (xmlns != null)
        {
            if (!string.IsNullOrWhiteSpace(_prefix))
                SetNamespace(_prefix, xmlns);
            else
                SetNamespace(xmlns);
        }

        if (text != null)
            Value = text;
    }

    public string Name
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_prefix))
                return _localName;

            return string.Concat(_prefix, ':', _localName);
        }
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var ofs = value.IndexOf(':');

            if (ofs == -1)
                _localName = XmlConvert.EncodeLocalName(value);
            else
            {
                _prefix = XmlConvert.EncodeLocalName(value[0..ofs]);
                _localName = XmlConvert.EncodeName(value[(ofs + 1)..]);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string LocalName => _localName;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string Prefix => _prefix;

    public Element Parent
    {
        get => _parent;
        set
        {
            _parent?.RemoveChild(this);
            _parent = value;
        }
    }

    public void AddChild(Element e)
    {
        if (e._parent == this)
            return;

        e.Remove();

        lock (_children)
            _children.Add(e);

        e._parent = this;
    }

    public void RemoveChild(Element e)
    {
        if (e._parent != this)
            return;

        lock (_children)
            _children.Remove(e);

        e._parent = null;
    }

    public void Remove()
        => _parent?.RemoveChild(this);

    public string Value
    {
        get => _value;
        set => _value = HttpUtility.HtmlEncode(value);
    }

    public void SetAttribute(string name, string value)
    {
        name = XmlConvert.EncodeName(name);

        lock (_attributes)
        {
            if (value == null)
                _attributes.Remove(name);
            else
                _attributes[name] = value;
        }
    }

    public string GetAttribute(string name, string defaultValue = default)
    {
        name = XmlConvert.EncodeName(name);

        lock (_attributes)
        {
            if (!_attributes.TryGetValue(name, out var result))
                return defaultValue;

            return result;
        }
    }

    public void RemoveAttribute(string name)
    {
        name = XmlConvert.EncodeName(name);

        lock (_attributes)
            _attributes.Remove(name);
    }

    public string Namespace
    {
        get => GetNamespace(Prefix);
        set
        {
            if (Prefix != null)
                SetNamespace(Prefix, value);
            else
                SetNamespace(value);
        }
    }

    public string GetNamespace(string prefix = default)
    {
        var ns = prefix == null ? GetAttribute("xmlns") : GetAttribute($"xmlns:{prefix}"); ;

        if (ns != null)
            return ns;

        return _parent?.GetNamespace(prefix);
    }

    public bool IsRoot
        => _parent is null;

    public void SetNamespace(string uri)
        => SetAttribute("xmlns", uri);

    public void SetNamespace(string prefix, string uri)
        => SetAttribute($"xmlns:{prefix}", uri);

    public override string ToString()
        => ToString(false);

    public IReadOnlyDictionary<string, string> Attributes()
    {
        KeyValuePair<string, string>[] result;

        lock (_attributes)
            result = _attributes.ToArray();

        return result.ToDictionary(x => x.Key, x => x.Value);
    }

    public IEnumerable<Element> Elements()
    {
        Element[] result;

        lock (_children)
            result = _children.ToArray();

        return result.AsEnumerable();
    }

    public string ToString(bool indented)
    {
        var sb = new StringBuilder();

        using (var sw = new StringWriter(sb))
        {
            var settings = new XmlWriterSettings
            {
                Indent = indented,
                IndentChars = " ",
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true,
            };

            using (var writer = XmlWriter.Create(sw, settings))
                ToXmlString(this, writer, true);
        }

        return sb.ToString();
    }

    static void ToXmlString(Element element, XmlWriter writer, bool includeSelf)
    {
        if (includeSelf)
        {
            string skipNamespace = "xmlns";

            if (element.Prefix != null)
            {
                writer.WriteStartElement(element.Prefix, element.LocalName, element.GetNamespace(element.Prefix));
                skipNamespace = $"xmlns:{element.Prefix}";
            }
            else
                writer.WriteStartElement(element.Name, element.GetNamespace());

            foreach (var (name, value) in element.Attributes())
            {
                if (skipNamespace != name)
                {
                    var hasPrefix = name.ExtractQualifiedName(out var localName, out var prefix);

                    if (!hasPrefix)
                        writer.WriteAttributeString(name, value);
                    else
                    {
                        var ns = prefix switch
                        {
                            "xml" => Namespaces.Xml,
                            "xmlns" => Namespaces.Xmlns,
                            var other => element.GetNamespace(prefix)
                        };

                        writer.WriteAttributeString(localName, ns, value);
                    }
                }
            }

            if (element.Value != null)
                writer.WriteString(element.Value);
        }

        foreach (var child in element.Elements())
            ToXmlString(child, writer, true);

        if (includeSelf)
            writer.WriteEndElement();
    }

    public void RemoveAll()
    {
        lock (_attributes)
            _attributes.Clear();

        RemoveAllChildren();
    }

    public void RemoveAllChildren()
    {
        foreach (var el in Elements())
            RemoveChild(el);

        _value = null;
    }
}