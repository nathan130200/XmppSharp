using System.Collections.Concurrent;

namespace System.Text;

public static class StringBuilderPool
{
	static readonly ConcurrentBag<StringBuilder> s_Pool = new();

	public static StringBuilder Rent()
	{
		if (s_Pool.TryTake(out var res))
			return res;

		return new();
	}

	public static IDisposable Rent(out StringBuilder sb)
		=> new Entry(sb = Rent());

	public static string Return(StringBuilder sb)
	{
		var result = sb.ToString();
		sb.Clear();

		if (!s_Pool.Contains(sb))
			s_Pool.Add(sb);

		return result;
	}

	public static void Return(StringBuilder sb, out string result)
		=> result = Return(sb);

	static void ReturnInplace(StringBuilder sb)
	{
		sb.Clear();

		if (!s_Pool.Contains(sb))
			s_Pool.Add(sb);
	}

	readonly struct Entry : IDisposable
	{
		readonly StringBuilder _builder;

		public Entry(StringBuilder builder)
			=> this._builder = builder;

		public void Dispose()
			=> ReturnInplace(this._builder);
	}
}