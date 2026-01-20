namespace XmppSharp.Xml.Parser;

public class NamespaceStack
{
	readonly Stack<Dictionary<string, string>> _stack = [];

	public void Reset()
	{
		lock (_stack)
		{
			while (_stack.TryPop(out var scope))
				scope.Clear();
		}
	}

	public void PushScope()
	{
		lock (_stack)
			_stack.Push(new(StringComparer.Ordinal));
	}

	public void PopScope()
	{
		lock (_stack)
			if (_stack.TryPop(out var scope))
				scope.Clear();
	}

	public void AddNamespace(string prefix, string uri)
	{
		lock (_stack)
		{
			prefix ??= string.Empty;

			if (!string.IsNullOrWhiteSpace(prefix))
				ArgumentException.ThrowIfNullOrWhiteSpace(uri);

			_stack.Peek().Add(prefix, uri);
		}
	}

	public string LookupNamespace(string? prefix)
	{
		prefix ??= string.Empty;

		lock (_stack)
		{
			foreach (var scope in _stack)
			{
				if (scope.TryGetValue(prefix, out var uri))
					return uri;
			}
		}

		return string.Empty;
	}
}
