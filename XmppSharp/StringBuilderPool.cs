using System.Collections.Concurrent;

namespace System.Text;

public static class StringBuilderPool
{
    private static ConcurrentBag<StringBuilder> s_Pool = new();

    public static StringBuilder Rent()
    {
        if (s_Pool.TryTake(out var res))
            return res;

        return new();
    }

    public static string Return(StringBuilder sb)
    {
        var result = sb.ToString();
        sb.Clear();
        s_Pool.Add(sb);
        return result;
    }
    public static void Return(StringBuilder sb, out string result)
        => result = Return(sb);
}
