namespace XmppSharp.Utilities;

public class NamespaceStack
{
    private readonly Stack<Dictionary<string, string>> _stack = new();
    private readonly object _syncRoot = new();

    public NamespaceStack()
    {
        Reset();
    }

    public void PushScope()
    {
        lock (_syncRoot)
            _stack.Push(new(StringComparer.Ordinal));
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
            _stack.Peek()[prefix] = uri;
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
                    return entry[prefix]!;
            }
        }

        return null;
    }

    public void Reset()
    {
        lock (_syncRoot)
        {
            while (_stack.Count > 0)
                _ = _stack.Pop();

            _stack.Push(new()
            {
                ["xml"] = Namespaces.Xml,
                ["xmlns"] = Namespaces.Xmlns
            });
        }
    }

    public string? DefaultNamespace
        => LookupNamespace(string.Empty);
}
