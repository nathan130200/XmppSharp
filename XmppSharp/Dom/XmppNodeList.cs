using System.Collections;

namespace XmppSharp.Dom;

public class XmppNodeList : IEnumerable<XmppNode>
{
    readonly XmppElement _parent;
    readonly List<XmppNode> _nodes = new();

    public XmppNodeList(XmppElement parent)
        => _parent = parent;

    public void Add(XmppNode? node)
    {
        if (node is null)
            return;

        if (node.Parent is not null)
            node = node.Clone();

        lock (_nodes)
        {
            node._parent = _parent;
            _nodes.Add(node);
        }
    }

    public void Remove(XmppNode? node)
    {
        if (node is null)
            return;

        if (node.Parent != _parent)
            return;

        lock (_nodes)
        {
            node._parent = null;
            _nodes.Remove(node);
        }
    }

    public IEnumerator<XmppNode> GetEnumerator()
    {
        lock (_nodes)
        {
            foreach (var node in _nodes)
                yield return node;
        }
    }

    public void Clear()
    {
        lock (_nodes)
        {
            foreach (var item in _nodes)
                item._parent = null;

            _nodes.Clear();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
