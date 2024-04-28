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

	public static void Rent(out StringBuilder result)
		=> result = Rent();


	public static string Return(StringBuilder self)
	{
		var result = self.ToString();

		self.Clear();

		if (!s_Pool.Contains(self))
			s_Pool.Add(self);

		return result;
	}

	public static void Return(StringBuilder sb, out string result)
		=> result = Return(sb);
}