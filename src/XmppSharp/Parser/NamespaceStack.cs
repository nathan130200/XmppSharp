namespace XmppSharp.Parser;

public class NamespaceStack
{
    readonly Stack<Dictionary<string, string>> _scopes = [];

    public string? DefaultNamespace => LookupNamespace(default);

    public void PushScope()
    {
        lock (this)
        {
            _scopes.Push(new(2, StringComparer.Ordinal));
        }
    }

    public bool PopScope()
    {
        lock (this)
        {
            if (_scopes.TryPop(out var scope))
            {
                scope.Clear();
                return true;
            }
        }

        return false;
    }

    public void AddNamespace(string prefix, string uri)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        if (!string.IsNullOrWhiteSpace(prefix))
            ArgumentException.ThrowIfNullOrWhiteSpace(uri);

        lock (this)
        {
            _scopes.Peek().Add(prefix, uri);
        }
    }

    public IReadOnlyDictionary<string, string> GetNamespacesInScope()
    {
        lock (_scopes)
            return _scopes.Peek();
    }

    public string? LookupNamespace(ReadOnlySpan<char> prefix)
    {
        if (prefix.Equals("xml", StringComparison.Ordinal))
            return Namespaces.Xml;

        if (prefix.Equals("xmlns", StringComparison.Ordinal))
            return Namespaces.Xmlns;

        lock (this)
        {
            foreach (var scope in _scopes)
            {
                var lookup = scope.GetAlternateLookup<ReadOnlySpan<char>>();

                if (lookup.TryGetValue(prefix, out var result))
                    return result;
            }
        }

        return null;
    }
}
