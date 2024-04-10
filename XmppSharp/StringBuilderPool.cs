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
        s_Pool.Add(sb);
        return result;
    }
    public static void Return(StringBuilder sb, out string result)
        => result = Return(sb);

    class Entry : IDisposable
    {
        private StringBuilder _builder;

        public Entry(StringBuilder builder)
            => _builder = builder;

        public void Dispose()
        {
            if (_builder != null)
            {
                StringBuilderPool.Return(_builder);
                _builder = null;
            }
        }
    }
}