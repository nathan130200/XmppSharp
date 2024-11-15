using System.Runtime.CompilerServices;
using System.Text;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Helpers
{
    public static byte[] GetBytes(this string str, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(str);

    public static string GetString(this byte[] bytes, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(bytes);

    public static byte[] GetBytes(this Node node)
        => node.ToString()!.GetBytes();
}

public static class ThrowHelper
{
    public static void ThrowIfNull(object? obj, [CallerArgumentExpression(nameof(obj))] string? paramName = default)
    {
        if (obj is null)
            throw new ArgumentNullException(paramName);
    }

    public static void ThrowIfNullOrWhiteSpace(string? s, [CallerArgumentExpression(nameof(s))] string? paramName = default)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException(null, paramName);
    }

    public static void ThrowIfNullOrEmpty(string? s, [CallerArgumentExpression(nameof(s))] string? paramName = default)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException(null, paramName);
    }
}
