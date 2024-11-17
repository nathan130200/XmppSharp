using System.Runtime.CompilerServices;

namespace XmppSharp;

public static class ThrowHelper
{
    public static void ThrowIfNotEquals(string? expected, string? current,
        StringComparison comparer = StringComparison.Ordinal,
        [CallerArgumentExpression(nameof(current))] string paramName = default!)
    {
        if (string.Compare(expected, current, comparer) != 0)
            throw new ArgumentException(default, paramName);
    }

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
