using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;

namespace XmppSharp.Xmpp.Dom;

public class Element : ICloneable
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly List<Element> _children;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, string> _attributes;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Element _parent;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _localName, _prefix, _value;

    protected Element()
    {
        _children = new();
        _attributes = new();
    }

    static readonly ConstructorInfo s_DefaultConstructor = typeof(Element)
        .GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic, new[] { typeof(Element) });

    public Element Clone()
        => s_DefaultConstructor.Invoke(new object[] { this }) as Element;

    object ICloneable.Clone()
        => Clone();

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

    public Element(string name, string xmlns = default, string text = default) : this()
    {
        var result = Xml.ExtractQName(name);

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
            Value = text;
    }

    public string LocalName
    {
        get => _localName;
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            _localName = value;
        }
    }

    public string Prefix
    {
        get => _prefix;
        set
        {
            _prefix = string.IsNullOrWhiteSpace(value)
                ? null : value;
        }
    }

    public string Value
    {
        get => _value;
        set => _value = value;
    }

    public Element Parent
    {
        get => _parent;
        set
        {
            _parent?.RemoveChild(this);
            (_parent = value)?.AddChild(this);
        }
    }

    public string ToString(bool indented)
    {
        var (output, writer) = Xml.CreateXmlWriter(indented);

        using (writer)
            Xml.WriteXmlTree(this, writer);

        return output.ToString();
    }

    public void Remove()
    {
        _parent?.RemoveChild(this);
        _parent = null;
    }

    public void AddChild(Element e)
    {
        ArgumentNullException.ThrowIfNull(e);

        e.Remove();

        lock (_children)
            _children.Add(e);

        e._parent = this;
    }

    public void RemoveChild(Element e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (e._parent != this)
            return;

        lock (_children)
            _children.Remove(e);

        e._parent = null;

        if (e._prefix != null)
            e.SetNamespace(e._prefix, GetNamespace(e.Prefix));
        else
            e.SetNamespace(e.GetNamespace());
    }

    public string? GetAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string result = default;

        lock (_attributes)
            _attributes.TryGetValue(name, out result);

        return result;
    }

    public void SetAttribute(string name, string? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        lock (_attributes)
        {
            if (value == null)
                _attributes.Remove(name);
            else
                _attributes[name] = value;
        }
    }

    public bool HasAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        lock (_attributes)
            return _attributes.ContainsKey(name);
    }

    public bool RemoveAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        lock (_attributes)
            return _attributes.Remove(name);
    }

    public void SetNamespace(string uri)
        => SetAttribute("xmlns", uri);

    public void SetNamespace(string prefix, string uri)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);
        SetAttribute($"xmlns:{prefix}", uri);
    }

    public bool HasNamespace(string prefix = default)
    {
        if (prefix != null)
            return HasAttribute($"xmlns:{prefix}");

        return HasAttribute("xmlns");
    }

    public IReadOnlyDictionary<string, string> Attributes
    {
        get
        {
            lock (_attributes)
                return _attributes.ToArray().ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public IEnumerable<Element> Children()
    {
        lock (_children)
            return _children.ToArray();
    }

    public T Child<T>() where T : Element
        => Children().OfType<T>().FirstOrDefault();

    public void ReplaceChild<T>(T newElement) where T : Element
    {
        Child(x => DefaultElementFilterImpl(x, newElement.TagName, newElement.GetNamespace()))?.Remove();

        if (newElement != null)
            AddChild(newElement);
    }

    public void ReplaceChild(Element element)
    {
        Child(x => DefaultElementFilterImpl(x, element.TagName, element.GetNamespace()))?.Remove();
        AddChild(element);
    }

    public Element Child(Func<Element, bool> predicate)
        => Children().FirstOrDefault(predicate);

    public IReadOnlyList<Element> Descendants()
    {
        var result = new List<Element>();
        GetDescendants(result);
        return result;
    }

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

    public string TagName
    {
        get
        {
            if (_prefix == null)
                return _localName;

            return string.Concat(_prefix, ':', _localName);
        }
    }

    public string? DefaultNamespace
    {
        get => GetNamespace();
        set => SetNamespace(value);
    }

    public bool IsRootElement
        => _parent == null;

    public bool IsEmptyElement
    {
        get
        {
            lock (_children)
            {
                if (string.IsNullOrEmpty(_value) || _children.Count == 0)
                    return false;
            }

            return true;
        }
    }

    public string GetNamespace(string? prefix = default)
    {
        var value = GetAttribute(!string.IsNullOrWhiteSpace(prefix)
            ? $"xmlns:{prefix}" : "xmlns");

        if (value != null)
            return value;

        return _parent?.GetNamespace(prefix);
    }

    // ------------------------------------------------------------------------------------

    static bool DefaultElementFilterImpl(Element e, string name, string xmlns)
        => e.TagName == name && (xmlns == null || xmlns == e.GetNamespace());

    public Element GetChild(string name, string xmlns = default)
        => Child(x => DefaultElementFilterImpl(x, name, xmlns));

    // ------------------------------------------------------------------------------------

    public bool HasTag(string name, string? xmlns = default)
        => Child(x => DefaultElementFilterImpl(x, name, xmlns)) != null;

    public string GetTag(string name, string? xmlns = default)
        => Child(x => DefaultElementFilterImpl(x, name, xmlns))?.Value;

    public void SetTag(string name)
        => SetTag(name, null, null);

    public void RemoveTag(string name, string xmlns = default)
        => Child(x => DefaultElementFilterImpl(x, name, xmlns))?.Remove();

    public void SetTag(string name, string? value = default)
        => SetTag(name, null, value);

    public void SetTag(string name, string? xmlns = default, string? value = default)
    {
        var element = Child(x => DefaultElementFilterImpl(x, name, xmlns));

        if (element != null)
            element.Value = value;
        else
            AddChild(new Element(name, xmlns, value));
    }

    public override string ToString()
        => StartTag();

    public string StartTag()
    {
        var sb = new StringBuilder($"<{XmlConvert.EncodeName(TagName)}");

        foreach (var (name, value) in Attributes)
            sb.AppendFormat(" {0}=\"{1}\"", XmlConvert.EncodeName(name), HttpUtility.HtmlAttributeEncode(value));

        return sb.Append('>').ToString();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string InnerXml
    {
        get
        {
            var (result, writer) = Xml.CreateXmlWriter(false);

            using (writer)
            {
                foreach (var child in Children())
                    Xml.WriteXmlTree(child, writer);
            }

            return result.ToString();
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string OuterXml
        => ToString(false);

    public byte[] GetContentFromBase64()
    {
        if (_value == null)
            return Array.Empty<byte>();
        else
            return Convert.FromBase64String(_value);
    }

    public void SetContentAsBase64(byte[]? buffer)
    {
        if (buffer == null)
            _value = null;
        else
            _value = Convert.ToBase64String(buffer);
    }

    public string GetContentFromBase64String(Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(GetContentFromBase64());

    public void SetContentAsBase64String(string value, Encoding? encoding = default)
        => SetContentAsBase64((encoding ?? Encoding.UTF8).GetBytes(value));
}
