namespace XmppSharp.Xml;

/// <summary>
/// Represents a stack of XML namespaces, allowing for the management of namespace scopes and lookups.
/// </summary>
public sealed class NamespaceStack
{
    /// <summary>
    /// The reserved prefix "xml" is associated with the XML namespace URI "http://www.w3.org/XML/1998/namespace".
    /// </summary>
    public const string PrefixXml = "xml";

    /// <summary>
    /// The reserved prefix "xmlns" is associated with the XML namespace URI "http://www.w3.org/2000/xmlns/".
    /// </summary>
    public const string PrefixXmlns = "xmlns";

    readonly Stack<Dictionary<string, string>> _namespaces = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="NamespaceStack"/> class with no XML namespaces defined.
    /// </summary>
    public NamespaceStack()
    {

    }

    /// <summary>
    /// Pushes a new scope onto the namespace stack.
    /// </summary>
    /// <remarks>
    /// This is typically used when entering a new XML element that may define its own namespaces.
    /// </remarks>
    public void PushScope()
    {
        lock (_namespaces)
        {
            _namespaces.Push([]);
        }
    }


    /// <summary>
    /// Pops the current scope from the namespace stack.
    /// </summary>
    /// <remarks>
    /// This is typically used when exiting an XML element, ensuring that namespaces defined within that element are no longer accessible.
    /// </remarks>
    /// <returns>True if a scope was successfully popped; otherwise, false.</returns>
    public bool PopScope()
    {
        bool result = false;

        lock (_namespaces)
        {
            if (_namespaces.TryPop(out var scope))
            {
                scope.Clear();
                result = true;
            }
        }

        return result;
    }

    /// <summary>
    /// Adds a namespace declaration to the current scope.
    /// </summary>
    /// <param name="prefix">The prefix of the namespace.</param>
    /// <param name="uri">The URI of the namespace.</param>
    /// <exception cref="InvalidOperationException">Thrown if the prefix is reserved or stack is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the URI is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the prefix is not empty and the URI is empty.</exception>
    public void AddNamespace(string? prefix, string uri)
    {
        prefix ??= string.Empty;

        if (prefix == PrefixXml || prefix == PrefixXmlns)
            throw new InvalidOperationException($"The prefix '{prefix}' is reserved and cannot be used for namespace declarations.");

        ArgumentNullException.ThrowIfNull(uri);

        if (!string.IsNullOrWhiteSpace(prefix))
            ArgumentException.ThrowIfNullOrWhiteSpace(uri);

        lock (_namespaces)
            _namespaces.Peek()[string.Intern(prefix)] = string.Intern(uri);
    }

    /// <summary>
    /// Looks up the namespace URI associated with the given prefix in the current stack of namespaces.
    /// </summary>
    /// <remarks>
    /// The lookup will search from the top of the stack downwards, returning the first matching namespace URI.
    /// </remarks>
    /// <param name="prefix">The prefix of the namespace to look up.</param>
    /// <returns>The URI of the namespace if found; otherwise, an empty string.</returns>
    public string LookupNamespace(string? prefix)
    {
        prefix ??= string.Empty;

        if (prefix == PrefixXml) return Namespaces.Xml;
        if (prefix == PrefixXmlns) return Namespaces.Xmlns;

        lock (_namespaces)
        {
            foreach (var scope in _namespaces)
            {
                if (scope.TryGetValue(prefix, out var uri))
                    return string.Intern(uri);
            }
        }

        return string.Empty;
    }
}
