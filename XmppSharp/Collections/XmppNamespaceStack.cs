namespace XmppSharp.Collections;

public class XmppNamespaceStack
{
    private readonly Stack<Dictionary<string, string>> _stack = new();

    public XmppNamespaceStack()
    {
        Reset();
    }

    public void PushScope()
    {
        lock (_stack)
            _stack.Push(new(StringComparer.Ordinal));
    }

    public void PopScope()
    {
        lock (_stack)
        {
            if (_stack.Count > 1)
                _stack.Pop();
        }
    }

    public void AddNamespace(string prefix, string uri)
    {
        if (prefix == "xmlns" || prefix == "xml")
            throw new InvalidOperationException("Reserved XML prefixes cannot be redefined.");

        lock (_stack)
        {
            _stack.Peek()[prefix] = uri;
        }
    }

    public string? LookupNamespace(string? prefix)
    {
        prefix ??= string.Empty;

        lock (_stack)
        {
            foreach (var entry in _stack)
            {
                if (entry.TryGetValue(prefix, out var result))
                    return result;
            }
        }

        return null;
    }

    public void Reset()
    {
        lock (_stack)
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
