namespace XmppSharp.Collections;

public class XmppNamespaceStack
{
    private readonly Stack<Dictionary<string, string>> _scopes = new();

    public XmppNamespaceStack()
    {
        Reset();
    }

    public void PushScope()
    {
        lock (_scopes)
            _scopes.Push(new(StringComparer.Ordinal));
    }

    public void PopScope()
    {
        lock (_scopes)
        {
            if (_scopes.Count > 1)
                _scopes.Pop();
        }
    }

    public void AddNamespace(string prefix, string uri)
    {
        if (prefix == "xmlns" || prefix == "xml")
            throw new InvalidOperationException("Reserved XML prefixes cannot be redefined.");

        lock (_scopes)
        {
            _scopes.Peek()[prefix] = uri;
        }
    }

    public string? LookupNamespace(string? prefix)
    {
        prefix ??= string.Empty;

        lock (_scopes)
        {
            foreach (var entry in _scopes)
            {
                if (entry.TryGetValue(prefix, out var result))
                    return result;
            }
        }

        return null;
    }

    public void Reset()
    {
        lock (_scopes)
        {
            UnsafeClear();

            _scopes.Push(new()
            {
                ["xml"] = Namespaces.Xml,
                ["xmlns"] = Namespaces.Xmlns
            });
        }
    }

    void UnsafeClear()
    {
        while (_scopes.TryPop(out var dict))
            dict.Clear();
    }

    public void Clear()
    {
        lock (_scopes)
        {
            UnsafeClear();
        }
    }

    public string? DefaultNamespace
        => LookupNamespace(string.Empty);
}
