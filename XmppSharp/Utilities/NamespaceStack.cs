using System.Collections;

namespace XmppSharp.Utilities;

public class NamespaceStack
{
    private readonly Stack<Hashtable> _stack = new();
    private readonly object _syncRoot = new();

    public NamespaceStack()
    {
        PushScope();
        AddNamespace("xml", Namespaces.Xml);
        AddNamespace("xmlns", Namespaces.Xmlns);
    }

    public void PushScope()
    {
        lock (_syncRoot)
            _stack.Push(new Hashtable());
    }

    public void PopScope()
    {
        lock (_syncRoot)
        {
            if (_stack.Count > 1)
                _stack.Pop();
        }
    }

    public void AddNamespace(string prefix, string uri)
    {
        lock (_syncRoot)
        {
            var dict = _stack.Peek();
            dict.Add(prefix, uri);
        }
    }

    public string? LookupNamespace(string? prefix)
    {
        prefix ??= string.Empty;

        lock (_syncRoot)
        {
            foreach (var entry in _stack)
            {
                if (entry.ContainsKey(prefix))
                    return (string)entry[prefix];
            }
        }

        return null;
    }

    public void Clear()
    {
        lock (_syncRoot)
        {
            while (_stack.Count > 1)
                _stack.Pop();
        }
    }

    public string DefaultNamespace
        => LookupNamespace(string.Empty);
}
