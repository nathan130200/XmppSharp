using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace XmppSharp;

public static class StringBuilderPool
{
	private static readonly ConcurrentBag<StringBuilder> _pool = [];

	public static StringBuilder Rent(string? initialValue = default)
	{
		if (!_pool.TryTake(out var result))
			result = new(initialValue);
		else
			result.Append(initialValue);

		return result;
	}

	public static StringBuilder Take()
	{
		if (!_pool.TryTake(out var result))
			return new();

		return result;
	}

	public static void Return(StringBuilder sb)
	{
		Debug.Assert(sb != null);
		sb.Clear();
		_pool.Add(sb);
	}
}
