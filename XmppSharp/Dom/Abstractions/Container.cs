using System.Diagnostics;

namespace XmppSharp.Dom.Abstractions;

public abstract class Container : Node
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal readonly List<Node> _children = [];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal static readonly IEnumerable<Node> s_EmptyNodeList = [];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    internal static readonly IEnumerable<Element> s_EmptyElementList = [];

    public IEnumerable<Node> Nodes()
    {
        List<Node>? result = null;

        lock (_children)
        {
            if (_children.Count > 0)
            {
                result = [];

                foreach (var node in _children)
                    result.Add(node);
            }
        }

        return result ?? s_EmptyNodeList;
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

        return result ?? s_EmptyElementList;
    }

    public Element? Element(string name, string? uri = default)
    {
        uri ??= (this as Element)?.Namespace;

        Element? result = null;

        foreach (var element in Elements())
        {
            if (element.Name == name && element.Namespace == uri)
            {
                result = element;
                break;
            }
        }

        return result;
    }

    public IEnumerable<Element> Elements(string name, string? uri = default)
    {
        uri ??= (this as Element)?.Namespace;

        foreach (var element in Elements())
        {
            if (element.Name == name && element.Namespace == uri)
                yield return element;
        }
    }

    public void RemoveNodes()
    {
        lock (_children)
        {
            foreach (var node in _children)
                node._parent = null;

            _children.Clear();

            _children.Capacity = 0;
        }
    }

    public IEnumerable<Node> DescendantNodes()
    {
        List<Node>? result = null;
        BuildDescendantNodesList(this, ref result);
        return result ?? s_EmptyNodeList;
    }

    public IEnumerable<Node> DescendantNodesAndSelf()
    {
        yield return this;

        foreach (var node in DescendantNodes())
            yield return node;
    }

    static void BuildDescendantNodesList(Container source, ref List<Node>? result)
    {
        foreach (var node in source.Nodes())
        {
            result ??= [];

            result.Add(node);

            if (node is Container other)
                BuildDescendantNodesList(other, ref result);
        }
    }
}
