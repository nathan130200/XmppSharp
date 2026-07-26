using System.Diagnostics.CodeAnalysis;

namespace XmppSharp.Parser;

public class NamespaceStack
{
	readonly Stack<Scope> _scopes = [];

	class Scope
	{
		Dictionary<string, string>? _dictionary;

		public void Add(string key, string value)
		{
			_dictionary ??= new(2, StringComparer.Ordinal);

			_dictionary.Add(key, value);
		}

		public void Clear()
		{
			_dictionary?.Clear();

			_dictionary = null;
		}

		public bool TryLookup(ReadOnlySpan<char> prefix, [NotNullWhen(true)] out string? uri)
		{
			uri = default;

			if (_dictionary is null)
				return false;

#if NET9_0_OR_GREATER

			var lookup = _dictionary.GetAlternateLookup<ReadOnlySpan<char>>();

			return lookup.TryGetValue(prefix, out uri);

#else // < NET8_0

			foreach (var (key, value) in _dictionary)
			{
				if (prefix.Equals(key, StringComparison.Ordinal))
				{
					uri = value;
					return true;
				}
			}

			return false;

#endif // >= NET9_0_OR_GREATER
		}
	}

	public string? DefaultNamespace => LookupNamespace(default);

	public void PushScope()
	{
		lock (this)
		{
			_scopes.Push(new());
		}
	}

	public bool PopScope()
	{
		lock (this)
		{
			if (_scopes.TryPop(out var scope))
			{
				scope?.Clear();
				return true;
			}
		}

		return false;
	}

	public void AddNamespace(string prefix, string uri)
	{
		ArgumentNullException.ThrowIfNull(prefix);

		// Declaring namespace without prefix with empty uri is used to "unset" or "clear" uri in current scope. (xmlns="")
		// But declaring namespace with an prefix but empty uri is invalid. (xmlns:prefix="")

		if (string.IsNullOrWhiteSpace(uri))
			throw new ArgumentException("Namespace with prefix must have an URI.", nameof(uri));

		lock (this)
		{
			_scopes.Peek().Add(prefix, uri);
		}
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
				if (scope.TryLookup(prefix, out var result))
					return result;
			}
		}

		return null;
	}
}
