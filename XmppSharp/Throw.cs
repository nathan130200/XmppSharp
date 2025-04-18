﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace XmppSharp;

internal static class Throw
{
    public static void IfDisposed(object obj, [DoesNotReturnIf(true)] bool condition)
    {
        IfNull(obj);

        if (condition)
            throw new ObjectDisposedException(obj.GetType().FullName);
    }

    [DoesNotReturn]
    public static void InvalidArgument(string? paramName = default)
        => throw new ArgumentException(default, paramName);

    [DoesNotReturn]
    public static void InvalidArgument(object value, [CallerArgumentExpression(nameof(value))] string? paramName = default)
        => throw new ArgumentException(default, paramName);

    public static void IfStringNotEquals(string? expected, string? current,
        StringComparison comparer = StringComparison.Ordinal,
        [CallerArgumentExpression(nameof(current))] string paramName = default!)
    {
        if (string.Compare(expected, current, comparer) != 0)
            throw new ArgumentException(default, paramName);
    }

    public static void IfNull([NotNull] object? obj, [CallerArgumentExpression(nameof(obj))] string? paramName = default)
    {
        if (obj is null)
            throw new ArgumentNullException(paramName);
    }

    public static void IfNullOrWhiteSpace(string? s, [CallerArgumentExpression(nameof(s))] string? paramName = default)
    {
        if (string.IsNullOrWhiteSpace(s))
            throw new ArgumentException(null, paramName);
    }

    public static void IfNullOrEmpty(string? s, [CallerArgumentExpression(nameof(s))] string? paramName = default)
    {
        if (string.IsNullOrEmpty(s))
            throw new ArgumentException(null, paramName);
    }
}
