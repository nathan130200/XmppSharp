namespace XmppSharp.Dom.Abstractions;

public abstract class Container : Node
{
    internal readonly List<Node> _children = [];

    public IEnumerable<Node> Nodes()
    {
        lock (_children)
            return [.. _children];
    }

    public Node? FirstNode
    {
        get
        {
            lock (_children)
                return _children.Count > 0 ? _children[0] : null;
        }
    }

    public Node? LastNode
    {
        get
        {
            lock (_children)
                return _children.Count > 0 ? _children[^1] : null;
        }
    }

    public Element? FirstChild
    {
        get
        {
            Element? temp = null;

            lock (_children)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    if (_children[i] is Element e)
                    {
                        temp = e;
                        break;
                    }
                }
            }

            return temp;
        }
    }

    public Element? LastChild
    {
        get
        {
            Element? temp = null;

            lock (_children)
            {
                for (int i = _children.Count - 1; i >= 0; i--)
                {
                    if (_children[i] is Element e)
                    {
                        temp = e;
                        break;
                    }
                }
            }

            return temp;
        }
    }

    public IEnumerable<Element> Elements()
    {
        List<Element>? result = null;

        lock (_children)
        {
            foreach (var node in _children)
            {
                if (node is Element e)
                {
                    result ??= [];
                    result.Add(e);
                }
            }
        }

        return result ?? [];
    }

    public T? Element<T>() where T : Element
    {
        T? result = null;

        lock (_children)
        {
            foreach (var node in _children)
            {
                if (node is T entry)
                {
                    result = entry;
                    break;
                }
            }
        }

        return result;
    }

    public IEnumerable<T> Elements<T>() where T : Element
    {
        List<T>? result = null;

        lock (_children)
        {
            foreach (var node in _children)
            {
                if (node is T entry)
                {
                    result ??= [];
                    result.Add(entry);
                }
            }
        }

        return result ?? [];
    }

    public IEnumerable<Element> Elements(string name, string? ns = null)
    {
        List<Element>? result = null;

        foreach (var el in Elements())
        {
            if (el.Name == name && (ns == null || el.Namespace == ns))
            {
                result ??= [];
                result.Add(el);
            }
        }

        return result ?? [];
    }

    public Element? Element(string name, string? ns = null)
    {
        Element? result = null;

        foreach (var el in Elements())
        {
            if (el.Name == name && (ns == null || el.Namespace == ns))
            {
                result = el;
                break;
            }
        }

        return result;
    }

    public IEnumerable<Node> Descendants()
    {
        List<Node>? result = null;
        BuildDescendantsList(this, ref result);
        return result ?? [];
    }

    public IEnumerable<Node> DescendantsAndSelf()
    {
        yield return this;

        foreach (var node in Descendants())
            yield return node;
    }

    static void BuildDescendantsList(Container parent, ref List<Node>? result)
    {
        foreach (var node in parent.Nodes())
        {
            result ??= [];

            result.Add(node);

            if (node is Container other)
                BuildDescendantsList(other, ref result);
        }
    }

    public void RemoveNodes()
    {
        lock (_children)
        {
            foreach (var node in _children)
                node._parent = null;

            _children.Clear();
        }
    }
}
