using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;

/* Unmerged change from project 'XmppSharp (net7.0)'
Before:
using XmppSharp.Factory;
After:
using XmppSharp.Factory;
using XmppSharp.Xmpp;
using XmppSharp.Xmpp;
using XmppSharp.Xmpp.Dom;
*/
using XmppSharp.Factory;

namespace XmppSharp.Dom;

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

    public Element Clone()
    {
        var result = ElementFactory.Create(_localName, _prefix, GetNamespace(_prefix));

        foreach (var (key, value) in Attributes)
            result.SetAttribute(key, value);

        foreach (var child in Children())
            result.AddChild(child.Clone());

        return result;
    }

    object ICloneable.Clone()
        => Clone();

    public Element? FirstChild
    {
        get
        {
            lock (_children)
                return _children.Count > 0 ? _children[0] : default;
        }
    }

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
            Value = text;
    }

    public string LocalName
    {
        get => _localName;
        set
        {
            Require.NotNullOrEmpty(value);
            _localName = value;
        }
    }

    public string? Prefix
    {
        get => _prefix;
        set
        {
            _prefix = string.IsNullOrWhiteSpace(value)
                ? null : value;
        }
    }

    public string? Value
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
            Xml.ToStringXml(this, writer);

        return output.ToString();
    }

    public void Remove()
    {
        _parent?.RemoveChild(this);
        _parent = null;
    }

    public void AddChild(Element e)
    {
        Require.NotNull(e);

        e.Remove();

        lock (_children)
            _children.Add(e);

        e._parent = this;
    }

    public void RemoveChild(Element e)
    {
        Require.NotNull(e);

        if (e._parent != this)
            return;

        lock (_children)
            _children.Remove(e);

        e._parent = null;

        string ns = e._prefix != null ? GetNamespace(e._prefix) : GetNamespace();

        if (ns != null)
        {
            if (e._prefix != null)
                e.SetNamespace(e._prefix, ns);
            else
                e.SetNamespace(ns);
        }
    }

    public string? GetAttribute(string name)
    {
        Require.NotNullOrEmpty(name);

        string result = default;

        lock (_attributes)
            _attributes.TryGetValue(name, out result);

        return result;
    }

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

    public bool HasAttribute(string name)
    {
        Require.NotNullOrEmpty(name);

        lock (_attributes)
            return _attributes.ContainsKey(name);
    }

    public bool RemoveAttribute(string name)
    {
        Require.NotNullOrEmpty(name);

        lock (_attributes)
            return _attributes.Remove(name);
    }

    public void SetNamespace(string uri)
        => SetAttribute("xmlns", uri);

    public void SetNamespace(string prefix, string uri)
    {
        Require.NotNullOrEmpty(prefix);
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

    // --------------------------------------------------------------------------------------- //

    public IEnumerable<Element> Children()
    {
        lock (_children)
            return _children.ToArray();
    }

    public IEnumerable<T> Children<T>() where T : Element
        => Children().OfType<T>();

    public IEnumerable<Element> Children(Func<Element, bool> predicate)
        => Children().Where(predicate);

    // --------------------------------------------------------------------------------------- //

    public T? Child<T>() where T : Element
        => Children().OfType<T>().FirstOrDefault();

    public Element Child(Func<Element, bool> predicate)
        => Children().FirstOrDefault(predicate);

    public void ReplaceChild<T>(T element) where T : Element
    {
        foreach (var entry in ElementFactory.GetTags<T>())
            Child(entry.HasName)?.Remove();

        if (element != null)
            AddChild(element);
    }
    public void ReplaceChild(Element element)
    {
        Require.NotNull(element);
        Child(element.TagName, element.GetNamespace(element.Prefix))?.Remove();
        AddChild(element);
    }

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

    public string? Namespace
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
                if (string.IsNullOrEmpty(_value) && _children.Count == 0)
                    return true;
            }

            return false;
        }
    }

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

    public Element Child(string name, string xmlns = default)
        => Child(x => SelectElementImpl(x, name, xmlns));

    public bool TryGetChild<T>(out T result) where T : Element, new()
    {
        result = Child<T>();
        return result != null;
    }

    public bool TryGetChild(string name, string xmlns, out Element result)
    {
        result = Child(name, xmlns);
        return result != null;
    }

    public bool TryGetChild(string name, out Element result)
    {
        result = Child(name);
        return result != null;
    }

    public bool TryGetChild(Func<Element, bool> filter, out Element result)
    {
        result = Child(filter);
        return result != null;
    }

    // ------------------------------------------------------------------------------------

    public string GetValue() => Value;
    public void SetValue(string s) => Value = s;

    // ------------------------------------------------------------------------------------

    public bool HasTag(string name, string? xmlns = default)
        => Child(x => SelectElementImpl(x, name, xmlns)) != null;

    public string GetTag(string name, string? xmlns = default)
        => Child(x => SelectElementImpl(x, name, xmlns))?.Value;

    public void SetTag(string name)
        => SetTag(name, null, null);

    public void RemoveTag(string name, string xmlns = default)
        => Child(x => SelectElementImpl(x, name, xmlns))?.Remove();

    public void SetTag(string name, string? value = default)
        => SetTag(name, null, value);

    public void SetTag(string name, string? xmlns = default, string? value = default)
    {
        var element = Child(x => SelectElementImpl(x, name, xmlns));

        if (element != null)
            element.Value = value;
        else
            AddChild(new Element(name, xmlns, value));
    }

    public override string ToString()
        => ToString(false);

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
                    Xml.ToStringXml(child, writer);
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
