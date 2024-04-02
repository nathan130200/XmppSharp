#pragma warning disable

using System.Globalization;

namespace XmppSharp;

/// <summary>
/// Represents the delegate type of a function that converts string to <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Type of value that will be parsed.</typeparam>
/// <param name="s">Read only span containing the string for parsing.</param>
/// <param name="result">Output variable for the parsed value.</param>
/// <returns><see langword="true" /> if the parser was done successfully, <see langword="false" /> otherwise.</returns>
public delegate bool TryParseDelegate<T>(ReadOnlySpan<char> s, out T result);

/// <summary>
/// Collection of common parsers of primitive types.
/// </summary>
public static class TryParseHelpers
{
    /// <summary>
    /// Parser implementation for the <see cref="System.UInt16"/> type.
    /// </summary>
    public static TryParseDelegate<ushort> UInt16 { get; } = ushort.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.UInt32"/> type.
    /// </summary>
    public static TryParseDelegate<uint> UInt32 { get; } = uint.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.UInt64"/> type.
    /// </summary>
    public static TryParseDelegate<ulong> UInt64 { get; } = ulong.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.Int16"/> type.
    /// </summary>
    public static TryParseDelegate<short> Int16 { get; } = short.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.Int32"/> type.
    /// </summary>
    public static TryParseDelegate<int> Int32 { get; } = int.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.Int64"/> type.
    /// </summary>
    public static TryParseDelegate<long> Int64 { get; } = long.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.SByte"/> type.
    /// </summary>
    public static TryParseDelegate<sbyte> Int8 { get; } = sbyte.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.Byte"/> type.
    /// </summary>
    public static TryParseDelegate<byte> UInt8 { get; } = byte.TryParse;

    /// <summary>
    /// Parser implementation for the <see cref="System.Boolean"/> type.
    /// </summary>
    public static TryParseDelegate<bool> Boolean { get; } = TryParseBoolImpl;

    /// <summary>
    /// Parser implementation for the <see cref="System.Single"/> type.
    /// </summary>
    public static TryParseDelegate<float> Float { get; } = TryParseFloat;

    /// <summary>
    /// Parser implementation for the <see cref="System.Double"/> type.
    /// </summary>
    public static TryParseDelegate<double> Double { get; } = TryParseDouble;

    /// <summary>
    /// Parser implementation for the <see cref="System.Guid"/> type.
    /// </summary>
    public static TryParseDelegate<Guid> Guid { get; } = System.Guid.TryParse;

    static bool TryParseFloat(ReadOnlySpan<char> span, out float result)
        => float.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

    static bool TryParseDouble(ReadOnlySpan<char> span, out double result)
        => double.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

    static bool TryParseBoolImpl(ReadOnlySpan<char> s, out bool result)
    {
        result = default;

        if (s.Length == 0)
            return false;
        else if (s.Length == 1)
            result = s[0] == '1';
        else
            result = s == "true";

        return true;
    }
}

#pragma warning restore